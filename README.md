# Sailboat

A small sailing simulation made in Unity.
A single-sail catboat moves on the water under the influence of wind and drag, computed by a custom
sailing model.

![Sailboat](Docs/sailboat.gif)

## Features
 
- **Custom sailing physics** — sail lift and drag from the apparent wind, keel and hull resistance,
  speed-dependent rudder; realistic behaviour such as a no-go zone upwind and the need to tack.
- **One fore-and-aft gaff sail** with opening 0–100% and rotation ±45°, shown on the model:
  the sail turns around the mast, furls down to the boom and the gaff follows its top edge.
- **Varying wind** — slow changes of direction and speed, visualised by white streaks in the air.
- **Visual feedback** — the boat heels with the sideways sail force, bobs on the water, and the rudder blade turns.
- **Follow camera** with smoothed position and heading.
- **HUD** with two labels: sail opening and sail rotation.
- **Low-poly catboat model** made in Blender.

## Technical overview
 
| Area | Implementation |
|---|---|
| Physics | `SailingModel` — plain C# model with custom forces and semi-implicit Euler integration, applied directly to the transform |
| UI | UI Toolkit: `Hud.uxml` / `Hud.uss` + `HudView` (`PanelRenderer`) |
| Input | New Input System with a generated C# class (`SailboatInputActions`) |
| Rendering | Universal Render Pipeline |
| Build | IL2CPP, managed stripping level High, C++ compiler configuration Master |
| Tests | EditMode unit tests of the sailing model |
| Unity version | 6000.5.11f1 |

## Controls

| Action | Keyboard |
|---|---|
| Sail opening (hoist / lower) | W / S |
| Sail rotation (±45°) | Q / E |
| Rudder | A / D |
| Quit | Esc |

**How to sail:** keep the sail on the side away from the wind, at roughly half the angle between the bow and the wind. With the wind coming from starboard rotate the sail with E, from port with Q.
Sailing closer than about 35° to the wind is not possible (the "no-go zone"), so reach an upwind target by zigzagging (tacking). The white streaks show the true wind.

## Running the project

1. Open the project in Unity **6000.5.11f1**.
2. Open `Assets/_Project/Scenes/Sailboat.unity` and press Play.

The orange markers in the scene form a slalom course - a quick way to try tacking, gybing and steering.

## Sailing model

The boat is simulated in 2D on the water plane (world X/Z). `FixedUpdate` in `SailboatController` asks `SailingModel` for the next state and copies it to the transform.

**Conventions:** heading 0° points along world +Z and grows clockwise seen from above, matching Unity's
Y rotation. The wind direction is given as the direction it blows *from* (0° = from +Z, 90° = from +X).

**Forces and torques:**

- **Apparent wind** = true wind − boat velocity.
- **Sail** — a flat plate foil: lift coefficient `CL = sin 2α`, drag coefficient `CD = 1 − cos 2α`
  plus a constant parasitic drag, scaled by the dynamic pressure `q = ½ρv²`, sail area and sail opening.
  The angle of attack is measured on the leeward side of the sail.
- **Hull and keel** — anisotropic resistance with linear and quadratic terms: low along the hull,
  very high sideways (the keel), which turns the sail force into forward motion. Moving stern first meets
  ten times more resistance, because the hull, keel and rudder are shaped for water flowing from the bow —
  the boat can drift backwards when it stalls head to wind, but cannot sail backwards fast.
- **Rudder** — torque proportional to the rudder input and the forward speed (linear and quadratic terms),
  so the boat steers only when it moves through the water.
- **Yaw damping** — resists rotation.

**Integration:** semi-implicit Euler. Resistance is applied so that it can slow a velocity down to zero but never reverse it, which keeps the simulation stable at low speeds.

The wind varies slowly in direction (±20°) and speed (±20%) using Perlin noise.

All parameters live in ScriptableObjects (`SailboatConfig`, `WindConfig`) and can be tuned in the Inspector.

## Project structure

```
Assets/_Project/
├── Config/        ScriptableObject assets
├── Input/         Input actions asset
├── Materials/  Models/  Textures/
├── Scenes/        Sailboat.unity
├── Scripts/Runtime/
│   ├── Simulation/   pure C# model: SailingModel, FoilMath, WindMath, state and control structs
│   ├── Config/       SailboatConfig, WindConfig
│   ├── Components/
│   │   ├── Boat/          Sail, SailboatController, SailboatInputHandler
│   │   ├── BoatVisuals/   SailVisual, RudderVisual, HullMotionVisual
│   │   ├── Environment/   WindProvider, WindStreaks, WaterFollow
│   │   └── Presentation/  CameraFollow, HudView
│   └── Input/        generated input actions class
└── UI/            Hud.uxml, Hud.uss, panel settings
Assets/Tests/EditMode/   unit tests of the model
Art/                     Blender source of the boat model
```

## Simplifications

- The simulation is 2D. Heel and bobbing are purely visual and do not affect speed.
- The sail is modelled as a flat plate; there is no sail twist, camber or luffing.
- The wind is uniform in space; there are no gusts localised on the water and no waves in the simulation.

## Tests

`Test Runner` runs the unit tests of `FoilMath`, `WindMath` and `SailingModel` (force directions, no-go zone, drag behaviour including higher drag when moving backwards, steering, integration).