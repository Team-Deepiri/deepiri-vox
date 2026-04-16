# Fox Rocket Arcade - Game Specification

## Project Overview
- **Name**: Fox Rocket Arcade
- **Type**: Classic arcade shooter with pseudo-3D visuals
- **Core Functionality**: Fox character on rocket dodging AI aliens, switching directions through a rotating landscape
- **Target Users**: Arcade game enthusiasts

## Visual Specification

### Fox Character
- Orange fox with white details
- Green shirt (no pants)
- Small backpack on back
- Expressive sprites for movement states

### Rocket
- Single cylindrical rocket (not ship)
- Orange/white with fins
- Flame exhaust animation
- Explodes after 25 seconds with visual countdown
- New rocket spawns when current explodes

### Backgrounds (Cycle through)
1. **Space**: Deep purple with stars, distant galaxies
2. **City**: Neon Tokyo-style at night
3. **Desert**: Orange/red canyons at sunset
4. **Forest**: Dark green trees, misty

### AI Aliens
- Various enemy types:
  - Drone scouts (small, fast)
  - Fighter ships
  - Mother ships (bosses)
- Red/orange glow, robotic designs

### Pseudo-3D Direction System
- Base direction: UP (flying upward through screen)
- Turn sequence: 0° → 50° → 90° → 180° (full rotation)
- Screen rotates with player direction change
- Landscape shifts to show new approach angle
- 50° turn = slight angle shift
- 90° turn = major direction change

## Gameplay Specification

### Controls
- **Arrow Keys / WASD**: Move fox (up/down/left/right)
- **Space**: Fire laser
- **Q/E**: Turn left/right (50° increments)
- **Enter**: Hop to new rocket (when one spawns)

### Mechanics
- Rocket explodes 25 seconds after launch
- New rocket spawns at random edge when timer hits 0
- Player must press Enter to hop to new rocket within 3 seconds
- Miss the window = game over
- Enemies shoot at player
- Collect power-ups for:
  - Rapid fire
  - Shield (temporary invincibility)
  - Score multiplier

### Enemy Behavior
- Drones: Zigzag patterns, shoot occasionally
- Fighters: Chase player, shoot frequently
- Mothers: Slow, heavy fire, drop power-ups

### Scoring
- Drone kill: 100 points
- Fighter kill: 250 points
- Mother kill: 1000 points
- Close call bonus: 500 points (near-miss with rocket)
- Direction change bonus: 200 points

## Technical Architecture

### Game Loop (60 FPS)
```
- Input handling
- Update entities
- Check collisions
- Update pseudo-3D rotation
- Render frame
```

### Entity System
- Fox (player)
- Rocket (vehicle)
- Enemies (list)
- Bullets (player + enemy)
- Power-ups
- Particles (explosions, flames)

### State Machine
- MENU → PLAYING → PAUSED → GAME_OVER
- Direction states: NORTH → NORTHEAST → EAST → SOUTHEAST → SOUTH

## Acceptance Criteria
- [ ] Fox renders with green shirt, backpack, no pants
- [ ] Rocket displays with countdown timer
- [ ] Rocket explosion triggers at 25s
- [ ] New rocket spawns after explosion
- [ ] Player can hop to new rocket
- [ ] Four distinct backgrounds cycle
- [ ] AI aliens spawn and shoot
- [ ] Player can shoot enemies
- [ ] Screen rotation on direction change
- [ ] Score tracking
- [ ] Game over on collision with enemy/projectile