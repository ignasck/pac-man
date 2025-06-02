# 🟡 Pac-Man Game

A classic Pac-Man game implemented in **C#** using **Windows Forms**. Navigate Pac-Man through a maze, eat pellets, avoid ghosts, and score points by collecting fruit and energizers. This project replicates the nostalgic arcade experience with custom ghost behaviors and smooth gameplay.

---

## ✨ Features

* **Classic Gameplay**
  Control Pac-Man to eat all pellets and energizers while avoiding four distinct ghosts (red, blue, pink, orange).

* **Intelligent Ghost Behavior**
  Ghosts exhibit unique movement patterns and strategies. After being eaten, they temporarily pause before returning to the maze. They navigate smoothly, avoiding wall collisions and exiting their "home" area to pursue Pac-Man.

* **Dynamic Scoring**

  * Pellets: 10 points each
  * Energizers: 50 points, activating a temporary ghost-eating mode
  * Ghosts eaten during energizer mode: 200 points each
  * Fruit: 100 bonus points per collection

* **Engaging Game States**

  * An interactive start screen with instructions
  * Win condition when all pellets are cleared
  * Game over when all three lives are lost
  * Smooth animations for Pac-Man’s death and game victory

* **Crisp Visuals**

  * Custom sprites for Pac-Man, ghosts, pellets, energizers, and fruit
  * Maze transforms with alternating colors during the win animation
  * Uses the classic OCR-A font for an authentic retro look (fallback: Courier New)

---

## 💻 Requirements

* **Operating System:** Windows
* **Development Environment:** Visual Studio 2019 or later
* **.NET Framework:** 4.7.2 or later
* **Dependencies:** Built purely with Windows Forms, no external libraries
* **Assets:**

  * All image files (e.g., Pac-Man sprites, ghost sprites, fruit) are located in the `Images/` folder
  * The OCR-A font is optionally used for text rendering (fallback to Courier New)

---

## 🚀 Installation

### Clone the Repository

```bash
git clone https://github.com/your-username/pacman-game.git
cd pacman-game
```

### Open in Visual Studio

Open the `PacMan.sln` file in Visual Studio.

### Set Up Assets

* Ensure the `Images/` folder is placed within the project directory (`PacMan/Images/`)
* Verify that all image files (e.g., `pacman_0.png`, `ghost_red.png`) are included in the project
* Set their **Copy to Output Directory** property to **Copy if newer**

### Build and Run

* **Build the solution:** Press `Ctrl+Shift+B`
* **Run the application:** Press `F5`

> **Note:** If the OCR-A font is not available, the game will default to Courier New for text display.

---

## ▶️ How to Play

### Start the Game

Press button in the initial start screen to begin your arcade adventure!

### Controls

Use the **Arrow keys** (↑, ↓, ←, →) to guide Pac-Man through the maze.

### Objective

* Eat all small pellets (`.`) and large energizers (`o`) to clear the level
* Avoid direct contact with ghosts unless in **energizer mode** (when ghosts turn blue and become edible)
* Collect fruit for bonus points

### Game Mechanics

* **Ghosts:** Ghosts pursue Pac-Man through the maze. They exit their home after a short delay and return after being eaten.
* **Energizer Mode:** Eating an energizer turns ghosts blue and lets Pac-Man eat them for bonus points.
* **Lives:** You start with 3 lives. You lose one if touched by a non-blue ghost.
* **Win/Lose Conditions:** Win by clearing all pellets. Lose when all lives are lost.

---

## 🏗️ Project Structure

```
PacMan/
├── Form1.cs          # Core game logic and rendering
├── Images/           # Sprite assets (Pac-Man, ghosts, fruit, etc.)
├── PacMan.sln        # Visual Studio solution file
```

---

## ⚙️ Technical Details (For Developers)

* **Game Loop**

  * Runs on a 20ms `Timer`
  * Pac-Man moves every 2 ticks
  * Ghosts move every 5 ticks (or 3 during energizer mode)

* **Board Representation**

  * The board is a 36x28 `char[][]` grid
  * Symbols include:

    * `#` - wall
    * `.` - pellet
    * `o` - energizer
    * `-` - ghost door
    * `0` - Pac-Man
    * `1` to `4` - ghosts

* **Rendering**

  * Uses `System.Drawing` to render sprites and text
  * Each tile is 20x20 pixels

* **Animations**

  * Pac-Man: 4 frames
  * Ghosts: 2 frames
  * Fruit: 2 frames

---

## ⚠️ Known Issues

* **Ghost Movement Oddities**
  Ghosts may sometimes linger at intersections. Check the `MoveGhost` logic if needed.

* **Delay Timing**
  If ghost respawn delay feels off, ensure `gameTimer.Interval` is 20ms and system performance is adequate.

* **Performance**
  May vary on lower-end machines. Occasional stutters could occur.

* **Font Fallback**
  If OCR-A is not installed, game will use Courier New. Text appearance may differ slightly.
