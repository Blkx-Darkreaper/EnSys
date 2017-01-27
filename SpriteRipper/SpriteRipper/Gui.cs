using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SpriteRipper
{
    public partial class Gui : Form
    {
        protected static string loadedFilename { get; set; }
        protected const string INIT_TOTAL = "Initial total";
        protected const string FINAL_TOTAL = "Final total";
        protected const string DUPLICATES = "Duplicates";
        protected const string SORT_TIME = "Sort time (ms)";
        protected const string TIME_PER_TILE = "Time/Tile (ms)";
        protected float miniDisplayScale { get; set; }

        public Gui()
        {
            InitializeComponent();
            Status.Text = String.Empty;
        }

        protected void BackgroundProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            while (ProgressBar.Value < ProgressBar.Maximum)
            {
                int progress = Program.TasksComplete;
                BackgroundProgress.ReportProgress(progress);
            }
        }

        protected void BackgroundProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        protected void BackgroundProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBar.Value = ProgressBar.Maximum;
            ProgressBar.Enabled = false;
        }

        protected void TileSizeChanged()
        {
            EnableUpdating();
            Reset();
            SetOffsetMax();
        }

        protected void TileSize_ValueChanged(object sender, EventArgs e)
        {
            TileSizeChanged();
        }

        protected void Bits_ValueChanged(object sender, EventArgs e)
        {
            EnableSorting();
        }

        protected void Zoom_ValueChanged(object sender, EventArgs e)
        {
            DrawDisplay();
        }

        private void Padding_CheckedChanged(object sender, EventArgs e)
        {
            DrawDisplay();
        }

        protected void PatternThreshold_ValueChanged(object sender, EventArgs e)
        {
            EnableSorting();
        }

        protected void ColourThreshold_ValueChanged(object sender, EventArgs e)
        {
            EnableSorting();
        }

        protected void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the Filename of the selected file
            OpenFileDialog dialog = Program.GetFilenameToOpen();
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            string filename = dialog.FileName;

            if (filename == null)
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (filename.Equals(string.Empty))
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Program.ResetAll();

            int tileSize = (int)TileSize.Value;
            int offsetX = (int)OffsetX.Value;
            int offsetY = (int)OffsetY.Value;

            Program.LoadImage(filename, tileSize, offsetX, offsetY);

            DisableSaving();

            int tasks = Program.Images.GetSubImageTileCount(0);
            //StartProgressBar(tasks);

            int bitsPerColour = (int)Bits.Value;
            Program.LoadSubImage(bitsPerColour, tileSize);
            EnableSubImageSelection();

            DrawDisplay();
            DisableUpdating();

            int totalTiles = Program.GetTileCount();
            DisplayResult(InitTotal, totalTiles);

            EnableSorting();

            Status.Text = String.Format("Loaded {0}", filename);
            loadedFilename = filename;
        }

        protected void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TilesetDisplay.Image == null)
            {
                return;
            }
            if (Program.TilesetReady == false)
            {
                return;
            }

            // Get the Filename of the selected file
            SaveFileDialog dialog = Program.GetFilenameToSave();
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            string filename = dialog.FileName;

            if (filename == null)
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (filename.Equals(string.Empty))
            {
                MessageBox.Show("Invalid filename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ImageFormat format;
            try
            {
                format = Program.GetImageFormat(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid filetype.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Image tileset = GetTileset();
            tileset.Save(filename, format);

            Status.Text = String.Format("Saved {0}", filename);
        }

        protected void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected void Update_Click(object sender, EventArgs e)
        {
            int bitsPerColour = (int)Bits.Value;
            int tileSize = (int)TileSize.Value;

            int offsetX = (int)OffsetX.Value;
            int offsetY = (int)OffsetY.Value;

            Status.Text = String.Format("Displaying sub image");

            if (Program.Images == null)
            {
                Program.LoadImage(loadedFilename, tileSize, offsetX, offsetY);
            }

            int subImageIndex = (int)SubImageSelector.Value;
            int tasks = Program.Images.GetSubImageTileCount(subImageIndex);
            //StartProgressBar(tasks);

            Program.LoadSubImage(bitsPerColour, tileSize, subImageIndex);

            //int tasks = Program.GetTileCount(canvas.Width, canvas.Height, TileSize);

            DrawDisplay();
            DisableUpdating();

            int totalTiles = Program.GetTileCount();
            DisplayResult(InitTotal, totalTiles);

            EnableSorting();
        }

        protected void Sort_Click(object sender, EventArgs e)
        {
            int bitsPerColour = (int)Bits.Value;
            int tileSize = (int)TileSize.Value;
            float patternThreshold = (float)PatternThreshold.Value;
            float colourThreshold = (float)ColourThreshold.Value;

            Status.Text = String.Format("Sorting image tiles");

            //int tasks = Program.GetTileCount(canvas.Width, canvas.Height, TileSize);
            int tasks = Program.GetTileCount();
            //StartProgressBar(tasks);  //testing
            Program.SortTiles(patternThreshold, colourThreshold);

            int subImageIndex = (int)SubImageSelector.Value;
            Program.Images.SetSubImageSorted(subImageIndex, true);
            DrawMiniDisplay();

            EnableTilesetBuild();
        }

        protected void BuildTileset_Click(object sender, EventArgs e)
        {
            DrawTileset();

            EnableSaving();
            Sort.Enabled = false;

            int totalAfterSorting = Program.GetSortedTileCount();
            DisplayResult(FinalTotal, totalAfterSorting);

            int duplicates = Program.Duplicates;
            DisplayResult(Duplicates, duplicates);

            long sortTime = Program.SortTime;
            DisplayResult(SortTime, sortTime);

            int totalTiles = Program.GetTileCount();
            double timePerTile = Math.Round((double)sortTime / (double)totalTiles, 2);
            DisplayResult(TimePerTile, timePerTile);

            DisableTilesetBuild();
        }

        protected void DisplayResult(Label label, double result)
        {
            string resultName = string.Empty;
            string resultValue = String.Format("{0}", result);

            string name = label.Name;
            switch (name)
            {
                case "InitTotal":
                    resultName = INIT_TOTAL;
                    break;

                case "FinalTotal":
                    resultName = FINAL_TOTAL;
                    break;

                case "Duplicates":
                    resultName = DUPLICATES;
                    break;

                case "SortTime":
                    resultName = SORT_TIME;
                    resultValue = result.ToString("#,##0");
                    break;

                case "TimePerTile":
                    resultName = TIME_PER_TILE;
                    resultValue = String.Format("{0:N2}", result);
                    break;

                default:
                    return;
            }

            label.Text = String.Format("{0}: {1}", resultName, resultValue);
        }

        protected void DrawDisplay()
        {
            if (Program.DisplayReady == false)
            {
                return;
            }

            int tileSize = (int)TileSize.Value;

            int offsetX = (int)OffsetX.Value;
            int offsetY = (int)OffsetY.Value;

            int zoom = (int)ImageZoom.Value;

            //int subImageIndex = (int)SubImageSelector.Value;

            Size subImageSize = Program.Images.CurrentSubImageSize;
            if (subImageSize == null)
            {
                throw new NullReferenceException("No sub image loaded");
            }

            int width = subImageSize.Width;
            int height = subImageSize.Height;

            int tilesWide = width / tileSize;
            int tilesHigh = height / tileSize;

            width *= zoom;
            height *= zoom;

            bool addPadding = DisplayPadding.Checked;
            if (addPadding == true)
            {
                // increase TileSize to account for spaces between tiles
                width += tilesWide - 1;
                height += tilesHigh - 1;
            }

            Bitmap displayImage = new Bitmap(width, height);
            Program.DrawAllTilesOntoImage(ref displayImage, tilesWide, tileSize, zoom, addPadding);

            ImageDisplay.Width = width;
            ImageDisplay.Height = height;
            ImageDisplay.Image = displayImage;

            Status.Text = String.Format("Displayed image tiles");

            DrawMiniDisplay();
        }

        protected void DrawMiniDisplay()
        {
            if (Program.DisplayReady == false)
            {
                return;
            }

            int tileSize = (int)TileSize.Value;

            int subImageIndex = (int)SubImageSelector.Value;

            Size imageSize = Program.Images.CroppedImageSize;
            if (imageSize == null)
            {
                throw new NullReferenceException("No sub image loaded");
            }

            int imageWidth = imageSize.Width;
            int subImagesWide = Program.Images.SubImagesWide;

            int imageHeight = imageSize.Height;
            int subImagesHigh = Program.Images.TotalSubImages / subImagesWide;

            int miniDisplayWidth = MiniDisplay.Width;
            float widthScale = miniDisplayWidth / (float)imageWidth;

            int miniDisplayHeight = MiniDisplay.Height;
            float heightScale = miniDisplayHeight / (float)imageHeight;

            this.miniDisplayScale = Math.Min(widthScale, heightScale);

            Bitmap miniDisplayImage = new Bitmap(MiniDisplay.Width, MiniDisplay.Height);
            using (Graphics graphics = Graphics.FromImage(miniDisplayImage))
            {
                int scaledImageWidth = (int)Math.Round(imageWidth * miniDisplayScale, 0) - subImagesWide;
                int scaledImageHeight = (int)Math.Round(imageHeight * miniDisplayScale, 0) - subImagesHigh;
                Rectangle imageBounds = new Rectangle(0, 0, scaledImageWidth, scaledImageHeight);

                Pen black = new Pen(Brushes.Black);
                black.Alignment = PenAlignment.Inset;

                graphics.DrawRectangle(black, imageBounds);

                Font font = new Font("Arial", 12, FontStyle.Regular);

                Pen blue = new Pen(Brushes.Blue);
                blue.Alignment = PenAlignment.Inset;

                Brush green = Brushes.Lime;

                int currentSubImageIndex = Program.Images.CurrentSubImageIndex;

                int x, y;
                Size subImageSize = Program.Images.GetSubImageSize(0);
                int scaledOffsetX = (int)Math.Round(subImageSize.Width * miniDisplayScale, 0) - 1;
                int scaledOffsetY = (int)Math.Round(subImageSize.Height * miniDisplayScale, 0) - 1;
                int scaledSubImageWidth, scaledSubImageHeight;

                int totalSubImages = Program.Images.TotalSubImages;
                for (int i = 0; i < totalSubImages; i++)
                {
                    x = 1 + (i % subImagesWide) * scaledOffsetX;
                    y = 1 + (i / subImagesWide) * scaledOffsetY;

                    subImageSize = Program.Images.GetSubImageSize(i);
                    scaledSubImageWidth = (int)Math.Round(subImageSize.Width * miniDisplayScale, 0) - 1;
                    scaledSubImageHeight = (int)Math.Round(subImageSize.Height * miniDisplayScale, 0) - 1;

                    imageBounds = new Rectangle(x, y, scaledSubImageWidth, scaledSubImageHeight);

                    graphics.DrawRectangle(blue, imageBounds);
                    graphics.DrawString(i.ToString(), font, Brushes.Black, x, y);

                    bool isSorted = Program.Images.IsSubImageSorted(i);
                    if (isSorted == false)
                    {
                        continue;
                    }

                    Rectangle insetBounds = new Rectangle(x + 15, y + 15, scaledSubImageWidth - 30, scaledSubImageHeight - 30);
                    graphics.FillRectangle(green, insetBounds);
                }

                // Highlight selected sub canvas
                Pen red = new Pen(Brushes.Red);
                red.Alignment = PenAlignment.Inset;

                subImageSize = Program.Images.CurrentSubImageSize;
                scaledSubImageWidth = (int)Math.Round(subImageSize.Width * miniDisplayScale, 0) - 1;
                scaledSubImageHeight = (int)Math.Round(subImageSize.Height * miniDisplayScale, 0) - 1;

                x = 1 + (currentSubImageIndex % subImagesWide) * scaledOffsetX;
                y = 1 + (currentSubImageIndex / subImagesWide) * scaledOffsetY;
                imageBounds = new Rectangle(x, y, scaledSubImageWidth, scaledSubImageHeight);

                graphics.DrawRectangle(red, imageBounds);
            }

            MiniDisplay.Image = miniDisplayImage;
        }

        private void DrawTileset()
        {
            if (Program.TilesetReady == false)
            {
                return;
            }

            int zoom = (int)TilesetZoom.Value;

            Bitmap tileset = GetTileset(zoom);
            TilesetDisplay.Image = tileset;

            Status.Text = String.Format("Displayed tileset");
        }

        private void TilesetZoom_ValueChanged(object sender, EventArgs e)
        {
            DrawTileset();
        }

        protected Bitmap GetTileset()
        {
            return GetTileset(1);
        }

        protected Bitmap GetTileset(int zoom)
        {
            if (Program.TilesetReady == false)
            {
                return null;
            }

            int tileSize = (int)TileSize.Value;

            bool addPadding = TilesetPadding.Checked;

            PixelFormat format = Program.Images.GetPixelFormat();

            Bitmap tileset;
            if (Grouped.Checked == true)
            {
                tileset = Program.GetGroupedTileset(format, tileSize, zoom, addPadding);
            }
            else
            {
                int tilesWide = (int)TilesWide.Value;
                tileset = Program.GetTileset(format, tileSize, tilesWide, zoom, addPadding);
            }

            return tileset;
        }

        public void StartProgressBar(int tasks)
        {
            ProgressBar.Enabled = true;
            ProgressBar.Value = 0;
            //ProgressBar.Step = 1;
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = tasks;

            BackgroundProgress.WorkerReportsProgress = true;
            while (BackgroundProgress.IsBusy == true)
            {
                Thread.Sleep(100);
            }

            BackgroundProgress.RunWorkerAsync();
        }

        protected void EnableSaving()
        {
            SaveAsMenuItem.Enabled = true;
            SaveMenuItem.Enabled = true;
        }

        protected void DisableSaving()
        {
            SaveAsMenuItem.Enabled = false;
            SaveMenuItem.Enabled = false;
        }

        protected void Reset()
        {
            DisableSaving();
            DisableSorting();

            Program.ResetAll();
        }

        protected void EnableUpdating()
        {
            ImageCollection imageGroup = Program.Images;
            if (imageGroup == null)
            {
                return;
            }

            Update.Enabled = true;
            ProgressBar.Value = 0;
        }

        protected void DisableUpdating()
        {
            Update.Enabled = false;
        }

        protected void EnableSubImageSelection()
        {
            SubImageSelector.Enabled = true;
            SubImageSelector.Maximum = Program.Images.TotalSubImages - 1;
        }

        protected void EnableSorting()
        {
            int tilesToSort = Program.GetTileCount();
            if (tilesToSort == 0)
            {
                return;
            }
            if (Program.DisplayReady == false)
            {
                return;
            }

            int subImageIndex = (int)SubImageSelector.Value;
            bool alreadySorted = Program.Images.IsSubImageSorted(subImageIndex);
            if (alreadySorted == true)
            {
                return;
            }

            Sort.Enabled = true;
            ProgressBar.Value = 0;
        }

        protected void DisableSorting()
        {
            Sort.Enabled = false;
        }

        protected void EnableTilesetBuild()
        {
            int sortedTiles = Program.GetSortedTileCount();
            if (sortedTiles == 0)
            {
                return;
            }
            if (Program.TilesetReady == false)
            {
                return;
            }

            BuildTileset.Enabled = true;
            ProgressBar.Value = 0;
            DisableSorting();
        }

        protected void DisableTilesetBuild()
        {
            BuildTileset.Enabled = false;
        }

        protected void Preset8_CheckedChanged(object sender, EventArgs e)
        {
            TileSize.Value = 8;
            TileSizeChanged();
        }

        protected void Preset16_CheckedChanged(object sender, EventArgs e)
        {
            TileSize.Value = 16;
            TileSizeChanged();
        }

        protected void Preset32_CheckedChanged(object sender, EventArgs e)
        {
            TileSize.Value = 32;
            TileSizeChanged();
        }

        protected void Preset64_CheckedChanged(object sender, EventArgs e)
        {
            TileSize.Value = 64;
            TileSizeChanged();
        }

        protected void Preset128_CheckedChanged(object sender, EventArgs e)
        {
            TileSize.Value = 128;
            TileSizeChanged();
        }

        protected void Preset256_CheckedChanged(object sender, EventArgs e)
        {
            TileSize.Value = 256;
            TileSizeChanged();
        }

        protected void SetOffsetMax()
        {
            int tileSize = (int)TileSize.Value;
            int maxOffset = tileSize - 1;

            OffsetX.Maximum = maxOffset;
            OffsetY.Maximum = maxOffset;

            TilesetDisplay.Image = null;
            Program.ResetTileset();
        }

        protected void OffsetX_ValueChanged(object sender, EventArgs e)
        {
            EnableUpdating();
            Reset();
        }

        protected void OffsetY_ValueChanged(object sender, EventArgs e)
        {
            EnableUpdating();
            Reset();
        }

        protected void Grouped_CheckedChanged(object sender, EventArgs e)
        {
            TilesWide.Enabled = false;
            EnableTilesetBuild();
        }

        protected void Compressed_CheckedChanged(object sender, EventArgs e)
        {
            TilesWide.Enabled = true;
            EnableTilesetBuild();
        }

        private void BackgroundColour_Click(object sender, EventArgs e)
        {
            DialogResult result = ColourPicker.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            BackgroundColour.BackColor = ColourPicker.Color;
            Program.BackgroundColour = ColourPicker.Color;

            EnableTilesetBuild();
        }

        private void TilesWide_ValueChanged(object sender, EventArgs e)
        {
            EnableTilesetBuild();
        }

        private void SubImageSelector_ValueChanged(object sender, EventArgs e)
        {
            EnableUpdating();
        }

        private void TilesetPadding_CheckedChanged(object sender, EventArgs e)
        {
            EnableTilesetBuild();
        }
    }
}