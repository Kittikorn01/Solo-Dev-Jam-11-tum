# Game Design Document: Everything Has a Cost

## 1. Game Overview
**Genre:** 2D Top-Down Arena Slasher
**Theme:** "Everything Has a Cost" (SoloDevJam 11)
**Art Style:** Geometric Minimalist (Shapes)
**Core Loop:** Choose Cost -> Fight Waves -> Survive -> Win/Lose

## 2. Technical Pillars & Constraints
To ensure a high "Polish" score within the jam's time limit, the following technical strategies are enforced:

### 2.1 Procedural Animation (Sine Waves)
*   **No Frame-by-Frame:** All character movements (Squash & Stretch) are handled via `sin(time)` functions.
*   **Implementation:**
    *   **Movement:** Apply Sine wave to Y-axis scale to create a "bouncy" walk feel.
    *   **Boomer (Pre-explosion):** Use multiplied Sine waves (e.g., `sin(time * 20) * sin(time * 5)`) for high-frequency pulsation.
    *   **Floating Items:** Floating weapons/pickups use Sine waves for a natural hover effect.

### 2.2 Randomized Level Design
*   **Single Arena:** Focus on a single, well-balanced combat space rather than multiple levels.
*   **Dynamic Floors:** 
    *   Script-driven spawning of decorative sprites (Dirt, Rocks).
    *   **Randomization:** Each decoration gets a random rotation (0-360°) and a 50% chance of X-Flip to break tiling patterns.

### 2.3 Cheaty Enemy AI (Wandering)
*   **The Problem:** Enemies usually stack in a straight line ("Conga line"), making them too easy to kite and look robotic.
*   **The Solution:** Add a randomized angular offset to the pathfinding vector.
*   **Result:** Enemies drift slightly while chasing, creating a more organic "swarm" behavior.

### 2.4 Difficulty Scaling (Compositional)
*   **No Bullet Sponges:** Enemy HP and Damage remain constant throughout the game.
*   **Scaling Strategy:** Increase difficulty strictly through **Wave Composition**.
*   **Mechanic:** Spawn higher quantities of enemies and mix "Chasers" with "Boomers" to pressure the player's positioning.

## 3. Core Mechanics
*   **The Cost System:** 
    *   **Blood Path:** High damage, but attacks cost Player HP.
    *   **Time Path:** High speed, but the game timer ticks faster.
*   **Combat:** Melee slashing with knockback and micro-dashes.
*   **Survival:** Enemies drop HP or Time pickups occasionally.

## 4. Visuals & Audio
*   **Juice:** Screen shake, Freeze frames (Hit stop), and Particle explosions.
*   **Audio:** Dynamic BGM that scales with wave intensity.
