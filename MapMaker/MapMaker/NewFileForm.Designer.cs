namespace MapMaker
{
    partial class NewFileForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.NewFileDoneButton = new System.Windows.Forms.Button();
            this.MapWidth = new System.Windows.Forms.NumericUpDown();
            this.MapHeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.NewFileCancelButton = new System.Windows.Forms.Button();
            this.LoadTilesetButton = new System.Windows.Forms.Button();
            this.TilesetNameLabel = new System.Windows.Forms.Label();
            this.TileLengthControl = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.MapWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TileLengthControl)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Width";
            // 
            // NewFileDoneButton
            // 
            this.NewFileDoneButton.Location = new System.Drawing.Point(12, 149);
            this.NewFileDoneButton.Name = "NewFileDoneButton";
            this.NewFileDoneButton.Size = new System.Drawing.Size(75, 23);
            this.NewFileDoneButton.TabIndex = 2;
            this.NewFileDoneButton.Text = "Done";
            this.NewFileDoneButton.UseVisualStyleBackColor = true;
            this.NewFileDoneButton.Click += new System.EventHandler(this.NewFileDone_Click);
            // 
            // MapWidth
            // 
            this.MapWidth.Location = new System.Drawing.Point(6, 32);
            this.MapWidth.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.MapWidth.Name = "MapWidth";
            this.MapWidth.Size = new System.Drawing.Size(120, 20);
            this.MapWidth.TabIndex = 3;
            this.MapWidth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.MapWidth.ValueChanged += new System.EventHandler(this.MapWidth_ValueChanged);
            // 
            // MapHeight
            // 
            this.MapHeight.Location = new System.Drawing.Point(132, 32);
            this.MapHeight.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.MapHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MapHeight.Name = "MapHeight";
            this.MapHeight.Size = new System.Drawing.Size(120, 20);
            this.MapHeight.TabIndex = 5;
            this.MapHeight.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.MapHeight.ValueChanged += new System.EventHandler(this.MapHeight_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Height";
            // 
            // NewFileCancelButton
            // 
            this.NewFileCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NewFileCancelButton.Location = new System.Drawing.Point(197, 149);
            this.NewFileCancelButton.Name = "NewFileCancelButton";
            this.NewFileCancelButton.Size = new System.Drawing.Size(75, 23);
            this.NewFileCancelButton.TabIndex = 6;
            this.NewFileCancelButton.Text = "Cancel";
            this.NewFileCancelButton.UseVisualStyleBackColor = true;
            this.NewFileCancelButton.Click += new System.EventHandler(this.NewFileCancelButton_Click);
            // 
            // LoadTilesetButton
            // 
            this.LoadTilesetButton.Location = new System.Drawing.Point(6, 58);
            this.LoadTilesetButton.Name = "LoadTilesetButton";
            this.LoadTilesetButton.Size = new System.Drawing.Size(86, 23);
            this.LoadTilesetButton.TabIndex = 7;
            this.LoadTilesetButton.Text = "Load tileset...";
            this.LoadTilesetButton.UseVisualStyleBackColor = true;
            this.LoadTilesetButton.Click += new System.EventHandler(this.LoadTilesetButton_Click);
            // 
            // TilesetNameLabel
            // 
            this.TilesetNameLabel.AutoSize = true;
            this.TilesetNameLabel.Location = new System.Drawing.Point(98, 63);
            this.TilesetNameLabel.Name = "TilesetNameLabel";
            this.TilesetNameLabel.Size = new System.Drawing.Size(35, 13);
            this.TilesetNameLabel.TabIndex = 8;
            this.TilesetNameLabel.Text = "label3";
            // 
            // TileLengthControl
            // 
            this.TileLengthControl.Location = new System.Drawing.Point(6, 100);
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
            this.TileLengthControl.TabIndex = 9;
            this.TileLengthControl.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.TileLengthControl.ValueChanged += new System.EventHandler(this.TileLengthControl_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Tile Size (px)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.MapWidth);
            this.groupBox1.Controls.Add(this.TileLengthControl);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.TilesetNameLabel);
            this.groupBox1.Controls.Add(this.MapHeight);
            this.groupBox1.Controls.Add(this.LoadTilesetButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 131);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map";
            // 
            // NewFileForm
            // 
            this.AcceptButton = this.NewFileDoneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.NewFileCancelButton;
            this.ClientSize = new System.Drawing.Size(287, 183);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.NewFileCancelButton);
            this.Controls.Add(this.NewFileDoneButton);
            this.Name = "NewFileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New File";
            ((System.ComponentModel.ISupportInitialize)(this.MapWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TileLengthControl)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button NewFileDoneButton;
        private System.Windows.Forms.NumericUpDown MapWidth;
        private System.Windows.Forms.NumericUpDown MapHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button NewFileCancelButton;
        private System.Windows.Forms.Button LoadTilesetButton;
        private System.Windows.Forms.Label TilesetNameLabel;
        private System.Windows.Forms.NumericUpDown TileLengthControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}