# SpaceRock

A mobile sandbox game built in Unity for Android and iOS. An asteroid floats in space — drag and fling it, pinch to resize it, or hold to charge a shatter. No objectives, just satisfying interactions.

**[Play on itch.io](https://maksymy11.itch.io/spacerock)**

## _▶ Click to watch gameplay showcase_

[<img src="https://img.youtube.com/vi/Q0Kc5yb4cIE/hqdefault.jpg" width="400">](https://youtube.com/shorts/Q0Kc5yb4cIE?feature=share)

## Gameplay

| Input               | Action                                                                                                    |
| ------------------- | --------------------------------------------------------------------------------------------------------- |
| **Drag + Fling**    | Drag the rock with one finger, release to send it flying; bounces off screen edges                        |
| **Pinch**           | Two fingers scale the rock up or down (clamped 0.3x – 2.0x)                                               |
| **Hold to Shatter** | Hold while dragging to charge a radial meter; at full charge the rock explodes into pieces, then respawns |

---

## Technical Highlights

### Physics & Input

- Drag uses `Rigidbody.MovePosition` instead of `transform.position` to keep the physics body in sync and prevent the snap/teleport on release
- Fling velocity derived from `touch.deltaPosition / touch.deltaTime` at the moment of release, scaled to world units, with a 50 units/sec cap to prevent runaway velocity after fast throws
- Pinch uses per-frame finger distance delta ratio applied to `localScale`, keeping the scaling feel proportional regardless of starting size
- Bounce checks the rock's surface (`position ± radius`) against screen-space world bounds converted each frame via `Camera.main.ScreenToWorldPoint`; uses `Mathf.Abs` on the reflected axis to guarantee the velocity always points away from the wall
- Drag and hold-to-shatter run simultaneously on the same touch — no separate input phases needed

### Shatter System

- Hold timer charges while `isDragging` is true, no separate raycast required, reusing the existing drag flag
- `ShatterAndReset()` coroutine hides the source rock (Renderer + Collider disabled), instantiates the `RockFracture7` prefab at matching position and scale, disables gravity on each piece, then applies `AddExplosionForce` per fragment
- Fracture prefab destroyed and original rock restored after 3 seconds
- Trail Renderer cleared on shatter and re-enabled on restore to prevent stale trail artifacts

### Juice & Feel

- Fire trail (`TrailRenderer`) width driven every frame by `localScale.x * 3.5f` so it stays proportional during pinch scaling
- Screen shake coroutine offsets `Camera.main.transform.localPosition` by `Random.insideUnitCircle * magnitude` each frame for a set duration, then restores original position
- Randomized audio pools: 2 fling whoosh clips, 5 bounce thud clips; clip selected via `Random.Range` on `PlayOneShot` to avoid repetition

---

## Stack

- **Engine:** Unity 6.3 LTS (URP)
- **Language:** C#
- **Platform:** Android (APK), iOS
- **Physics:** Unity Rigidbody — gravity disabled, `linearDamping = 1f`
- **Input:** `UnityEngine.Input` touch API
- **Assets used:**
  - BreakableAsteroids — Rock7 mesh + RockFracture7 shatter prefab
  - Free Fire VFX URP (Vefects) — fire trail material
  - Free Skyboxes - Space — environment skybox

---

## Project Structure

```
Assets/
  Scripts/
    RockController.cs   # All gameplay logic: fling, pinch, shatter, bounce, audio, trail
  Prefabs/              # Rock7, RockFracture7
  Materials/            # URP-converted rock + trail materials
  Audio/                # Fling, bounce, shatter SFX clips
  Scenes/               # Main scene
```

---

## Build

**Android APK**

1. Install Android Build Support via Unity Hub → Installations → Add Modules
2. File → Build Settings → switch platform to Android
3. Scripting Backend: Mono, Architecture: ARMv64
4. Hit Build

**iOS (testing via Unity Remote 5)**

1. Install iTunes (direct from apple.com — not Microsoft Store)
2. Edit → Project Settings → Editor → Unity Remote → Device: Any iOS Device
3. Connect iPhone via USB, run the scene in Play Mode

---

## License

MIT — covers code only. Third-party assets (BreakableAsteroids, Free Fire VFX URP, Free Skyboxes) are excluded and not redistributed.
