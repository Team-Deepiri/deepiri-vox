# Fox Rocket Arcade - Unity Project

An industry-standard arcade shooter game built in Unity.

## Requirements
- Unity 2021.3 LTS or newer
- 2D Package

## Setup

### 1. Open in Unity
```bash
# Open Unity Hub → Add → Select FoxRocketArcade/UnityProject
```

### 2. Setup Tags
Go to **Edit → Project Settings → Tags and Layers**:
- Add Tag: `Player`, `PlayerBullet`, `Enemy`, `EnemyBullet`
- Add Layer: `Player`, `Enemy`, `Bullet`, `Powerup`

### 3. Create Sprites (Placeholder)
Create simple sprites in Unity:
- **Fox**: Orange fox character, ~40x40px, green shirt area, backpack
- **Rocket**: Cylinder shape, orange/white, ~30x50px
- **Enemies**: 3 types (Drone, Fighter, Mother)
- **Bullets**: Green player bullets, red enemy bullets
- **Backgrounds**: 4 parallax layers

### 4. Create Prefabs
1. **FoxPrefab**: Sprite + BoxCollider2D + Rigidbody2D + FoxController
2. **RocketPrefab**: Sprite + RocketController + ParticleSystem (flame)
3. **EnemyPrefab** (x3): Sprite + BoxCollider2D + EnemyController
4. **BulletPrefab** (x2): Sprite + CircleCollider2D + BulletController
5. **StarPrefab**: Small white sprite for parallax
6. **PowerupPrefab**: Circle sprite with type
7. **ExplosionPrefab**: Particle system

### 5. Link References
In Hierarchy, create:
- `GameManager` object → attach GameManager.cs
- `Player` → attach FoxController.cs → link prefabs
- `Rocket` → attach RocketController.cs → link particles
- `DirectionManager` → attach DirectionController.cs
- `BackgroundManager` → attach BackgroundManager.cs
- `EnemySpawner` → attach EnemySpawner.cs → link enemy prefabs

### 6. Build
```bash
# File → Build Settings → Switch Platform → Windows/Mac/Linux
# Build and Run
```

## Controls
| Key | Action |
|-----|--------|
| Arrow/WASD | Move |
| Space | Fire |
| Q/E | Turn 45° |
| Enter | Hop rocket |
| Escape | Pause |

## Game Features
- Fox with green shirt, backpack, no pants
- Rocket explodes at 25s, new one spawns
- 4 backgrounds: Space, City, Desert, Forest
- 3 enemy types: Drone, Fighter, Mother
- Power-ups: Rapid Fire, Shield, 2x Score
- Pseudo-3D direction switching
- Parallax scrolling

## Project Structure
```
UnityProject/
├── Assets/
│   ├── Scripts/
│   │   ├── FoxController.cs
│   │   ├── RocketController.cs
│   │   ├── EnemyController.cs
│   │   ├── BulletController.cs
│   │   ├── GameManager.cs
│   │   ├── EnemySpawner.cs
│   │   ├── BackgroundManager.cs
│   │   ├── DirectionController.cs
│   │   ├── PlayerInput.cs
│   │   └── PowerupController.cs
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Materials/
│   ├── Sprites/
│   └── Audio/
└── ProjectSettings/
```

## Modifications
- Adjust `maxDuration` in RocketController for explosion time
- Modify enemy stats in EnemyController for difficulty
- Change backgrounds in BackgroundManager.cs colors
- Edit directionCount in DirectionController for turn precision

## Build Targets
- Windows (exe)
- macOS (app)
- Linux
- WebGL
- Android/iOS