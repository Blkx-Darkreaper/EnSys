using System;
using System.Drawing;

namespace Destination
{
    public class SideLine : Shape
    {
        public float distanceHead { get; protected set; }
        public float distanceTail { get; protected set; }
        public PointF head { get; protected set; }
        public PointF tail { get; protected set; }
        public float ratio { get; protected set; }
        public bool moveRight { get; protected set; }

        public SideLine(Color colour, float startingDistance, float startX, float startY, float length, float ratio) : base(colour)
        {
            this.ratio = ratio;
            this.moveRight = false;

            this.distanceHead = startingDistance;
            this.head = new PointF(startX, startY);

            this.distanceTail = startingDistance + length;
            //float deltaX = (float)(length / Math.Sqrt(Math.Pow(1 / ratio, 2) + 1));
            //float endX = startX + deltaX;

            //float deltaY = deltaX / ratio;
            //float endY = startY - deltaY;
            //this.tail = new PointF(endX, endY);
            this.tail = new PointF(startX, startY);
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            float deltaDistance = elapsedTime * Player.speed;

            int direction = 1;
            if (moveRight == false)
            {
                direction = -1;
            }

            int maxValue = 100000000;

            if ((head.X > 0 && head.X <= display.Width) || head.Y <= display.Height)
            {
                float deltaHeadY = MainForm.GetDeltaDrawY(distanceHead);

                float headX = head.X + direction * deltaHeadY * ratio;
                if (Math.Abs(headX) > maxValue)
                {
                    headX = direction * maxValue;
                }

                float headY = head.Y + deltaHeadY;
                if (headY > maxValue)
                {
                    headY = maxValue;
                }

                this.head = new PointF(headX, headY);
            }

            this.distanceHead -= deltaDistance;

            float deltaTailY = MainForm.GetDeltaDrawY(distanceTail);

            float tailX = tail.X + direction * deltaTailY * ratio;
            if (Math.Abs(tailX) > maxValue)
            {
                tailX = direction * maxValue;
            }

            float tailY = tail.Y + deltaTailY;
            if (tailY > maxValue)
            {
                tailY = maxValue;
            }

            this.tail = new PointF(tailX, tailY);
            this.distanceTail -= deltaDistance;

            this.Draw(graphics, display);
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            using (Pen pen = new Pen(lineColour, MainForm.GRID_LINE_THICKNESS))
            {
                graphics.DrawLine(pen, head, tail);
            }
        }
    }
}