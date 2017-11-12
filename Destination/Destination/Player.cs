using System;
using System.Drawing;
using System.Windows.Forms;

namespace Destination
{
    public class Player : Shape
    {
        public float centerX { get; protected set; }
        public float bottomOffset { get; protected set; }
        protected float baseBottomOffset { get; set; }
        public SizeF size { get; protected set; }
        public float leftBoundary { get; protected set; }
        public float rightBoundary { get; protected set; }
        public sbyte direction { get; protected set; }
        protected float startingSpeed { get; set; }
        public static float speed { get; protected set; }
        public float maxSpeed { get; protected set; }
        protected bool isShaking { get; set; }
        protected float chargePower { get; set; }
        protected float chargeCooldown { get; set; }
        public float currentPower { get; protected set; }
        public float powerThreshold { get; protected set; }
        public bool isInTurboMode { get; protected set; }
        protected float accelerationRate { get; set; }
        protected float chargeRate { get; set; }
        protected float dischargeRate { get; set; }
        protected Point[] cabinVertices { get; set; }
        protected Point[] frameVertices { get; set; }
        protected Point[] middleLineVertices { get; set; }
        protected Point[] lowerLineVertices { get; set; }
        protected Point[] leftRearDetailVertices { get; set; }
        protected Point[] rightRearDetailVertices { get; set; }
        protected Point[] leftTailLightVertices { get; set; }
        protected Point[] rightTailLightVertices { get; set; }
        protected Tuple<int, int> ventDetailUpperLowerPoints { get; set; }
        protected int[] ventDetailXPoints { get; set; }

        public Player(Color lineColour, Color fillColour, Color highlightColour, SizeF playerSize, float startingPoint, Tuple<float, float> playerBoundaries) : base(lineColour)
        {
            this.centerX = startingPoint;
            this.baseBottomOffset = 20f;
            this.bottomOffset = baseBottomOffset;
            this.leftBoundary = playerBoundaries.Item1;
            this.rightBoundary = playerBoundaries.Item2;
            this.size = playerSize;
            this.direction = 0;
            this.startingSpeed = 5f;
            speed = startingSpeed;
            this.maxSpeed = 20f;
            this.isShaking = false;
            this.currentPower = 0f;
            this.powerThreshold = 50f;
            this.isInTurboMode = false;
            this.accelerationRate = 0.05f;
            this.chargeRate = 2f;
            this.dischargeRate = 0.3f;

            BuildPlayerWireframe();
        }

        public void BuildPlayerWireframe()
        {
            int width = 120;
            int height = 66;

            this.cabinVertices = new Point[6];
            cabinVertices[0] = new Point(23, 11);
            cabinVertices[1] = new Point(29, 2);
            cabinVertices[2] = new Point(55, 0);
            cabinVertices[3] = new Point(width - 55, 0);
            cabinVertices[4] = new Point(width - 29, 2);
            cabinVertices[5] = new Point(width - 23, 11);

            this.frameVertices = new Point[15];
            frameVertices[0] = new Point(24, 64);
            frameVertices[1] = new Point(22, 66);
            frameVertices[2] = new Point(1, 66);
            frameVertices[3] = new Point(1, 62);
            frameVertices[4] = new Point(1, 58);
            frameVertices[5] = new Point(0, 25);
            frameVertices[6] = new Point(15, 11);
            frameVertices[7] = new Point(width - 15, 11);
            frameVertices[8] = new Point(width - 0, 25);
            frameVertices[9] = new Point(width - 1, 58);
            frameVertices[10] = new Point(width - 1, 62);
            frameVertices[11] = new Point(width - 1, 66);
            frameVertices[12] = new Point(width - 22, 66);
            frameVertices[13] = new Point(width - 24, 64);
            frameVertices[14] = frameVertices[0];

            this.leftTailLightVertices = new Point[7];
            leftTailLightVertices[0] = new Point(8, 31);
            leftTailLightVertices[1] = new Point(14, 19);
            leftTailLightVertices[2] = new Point(40, 21);
            leftTailLightVertices[3] = new Point(41, 24);
            leftTailLightVertices[4] = new Point(40, 27);
            leftTailLightVertices[5] = new Point(16, 27);
            leftTailLightVertices[6] = leftTailLightVertices[0];

            this.rightTailLightVertices = new Point[7];
            rightTailLightVertices[0] = new Point(width - 8, 31);
            rightTailLightVertices[1] = new Point(width - 14, 19);
            rightTailLightVertices[2] = new Point(width - 40, 21);
            rightTailLightVertices[3] = new Point(width - 41, 24);
            rightTailLightVertices[4] = new Point(width - 40, 27);
            rightTailLightVertices[5] = new Point(width - 16, 27);
            rightTailLightVertices[6] = rightTailLightVertices[0];

            this.middleLineVertices = new Point[13];
            middleLineVertices[0] = frameVertices[4];
            middleLineVertices[1] = new Point(8, 58);
            middleLineVertices[2] = new Point(7, 51);
            middleLineVertices[3] = new Point(7, 40);
            middleLineVertices[4] = new Point(10, 37);
            middleLineVertices[5] = new Point(37, 37);
            middleLineVertices[6] = new Point(40, 34);
            middleLineVertices[7] = new Point(width - 40, 34);
            middleLineVertices[8] = new Point(width - 37, 37);
            middleLineVertices[9] = new Point(width - 10, 37);
            middleLineVertices[10] = new Point(width - 7, 40);
            middleLineVertices[11] = new Point(width - 8, 58);
            middleLineVertices[12] = frameVertices[9];

            this.lowerLineVertices = new Point[6];
            lowerLineVertices[0] = frameVertices[3];
            lowerLineVertices[1] = new Point(21, 62);
            lowerLineVertices[2] = new Point(29, 50);
            lowerLineVertices[3] = new Point(width - 29, 50);
            lowerLineVertices[4] = new Point(width - 21, 62);
            lowerLineVertices[5] = frameVertices[10];

            this.leftRearDetailVertices = new Point[3];
            leftRearDetailVertices[0] = middleLineVertices[5];
            leftRearDetailVertices[1] = new Point(42, 50);
            leftRearDetailVertices[2] = middleLineVertices[6];

            this.rightRearDetailVertices = new Point[3];
            rightRearDetailVertices[0] = middleLineVertices[8];
            rightRearDetailVertices[1] = new Point(width - 42, 50);
            rightRearDetailVertices[2] = middleLineVertices[7];

            this.ventDetailXPoints = new int[] { 35, 47, 60, width - 47, width - 35 };
            this.ventDetailUpperLowerPoints = new Tuple<int, int>(52, 62);
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            if (Player.speed <= 3)
            {
                MainForm.isGameOver = true;
                return;
            }

            UpdatePosition(elapsedTime);
            PlayShakeAnimation(elapsedTime);

            ChargePower(elapsedTime);
            LosePower(elapsedTime);
            Turbo(elapsedTime);

            Draw(graphics, display);
        }

        protected void UpdatePosition(float elapsedTime)
        {
            if (direction == 0)
            {
                return;
            }

            float deltaPosition = direction * speed;
            this.centerX += deltaPosition;

            if (centerX < leftBoundary)
            {
                this.centerX = leftBoundary;
            }

            if (centerX > rightBoundary)
            {
                this.centerX = rightBoundary;
            }
        }

        protected void PlayShakeAnimation(float elapsedTime)
        {
            if(isShaking == false)
            {
                return;
            }

            if(bottomOffset == baseBottomOffset)
            {
                float randomFloat = MainForm.random.Next(10 * 10, 20 * 10) / 100f;
                this.bottomOffset = baseBottomOffset + randomFloat;
            } else
            {
                this.bottomOffset = baseBottomOffset;
                this.isShaking = false;
            }
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            using (Pen pen = new Pen(lineColour, 2f))
            {
                //RectangleF bounds = GetBounds();
                //graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width, bounds.Height);

                DrawVertices(graphics, pen, cabinVertices);
                DrawVertices(graphics, pen, frameVertices);
                DrawVertices(graphics, pen, leftTailLightVertices);
                DrawVertices(graphics, pen, rightTailLightVertices);
                DrawVertices(graphics, pen, middleLineVertices);
                DrawVertices(graphics, pen, lowerLineVertices);
                DrawVertices(graphics, pen, leftRearDetailVertices);
                DrawVertices(graphics, pen, rightRearDetailVertices);

                float offsetY = MainForm.display.Height - size.Height - bottomOffset;
                foreach (float x in ventDetailXPoints)
                {
                    float drawX = x - 60 + centerX;
                    float drawUpperY = ventDetailUpperLowerPoints.Item1 + offsetY;
                    float drawLowerY = ventDetailUpperLowerPoints.Item2 + offsetY;

                    graphics.DrawLine(pen, drawX, drawUpperY, drawX, drawLowerY);
                }
            }
        }

        protected void DrawVertices(Graphics graphics, Pen pen, Point[] vertices)
        {
            float offsetY = MainForm.display.Height - size.Height - bottomOffset;

            PointF[] verticesToDraw = new PointF[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float x = vertices[i].X - 60 + centerX;
                float y = vertices[i].Y + offsetY;
                verticesToDraw[i] = new PointF(x, y);
            }

            graphics.DrawLines(pen, verticesToDraw);
        }

        public RectangleF GetHitBox()
        {
            float x = centerX - size.Width / 2;
            float y = MainForm.display.Height - 10 - 20;

            return new RectangleF(x, y, size.Width, size.Height);
        }

        public RectangleF GetBounds()
        {
            float x = centerX - size.Width / 2;
            float y = MainForm.display.Height - size.Height - 20;

            return new RectangleF(x, y, size.Width, size.Height);
        }

        public void UserInput(KeyEventArgs keyEvent, bool keyDown)
        {
            if (keyDown == false)
            {
                keyEvent.Handled = true;
                StopMoving();
                return;
            }

            switch (keyEvent.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    MoveLeft();
                    break;

                case Keys.Right:
                case Keys.D:
                    MoveRight();
                    break;

                case Keys.R:
                    RestartGame();
                    break;
            }

            keyEvent.Handled = true;
        }

        protected void RestartGame()
        {
            MainForm.isGameOver = false;
            Player.speed = startingSpeed;
            MainForm.timer.Start();
        }

        protected void MoveLeft()
        {
            this.direction = -1;
        }

        protected void MoveRight()
        {
            this.direction = 1;
        }

        protected void StopMoving()
        {
            this.direction = 0;
        }

        public void HitObstacle()
        {
            Player.speed -= 0.1f;
            this.isShaking = true;
        }

        public float GetPowerPercentage()
        {
            float percentage = currentPower / powerThreshold;
            if (percentage > 1)
            {
                percentage = 1f;
            }

            return percentage;
        }

        protected void Turbo(float timeElapsed)
        {
            if (currentPower < powerThreshold)
            {
                this.isInTurboMode = false;
                return;
            }

            this.isInTurboMode = true;
            speed += timeElapsed * accelerationRate;
            if(speed <= maxSpeed)
            {
                return;
            }

            speed = maxSpeed;
        }

        public void AddPower(float power)
        {
            this.chargePower += power;
            this.chargeCooldown += power;
        }

        public void ChargePower(float timeElapsed)
        {
            if (chargePower <= 0)
            {
                chargePower = 0;
                return;
            }

            float deltaPower = timeElapsed * chargeRate;
            if(deltaPower > chargePower)
            {
                deltaPower = chargePower;
            }

            this.currentPower += deltaPower;
            this.chargePower -= deltaPower;
        }

        public void LosePower(float timeElapsed)
        {
            if (currentPower <= 0)
            {
                this.currentPower = 0;
                return;
            }

            float multiplier = 1f;
            if (isInTurboMode == true)
            {
                multiplier += speed / 2;
            }

            if (chargeCooldown > 0)
            {
                this.chargeCooldown -= timeElapsed * dischargeRate * multiplier;
                return;
            }

            this.chargeCooldown = 0;

            if (currentPower <= 0)
            {
                this.currentPower = 0;
                return;
            }

            this.currentPower -= timeElapsed * dischargeRate * multiplier;
        }
    }
}