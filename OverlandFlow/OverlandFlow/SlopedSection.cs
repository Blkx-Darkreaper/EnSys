using System.Drawing;

namespace OpenChannelFlow
{
    class SlopedSection : Section
    {
        public PointF topLeftVertex { get; protected set; }
        public PointF topRightVertex { get; protected set; }
        private PointF bottomLeftVertex { get; set; }
        private PointF bottomRightVertex { get; set; }
        public PointF[] allVertices { get { return new PointF[] { topLeftVertex, topRightVertex, bottomRightVertex, bottomLeftVertex, topLeftVertex }; } }

        public SlopedSection(float startX, float startY, float width, float slopeHeight, float baseHeight) : base()
        {
            this.topLeftVertex = new PointF(startX, startY);
            this.bottomLeftVertex = new PointF(startX, baseHeight);
            this.topRightVertex = new PointF(startX + width, startY + slopeHeight);
            this.bottomRightVertex = new PointF(startX + width, baseHeight);
        }

        public override void DrawLine(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, topLeftVertex, topRightVertex);
        }

        public override void DrawShape(Graphics graphics, Brush brush)
        {
            graphics.FillPolygon(brush, allVertices);
        }
    }
}