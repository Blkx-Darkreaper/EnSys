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
    public partial class MapPropertiesForm : Form
    {
        public Size MapSize { get; protected set; }
        public string TilesetFilename { get; protected set; }
        public string Author { get; protected set; }
        protected int tileLength { get; set; }

        public MapPropertiesForm(Size mapSize, string tilesetFilename, int tileLength, string dateCreated)
        {
            InitializeComponent();
            this.MapSize = mapSize;
            this.TilesetFilename = tilesetFilename;
            this.tileLength = tileLength;

            MapWidth.Value = mapSize.Width;
            MapHeight.Value = mapSize.Height;
            TilesetNameLabel.Text = tilesetFilename;
            DateDisplay.Text = dateCreated;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (TilesetFilename == null)
            {
                MessageBox.Show("You must select a tileset to load", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int width = (int)MapWidth.Value;
            int height = (int)MapHeight.Value;

            this.MapSize = new Size(width, height);

            this.Author = AuthorInput.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
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
            }

            int remainder = width % tileLength;
            if (remainder == 0)
            {
                MapWidth.Value = width;
                this.MapSize = new Size(width, height);
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
            }

            // Height can't be less than scaledWidth
            int heightMin = (int)MapWidth.Value;
            height = Program.ClampToMin(height, heightMin);

            int remainder = height % tileLength;
            if (remainder == 0)
            {
                MapHeight.Value = height;
                this.MapSize = new Size(width, height);
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

            MapHeight.Value = height;
            this.MapSize = new Size(width, height);
        }

        protected void LoadTilesetButton_Click(object sender, EventArgs e)
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
