using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destination
{
    public class Mountain : Shape
    {
        public Color fillColour { get; protected set; }

        public Mountain(Color lineColour, Color fillColour) : base(lineColour)
        {
            this.fillColour = fillColour;
        }

        public override void Update(Graphics graphics, Size display, float elapsedTime)
        {
            throw new NotImplementedException();
        }

        protected override void Draw(Graphics graphics, Size display)
        {
            throw new NotImplementedException();
        }
    }
}