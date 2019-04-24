using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OpenChannelFlow
{
    static class Program
    {
        public static Timer Timer { get; set; }
        private static List<Grid> allGrids { get; set; }
        private static int gridLengthPixels { get; set; }
        private static float pixelsPerMeter { get; set; }
        private static List<Section> allSections { get; set; }
        private static List<Section> allWaterSections { get; set; }
        private static List<PointF> allVertices { get; set; }
        public const float GRAVITY = 9.8f;
        public const float METERS_PER_PIXEL = 0.1f;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static void GenerateProfile(int drawWidth, int drawHeight)
        {
            allSections = new List<Section>();
            allWaterSections = new List<Section>();
            allVertices = new List<PointF>();

            float distance = 19.99975f; //drawWidth / 33;

            float horizontal = 0;
            float mild = 0.1f; //drawHeight / 75f;
            float critical = drawHeight / 30f;
            float steep = drawHeight / 15f;
            float adverse = -critical;
            float dropoff = drawHeight / 12f;

            float depth = 5;

            float x = 0;
            float y = 50;
            allSections.Add(new SlopedSection(x, y, distance, mild, drawHeight));
            allVertices.Add(new PointF(x, y));

            //allWaterSections.Add(new WaterSection(x, y, depth, distance, mild, depth));

            // Testing
            float width = 10;   // m
            float flowRate = 2.5f; // m^3/s
            float manningCoef = 0.03f;
            float waterDepth = 0.157f;  // m
            WaterSection currentSection = new WaterSection(x, y, distance, mild, width, width, flowRate, manningCoef, waterDepth);
            float slope = currentSection.slope;
            float criticalVelocity = currentSection.inflow.criticalVelocity;
            //float criticalDepth = inFlow.getCriticalDepthWithArea();
            //criticalDepth = inFlow.getCriticalDepthWithPerimeter();
            //criticalDepth = inFlow.getCriticalDepthWithVelocity(criticalVelocity);
            string flow = currentSection.inflow.flow.ToString();
            // End Testing

            allWaterSections.Add(currentSection);
            WaterSection previousSection = currentSection;

            x += distance;
            y += mild;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, horizontal, drawHeight));
            //allWaterSections.Add(new WaterSection(x, y, depth, distance, horizontal, depth));
            currentSection = new WaterSection(x, y, distance, mild, width, width, flowRate, manningCoef, previousSection.outflow.depth);
            allWaterSections.Add(currentSection);
            x += distance;
            y += horizontal;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, mild, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, mild, depth));
            x += distance;
            y += mild;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, critical, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, critical, depth));
            x += distance;
            y += critical;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, mild, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, mild, depth));
            x += distance;
            y += mild;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, steep, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, steep, depth));
            x += distance;
            y += steep;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, mild, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, mild, depth));
            x += distance;
            y += mild;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, adverse, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, adverse, depth));
            x += distance;
            y += adverse;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, horizontal, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, horizontal, depth));
            x += distance;
            y += horizontal;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, critical, drawHeight));
            allWaterSections.Add(new WaterSection(x, y, depth, distance, critical, depth));
            x += distance;
            y += critical;
            allVertices.Add(new PointF(x, y));

            // First drop off
            allSections.Add(new DropoffSection(x, y, distance, dropoff, drawHeight));
            y += dropoff;
            allVertices.Add(new PointF(x, y));
            x += distance;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, adverse, drawHeight));
            x += distance;
            y += adverse;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, mild, drawHeight));
            x += distance;
            y += mild;
            allVertices.Add(new PointF(x, y));

            // Second drop off
            allSections.Add(new DropoffSection(x, y, distance, dropoff, drawHeight));
            y += dropoff;
            allVertices.Add(new PointF(x, y));
            x += distance;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, adverse, drawHeight));
            x += distance;
            y += adverse;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, critical, drawHeight));
            x += distance;
            y += critical;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, horizontal, drawHeight));
            x += distance;
            y += horizontal;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, steep, drawHeight));
            x += distance;
            y += steep;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, horizontal, drawHeight));
            x += distance;
            y += horizontal;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, steep, drawHeight));
            x += distance;
            y += steep;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, critical, drawHeight));
            x += distance;
            y += critical;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, adverse, drawHeight));
            x += distance;
            y += adverse;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, steep, drawHeight));
            x += distance;
            y += steep;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, critical, drawHeight));
            x += distance;
            y += critical;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, steep, drawHeight));
            x += distance;
            y += steep;
            allVertices.Add(new PointF(x, y));

            // Third drop off
            allSections.Add(new DropoffSection(x, y, distance, dropoff, drawHeight));
            y += dropoff;
            allVertices.Add(new PointF(x, y));
            x += distance;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, steep, drawHeight));
            x += distance;
            y += steep;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, critical, drawHeight));
            x += distance;
            y += critical;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, mild, drawHeight));
            x += distance;
            y += mild;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, horizontal, drawHeight));
            x += distance;
            y += horizontal;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, adverse, drawHeight));
            x += distance;
            y += adverse;
            allVertices.Add(new PointF(x, y));

            allSections.Add(new SlopedSection(x, y, distance, horizontal, drawHeight));
            x += distance;
            y += horizontal;
            allVertices.Add(new PointF(x, y));
        }

        public static void GenerateTerrain(int displayWidth, int[] gridHeights)
        {
            int totalGrids = gridHeights.Length;
            gridLengthPixels = displayWidth / totalGrids;

            pixelsPerMeter = gridLengthPixels / (float)Grid.Length;

            allGrids = new List<Grid>(totalGrids);
            Grid previous = null;

            foreach (int height in gridHeights)
            {
                Grid gridToAdd = new Grid(height);
                allGrids.Add(gridToAdd);

                if (previous == null)
                {
                    previous = gridToAdd;
                    continue;
                }

                previous.Next = gridToAdd;
                previous = gridToAdd;
            }

            // Testing
            //allGrids[0].AddWater(400, 1);
            //allGrids[1].AddWater(200, 1);
            //allGrids[2].AddWater(325, 1);
            // End Testing
        }

        public static void AddWater(float water, float flowSpeed)
        {
            Grid firstGrid = allGrids[0];

            firstGrid.AddWater(water, flowSpeed);
        }

        public static void WaterFlow(float timeElapsed, bool isDraining)
        {
            // Update grids
            foreach (Grid grid in allGrids)
            {
                grid.RemoveWater(timeElapsed, isDraining);
            }
        }

        //public static void DrawTerrain(Graphics graphics, int bottom)
        //{
        //    Brush grey = Brushes.Gray;
        //    Brush blue = Brushes.Blue;

        //    Font font = new Font("Arial", 10f, FontStyle.Regular);
        //    int x = 0;
        //    int bedrock = bottom - 40;

        //    foreach (Grid grid in allGrids)
        //    {
        //        // Display Water depth and flow speed
        //        float depth = grid.GetDepth();
        //        string waterDepthText = string.Format("{0:0.0} m", depth);
        //        graphics.DrawString(waterDepthText, font, blue, new PointF(x, bottom - 15));

        //        string flowSpeedText = string.Format("{0:0.0} m/s", grid.FlowSpeed);
        //        graphics.DrawString(flowSpeedText, font, grey, new PointF(x, bottom - 30));

        //        int height = grid.Height;
        //        int pixelHeight = (int)pixelsPerMeter * height;

        //        int y = bedrock - pixelHeight;

        //        Rectangle bounds = new Rectangle(x, y, gridLengthPixels, pixelHeight);
        //        graphics.FillRectangle(grey, bounds);

        //        // Draw water
        //        int pixelDepth = (int)(pixelsPerMeter * depth);

        //        if (pixelDepth <= 0)
        //        {
        //            x += gridLengthPixels;
        //            continue;
        //        }

        //        y -= pixelDepth;

        //        bounds = new Rectangle(x, y, gridLengthPixels, pixelDepth);
        //        graphics.FillRectangle(blue, bounds);

        //        // Increment for next grid
        //        x += gridLengthPixels;
        //    }
        //}

        //public static void DrawTerrain(Graphics graphics, int bottom)
        //{
        //    Pen grey = Pens.Gray;

        //    PointF currentVertex = allVertices[0];

        //    for(int i = 1; i < allVertices.Count; i++)
        //    {
        //        PointF nextVertex = allVertices[i];

        //        graphics.DrawLine(grey, currentVertex, nextVertex);

        //        currentVertex = nextVertex;
        //    }
        //}

        public static void DrawTerrain(Graphics graphics, int bottom)
        {
            Pen grey = Pens.Gray;
            //Brush grey = Brushes.Gray;

            foreach (Section section in allSections)
            {
                section.DrawLine(graphics, grey);
            }

            Brush blue = Brushes.Blue;

            foreach(Section section in allWaterSections)
            {
                section.DrawShape(graphics, blue);
            }
        }
    }
}