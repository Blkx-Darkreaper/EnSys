namespace MapMaker
{
    partial class MapPropertiesForm
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
            this.AuthorInput = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.DateDisplay = new System.Windows.Forms.Label();
            this.TilesetNameLabel = new System.Windows.Forms.Label();
            this.LoadTilesetButton = new System.Windows.Forms.Button();
            this.MapHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.MapWidth = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.MapHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Author:";
            // 
            // AuthorInput
            // 
            this.AuthorInput.Location = new System.Drawing.Point(9, 33);
            this.AuthorInput.Name = "AuthorInput";
            this.AuthorInput.Size = new System.Drawing.Size(100, 20);
            this.AuthorInput.TabIndex = 1;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(12, 188);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 2;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(197, 188);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Date Created:";
            // 
            // DateDisplay
            // 
            this.DateDisplay.AutoSize = true;
            this.DateDisplay.Location = new System.Drawing.Point(140, 40);
            this.DateDisplay.Name = "DateDisplay";
            this.DateDisplay.Size = new System.Drawing.Size(30, 13);
            this.DateDisplay.TabIndex = 5;
            this.DateDisplay.Text = "Date";
            // 
            // TilesetNameLabel
            // 
            this.TilesetNameLabel.AutoSize = true;
            this.TilesetNameLabel.Location = new System.Drawing.Point(98, 63);
            this.TilesetNameLabel.Name = "TilesetNameLabel";
            this.TilesetNameLabel.Size = new System.Drawing.Size(35, 13);
            this.TilesetNameLabel.TabIndex = 14;
            this.TilesetNameLabel.Text = "label3";
            // 
            // LoadTilesetButton
            // 
            this.LoadTilesetButton.Location = new System.Drawing.Point(6, 58);
            this.LoadTilesetButton.Name = "LoadTilesetButton";
            this.LoadTilesetButton.Size = new System.Drawing.Size(86, 23);
            this.LoadTilesetButton.TabIndex = 13;
            this.LoadTilesetButton.Text = "Load tileset...";
            this.LoadTilesetButton.UseVisualStyleBackColor = true;
            this.LoadTilesetButton.Click += new System.EventHandler(this.LoadTilesetButton_Click);
            // 
            // MapHeight
            // 
            this.MapHeight.Location = new System.Drawing.Point(132, 32);
            this.MapHeight.Maximum = new decimal(new int[] {
            32000,
            0,
            0,
            0});
            this.MapHeight.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.MapHeight.Name = "MapHeight";
            this.MapHeight.Size = new System.Drawing.Size(120, 20);
            this.MapHeight.TabIndex = 12;
            this.MapHeight.Value = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.MapHeight.ValueChanged += new System.EventHandler(this.MapHeight_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(132, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Height";
            // 
            // MapWidth
            // 
            this.MapWidth.Location = new System.Drawing.Point(6, 32);
            this.MapWidth.Maximum = new decimal(new int[] {
            3200,
            0,
            0,
            0});
            this.MapWidth.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.MapWidth.Name = "MapWidth";
            this.MapWidth.Size = new System.Drawing.Size(120, 20);
            this.MapWidth.TabIndex = 10;
            this.MapWidth.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.MapWidth.ValueChanged += new System.EventHandler(this.MapWidth_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Width";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TilesetNameLabel);
            this.groupBox1.Controls.Add(this.MapWidth);
            this.groupBox1.Controls.Add(this.LoadTilesetButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.MapHeight);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 92);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.AuthorInput);
            this.groupBox2.Controls.Add(this.DateDisplay);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 72);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Misc";
            // 
            // MapPropertiesForm
            // 
            this.AcceptButton = this.SaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 223);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Name = "MapPropertiesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Map Properties";
            ((System.ComponentModel.ISupportInitialize)(this.MapHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AuthorInput;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label DateDisplay;
        private System.Windows.Forms.Label TilesetNameLabel;
        private System.Windows.Forms.Button LoadTilesetButton;
        private System.Windows.Forms.NumericUpDown MapHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown MapWidth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}