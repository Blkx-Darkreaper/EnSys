using System.Drawing;

namespace OpenChannelFlow
{
    class DropoffSection : Section
    {
        public PointF topLeftVertex { get; protected set; }
        public PointF midLeftVertex { get; protected set; }
        public PointF midRightVertex { get; protected set; }
        private PointF bottomLeftVertex { get; set; }
        private PointF bottomRightVertex { get; set; }
        public PointF[] allVertices { get { return new PointF[] { topLeftVertex, midLeftVertex, midRightVertex, bottomRightVertex, bottomLeftVertex, topLeftVertex }; } }

        public DropoffSection(float startX, float startY, float width, float dropoffHeight, float baseHeight) : base()
        {
            this.topLeftVertex = new PointF(startX, startY);
            this.midLeftVertex = new PointF(startX, startY + dropoffHeight);
            this.midRightVertex = new PointF(startX + width, startY + dropoffHeight);
            this.bottomLeftVertex = new PointF(startX, baseHeight);
            this.bottomRightVertex = new PointF(startX + width, baseHeight);
        }

        public override void DrawLine(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, topLeftVertex, midLeftVertex);
            graphics.DrawLine(pen, midLeftVertex, midRightVertex);
        }

        public override void DrawShape(Graphics graphics, Brush brush)
        {
            graphics.FillPolygon(brush, allVertices);
        }
    }
}