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
    public partial class NewFileForm : Form
    {
        public Size MapSize { get; protected set; }
        public string TilesetFilename { get; protected set; }
        protected int tileLength { get; set; }

        public NewFileForm(int tileLength)
        {
            InitializeComponent();
            this.TilesetNameLabel.Text = string.Empty;

            int initWidth = (int)this.MapWidth.Value;
            int initHeight = (int)this.MapWidth.Value;
            this.MapSize = new Size(initWidth, initHeight);
            this.TilesetFilename = null;
            this.tileLength = tileLength;
            this.MapWidth.Minimum = tileLength;
            this.MapHeight.Minimum = tileLength;
        }

        protected void NewFileDone_Click(object sender, EventArgs e)
        {
            if (TilesetFilename == null)
            {
                MessageBox.Show("You must select a tileset to load", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int width = (int)MapWidth.Value;
            int height = (int)MapHeight.Value;

            this.MapSize = new Size(width, height);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        protected void NewFileCancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        protected void MapWidth_ValueChanged(object sender, EventArgs e)
        {
            int width = (int)MapWidth.Value;
            int oldWidth = MapSize.Width;

            int remainder = width % tileLength;
            if (remainder == 0)
            {
                MapWidth.Value = width;
                this.MapSize = new Size(width, MapSize.Height);
                return;
            }

            if (width > oldWidth)
            {
                width += (tileLength - remainder);
            }
            else
            {
                width -= remainder;
            }

            MapWidth.Value = width;
            this.MapSize = new Size(width, MapSize.Height);
        }

        protected void MapHeight_ValueChanged(object sender, EventArgs e)
        {
            int height = (int)MapHeight.Value;
            int oldHeight = MapSize.Height;

            int remainder = height % tileLength;
            if (remainder == 0)
            {
                MapHeight.Value = height;
                this.MapSize = new Size(MapSize.Width, height);
                return;
            }

            if (height > oldHeight)
            {
                height += (tileLength - remainder);
            }
            else
            {
                height -= remainder;
            }

            // Height can't be less than width
            int heightMin = (int)MapWidth.Value;
            height = Program.ClampToMin(height, heightMin);

            MapHeight.Value = height;
            this.MapSize = new Size(MapSize.Width, height);
        }

        private void LoadTilesetButton_Click(object sender, EventArgs e)
        {
            // Get the Filename of the selected file
            OpenFileDialog dialog = Program.GetImageOpenDialog();
            string filename = Program.GetFilenameToOpen(dialog);
            if (filename == null)
            {
                return;
            }

            this.TilesetFilename = filename;
            this.TilesetNameLabel.Text = filename;
        }
    }
}
