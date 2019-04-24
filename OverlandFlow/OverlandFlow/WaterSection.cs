using System;
using System.Drawing;

namespace OpenChannelFlow
{
    public enum Reach { horizontal, mild, critical, steep, adverse };

    class WaterSection : Section
    {
        public PointF topLeftVertex { get { return new PointF(bottomLeftVertex.X, bottomLeftVertex.Y - leftDepth); } }
        public PointF topRightVertex { get { return new PointF(bottomRightVertex.X, bottomRightVertex.Y - rightDepth); } }
        private PointF bottomLeftVertex { get; set; }
        private PointF bottomRightVertex { get; set; }
        public PointF[] allVertices { get { return new PointF[] { topLeftVertex, topRightVertex, bottomRightVertex, bottomLeftVertex, topLeftVertex }; } }
        public FlowPoint inflow { get; set; }
        public FlowPoint outflow { get; protected set; }
        public float leftDepth { get { return inflow.depth; } }
        public float rightDepth { get { return outflow.depth; } }
        public float manningCoef { get; protected set; }
        public float deltaElevation
        {
            get
            {
                float deltaElevation = bottomRightVertex.Y * Program.METERS_PER_PIXEL - bottomLeftVertex.Y * Program.METERS_PER_PIXEL;
                return deltaElevation;
            }
        }
        public float distance
        {
            get
            {
                float deltaX = bottomLeftVertex.X * Program.METERS_PER_PIXEL - bottomRightVertex.X * Program.METERS_PER_PIXEL;
                float distance = (float)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaElevation, 2));
                return distance;
            }
        }
        public float slope
        {
            get
            {

                float slope = deltaElevation / distance;
                return slope;
            }
        }
        public Reach reach
        {
            get
            {
                if (this.slope == 0)
                {
                    return Reach.horizontal;
                }

                float criticalDepth = inflow.getCriticalDepthWithArea();
                criticalDepth = inflow.getCriticalDepthWithPerimeter();
                criticalDepth = inflow.getCriticalDepthWithHead();

                if (normalDepth > criticalDepth)
                {
                    return Reach.mild;
                }

                if (normalDepth < criticalDepth)
                {
                    return Reach.steep;
                }

                if (normalDepth == criticalDepth)
                {
                    return Reach.critical;
                }

                return Reach.adverse;
            }
        }
        public int surfaceProfile
        {
            get
            {
                float criticalDepth = inflow.getCriticalDepthWithArea();
                criticalDepth = inflow.getCriticalDepthWithPerimeter();
                criticalDepth = inflow.getCriticalDepthWithHead();

                if (inflow.depth > normalDepth && inflow.depth > criticalDepth)
                {
                    bool valid = inflow.flow == Flow.subcritical;
                    return 1;
                }

                if (normalDepth > inflow.depth && inflow.depth > criticalDepth)
                {
                    bool valid = inflow.flow == Flow.critical;
                    return 2;
                }

                if (inflow.depth < normalDepth && inflow.depth < criticalDepth)
                {
                    bool valid = inflow.flow == Flow.supercritical;
                    return 3;
                }

                return -1;
            }
        }
        public float frictionSlope
        {
            get
            {
                float frictionSlope = (float)(Math.Pow(manningCoef, 2) * Math.Pow(inflow.velocity, 2) / Math.Pow(inflow.hydraulicRadius, 4f / 3f));
                frictionSlope = (float)Math.Pow((inflow.volumetricFlowRate * manningCoef) / (inflow.crossSectionalArea * Math.Pow(inflow.hydraulicRadius, 2f/3f)), 2);

                return frictionSlope;
            }
        }
        public float normalDepth
        {
            get
            {
                float normalDepth = (float)(manningCoef * inflow.volumetricFlowRate / (Math.Pow(inflow.hydraulicRadius, 2 / 3) * Math.Sqrt(deltaElevation / distance) * inflow.baseWidth));
                return normalDepth;
            }
        }
        public float headLoss
        {
            get
            {
                float maxHeadLoss = 0.95f*(inflow.head - inflow.elevation - inflow.depth);

                float headLoss = frictionSlope * distance;

                //return Math.Min(headLoss, maxHeadLoss);
                return headLoss;
            }
        }

        public WaterSection(float startX, float startY, float startDepth, float width, float slopeHeight, float endDepth) : base()
        {
            this.bottomLeftVertex = new PointF(startX, startY);
            this.bottomRightVertex = new PointF(startX + width, startY + slopeHeight);
        }

        public WaterSection(float startX, float startY, float distance, float slopeHeight, float startWidth, float endWidth, float flowRate, float manningCoef, float startDepth) : base()
        {
            this.bottomLeftVertex = new PointF(startX, startY);
            this.bottomRightVertex = new PointF(startX + distance, startY + slopeHeight);

            this.manningCoef = manningCoef;

            this.inflow = new FlowPoint(bottomLeftVertex.Y * Program.METERS_PER_PIXEL, startWidth, flowRate);
            this.inflow.setDepth_getVelocity(startDepth);

            this.outflow = new FlowPoint(bottomRightVertex.Y * Program.METERS_PER_PIXEL, endWidth, this.inflow.volumetricFlowRate);

            float velocity = getFinalVelocity();
            this.outflow.setVelocity_getDepth(velocity);
        }

        public override void DrawLine(Graphics graphics, Pen pen)
        {
            graphics.DrawPolygon(pen, allVertices);
        }

        public override void DrawShape(Graphics graphics, Brush brush)
        {
            graphics.FillPolygon(brush, allVertices);
        }

        public float getFinalVelocity()
        {
            float finalVelocity = (float)Math.Sqrt(2 * Program.GRAVITY * (inflow.head - headLoss - inflow.elevation - inflow.depth));
            return finalVelocity;
        }
    }
}