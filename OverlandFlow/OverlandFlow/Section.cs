using System.Drawing;

namespace OpenChannelFlow
{
    abstract class Section
    {
        public int id { get; protected set; }
        public static int nextId = 1;

        public Section()
        {
            this.id = nextId++;
        }

        public abstract void DrawLine(Graphics graphics, Pen pen);

        public abstract void DrawShape(Graphics graphics, Brush brush);
    }
}