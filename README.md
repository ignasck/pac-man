Pac-Man Game
A classic Pac-Man game implemented in C# using Windows Forms. Navigate Pac-Man through a maze, eat pellets, avoid ghosts, and score points by collecting fruit and energizers. This project replicates the nostalgic arcade experience with custom ghost behaviors and smooth gameplay.

✨ Features
Classic Gameplay: Control Pac-Man to eat all pellets and energizers while avoiding four distinct ghosts (red, blue, pink, orange).
Intelligent Ghost Behavior: Ghosts exhibit unique movement patterns and strategies. After being eaten, they temporarily pause before returning to the maze. They navigate smoothly, avoiding wall collisions and exiting their "home" area to pursue Pac-Man.
Dynamic Scoring:
Pellets: 10 points each.
Energizers: 50 points, activating a temporary ghost-eating mode.
Ghosts eaten during energizer mode: 200 points each.
Fruit: 100 bonus points per collection.
Engaging Game States:
An interactive start screen with instructions.
Win condition when all pellets are cleared.
Game over when all three lives are lost.
Smooth animations for Pac-Man’s death and game victory.
Crisp Visuals:
Custom sprites for Pac-Man, ghosts, pellets, energizers, and fruit, staying true to the arcade feel.
The maze visually transforms with alternating colors during the win animation.
Uses the classic OCR-A font for an authentic retro look (with a fallback to Courier New).
💻 Requirements
Operating System: Windows
Development Environment: Visual Studio 2019 or later
.NET Framework: 4.7.2 or later
Dependencies: Built purely with Windows Forms for the GUI, no external libraries are required.
Assets: All necessary image files (e.g., Pac-Man sprites, ghost sprites, fruit) are located in the Images folder. The OCR-A font is optionally used for text rendering, falling back to Courier New if unavailable.
🚀 Installation
Clone the Repository:

Bash

git clone https://github.com/your-username/pacman-game.git
cd pacman-game
Open in Visual Studio:
Open the PacMan.sln file in Visual Studio.

Set Up Assets:
Ensure the Images folder is correctly placed within your project directory (e.g., PacMan/Images/). Verify that all image files (e.g., pacman_0.png, ghost_red.png) are included in the project and have their "Copy to Output Directory" property set to "Copy if newer".

Build and Run:

Build the solution by pressing Ctrl+Shift+B.
Run the application by pressing F5.
(Note: If the OCR-A font is not available on your system, the game will gracefully default to Courier New for text display.)

▶️ How to Play
Start the Game: Press any key on the initial start screen to begin your arcade adventure!
Controls:
Use the Arrow keys (Up, Down, Left, Right) to guide Pac-Man through the maze.
Objective:
Maneuver Pac-Man to eat all the small pellets (.) and large energizers (o) scattered throughout the maze to clear the level and win.
Avoid direct contact with ghosts unless you are in energizer mode (when ghosts turn blue and become edible!).
Collect any fruit that appears for valuable bonus points.
Game Mechanics:
Ghosts: Ghosts will pursue Pac-Man throughout the maze, strategically exiting their home area after a short delay. After being eaten, they return to their home, pause, and then re-enter the game.
Energizer Mode: Eating an energizer temporarily empowers Pac-Man, turning ghosts blue and allowing him to consume them for bonus points.
Lives: You start with 3 lives. Losing a life occurs when Pac-Man collides with a non-blue ghost.
Win/Lose: The game concludes when you either clear all pellets to win or lose all your lives, resulting in a game over.
🏗️ Project Structure
Form1.cs: Contains the core game logic, including board initialization and rendering, Pac-Man and ghost movement mechanics, collision detection, and scoring. Game updates are handled by a 20ms timer.
Images/: This folder holds all the sprite assets for the game, such as pacman_*.png, ghost_*.png, and fruit.png.
PacMan.sln: The primary Visual Studio solution file for the project.
⚙️ Technical Details (For Developers)
Game Loop: The game runs on a Timer with a 20ms interval. Pac-Man moves every 2 ticks, while ghosts move every 5 ticks (or every 3 ticks during energizer chase mode).
Board Representation: The game board is a 36x28 grid, defined as a char[][], using symbols like # (wall), . (pellet), o (energizer), - (door), 0 (Pac-Man), and 1–4 (ghosts).
Rendering: Game elements are rendered using System.Drawing for sprites and text. Board tiles are 20x20 pixels.
Animations: Pac-Man has 4 animation frames, ghosts have 2, and fruit has 2 frames for visual variety.
⚠️ Known Issues
Ghost Movement Oddities: In rare cases, ghosts might exhibit unusual lingering at specific maze intersections. Review the MoveGhost method's post-exit logic if this occurs.
Delay Timing: If the ghost re-spawn delay feels inconsistent, verify the gameTimer.Interval is set to 20ms and check for system performance issues.
Game Smoothness: While efforts have been made for smooth gameplay, performance can vary based on system specifications and background processes. Occasional minor stutters might occur on less powerful machines.
Font Fallback: Text rendering might appear slightly different if the OCR-A font is not installed, as it will default to Courier New.
