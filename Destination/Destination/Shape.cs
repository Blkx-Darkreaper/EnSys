using System;
using System.Drawing;

namespace Destination
{
    public abstract class Shape
    {
        public int id { get; protected set; }
        public Color lineColour { get; protected set; }
        public bool isVisible { get; protected set; }

        public Shape(Color lineColour)
        {
            this.id = MainForm.GetNextId();
            this.lineColour = lineColour;
            this.isVisible = true;
        }

        public abstract void Update(Graphics graphics, Size display, float elapsedTime);

        protected abstract void Draw(Graphics graphics, Size display);
    }
}