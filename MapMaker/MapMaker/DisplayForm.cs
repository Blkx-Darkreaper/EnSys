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

        public DisplayForm()
        {
            InitializeComponent();
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string selectedRes = ResolutionDdl.Text;
            //string selectedRes = ResolutionDdl.GetItemText(ResolutionDdl.SelectedValue);
            string widthStr = selectedRes.Split('x')[0].Trim();
            int width = (int)Convert.ToInt32(widthStr);

            string heightStr = selectedRes.Split('x')[1].Trim();
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
