using UnityEngine;

public class RockController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 touchStartPos;
    private bool isDragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1f; // Sphere should loose speed over time

    }

    // Update is called once per frame
    void Update()
    {
        Fling();
        Pinch();
        Press();
        Bounce();
    }

    // INPUT 1: Drag the rock with one finger, then release it to fling it
    void Fling()
    {
        // Running swipe logic only if one finger is one the screen, so that Pinch() with 2 fingers doesn't interfere.
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0); // Get data for first finger (position, phase, etc.)
            Ray ray = Camera.main.ScreenPointToRay(touch.position); // Creating invisible ray that starts at camera and shoots thru finger position
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                // Fires the ray and if it hits the collider fills hit with result
                if (Physics.Raycast(ray, out hit))
                {   
                    // checks if what was hit is our rock prefab
                    if (hit.transform == transform)
                    {
                        isDragging = true; // mark that we're dragging it
                        rb.linearVelocity = Vector3.zero; // and set it's it's motion to zero (bc we're dragging it)
                    }
                }
                touchStartPos = touch.position;
            }
            
            // Drag sphere to follow finger each frame
            if (touch.phase == TouchPhase.Moved && isDragging == true)
            {
                float distZ = transform.position.z - Camera.main.transform.position.z; // Calculate how far sphere is from camera in z-axis so that world position stays at same depth
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distZ)); // Convert 2D finger position to 3D world at that depth
                rb.MovePosition(worldPos);
            }

            // Fling sphere once touch ends
            if (touch.phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    Vector2 fling = touch.deltaPosition / touch.deltaTime; // fling vector is (how far finger moved in last frame) / (finger's speed at moment of release)
                    rb.linearVelocity = new Vector3(fling.x, fling.y, 0) *0.01f; // assing sphere's velocity with 
                    isDragging = false;
                }
            }
        }
    }

    // INPUT 2: Using two fingers, measures distance between fingers for each frame, applies ratio to scale, clamped between 0.3 and 5.0
    void Pinch()
    {  
        // Key difference between Pinch() and Fling(), using 2 fingers activates pinch, so they don't interfere with each other.
        if (Input.touchCount == 2)
        {
            isDragging = false; // To prevent Fling() from dragging object (just in case)

            // Scale clamps
            float maxScale = 5f;
            float minScale = 0.3f;

            // Data for each finger
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Previous position for each finger
            Vector2 prev1 = touch1.position - touch1.deltaPosition;
            Vector2 prev2 = touch2.position - touch2.deltaPosition;

            // Core logic: if fingers move apart -> scale the object up, if fingers move together -> scale object down
            float prevDist = (prev1 - prev2).magnitude; // Distance between fingers in previous frame
            float currDist = (touch1.position - touch2.position).magnitude; // Distance between fingers in current frame
            float ratio = currDist/prevDist;
            float clampedScale = Mathf.Clamp(transform.localScale.x*ratio, minScale, maxScale); // We scale uniformly with Vector3.one, so it doesn't matter which axis we choose.
            transform.localScale = Vector3.one * clampedScale;

        }
    }

    void Press()
    {

    }

    // Bounces rock along screen edges by 
    void Bounce()
    {
        if (isDragging) return;

        Vector3 pos = transform.position;
        float radius = transform.localScale.x * 0.5f;
        float maxSpeed = 50f;
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        //Converting screen corners to world space
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z - Camera.main.transform.position.z)); // Top right corner is always Screen.width, Screen.height
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, transform.position.z - Camera.main.transform.position.z)); // Bottom left corner is always 0,0

        // Each of these if statements checks if sphere has crossed the boundary with pos+-radius,
        // uses Mathf.Abs to guarantee velocity points away from the wall,
        // and clamps position to prevent bounce triggering again if in the next frame sphere is still overlapping the edge

        // Left
        if (pos.x - radius < bottomLeft.x)
        {
            rb.linearVelocity = new Vector3(Mathf.Abs(rb.linearVelocity.x), rb.linearVelocity.y, 0);
            pos.x = bottomLeft.x + radius;
        }

        // Bottom
        if (pos.y - radius < bottomLeft.y)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Abs(rb.linearVelocity.y), 0);
            pos.y = bottomLeft.y + radius;
        }

        // Right
        if (pos.x + radius > topRight.x)
        {   
            rb.linearVelocity = new Vector3(-Mathf.Abs(rb.linearVelocity.x), rb.linearVelocity.y, 0);
            pos.x = topRight.x - radius;
        }

        // Top
        if (pos.y + radius > topRight.y)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -Mathf.Abs(rb.linearVelocity.y), 0);
            pos.y = topRight.y - radius;
        }

        rb.MovePosition(pos); // Applies clamped position back to rb

    }
}
