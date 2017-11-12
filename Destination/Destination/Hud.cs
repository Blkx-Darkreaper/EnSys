using System;
using System.Drawing;

namespace Destination
{
    public class Hud : Shape
    {
        protected Player player { get; set; }
        protected Color flashColour { get; set; }
        protected bool isFlashing { get; set; }
        protected float flashSpeed { get; set; }
        protected float distanceTravelled { get; set; }

        public Hud(Player player, Color lineColour, Color flashColour) : base(lineColour)
        {
            this.player = player;
            this.flashColour = flashColour;
            this.isFlashing = false;
            this.flashSpeed = 2f;
            this.distanceTravelled = 0;
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            this.distanceTravelled += Player.speed;

            Draw(graphics, display);
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            DrawScore(graphics);
            DrawPowerBar(graphics, display);
        }

        protected void DrawPowerBar(Graphics graphics, Size display)
        {
            int barWidth = 25;
            int offsetX = display.Width - 20 - barWidth;

            int barHeight = 250;
            int offsetY = 20;

            using (Pen pen = new Pen(lineColour))
            {
                graphics.DrawRectangle(pen, offsetX, offsetY, barWidth, barHeight);
            }

            float powerPercentage = player.GetPowerPercentage();
            isFlashing = powerPercentage == 1f;

            float fillOffset = (barHeight - 4) * (1f - powerPercentage);
            float fillHeight = barHeight - 4 - fillOffset;
            if (fillHeight <= 0)
            {
                return;
            }

            if (isFlashing && MainForm.totalTimeElapsed % 1 / flashSpeed == 0)
            {
                using (Brush brush = new SolidBrush(flashColour))
                {
                    graphics.FillRectangle(brush, offsetX + 2, offsetY + 2 + fillOffset, barWidth - 4, fillHeight);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(lineColour))
                {
                    graphics.FillRectangle(brush, offsetX + 2, offsetY + 2 + fillOffset, barWidth - 4, fillHeight);
                }
            }
        }

        public void DrawScore(Graphics graphics)
        {
            string score = distanceTravelled.ToString("N");
            using (Brush brush = new SolidBrush(lineColour))
            {
                using (Font courier = new Font("Courier Regular", 16))
                {
                    graphics.DrawString(score, courier, brush, 20, 20);
                }
            }
        }

        public void DrawGameOver(Graphics graphics, Size display)
        {
            string gameOver = "Game Over";
            using (Brush brush = new SolidBrush(Color.White))
            {
                StringFormat format = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                Font courier = new Font("Courier Regular", 32);
                graphics.DrawString(gameOver, courier, brush, new RectangleF(0, 0, display.Width, display.Height / 2), format);
                //graphics.DrawString(gameOver, courier, brush, 100, 200);

                courier = new Font("Courier Regular", 20);
                string finalScore = distanceTravelled.ToString("N");
                this.distanceTravelled = 0f;
                graphics.DrawString(finalScore, courier, brush, new RectangleF(0, display.Height / 2, display.Width, display.Height / 2), format);
                //graphics.DrawString(finalScore, courier, brush, 120, 300);
            }
        }
    }
}