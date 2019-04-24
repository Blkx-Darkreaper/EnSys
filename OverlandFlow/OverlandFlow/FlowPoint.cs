using System;

namespace OpenChannelFlow
{
    public enum Flow { subcritical, critical, supercritical };

    class FlowPoint
    {
        public float elevation { get; protected set; }
        public float baseWidth { get; protected set; }
        public float bankSlope { get; protected set; }
        public float depth { get; protected set; }
        public float crossSectionalArea
        {
            get
            {
                float crossSectionalArea = baseWidth * depth + bankSlope * depth * depth;
                return crossSectionalArea;
            }
        }
        public float wettedPerimeter
        {
            get
            {
                float wettedPerimeter = baseWidth + 2 * (float)Math.Sqrt(Math.Pow(depth, 2) + Math.Pow(bankSlope * depth, 2));
                return wettedPerimeter;
            }
        }
        public float hydraulicRadius
        {
            get
            {
                float hydraulicRadius = crossSectionalArea / wettedPerimeter;
                return hydraulicRadius;
            }
        }
        public float velocity { get; protected set; }
        public float criticalVelocity
        {
            get
            {
                float criticalDepth = getCriticalDepthWithArea();
                criticalDepth = getCriticalDepthWithPerimeter();
                criticalDepth = getCriticalDepthWithHead();

                float criticalVelocity = (float)Math.Sqrt(criticalDepth * Program.GRAVITY);
                return criticalVelocity;
            }
        }
        public float volumetricFlowRate { get; protected set; }
        public float froudeNumber
        {
            get
            {
                float froudeNumber = velocity / (float)Math.Sqrt(depth * Program.GRAVITY);
                return froudeNumber;
            }
        }
        public Flow flow
        {
            get
            {
                float criticalDepth = getCriticalDepthWithArea();
                criticalDepth = getCriticalDepthWithPerimeter();
                criticalDepth = getCriticalDepthWithHead();

                Flow check = Flow.critical;
                if (depth > criticalDepth)
                {
                    check = Flow.subcritical;
                }
                if (depth < criticalDepth)
                {
                    check = Flow.supercritical;
                }

                bool valid;
                if (froudeNumber > 1)
                {
                    valid = check == Flow.supercritical;
                    return Flow.supercritical;
                }

                if (froudeNumber < 1)
                {
                    valid = check == Flow.subcritical;
                    return Flow.subcritical;
                }

                valid = check == Flow.critical;
                return Flow.critical;
            }
        }
        public float head
        {
            get
            {
                float head = (float)(elevation + depth + 0.5 * Math.Pow(velocity, 2) / Program.GRAVITY);
                return head;
            }
        }
        
        public FlowPoint(float elevation, float width, float flowRate)
        {
            this.elevation = elevation;
            this.baseWidth = width;
            this.bankSlope = 0;
            this.volumetricFlowRate = flowRate;
        }

        public float getCriticalDepthWithHead()
        {
            float criticalDepth = (float)(2 / 3f * (elevation + depth + 0.5f * Math.Pow(velocity, 2) / Program.GRAVITY));
            return criticalDepth;
        }

        public float getCriticalDepthWithArea()
        {
            float criticalDepth = (float)Math.Pow(volumetricFlowRate / crossSectionalArea, 2) / Program.GRAVITY;
            return criticalDepth;
        }

        public float getCriticalDepthWithPerimeter()
        {
            if(flow != Flow.critical)
            {
                return float.NaN;
            }

            float criticalDepth = (float)Math.Pow(Math.Pow(volumetricFlowRate / wettedPerimeter, 2) / Program.GRAVITY, 1 / 3f);
            return criticalDepth;
        }

        public float getCriticalDepthWithVelocity(float criticalVelocity)
        {
            float criticalDepth = (float)Math.Pow(criticalVelocity, 2) / Program.GRAVITY;
            return criticalDepth;
        }

        public float setDepth_getVelocity(float depth)
        {
            this.depth = depth;

            this.velocity = volumetricFlowRate / crossSectionalArea;
            return velocity;
        }

        public float setVelocity_getDepth(float velocity)
        {
            this.velocity = velocity;

            float area = volumetricFlowRate / velocity;

            this.depth = (-bankSlope - (float)Math.Sqrt(Math.Pow(bankSlope, 2) + 4 * baseWidth * area)) / (2 * baseWidth);
            this.depth = (-bankSlope + (float)Math.Sqrt(Math.Pow(bankSlope, 2) + 4 * baseWidth * area)) / (2 * baseWidth);
            return depth;
        }
    }
}