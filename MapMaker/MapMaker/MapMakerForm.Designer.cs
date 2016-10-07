namespace MapMaker
{
    partial class MapMakerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated resolutionCode

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the resolutionCode editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.constructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drivableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flyableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuPanel = new System.Windows.Forms.Panel();
            this.SectorChkptToggle = new System.Windows.Forms.CheckBox();
            this.AddToSectorButton = new System.Windows.Forms.Button();
            this.FillTool = new System.Windows.Forms.Button();
            this.RectangleTool = new System.Windows.Forms.Button();
            this.LineTool = new System.Windows.Forms.Button();
            this.PenTool = new System.Windows.Forms.Button();
            this.SaveFileButton = new System.Windows.Forms.Button();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.NewFileButton = new System.Windows.Forms.Button();
            this.TileLengthControl = new System.Windows.Forms.NumericUpDown();
            this.ZoomX4 = new System.Windows.Forms.Button();
            this.ZoomX2 = new System.Windows.Forms.Button();
            this.ZoomX1 = new System.Windows.Forms.Button();
            this.MapPanel = new System.Windows.Forms.Panel();
            this.MapDisplay = new System.Windows.Forms.PictureBox();
            this.TilesetPanel = new System.Windows.Forms.Panel();
            this.TilesetDisplay = new System.Windows.Forms.PictureBox();
            this.MiniMapDisplay = new MapMaker.MiniMap();
            this.MainMenu.SuspendLayout();
            this.MenuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TileLengthControl)).BeginInit();
            this.MapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapDisplay)).BeginInit();
            this.TilesetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TilesetDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MiniMapDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(624, 24);
            this.MainMenu.TabIndex = 2;
            this.MainMenu.Text = "MenuBar";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noOverlayToolStripMenuItem,
            this.sectorsToolStripMenuItem,
            this.constructionToolStripMenuItem,
            this.drivableToolStripMenuItem,
            this.flyableToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // noOverlayToolStripMenuItem
            // 
            this.noOverlayToolStripMenuItem.Name = "noOverlayToolStripMenuItem";
            this.noOverlayToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.noOverlayToolStripMenuItem.Text = "Map";
            this.noOverlayToolStripMenuItem.Click += new System.EventHandler(this.noOverlayToolStripMenuItem_Click);
            // 
            // sectorsToolStripMenuItem
            // 
            this.sectorsToolStripMenuItem.Name = "sectorsToolStripMenuItem";
            this.sectorsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.sectorsToolStripMenuItem.Text = "Sectors";
            this.sectorsToolStripMenuItem.Click += new System.EventHandler(this.sectorsToolStripMenuItem_Click);
            // 
            // constructionToolStripMenuItem
            // 
            this.constructionToolStripMenuItem.Name = "constructionToolStripMenuItem";
            this.constructionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.constructionToolStripMenuItem.Text = "Constructable";
            this.constructionToolStripMenuItem.Click += new System.EventHandler(this.constructionToolStripMenuItem_Click);
            // 
            // drivableToolStripMenuItem
            // 
            this.drivableToolStripMenuItem.Name = "drivableToolStripMenuItem";
            this.drivableToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.drivableToolStripMenuItem.Text = "Drivable";
            this.drivableToolStripMenuItem.Click += new System.EventHandler(this.drivableToolStripMenuItem_Click);
            // 
            // flyableToolStripMenuItem
            // 
            this.flyableToolStripMenuItem.Name = "flyableToolStripMenuItem";
            this.flyableToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.flyableToolStripMenuItem.Text = "Flyable";
            this.flyableToolStripMenuItem.Click += new System.EventHandler(this.flyableToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayToolStripMenuItem,
            this.mapToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.displayToolStripMenuItem.Text = "Display";
            this.displayToolStripMenuItem.Click += new System.EventHandler(this.displayToolStripMenuItem_Click);
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.mapToolStripMenuItem.Text = "Map Properties";
            this.mapToolStripMenuItem.Click += new System.EventHandler(this.mapToolStripMenuItem_Click);
            // 
            // MenuPanel
            // 
            this.MenuPanel.Controls.Add(this.SectorChkptToggle);
            this.MenuPanel.Controls.Add(this.AddToSectorButton);
            this.MenuPanel.Controls.Add(this.FillTool);
            this.MenuPanel.Controls.Add(this.RectangleTool);
            this.MenuPanel.Controls.Add(this.LineTool);
            this.MenuPanel.Controls.Add(this.PenTool);
            this.MenuPanel.Controls.Add(this.SaveFileButton);
            this.MenuPanel.Controls.Add(this.LoadFileButton);
            this.MenuPanel.Controls.Add(this.NewFileButton);
            this.MenuPanel.Controls.Add(this.TileLengthControl);
            this.MenuPanel.Controls.Add(this.ZoomX4);
            this.MenuPanel.Controls.Add(this.ZoomX2);
            this.MenuPanel.Controls.Add(this.ZoomX1);
            this.MenuPanel.Location = new System.Drawing.Point(12, 27);
            this.MenuPanel.Name = "MenuPanel";
            this.MenuPanel.Size = new System.Drawing.Size(600, 35);
            this.MenuPanel.TabIndex = 4;
            // 
            // SectorChkptToggle
            // 
            this.SectorChkptToggle.AutoSize = true;
            this.SectorChkptToggle.Checked = true;
            this.SectorChkptToggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SectorChkptToggle.Enabled = false;
            this.SectorChkptToggle.Location = new System.Drawing.Point(492, 10);
            this.SectorChkptToggle.Name = "SectorChkptToggle";
            this.SectorChkptToggle.Size = new System.Drawing.Size(62, 17);
            this.SectorChkptToggle.TabIndex = 12;
            this.SectorChkptToggle.Text = "Sectors";
            this.SectorChkptToggle.UseVisualStyleBackColor = true;
            // 
            // AddToSectorButton
            // 
            this.AddToSectorButton.Enabled = false;
            this.AddToSectorButton.Location = new System.Drawing.Point(560, 6);
            this.AddToSectorButton.Name = "AddToSectorButton";
            this.AddToSectorButton.Size = new System.Drawing.Size(37, 23);
            this.AddToSectorButton.TabIndex = 11;
            this.AddToSectorButton.Text = "Add";
            this.AddToSectorButton.UseVisualStyleBackColor = true;
            this.AddToSectorButton.Click += new System.EventHandler(this.AddToSectorsButton_Click);
            // 
            // FillTool
            // 
            this.FillTool.Location = new System.Drawing.Point(429, 6);
            this.FillTool.Name = "FillTool";
            this.FillTool.Size = new System.Drawing.Size(27, 23);
            this.FillTool.TabIndex = 10;
            this.FillTool.Text = "Fill";
            this.FillTool.UseVisualStyleBackColor = true;
            this.FillTool.Click += new System.EventHandler(this.FillTool_Click);
            // 
            // RectangleTool
            // 
            this.RectangleTool.Location = new System.Drawing.Point(384, 6);
            this.RectangleTool.Name = "RectangleTool";
            this.RectangleTool.Size = new System.Drawing.Size(39, 23);
            this.RectangleTool.TabIndex = 9;
            this.RectangleTool.Text = "Rect";
            this.RectangleTool.UseVisualStyleBackColor = true;
            this.RectangleTool.Click += new System.EventHandler(this.RectangleTool_Click);
            // 
            // LineTool
            // 
            this.LineTool.Location = new System.Drawing.Point(341, 6);
            this.LineTool.Name = "LineTool";
            this.LineTool.Size = new System.Drawing.Size(37, 23);
            this.LineTool.TabIndex = 8;
            this.LineTool.Text = "Line";
            this.LineTool.UseVisualStyleBackColor = true;
            this.LineTool.Click += new System.EventHandler(this.LineTool_Click);
            // 
            // PenTool
            // 
            this.PenTool.Location = new System.Drawing.Point(301, 6);
            this.PenTool.Name = "PenTool";
            this.PenTool.Size = new System.Drawing.Size(34, 23);
            this.PenTool.TabIndex = 7;
            this.PenTool.Text = "Pen";
            this.PenTool.UseVisualStyleBackColor = true;
            this.PenTool.Click += new System.EventHandler(this.PenTool_Click);
            // 
            // SaveFileButton
            // 
            this.SaveFileButton.Location = new System.Drawing.Point(94, 6);
            this.SaveFileButton.Name = "SaveFileButton";
            this.SaveFileButton.Size = new System.Drawing.Size(44, 23);
            this.SaveFileButton.TabIndex = 6;
            this.SaveFileButton.Text = "Save";
            this.SaveFileButton.UseVisualStyleBackColor = true;
            this.SaveFileButton.Click += new System.EventHandler(this.SaveFileButton_Click);
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.Location = new System.Drawing.Point(46, 6);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(42, 23);
            this.LoadFileButton.TabIndex = 5;
            this.LoadFileButton.Text = "Load";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // NewFileButton
            // 
            this.NewFileButton.Location = new System.Drawing.Point(3, 6);
            this.NewFileButton.Name = "NewFileButton";
            this.NewFileButton.Size = new System.Drawing.Size(37, 23);
            this.NewFileButton.TabIndex = 4;
            this.NewFileButton.Text = "New";
            this.NewFileButton.UseVisualStyleBackColor = true;
            this.NewFileButton.Click += new System.EventHandler(this.NewFile_Click);
            // 
            // TileLengthControl
            // 
            this.TileLengthControl.Location = new System.Drawing.Point(260, 9);
            this.TileLengthControl.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.TileLengthControl.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.TileLengthControl.Name = "TileLengthControl";
            this.TileLengthControl.Size = new System.Drawing.Size(35, 20);
            this.TileLengthControl.TabIndex = 3;
            this.TileLengthControl.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.TileLengthControl.ValueChanged += new System.EventHandler(this.TileWidth_ValueChanged);
            // 
            // ZoomX4
            // 
            this.ZoomX4.Location = new System.Drawing.Point(221, 6);
            this.ZoomX4.Name = "ZoomX4";
            this.ZoomX4.Size = new System.Drawing.Size(33, 23);
            this.ZoomX4.TabIndex = 2;
            this.ZoomX4.Text = "1/4";
            this.ZoomX4.UseVisualStyleBackColor = true;
            this.ZoomX4.Click += new System.EventHandler(this.ScaleQuarter_Click);
            // 
            // ZoomX2
            // 
            this.ZoomX2.Location = new System.Drawing.Point(182, 6);
            this.ZoomX2.Name = "ZoomX2";
            this.ZoomX2.Size = new System.Drawing.Size(33, 23);
            this.ZoomX2.TabIndex = 1;
            this.ZoomX2.Text = "1/2";
            this.ZoomX2.UseVisualStyleBackColor = true;
            this.ZoomX2.Click += new System.EventHandler(this.ScaleHalf_Click);
            // 
            // ZoomX1
            // 
            this.ZoomX1.Location = new System.Drawing.Point(144, 6);
            this.ZoomX1.Name = "ZoomX1";
            this.ZoomX1.Size = new System.Drawing.Size(32, 23);
            this.ZoomX1.TabIndex = 0;
            this.ZoomX1.Text = "1/1";
            this.ZoomX1.UseVisualStyleBackColor = true;
            this.ZoomX1.Click += new System.EventHandler(this.ScaleNone_Click);
            // 
            // MapPanel
            // 
            this.MapPanel.AutoScroll = true;
            this.MapPanel.Controls.Add(this.MapDisplay);
            this.MapPanel.Location = new System.Drawing.Point(178, 68);
            this.MapPanel.Name = "MapPanel";
            this.MapPanel.Size = new System.Drawing.Size(434, 361);
            this.MapPanel.TabIndex = 5;
            this.MapPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.MapPanel_Scroll);
            // 
            // MapDisplay
            // 
            this.MapDisplay.Location = new System.Drawing.Point(3, 3);
            this.MapDisplay.MinimumSize = new System.Drawing.Size(8, 8);
            this.MapDisplay.Name = "MapDisplay";
            this.MapDisplay.Size = new System.Drawing.Size(100, 100);
            this.MapDisplay.TabIndex = 6;
            this.MapDisplay.TabStop = false;
            this.MapDisplay.Click += new System.EventHandler(this.MapDisplay_Click);
            this.MapDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.MapDisplay_Paint);
            this.MapDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseDown);
            this.MapDisplay.MouseEnter += new System.EventHandler(this.MapDisplay_MouseEnter);
            this.MapDisplay.MouseLeave += new System.EventHandler(this.MapDisplay_MouseLeave);
            this.MapDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseMove);
            this.MapDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapDisplay_MouseUp);
            // 
            // TilesetPanel
            // 
            this.TilesetPanel.AutoScroll = true;
            this.TilesetPanel.Controls.Add(this.TilesetDisplay);
            this.TilesetPanel.Location = new System.Drawing.Point(12, 234);
            this.TilesetPanel.Name = "TilesetPanel";
            this.TilesetPanel.Size = new System.Drawing.Size(160, 195);
            this.TilesetPanel.TabIndex = 6;
            // 
            // TilesetDisplay
            // 
            this.TilesetDisplay.Location = new System.Drawing.Point(3, 3);
            this.TilesetDisplay.Name = "TilesetDisplay";
            this.TilesetDisplay.Size = new System.Drawing.Size(128, 117);
            this.TilesetDisplay.TabIndex = 7;
            this.TilesetDisplay.TabStop = false;
            this.TilesetDisplay.Click += new System.EventHandler(this.TilesetDisplay_Click);
            // 
            // MiniMapDisplay
            // 
            this.MiniMapDisplay.Location = new System.Drawing.Point(12, 68);
            this.MiniMapDisplay.Name = "MiniMapDisplay";
            this.MiniMapDisplay.scale = 0D;
            this.MiniMapDisplay.Size = new System.Drawing.Size(160, 160);
            this.MiniMapDisplay.TabIndex = 3;
            this.MiniMapDisplay.TabStop = false;
            this.MiniMapDisplay.ViewBox = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // MapMakerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.TilesetPanel);
            this.Controls.Add(this.MapPanel);
            this.Controls.Add(this.MenuPanel);
            this.Controls.Add(this.MiniMapDisplay);
            this.Controls.Add(this.MainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MapMakerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MapMaker";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapMakerForm_KeyDown);
            this.Resize += new System.EventHandler(this.MapMaker_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.MenuPanel.ResumeLayout(false);
            this.MenuPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TileLengthControl)).EndInit();
            this.MapPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MapDisplay)).EndInit();
            this.TilesetPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TilesetDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MiniMapDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        public MiniMap MiniMapDisplay;
        private System.Windows.Forms.Panel MenuPanel;
        private System.Windows.Forms.Panel MapPanel;
        public System.Windows.Forms.PictureBox MapDisplay;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.Panel TilesetPanel;
        public System.Windows.Forms.PictureBox TilesetDisplay;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.Button ZoomX4;
        private System.Windows.Forms.Button ZoomX2;
        private System.Windows.Forms.Button ZoomX1;
        private System.Windows.Forms.NumericUpDown TileLengthControl;
        private System.Windows.Forms.Button SaveFileButton;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.Button NewFileButton;
        private System.Windows.Forms.Button PenTool;
        private System.Windows.Forms.Button LineTool;
        private System.Windows.Forms.Button FillTool;
        private System.Windows.Forms.Button RectangleTool;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem constructionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sectorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drivableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flyableToolStripMenuItem;
        private System.Windows.Forms.Button AddToSectorButton;
        private System.Windows.Forms.CheckBox SectorChkptToggle;
    }
}

