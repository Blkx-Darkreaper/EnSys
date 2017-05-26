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
        public Size MapPixelSize { get; protected set; }
        public string TilesetFilename { get; protected set; }
        public int TileLength { get; protected set; }

        public NewFileForm()
        {
            InitializeComponent();
            this.TilesetNameLabel.Text = string.Empty;

            int initWidth = (int)this.MapWidth.Value;
            int initHeight = (int)this.MapWidth.Value;
            this.MapPixelSize = new Size(initWidth, initHeight);
            this.TilesetFilename = null;
            this.TileLength = (int)this.TileLengthControl.Value;
            this.MapWidth.Minimum = TileLength;
            this.MapHeight.Minimum = TileLength;
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

            this.MapPixelSize = new Size(width, height);

            this.TileLength = (int)TileLengthControl.Value;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        protected void NewFileCancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        protected void MapWidth_ValueChanged(object sender, EventArgs e)
        {
            int width = (int)MapWidth.Value;
            int height = MapPixelSize.Height;
            int oldWidth = MapPixelSize.Width;

            // Width cannot exceed imageHeight
            if (width > height)
            {
                width = height;
            }

            int remainder = width % TileLength;
            if (remainder == 0)
            {
                MapWidth.Value = width;
                this.MapPixelSize = new Size(width, height);
                return;
            }

            if (width > oldWidth)
            {
                width += (TileLength - remainder);
            }
            else
            {
                width -= remainder;
            }

            MapWidth.Value = width;
            this.MapPixelSize = new Size(width, height);
        }

        protected void MapHeight_ValueChanged(object sender, EventArgs e)
        {
            int height = (int)MapHeight.Value;
            int width = MapPixelSize.Width;
            int oldHeight = MapPixelSize.Height;

            // Height cannot deceed imageWidth
            if (height < width)
            {
                height = width;
            }

            // Height can't be less than scaledWidth
            int heightMin = (int)MapWidth.Value;
            height = Program.ClampToMin(height, heightMin);

            int remainder = height % TileLength;
            if (remainder == 0)
            {
                MapHeight.Value = height;
                this.MapPixelSize = new Size(width, height);
                return;
            }

            if (height > oldHeight)
            {
                height += (TileLength - remainder);
            }
            else
            {
                height -= remainder;
            }

            MapHeight.Value = height;
            this.MapPixelSize = new Size(width, height);
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

        private void TileLengthControl_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)TileLengthControl.Value;
            int tileLength = Program.TileLength;

            int remainder = value % 8;
            if (remainder == 0)
            {
                TileLengthControl.Value = value;
                tileLength = (int)TileLengthControl.Value;
                Program.TileLength = tileLength;
                return;
            }

            if (value > tileLength)
            {
                TileLengthControl.Value = tileLength * 2;
            }
            else
            {
                TileLengthControl.Value = tileLength / 2;
            }

            tileLength = (int)TileLengthControl.Value;
            Program.TileLength = tileLength;
        }
    }
}
