using System;
using System.Drawing;

namespace Destination
{
    public class Powerup : Shape
    {
        public float distance { get; protected set; }
        public PointF drawPosition { get; protected set; }
        public float ratio { get; protected set; }
        public float size { get; protected set; }
        public float rotation { get; protected set; }
        public float rotationSpeed { get; protected set; }
        public float power { get; protected set; }

        public Powerup(Color colour, float startingDistance, float drawY, float drawX, float ratio, float power, float startRotation) : base(colour)
        {
            this.distance = startingDistance;
            this.drawPosition = new PointF(drawX, drawY);
            this.ratio = ratio;
            this.size = 1;
            this.rotation = startRotation % 90;
            this.rotationSpeed = 8f;
            this.power = power;
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            float deltaDistance = elapsedTime * Player.speed;

            float deltaY = MainForm.GetDeltaDrawY(distance);
            this.distance -= deltaDistance;
            this.drawPosition = new PointF(drawPosition.X + deltaY * ratio, drawPosition.Y + deltaY);
            this.size += deltaY / 2;
            this.rotation += elapsedTime * rotationSpeed;
            this.rotation %= 90;

            this.Draw(graphics, display);
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            if(isVisible == false)
            {
                return;
            }

            using (Pen pen = new Pen(lineColour))
            {
                float leftX = display.Width / 2f - drawPosition.X;
                float middleX = leftX + size / 2;
                float rightX = leftX + size;
                float topY = drawPosition.Y - size / 2;
                float middleY = topY + size / 2;
                float bottomY = topY + size;

                //graphics.DrawRectangle(pen, leftX, drawPosition.Y, size, size);   // Testing

                PointF[] outerVertices = new PointF[5];
                outerVertices[0] = new PointF(leftX, middleY);   // Left
                outerVertices[1] = new PointF(middleX, topY);    // Top
                outerVertices[2] = new PointF(rightX, middleY);  // Right
                outerVertices[3] = new PointF(middleX, bottomY); // Bottom
                outerVertices[4] = outerVertices[0];
                graphics.DrawLines(pen, outerVertices);

                PointF[] centerVertices = new PointF[3];
                centerVertices[0] = outerVertices[1];           // Top
                centerVertices[2] = outerVertices[3];           // Bottom
                float centerPointX = size * rotation / 90 + leftX;

                float modifier = 1 - Math.Abs(45 - (90 - rotation)) / 45;
                float centerPointY = middleY + 0.2f * size * modifier;
                centerVertices[1] = new PointF(centerPointX, centerPointY);    // Center
                graphics.DrawLines(pen, centerVertices);
            }
        }

        public RectangleF GetBounds()
        {
            float leftX = MainForm.display.Width / 2f - drawPosition.X;
            float topY = drawPosition.Y - size / 2;

            return new RectangleF(leftX, topY, size, size);
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