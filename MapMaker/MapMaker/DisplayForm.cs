using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapMaker
{
    public partial class DisplayForm : Form
    {
        public Size Resolution { get; protected set; }

        public DisplayForm(Size windowSize)
        {
            InitializeComponent();

            SetDefaultRes(windowSize);
        }

        protected void SetDefaultRes(Size windowSize)
        {
            string res = String.Format("{0} x {1}", windowSize.Width, windowSize.Height);

            int index = ResolutionDdl.Items.IndexOf(res);
            if (index == -1)
            {
                if (windowSize.Width == 640)
                {
                    index = 0;
                }
                else
                {
                    index = ResolutionDdl.Items.Count - 1;
                }
            }

            ResolutionDdl.SelectedIndex = index;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string selectedRes = ResolutionDdl.GetItemText(ResolutionDdl.SelectedItem);
            string[] dimensions = selectedRes.Split('x');

            string widthStr = dimensions[0].Trim();
            int width = (int)Convert.ToInt32(widthStr);

            string heightStr = dimensions[1].Trim();
            int height = (int)Convert.ToInt32(heightStr);

            this.Resolution = new Size(width, height);
            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
