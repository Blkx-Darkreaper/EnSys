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
        public int TileLength { get; protected set; }

        public NewFileForm()
        {
            InitializeComponent();
            this.TilesetNameLabel.Text = string.Empty;

            int initWidth = (int)this.MapWidth.Value;
            int initHeight = (int)this.MapWidth.Value;
            this.MapSize = new Size(initWidth, initHeight);
            this.TilesetFilename = null;
            this.TileLength = (int)this.TileLengthControl.Value;
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
            int height = MapSize.Height;
            int oldWidth = MapSize.Width;

            // Width cannot exceed height
            if (width > height)
            {
                width = height;
                MapWidth.Value = width;
            }

            this.MapSize = new Size(width, height);
        }

        protected void MapHeight_ValueChanged(object sender, EventArgs e)
        {
            int height = (int)MapHeight.Value;
            int width = MapSize.Width;
            int oldHeight = MapSize.Height;

            // Height cannot deceed width
            if (height < width)
            {
                height = width;
                MapHeight.Value = height;
            }

            this.MapSize = new Size(width, height);
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
