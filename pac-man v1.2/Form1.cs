using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System;

namespace pac_man_v1._2
{
    public partial class Form1 : Form
    {
        // Pac-Man board definition
        private char[][] board;
        private char[][] initialBoardState;

        // Scores
        private int score = 0;
        private int highScore = 0;
        private const string HighScoreFileName = "highscore.txt";

        // Game state
        private bool gameStarted = false;
        private bool gameOver = false;
        private bool gameWon = false;
        private int pelletsRemaining;
        private int lives = 3;

        // Image assets
        private Image mazeImage;
        private Image pacManImage;
        private Image pelletImage;
        private Image energizerImage;
        private Image fruitImage;
        private Image ghostBodyImage;
        private Image ghostEyesImage;
        private Image doorImage;
        private Image livesImage;

        // Animation frame counts
        private const int LivingPacFrames = 3;
        private const int DeathPacFrames = 10;
        private const int GhostBodyFrames = 6;
        private const int GhostEyeFrames = 5;
        private const int FruitFrames = 8;

        // Animation state
        private int pacManFrameIndex = 0;
        private bool isPacManAlive = true;
        private int ghostBodyFrameIndex = 0;
        private int fruitFrameIndex = 0;
        private int frameCounter = 0;
        private const int FramesPerAnimationUpdate = 10;
        private int winAnimationTimer = 0;
        private int deathAnimationTimer = 0;
        private const int WinAnimationDuration = 100;
        private const int DeathAnimationDuration = 50;
        private bool alternateMazeColor = false;

        // Movement and game state
        private Point pacManPosition;
        private int pacManDirection = 0; // 0: right, 1: up, 2: left, 3: down
        private int requestedDirection = 0;
        private bool energizerActive = false;
        private int energizerTimer = 0;
        private const int EnergizerDuration = 250;

        // Movement timing
        private int moveCounter = 0;
        private const int FramesPerPacMove = 3;
        private const int FramesPerGhostMove = 5;
        private const int FramesPerGhostChase = 3;

        // Font for score display
        private readonly Font scoreFont = new Font("OCR-A", 14, FontStyle.Bold);

        // Fruit spawning
        private List<Point> pelletPositions = new List<Point>();
        private Point? currentFruitPosition = null;
        private Random random = new Random();

        // Ghost states
        private Dictionary<char, Point> ghostInitialPositions = new Dictionary<char, Point>();
        private Dictionary<char, Point> ghostCurrentPositions = new Dictionary<char, Point>();
        private Dictionary<char, bool> ghostEatenStates = new Dictionary<char, bool>
        {
            { '1', false }, { '2', false }, { '3', false }, { '4', false }
        };
        private Dictionary<char, int> ghostDirections = new Dictionary<char, int>
        {
            { '1', 0 }, { '2', 0 }, { '3', 0 }, { '4', 0 }
        };
        private Dictionary<char, Color> ghostColors = new Dictionary<char, Color>
        {
            { '1', Color.Red }, { '2', Color.Blue }, { '3', Color.Pink }, { '4', Color.Orange }
        };
        private Dictionary<char, int> ghostExitCounters = new Dictionary<char, int>
        {
            { '1', 0 }, { '2', 0 }, { '3', 0 }, { '4', 0 }
        };
        private Dictionary<char, int> ghostExitDelays = new Dictionary<char, int>
        {
            { '1', 0 }, { '2', 50 }, { '3', 100 }, { '4', 150 }
        };
        private const int GhostHouseDoorY = 15;

        // Component container
        private IContainer components = null;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            int cellSize = 24;
            LoadImages(cellSize);

            string[] tempBoard = new string[]
            {
                "                            ",
                "                            ",
                "                            ",
                "############################",
                "#............##............#",
                "#.####.#####.##.#####.####.#",
                "#o####.#####.##.#####.####o#",
                "#.####.#####.##.#####.####.#",
                "#..........................#",
                "#.####.##.########.##.####.#",
                "#.####.##.########.##.####.#",
                "#......##....##....##......#",
                "######.##### ## #####.######",
                "     #.##### ## #####.#     ",
                "     #.##   1      ##.#     ",
                "     #.## ###--### ##.#     ",
                "######.## #      # ##.######",
                "        . #2 3 4 # .        ",
                "######.## #      # ##.######",
                "     #.## ######## ##.#     ",
                "     #.##          ##.#     ",
                "     #.## ######## ##.#     ",
                "######.## ######## ##.######",
                "#............##............#",
                "#.####.#####.##.#####.####.#",
                "#.####.#####.##.#####.####.#",
                "#o..##.......0 .......##..o#",
                "###.##.##.########.##.##.###",
                "###.##.##.########.##.##.###",
                "#......##....##....##......#",
                "#.##########.##.##########.#",
                "#.##########.##.##########.#",
                "#..........................#",
                "############################",
                "                            ",
                "                            "
            };
            board = new char[tempBoard.Length][];
            initialBoardState = new char[tempBoard.Length][];
            for (int i = 0; i < tempBoard.Length; i++)
            {
                board[i] = tempBoard[i].ToCharArray();
                initialBoardState[i] = tempBoard[i].ToCharArray();
            }

            LoadHighScore();
            SetupStartScreen();

            if (mazeImage != null)
            {
                this.ClientSize = new Size(mazeImage.Width, mazeImage.Height + 50);
            }
            else
            {
                int defaultWidth = board[0].Length * cellSize;
                int defaultHeight = 31 * cellSize + 50;
                this.ClientSize = new Size(defaultWidth, defaultHeight);
            }

            this.BackColor = Color.Black;
        }

        private void SetupStartScreen()
        {
            gameTimer.Stop();

            Button startGameButton = new Button
            {
                Text = "START",
                Font = new Font("OCR-A", 24, FontStyle.Bold),
                AutoSize = true
            };
            startGameButton.Location = new Point((this.ClientSize.Width - startGameButton.Width) / 2,
                                                (this.ClientSize.Height - startGameButton.Height) / 2);
            startGameButton.Click += StartGameButton_Click;
            this.Controls.Add(startGameButton);

            Label titleLabel = new Label
            {
                Text = "PAC-MAN",
                Font = new Font("OCR-A", 48, FontStyle.Bold),
                ForeColor = Color.Yellow,
                AutoSize = true
            };
            titleLabel.Location = new Point((this.ClientSize.Width - titleLabel.Width) / 2,
                                           startGameButton.Top - titleLabel.Height - 30);
            this.Controls.Add(titleLabel);

            Label instructionsLabel = new Label
            {
                Text = "Use Arrow Keys to Move Pac-Man",
                Font = new Font("OCR-A", 14),
                ForeColor = Color.White,
                AutoSize = true
            };
            instructionsLabel.Location = new Point((this.ClientSize.Width - instructionsLabel.Width) / 2,
                                                  startGameButton.Bottom + 20);
            this.Controls.Add(instructionsLabel);
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls.Cast<Control>().ToList())
            {
                this.Controls.Remove(control);
                control.Dispose();
            }

            ResetGame();
            gameStarted = true;
            gameTimer.Start();
        }



        private void ResetGame()
        {
            for (int i = 0; i < initialBoardState.Length; i++)
            {
                board[i] = (char[])initialBoardState[i].Clone();
            }

            score = 0;
            lives = 3;
            isPacManAlive = true;
            energizerActive = false;
            energizerTimer = 0;
            pacManFrameIndex = 0;
            ghostBodyFrameIndex = 0;
            fruitFrameIndex = 0;
            frameCounter = 0;
            moveCounter = 0;
            gameWon = false;
            gameOver = false;
            alternateMazeColor = false;
            winAnimationTimer = 0;
            deathAnimationTimer = 0;

            pelletPositions.Clear();
            ghostInitialPositions.Clear();
            ghostCurrentPositions.Clear();
            foreach (var key in ghostEatenStates.Keys.ToList())
            {
                ghostEatenStates[key] = false;
                ghostDirections[key] = 0;
                ghostExitCounters[key] = 0;
            }

            currentFruitPosition = null;

            InitializeGameElements();
            SpawnFruit();
        }

        private void InitializeGameElements()
        {
            pelletsRemaining = 0;
            for (int y = 0; y < board.Length; y++)
            {
                for (int x = 0; x < board[y].Length; x++)
                {
                    char c = board[y][x];
                    if (c == '.' || c == 'o')
                    {
                        pelletPositions.Add(new Point(x, y));
                        pelletsRemaining++;
                    }
                    else if (c == '0')
                    {
                        pacManPosition = new Point(x, y);
                    }
                    else if (c >= '1' && c <= '4')
                    {
                        ghostInitialPositions[c] = new Point(x, y);
                        ghostCurrentPositions[c] = new Point(x, y);
                    }
                }
            }
        }

        private void SpawnFruit()
        {
            if (pelletPositions.Count > 0 && !currentFruitPosition.HasValue)
            {
                List<Point> possibleFruitPositions = new List<Point>();
                for (int y = 0; y < board.Length; y++)
                {
                    for (int x = 0; x < board[y].Length; x++)
                    {
                        if (board[y][x] == ' ' && !ghostCurrentPositions.ContainsValue(new Point(x, y)) && pacManPosition != new Point(x, y))
                        {
                            possibleFruitPositions.Add(new Point(x, y));
                        }
                    }
                }

                if (possibleFruitPositions.Count > 0)
                {
                    currentFruitPosition = possibleFruitPositions[random.Next(possibleFruitPositions.Count)];
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted || gameOver || gameWon || !isPacManAlive) return;

            switch (e.KeyCode)
            {
                case Keys.Up: requestedDirection = 1; break;
                case Keys.Left: requestedDirection = 2; break;
                case Keys.Right: requestedDirection = 0; break;
                case Keys.Down: requestedDirection = 3; break;
            }
        }

        private bool CanMove(Point position, int direction)
        {
            int newX = position.X;
            int newY = position.Y;

            switch (direction)
            {
                case 0: newX++; break;
                case 1: newY--; break;
                case 2: newX--; break;
                case 3: newY++; break;
            }

            if (newX < 0) newX = board[0].Length - 1;
            else if (newX >= board[0].Length) newX = 0;

            if (newY < 0 || newY >= board.Length)
                return false;

            char targetChar = board[newY][newX];
            if (targetChar == '#') return false;
            if (targetChar == '-' && position == pacManPosition) return false;

            return true;
        }

        private Dictionary<char, int> ghostSpawnDelayCounters = new Dictionary<char, int>
        {
            { '1', 0 }, { '2', 0 }, { '3', 0 }, { '4', 0 }
        };

        private void MoveGhost(char ghostId)
        {
            Point currentPos = ghostCurrentPositions[ghostId];
            int direction = ghostDirections[ghostId];

            // Ghost Exit Logic
            if (ghostExitCounters[ghostId] < ghostExitDelays[ghostId])
            {
                ghostExitCounters[ghostId]++;
                if (currentPos.Y > GhostHouseDoorY)
                {
                    if (CanMove(currentPos, 1))
                    {
                        ghostDirections[ghostId] = 1;
                        currentPos.Y--;
                        ghostCurrentPositions[ghostId] = currentPos;
                        return;
                    }
                    List<int> horizontalDirections = new List<int> { 0, 2 }; // Right, Left
                    List<int> possibleHorizontal = horizontalDirections.Where(d => CanMove(currentPos, d)).ToList();
                    if (possibleHorizontal.Count > 0)
                    {
                        direction = possibleHorizontal[random.Next(possibleHorizontal.Count)];
                        int horizX = currentPos.X;
                        switch (direction)
                        {
                            case 0: horizX++; break;
                            case 2: horizX--; break;
                        }
                        ghostDirections[ghostId] = direction;
                        ghostCurrentPositions[ghostId] = new Point(horizX, currentPos.Y);
                        return;
                    }
                }
                else if (currentPos.Y == GhostHouseDoorY && board[currentPos.Y][currentPos.X] == '-')
                {
                    if (CanMove(currentPos, 1))
                    {
                        ghostDirections[ghostId] = 1;
                        currentPos.Y--;
                        ghostCurrentPositions[ghostId] = currentPos;
                        return;
                    }
                }
                // Fallback: If stuck in spawn, try any valid move
                List<int> anyDirections = new List<int> { 0, 1, 2, 3 }.Where(d => CanMove(currentPos, d)).ToList();
                if (anyDirections.Count > 0)
                {
                    direction = anyDirections[random.Next(anyDirections.Count)];
                    int fallbackX = currentPos.X;
                    int fallbackY = currentPos.Y;
                    switch (direction)
                    {
                        case 0: fallbackX++; break;
                        case 1: fallbackY--; break;
                        case 2: fallbackX--; break;
                        case 3: fallbackY++; break;
                    }
                    ghostDirections[ghostId] = direction;
                    ghostCurrentPositions[ghostId] = new Point(fallbackX, fallbackY);
                    return;
                }
                return;
            }

            // Post-Exit Logic: At Y=14, move to X=10 (left) or X=18 (right)
            if (currentPos.Y == 14)
            {
                int targetX = currentPos.X < 14 ? 10 : 18; // Move left if X<14, right if X>=14
                int newDirection = currentPos.X < targetX ? 0 : 2; // Right (0) or Left (2)
                if (CanMove(currentPos, newDirection))
                {
                    int postExitX = currentPos.X + (newDirection == 0 ? 1 : -1);
                    ghostDirections[ghostId] = newDirection;
                    ghostCurrentPositions[ghostId] = new Point(postExitX, currentPos.Y);
                    return;
                }
                // Fallback: Try any valid move to avoid being stuck
                List<int> anyDirections = new List<int> { 0, 1, 2, 3 }.Where(d => CanMove(currentPos, d)).ToList();
                if (anyDirections.Count > 0)
                {
                    direction = anyDirections[random.Next(anyDirections.Count)];
                    int fallbackX = currentPos.X;
                    int fallbackY = currentPos.Y;
                    switch (direction)
                    {
                        case 0: fallbackX++; break;
                        case 1: fallbackY--; break;
                        case 2: fallbackX--; break;
                        case 3: fallbackY++; break;
                    }
                    ghostDirections[ghostId] = direction;
                    ghostCurrentPositions[ghostId] = new Point(fallbackX, fallbackY);
                    return;
                }
            }

            // Normal Ghost Movement
            List<int> possibleDirections = new List<int>();
            for (int d = 0; d < 4; d++)
            {
                if (CanMove(currentPos, d))
                {
                    possibleDirections.Add(d);
                }
            }

            if (possibleDirections.Count > 0)
            {
                // Prioritize current direction if valid
                if (possibleDirections.Contains(direction))
                {
                    // Continue in current direction
                }
                else
                {
                    // Choose a random non-reverse direction if possible
                    int reverseDirection = (direction + 2) % 4;
                    List<int> preferredDirections = possibleDirections.Where(d => d != reverseDirection).ToList();
                    direction = preferredDirections.Count > 0
                        ? preferredDirections[random.Next(preferredDirections.Count)]
                        : possibleDirections[random.Next(possibleDirections.Count)];
                }

                int nextX = currentPos.X;
                int nextY = currentPos.Y;

                switch (direction)
                {
                    case 0: nextX++; break;
                    case 1: nextY--; break;
                    case 2: nextX--; break;
                    case 3: nextY++; break;
                }

                if (nextX < 0) nextX = board[0].Length - 1;
                else if (nextX >= board[0].Length) nextX = 0;

                ghostCurrentPositions[ghostId] = new Point(nextX, nextY);
                ghostDirections[ghostId] = direction;
            }
        }
        private void MoveGhostToHouse(char ghostId)
        {
            Point currentPos = ghostCurrentPositions[ghostId];
            Point targetPos = ghostId == '1' ? new Point(12, 17) : ghostInitialPositions[ghostId]; // Red ghost to (12, 17)

            if (currentPos == targetPos)
            {
                ghostEatenStates[ghostId] = false;
                ghostExitCounters[ghostId] = 0; // Reset to trigger exit after delay
                ghostDirections[ghostId] = 1; // Start moving up to exit
                ghostSpawnDelayCounters[ghostId] = 150; // 3 seconds at 20ms per tick
                return;
            }

            List<int> possibleDirections = new List<int>();
            if (currentPos.X < targetPos.X && CanMove(currentPos, 0)) possibleDirections.Add(0);
            if (currentPos.X > targetPos.X && CanMove(currentPos, 2)) possibleDirections.Add(2);
            if (currentPos.Y < targetPos.Y && CanMove(currentPos, 3)) possibleDirections.Add(3);
            if (currentPos.Y > targetPos.Y && CanMove(currentPos, 1)) possibleDirections.Add(1);

            if (possibleDirections.Count == 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    if (CanMove(currentPos, d)) possibleDirections.Add(d);
                }
            }

            if (possibleDirections.Count > 0)
            {
                int direction = possibleDirections[random.Next(possibleDirections.Count)];
                int moveX = currentPos.X;
                int moveY = currentPos.Y;

                switch (direction)
                {
                    case 0: moveX++; break;
                    case 1: moveY--; break;
                    case 2: moveX--; break;
                    case 3: moveY++; break;
                }

                if (moveX < 0) moveX = board[0].Length - 1;
                else if (moveX >= board[0].Length) moveX = 0;

                ghostCurrentPositions[ghostId] = new Point(moveX, moveY);
                ghostDirections[ghostId] = direction;
            }
        }
        private void LoadImages(int cellSize)
        {
            try
            {
                string imageFolderPath = Path.Combine(Application.StartupPath, "Images");
                mazeImage = Image.FromFile(Path.Combine(imageFolderPath, "Map24.png"));
                pacManImage = Image.FromFile(Path.Combine(imageFolderPath, "PacMan32.png"));
                pelletImage = Image.FromFile(Path.Combine(imageFolderPath, "Pellet24.png"));
                energizerImage = Image.FromFile(Path.Combine(imageFolderPath, "Energizer24.png"));
                fruitImage = Image.FromFile(Path.Combine(imageFolderPath, "Fruit32.png"));
                ghostBodyImage = Image.FromFile(Path.Combine(imageFolderPath, "GhostBody32.png"));
                ghostEyesImage = Image.FromFile(Path.Combine(imageFolderPath, "GhostEyes32.png"));
                doorImage = Image.FromFile(Path.Combine(imageFolderPath, "Door.png"));
                livesImage = Image.FromFile(Path.Combine(imageFolderPath, "Lives32.png"));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}\nMake sure images are in the 'Images' folder.", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mazeImage = pacManImage = pelletImage = energizerImage = fruitImage = ghostBodyImage = ghostEyesImage = doorImage = livesImage = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error loading images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image TintImage(Image sourceImage, Color tintColor)
        {
            Bitmap tintedImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            using (Graphics graphics = Graphics.FromImage(tintedImage))
            {
                float r = tintColor.R / 255f;
                float g = tintColor.G / 255f;
                float b = tintColor.B / 255f;
                System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
                {
                    new float[] { r, 0, 0, 0, 0 },
                    new float[] { 0, g, 0, 0, 0 },
                    new float[] { 0, 0, b, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                System.Drawing.Imaging.ImageAttributes imageAttributes = new System.Drawing.Imaging.ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);

                graphics.DrawImage(sourceImage,
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    0, 0, sourceImage.Width, sourceImage.Height,
                    GraphicsUnit.Pixel, imageAttributes);
            }
            return tintedImage;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int cellSize = 24;
            int additionalYOffset = 72;

            Rectangle gameArea = new Rectangle(0, 50, this.ClientSize.Width, this.ClientSize.Height - 50);
            g.SetClip(gameArea);

            if (mazeImage != null)
            {
                int mazeOffsetY = 50;
                if (gameWon && winAnimationTimer > 0)
                {
                    Color mazeTint = alternateMazeColor ? Color.White : Color.DarkBlue;
                    using (Image tintedMaze = TintImage(mazeImage, mazeTint))
                    {
                        g.DrawImage(tintedMaze, 0, mazeOffsetY, mazeImage.Width, mazeImage.Height);
                    }
                }
                else
                {
                    g.DrawImage(mazeImage, 0, mazeOffsetY, mazeImage.Width, mazeImage.Height);
                }
            }

            if (gameStarted)
            {
                for (int y = 3; y < board.Length - 3; y++)
                {
                    for (int x = 0; x < board[y].Length; x++)
                    {
                        char c = board[y][x];
                        int px = x * cellSize;
                        int py = (y - 3) * cellSize + 50 + additionalYOffset;

                        bool isFruitPosition = currentFruitPosition.HasValue && currentFruitPosition.Value.X == x && currentFruitPosition.Value.Y == y;

                        switch (c)
                        {
                            case '.':
                                if (pelletImage != null)
                                {
                                    g.DrawImage(pelletImage, px + (cellSize - pelletImage.Width) / 2, py + (cellSize - pelletImage.Height) / 2);
                                }
                                else
                                {
                                    g.FillEllipse(Brushes.Yellow, px + cellSize / 3, py + cellSize / 3, cellSize / 3, cellSize / 3);
                                }
                                break;

                            case 'o':
                                if (energizerImage != null)
                                {
                                    g.DrawImage(energizerImage, px + (cellSize - energizerImage.Width) / 2, py + (cellSize - energizerImage.Height) / 2);
                                }
                                else
                                {
                                    g.FillEllipse(Brushes.Orange, px + cellSize / 4, py + cellSize / 4, cellSize / 2, cellSize / 2);
                                }
                                break;

                            case '-':
                                if (doorImage != null)
                                {
                                    g.DrawImage(doorImage, px + (cellSize - doorImage.Width) / 2, py + (cellSize - doorImage.Height) / 2);
                                }
                                break;
                        }

                        if (isFruitPosition && fruitImage != null)
                        {
                            int frameWidth = 32;
                            int frameHeight = 32;
                            int frameX = (fruitFrameIndex % FruitFrames) * frameWidth;
                            g.DrawImage(fruitImage,
                                px + (cellSize - frameWidth) / 2, py + (cellSize - frameHeight) / 2,
                                new Rectangle(frameX, 0, frameWidth, frameHeight),
                                GraphicsUnit.Pixel);
                        }
                    }
                }

                int pacPx = pacManPosition.X * cellSize;
                int pacPy = (pacManPosition.Y - 3) * cellSize + 50 + additionalYOffset;
                if (pacManImage != null && (isPacManAlive || deathAnimationTimer > 0))
                {
                    int frameCount = isPacManAlive ? LivingPacFrames : DeathPacFrames;
                    int frameWidth = 32;
                    int frameHeight = 32;
                    int frameX = isPacManAlive ? (pacManFrameIndex % frameCount) * frameWidth :
                        (int)((1 - (float)deathAnimationTimer / DeathAnimationDuration) * DeathPacFrames) * frameWidth;
                    float angle = pacManDirection switch
                    {
                        0 => 0f,
                        1 => 270f,
                        2 => 180f,
                        3 => 90f,
                        _ => 0f
                    };

                    g.TranslateTransform(pacPx + cellSize / 2, pacPy + cellSize / 2);
                    g.RotateTransform(angle);
                    g.DrawImage(pacManImage,
                        -(frameWidth / 2), -(frameHeight / 2),
                        new Rectangle(frameX, 0, frameWidth, frameHeight),
                        GraphicsUnit.Pixel);
                    g.ResetTransform();
                }

                foreach (char ghostId in ghostCurrentPositions.Keys)
                {
                    Point ghostPos = ghostCurrentPositions[ghostId];
                    int px = ghostPos.X * cellSize;
                    int py = (ghostPos.Y - 3) * cellSize + 50 + additionalYOffset;
                    int eyeFrameIndex = energizerActive && !ghostEatenStates[ghostId] ? 4 : ghostDirections[ghostId];

                    if (ghostBodyImage != null && ghostEyesImage != null)
                    {
                        int bodyFrameWidth = 32;
                        int bodyFrameHeight = 32;
                        int bodyFrameX = (ghostBodyFrameIndex % GhostBodyFrames) * bodyFrameWidth;

                        Color tintColor = ghostColors[ghostId];
                        if (energizerActive && !ghostEatenStates[ghostId])
                        {
                            tintColor = Color.DarkBlue;
                            if (energizerTimer < EnergizerDuration / 4 && (energizerTimer / (FramesPerAnimationUpdate * 2)) % 2 == 0)
                            {
                                tintColor = Color.White;
                            }
                        }
                        if (ghostEatenStates[ghostId])
                        {
                            tintColor = Color.Transparent;
                        }

                        using (Image coloredBody = TintImage(ghostBodyImage, tintColor))
                        {
                            g.DrawImage(coloredBody,
                                px + (cellSize - bodyFrameWidth) / 2, py + (cellSize - bodyFrameHeight) / 2,
                                new Rectangle(bodyFrameX, 0, bodyFrameWidth, bodyFrameHeight),
                                GraphicsUnit.Pixel);
                        }

                        int eyesFrameWidth = 32;
                        int eyesFrameHeight = 32;
                        int eyesFrameX = eyeFrameIndex * eyesFrameWidth;
                        g.DrawImage(ghostEyesImage,
                            px + (cellSize - eyesFrameWidth) / 2, py + (cellSize - eyesFrameHeight) / 2,
                            new Rectangle(eyesFrameX, 0, eyesFrameWidth, eyesFrameHeight),
                            GraphicsUnit.Pixel);
                    }
                }
            }

            g.ResetClip();
            g.DrawString("SCORE", scoreFont, Brushes.Yellow, 10, 5);
            g.DrawString(score.ToString(), scoreFont, Brushes.Yellow, 10, 25);
            g.DrawString("HIGH SCORE", scoreFont, Brushes.Yellow, this.ClientSize.Width / 2 - 50, 5);
            g.DrawString(highScore.ToString(), scoreFont, Brushes.Yellow, this.ClientSize.Width / 2 - 30, 25);

            if (livesImage != null)
            {
                int heartSize = 32;
                for (int i = 0; i < lives; i++)
                {
                    g.DrawImage(livesImage, 10 + i * (heartSize + 5), this.ClientSize.Height - heartSize - 10, heartSize, heartSize);
                }
            }
        }

        private void InitializeComponent()
        {
            components = new Container();
            gameTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();

            gameTimer.Enabled = true;
            gameTimer.Interval = 20;
            gameTimer.Tick += gameTimer_Tick;

            this.Text = "Pac-Man Game";
            this.Name = "Form1";
            ResumeLayout(false);
        }

        private System.Windows.Forms.Timer gameTimer;

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted)
            {
                Invalidate();
                return;
            }

            if (gameWon)
            {
                winAnimationTimer--;
                if (winAnimationTimer <= 0)
                {
                    gameTimer.Stop();
                    MessageBox.Show("You Won! Final Score: " + score);
                    UpdateHighScore();
                    SetupStartScreen();
                }
                else if (winAnimationTimer % 5 == 0)
                {
                    alternateMazeColor = !alternateMazeColor;
                }
                Invalidate();
                return;
            }

            if (!isPacManAlive)
            {
                deathAnimationTimer--;
                if (deathAnimationTimer <= 0)
                {
                    lives--;
                    if (lives <= 0)
                    {
                        gameOver = true;
                        gameTimer.Stop();
                        MessageBox.Show("Game Over! Final Score: " + score);
                        UpdateHighScore();
                        SetupStartScreen();
                    }
                    else
                    {
                        isPacManAlive = true;
                        pacManPosition = new Point(14, 26);
                        pacManDirection = requestedDirection = 0;
                        foreach (char ghostId in ghostInitialPositions.Keys)
                        {
                            ghostCurrentPositions[ghostId] = ghostInitialPositions[ghostId];
                            ghostEatenStates[ghostId] = false;
                            ghostDirections[ghostId] = 0;
                            ghostExitCounters[ghostId] = 0;
                            ghostSpawnDelayCounters[ghostId] = 0; // Reset delay counters
                        }
                        energizerActive = false;
                        energizerTimer = 0;
                    }
                }
                Invalidate();
                return;
            }

            // Decrement spawn delay counters every tick
            foreach (char ghostId in ghostSpawnDelayCounters.Keys.ToList())
            {
                if (ghostSpawnDelayCounters[ghostId] > 0)
                {
                    ghostSpawnDelayCounters[ghostId]--;
                }
            }

            if (energizerActive)
            {
                energizerTimer--;
                if (energizerTimer <= 0)
                {
                    energizerActive = false;
                    foreach (var ghostId in ghostEatenStates.Keys)
                    {
                        ghostEatenStates[ghostId] = false;
                    }
                }
            }

            moveCounter++;
            bool shouldMovePac = moveCounter % FramesPerPacMove == 0;
            bool shouldMoveGhosts = moveCounter % (energizerActive ? FramesPerGhostChase : FramesPerGhostMove) == 0;

            if (shouldMovePac)
            {
                if (CanMove(pacManPosition, requestedDirection))
                {
                    pacManDirection = requestedDirection;
                }
                if (CanMove(pacManPosition, pacManDirection))
                {
                    int pacNewX = pacManPosition.X;
                    int pacNewY = pacManPosition.Y;
                    switch (pacManDirection)
                    {
                        case 0: pacNewX++; break;
                        case 1: pacNewY--; break;
                        case 2: pacNewX--; break;
                        case 3: pacNewY++; break;
                    }

                    if (pacNewX < 0) pacNewX = board[0].Length - 1;
                    else if (pacNewX >= board[0].Length) pacNewX = 0;

                    pacManPosition = new Point(pacNewX, pacNewY);

                    char currentTile = board[pacNewY][pacNewX];
                    if (currentTile == '.')
                    {
                        score += 10;
                        board[pacNewY][pacNewX] = ' ';
                        pelletPositions.Remove(new Point(pacNewX, pacNewY));
                        pelletsRemaining--;
                    }
                    else if (currentTile == 'o')
                    {
                        score += 50;
                        board[pacNewY][pacNewX] = ' ';
                        pelletPositions.Remove(new Point(pacNewX, pacNewY));
                        pelletsRemaining--;
                        energizerActive = true;
                        energizerTimer = EnergizerDuration;
                    }
                    else if (currentFruitPosition.HasValue && currentFruitPosition.Value.X == pacNewX && currentFruitPosition.Value.Y == pacNewY)
                    {
                        score += 100;
                        currentFruitPosition = null;
                        SpawnFruit();
                    }
                }
            }

            if (shouldMoveGhosts)
            {
                foreach (char ghostId in ghostCurrentPositions.Keys.ToList())
                {
                    // Skip movement if ghost is in spawn delay
                    if (ghostSpawnDelayCounters[ghostId] > 0) continue;

                    if (ghostEatenStates[ghostId])
                    {
                        MoveGhostToHouse(ghostId);
                    }
                    else
                    {
                        MoveGhost(ghostId);
                    }

                    if (isPacManAlive && pacManPosition == ghostCurrentPositions[ghostId])
                    {
                        if (energizerActive && !ghostEatenStates[ghostId])
                        {
                            score += 200;
                            ghostEatenStates[ghostId] = true;
                        }
                        else if (!ghostEatenStates[ghostId])
                        {
                            isPacManAlive = false;
                            deathAnimationTimer = DeathAnimationDuration;
                        }
                    }
                }
            }

            frameCounter++;
            if (frameCounter >= FramesPerAnimationUpdate)
            {
                if (isPacManAlive)
                {
                    pacManFrameIndex = (pacManFrameIndex + 1) % LivingPacFrames;
                }
                ghostBodyFrameIndex = (ghostBodyFrameIndex + 1) % GhostBodyFrames;
                fruitFrameIndex = (fruitFrameIndex + 1) % FruitFrames;
                frameCounter = 0;
            }

            if (pelletsRemaining <= 0 && !gameWon)
            {
                gameWon = true;
                winAnimationTimer = WinAnimationDuration;
            }

            Invalidate();
        }

        private void LoadHighScore()
        {
            if (File.Exists(HighScoreFileName))
            {
                try
                {
                    highScore = int.Parse(File.ReadAllText(HighScoreFileName));
                }
                catch (FormatException)
                {
                    highScore = 0;
                }
            }
        }

        private void UpdateHighScore()
        {
            if (score > highScore)
            {
                highScore = score;
                File.WriteAllText(HighScoreFileName, highScore.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                mazeImage?.Dispose();
                pacManImage?.Dispose();
                pelletImage?.Dispose();
                energizerImage?.Dispose();
                fruitImage?.Dispose();
                ghostBodyImage?.Dispose();
                ghostEyesImage?.Dispose();
                doorImage?.Dispose();
                livesImage?.Dispose();
                scoreFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}