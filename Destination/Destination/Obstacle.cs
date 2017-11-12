using System;
using System.Drawing;

namespace Destination
{
    public class Obstacle : Shape
    {
        public float distance { get; protected set; }
        public PointF drawPosition { get; protected set; }
        protected float centerOffset { get; set; }
        public float ratio { get; protected set; }
        public SizeF size { get; protected set; }

        public Obstacle(Color lineColour, float startingDistance, float drawY, float drawX, float ratio, float offset) : base(lineColour)
        {
            this.distance = startingDistance;
            this.drawPosition = new PointF(drawX, drawY);
            this.ratio = ratio;
            this.size = new SizeF(1f, 0.1f);
            this.centerOffset = offset;
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            float deltaDistance = elapsedTime * Player.speed;

            float deltaY = MainForm.GetDeltaDrawY(distance);
            this.distance -= deltaDistance;
            this.drawPosition = new PointF(drawPosition.X + deltaY * ratio, drawPosition.Y + deltaY);
            this.size = new SizeF(size.Width + deltaY / 2, size.Height + deltaY /2);

            this.Draw(graphics, display);
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            if (isVisible == false)
            {
                return;
            }

            using (Pen pen = new Pen(lineColour, MainForm.GRID_LINE_THICKNESS))
            {
                float leftX = display.Width / 2f - drawPosition.X;
                float middleX = leftX + centerOffset * size.Width;
                float rightX = leftX + size.Width;
                float topY = drawPosition.Y - size.Height / 2;
                float middleY = topY + 0.75f * size.Height;
                float bottomY = topY + size.Height;

                PointF[] outerVertices = new PointF[5];
                outerVertices[0] = new PointF(leftX, middleY);   // Left
                outerVertices[1] = new PointF(middleX, topY);    // Top
                outerVertices[2] = new PointF(rightX, middleY);  // Right
                outerVertices[3] = new PointF(middleX, bottomY); // Bottom
                outerVertices[4] = outerVertices[0];
                graphics.DrawLines(pen, outerVertices);

                PointF[] centerVertices = new PointF[2];
                centerVertices[0] = outerVertices[1];
                centerVertices[1] = outerVertices[3];
                graphics.DrawLines(pen, centerVertices);
            }
        }

        public RectangleF GetBounds()
        {
            float leftX = MainForm.display.Width / 2f - drawPosition.X;
            float topY = drawPosition.Y - size.Height / 2;

            return new RectangleF(leftX, topY, size.Width, size.Height);
        }

        public bool HasCollidedWithPlayer(Player player)
        {
            RectangleF bounds = GetBounds();
            RectangleF playerHitbox = player.GetHitBox();

            bool hasCollided = bounds.IntersectsWith(playerHitbox);
            return hasCollided;
        }
    }
}