using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Destination
{
    public partial class MainForm : Form
    {
        public static Timer timer { get; set; }
        public static Random random { get; set; }
        public static bool isGameOver { get; set; }
        public static float totalTimeElapsed { get; set; }
        public const int HORIZON_Y = 300;
        public static Size display { get; set; }
        public static int nextId = 1;
        public static Color black = Color.FromArgb(0, 0, 0);
        public static Color skyPurple = Color.FromArgb(101, 13, 115);
        public static Color gridLightBlue = Color.FromArgb(99, 181, 254);
        public static Color groundBlue = Color.FromArgb(34, 107, 210);
        public static Color groundDarkBlue = Color.FromArgb(25, 7, 69);
        public static Color powerupLightPurple = Color.FromArgb(236, 203, 222);
        public static Color mountainPurple = Color.FromArgb(75, 35, 98);
        public static Color mountainDarkPurple = Color.FromArgb(25, 1, 61);
        public const float GRID_LINE_THICKNESS = 2f;
        public static Player player { get; set; }
        public static Hud hud { get; set; }
        public static LinkedList<MapLine> allMapLines = new LinkedList<MapLine>();
        public static LinkedList<SideLine> allSideLines = new LinkedList<SideLine>();
        public static LinkedList<Powerup> allPowerups = new LinkedList<Powerup>();
        public static LinkedList<Obstacle> allObstacles = new LinkedList<Obstacle>();
        public static Lane[] drivingLanes = new Lane[6];

        public struct Lane
        {
            public float startX, endX;

            public Lane(float startX, float endX)
            {
                this.startX = startX;
                this.endX = endX;
            }

            public float GetRatio()
            {
                float ratio = (endX - startX) / (float)(display.Height - HORIZON_Y);
                return ratio;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            Start();
        }

        public void Start()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(Refresh);
            timer.Interval = 50;
            timer.Start();

            isGameOver = false;

            random = new Random();

            display = Display.Size;

            player = new Player(skyPurple, skyPurple, gridLightBlue, new SizeF(120f, 66f), display.Width / 2, new Tuple<float, float>(75, display.Width - 75));
            hud = new Hud(player, gridLightBlue, skyPurple);

            drivingLanes[0] = new Lane(16, 138 * 3.5f);
            drivingLanes[1] = new Lane(8, 138 * 2);
            drivingLanes[2] = new Lane(4, 138);
            drivingLanes[3] = new Lane(-4, -18);
            drivingLanes[4] = new Lane(-8, -168);
            drivingLanes[5] = new Lane(-16, -328);
        }

        protected void Refresh(object sender, EventArgs e)
        {
            Display.Refresh();
            //Display.Invalidate();
        }

        protected void Update(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int updateInterval = timer.Interval;
            float elapsedTime = updateInterval / 100f;
            totalTimeElapsed += elapsedTime;

            // Fill background with black
            graphics.FillRectangle(new SolidBrush(groundDarkBlue), 0, 0, display.Width, display.Height);

            DrawTerrain(graphics);

            UpdateMapLines(graphics, elapsedTime);

            SpawnMapLines();
            SpawnSideLines();
            SpawnPowerups();
            SpawnObstacles();

            UpdateSideLines(graphics, elapsedTime);
            UpdatePowerups(graphics, elapsedTime);
            UpdateObstacles(graphics, elapsedTime);

            DrawSky(graphics);

            if (isGameOver == true)
            {
                hud.DrawGameOver(graphics, display);
                timer.Stop();
                return;
            }

            player.Update(graphics, display, elapsedTime);

            hud.Update(graphics, display, elapsedTime);
        }

        public static int GetNextId()
        {
            int id = nextId;
            nextId++;
            return id;
        }

        public static float GetDeltaDrawY(float distance)
        {
            float deltaY = 1800 * Player.speed / (float)Math.Pow(distance - 5, 2);

            return deltaY;
        }

        protected void SpawnSideLines()
        {
            int randomInt = random.Next(1, 10);
            if (totalTimeElapsed % randomInt != 0)
            {
                return;
            }

            float randomModifier = random.Next(10, 40) / 10f;
            float randomLength = random.Next(200, 1000) / 10f;

            int startingDistance = display.Height - HORIZON_Y;

            if (totalTimeElapsed % 2 == 0)
            {
                float ratio = 2.791489362f * randomModifier;
                allSideLines.AddLast(new SideLine(gridLightBlue, startingDistance, display.Width / 2 - 23 * randomModifier, HORIZON_Y, randomLength, ratio));
            } else
            {
                float ratio = -2.791489362f * randomModifier;
                allSideLines.AddLast(new SideLine(gridLightBlue, startingDistance, display.Width / 2 + 23 * randomModifier, HORIZON_Y, randomLength, ratio));
            }
        }

        protected static void SpawnMapLines()
        {
            if (totalTimeElapsed % 5 != 0)
            {
                return;
            }

            int startingDistance = display.Height - HORIZON_Y;

            if (totalTimeElapsed % 10 != 0)
            {
                float ratio = (138 * 2 - 8) / (float)(display.Height - HORIZON_Y);
                allMapLines.AddLast(new MapLine(gridLightBlue, startingDistance, HORIZON_Y, 8 * 2, ratio));
            }
            else
            {
                float ratio = (223 * 2 - 13) / (float)(display.Height - HORIZON_Y);
                allMapLines.AddLast(new MapLine(gridLightBlue, startingDistance, HORIZON_Y, 13 * 2, ratio));
            }
        }

        protected static void SpawnPowerups()
        {
            if (totalTimeElapsed % 48 * Player.speed != 0)
            {
                return;
            }

            int index = random.Next(0, drivingLanes.Length);
            Lane lane = drivingLanes[index];

            int startingDistance = display.Height - HORIZON_Y;
            float power = player.powerThreshold / Player.speed;
            float ratio = lane.GetRatio();

            int groupSize = random.Next(3, 5);
            for (int i = 0; i < groupSize; i++)
            {
                int startingRotation = random.Next(0, 90);
                allPowerups.AddLast(new Powerup(powerupLightPurple, startingDistance + i * 30, HORIZON_Y + 3, lane.startX, ratio, power, startingRotation));
            }
        }

        protected static void SpawnObstacles()
        {
            int difficulty = (int)(player.maxSpeed - Player.speed);
            int randomInt = random.Next(difficulty, difficulty * 6);
            if (totalTimeElapsed % randomInt != 0)
            {
                return;
            }

            int index = random.Next(0, drivingLanes.Length);
            Lane lane = drivingLanes[index];

            int startingDistance = display.Height - HORIZON_Y;
            float ratio = lane.GetRatio();

            float offset = random.Next(30, 70) / 100f;
            allObstacles.AddLast(new Obstacle(gridLightBlue, startingDistance, HORIZON_Y, lane.startX, ratio, offset));
        }

        protected static void UpdateSideLines(Graphics graphics, float elapsedTime)
        {
            LinkedListNode<SideLine> currentNode = allSideLines.First;
            while (currentNode != null)
            {
                SideLine line = currentNode.Value;
                line.Update(graphics, display, elapsedTime);
                LinkedListNode<SideLine> nextNode = currentNode.Next;

                if ((line.tail.X < 0 || line.tail.X > display.Width) && line.tail.Y > display.Height)
                {
                    allSideLines.Remove(line);
                }

                currentNode = nextNode;
            }
        }

        protected static void UpdateMapLines(Graphics graphics, float elapsedTime)
        {
            LinkedListNode<MapLine> currentNode = allMapLines.First;
            while (currentNode != null)
            {
                MapLine line = currentNode.Value;
                line.Update(graphics, display, elapsedTime);
                LinkedListNode<MapLine> nextNode = currentNode.Next;

                if ((line.drawPosition.X < 0 || line.drawPosition.X > display.Width) && line.drawPosition.Y > display.Height)
                {
                    allMapLines.Remove(line);
                }

                currentNode = nextNode;
            }
        }

        protected static void UpdatePowerups(Graphics graphics, float elapsedTime)
        {
            LinkedListNode<Powerup> currentNode = allPowerups.First;
            while (currentNode != null)
            {
                Powerup powerup = currentNode.Value;
                powerup.Update(graphics, display, elapsedTime);
                LinkedListNode<Powerup> nextNode = currentNode.Next;

                if ((powerup.drawPosition.X < 0 || powerup.drawPosition.X > display.Width) && powerup.drawPosition.Y > display.Height)
                {
                    allPowerups.Remove(powerup);
                }

                bool hasCollided = powerup.HasCollidedWithPlayer(player);
                if(hasCollided == true)
                {
                    player.AddPower(powerup.power);
                    allPowerups.Remove(powerup);
                }

                currentNode = nextNode;
            }
        }

        protected static void UpdateObstacles(Graphics graphics, float elapsedTime)
        {
            LinkedListNode<Obstacle> currentNode = allObstacles.First;
            while (currentNode != null)
            {
                Obstacle obstacle = currentNode.Value;
                obstacle.Update(graphics, display, elapsedTime);
                LinkedListNode<Obstacle> nextNode = currentNode.Next;

                if ((obstacle.drawPosition.X < 0 || obstacle.drawPosition.X > display.Width) && obstacle.drawPosition.Y > display.Height)
                {
                    allObstacles.Remove(obstacle);
                }

                bool hasCollided = obstacle.HasCollidedWithPlayer(player);
                if (hasCollided == true)
                {
                    player.HitObstacle();
                }

                currentNode = nextNode;
            }
        }

        protected void DrawTerrain(Graphics graphics)
        {
            int halfWidth = display.Width / 2;

            // Draw ground
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, HORIZON_Y, display.Width, 2 * (display.Height - HORIZON_Y));
            //path.AddEllipse(rect);
            path.AddRectangle(rect);
            using (PathGradientBrush groundGradient = new PathGradientBrush(path))
            {
                groundGradient.CenterColor = groundBlue;
                groundGradient.SurroundColors = new Color[] { groundDarkBlue };
                //groundGradient.CenterPoint = new Point(display.Width / 2, display.Height);

                graphics.FillRectangle(groundGradient, rect);
            }

            // Draw grid
            using (Pen gridPen = new Pen(gridLightBlue, GRID_LINE_THICKNESS))
            {
                int x = halfWidth;
                graphics.DrawLine(gridPen, x, HORIZON_Y, x, display.Height);

                DrawParallelVerticalLines(graphics, gridPen, 4, 70 * 2);
                DrawParallelVerticalLines(graphics, gridPen, 8, 138 * 2);
                DrawParallelVerticalLines(graphics, gridPen, 13, 223 * 2);
            }
        }

        protected static void DrawSky(Graphics graphics)
        {
            using (Brush skyGradient = new LinearGradientBrush(new Point(0, 0), new Point(0, HORIZON_Y), black, skyPurple))
            {
                graphics.FillRectangle(skyGradient, 0, 0, display.Width, HORIZON_Y);
            }
        }

        protected void DrawParallelVerticalLines(Graphics graphics, Pen pen, int startOffset, int endOffset)
        {
            int x, x2;
            int halfWidth = display.Width / 2;

            x = halfWidth - startOffset;
            x2 = halfWidth - endOffset;
            graphics.DrawLine(pen, x, HORIZON_Y - 2, x2, display.Height);

            x = halfWidth + startOffset;
            x2 = halfWidth + endOffset;
            graphics.DrawLine(pen, x, HORIZON_Y - 2, x2, display.Height);
        }

        protected void KeyRelease(object sender, KeyEventArgs e)
        {
            player.UserInput(e, false);
        }

        protected void KeyTriggered(object sender, KeyEventArgs e)
        {
            player.UserInput(e, true);
        }
    }
}
