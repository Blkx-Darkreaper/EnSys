using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenChannelFlow
{
    public class Grid
    {
        public int Id { get; protected set; }
        public static int NextId = 1;
        public int Height { get; protected set; }
        public static int Length = 10;
        public float Water { get; protected set; }
        public float FlowSpeed { get; protected set; }
        public Grid Next { get; set; }
        protected const int DECIMAL_PLACES = 1;

        public Grid(int height) : this(height, 0, 0) { }

        public Grid(int height, int water, int flowSpeed)
        {
            this.Id = NextId;
            NextId++;

            this.Height = height;
            this.Water = water;
            this.FlowSpeed = flowSpeed;
        }

        public void AddWater(float water, float flowSpeed)
        {
            float initialMomentum = this.Water * this.FlowSpeed;
            float addedMomentum = water * flowSpeed;

            float totalMomentum = initialMomentum + addedMomentum;

            float totalWater = this.Water + water;
            float finalFlowSpeed = totalMomentum / totalWater;

            this.Water = totalWater;
            this.FlowSpeed = finalFlowSpeed;
        }

        public float GetDepth()
        {
            int area = Grid.Length * Grid.Length;

            float depth = this.Water / area;
            return depth;
        }

        public float GetSurfaceHeight()
        {
            float depth = GetDepth();
            float surfaceHeight = this.Height + depth;
            return surfaceHeight;
        }

        public void RemoveWater(float timeElapsed, bool isDraining)
        {
            if (this.Water == 0)
            {
                return;
            }

            if (this.Next == null)
            {
                if (isDraining == false)
                {
                    return;
                }
            }

            float surfaceHeight = GetSurfaceHeight();

            float nextSurfaceHeight = 0;
            if (this.Next != null)
            {
                nextSurfaceHeight = this.Next.GetSurfaceHeight();
            }

            float diff = surfaceHeight - nextSurfaceHeight;
            if (diff <= 0f)
            {
                return;
            }

            float depth = GetDepth();
            if(diff > depth) {
                diff = depth;
            }

            float width = Grid.Length;
            float flux = diff * width * this.FlowSpeed * timeElapsed;

            // Round flux
            flux = (float)Math.Round(flux, DECIMAL_PLACES);
            if (flux <= 0f)
            {
                return;
            }

            if (float.IsNaN(flux) == true)
            {
                return;
            }

            if (flux > this.Water)
            {
                flux = this.Water;
            }

            this.Water -= flux;
            if (this.Water == 0f)
            {
                this.FlowSpeed = 0;
            }

            if (this.Next == null)
            {
                return;
            }

            this.Next.AddWater(flux, this.FlowSpeed);
        }
    }
}