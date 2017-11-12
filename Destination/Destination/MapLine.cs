using System;
using System.Drawing;

namespace Destination
{
    public class MapLine : Shape
    {
        public float distance { get; protected set; }
        public PointF drawPosition { get; protected set; }
        public float ratio { get; protected set; }

        public MapLine(Color colour, float startingDistance, float drawY, float width, float ratio) : base(colour)
        {
            this.distance = startingDistance;
            this.drawPosition = new PointF(width / 2, drawY);
            this.ratio = ratio;
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            using (Pen pen = new Pen(lineColour, MainForm.GRID_LINE_THICKNESS))
            {
                float x = display.Width / 2f - drawPosition.X;
                float x2 = display.Width / 2f + drawPosition.X;

                graphics.DrawLine(pen, x, drawPosition.Y, x2, drawPosition.Y);
            }
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            float deltaDistance = elapsedTime * Player.speed;

            float deltaY = MainForm.GetDeltaDrawY(distance);
            this.distance -= deltaDistance;
            this.drawPosition = new PointF(drawPosition.X + deltaY * ratio, drawPosition.Y + deltaY);

            this.Draw(graphics, display);
        }
    }
}