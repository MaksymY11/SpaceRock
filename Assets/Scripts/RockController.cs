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

    void Pinch()
    {

    }

    void Press()
    {

    }

    // Bounces rock along screen edges by 
    void Bounce()
    {
        Vector3 pos = transform.position;
        float maxSpeed = 50f;
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        //Converting screen corners to world space
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z - Camera.main.transform.position.z)); // Top right corner is always Screen.width, Screen.height
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, transform.position.z - Camera.main.transform.position.z)); // Bottom left corner is always 0,0

        if (pos.x < bottomLeft.x || pos.x > topRight.x)
        {
            rb.linearVelocity = new Vector3(-rb.linearVelocity.x, rb.linearVelocity.y, 0);
        }

        if (pos.y < bottomLeft.y || pos.y > topRight.y)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -rb.linearVelocity.y, 0);
        }
    }
}
