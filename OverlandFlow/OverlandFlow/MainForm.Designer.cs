namespace OpenChannelFlow
{
    partial class MainForm
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
            this.Display = new System.Windows.Forms.PictureBox();
            this.WaterFlow = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Drain = new System.Windows.Forms.CheckBox();
            this.FlowSpeed = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.Display)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WaterFlow)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FlowSpeed)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Display
            // 
            this.Display.Location = new System.Drawing.Point(3, 3);
            this.Display.Name = "Display";
            this.Display.Size = new System.Drawing.Size(5800, 600);
            this.Display.TabIndex = 0;
            this.Display.TabStop = false;
            this.Display.Paint += new System.Windows.Forms.PaintEventHandler(this.Display_Paint);
            // 
            // WaterFlow
            // 
            this.WaterFlow.DecimalPlaces = 1;
            this.WaterFlow.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.WaterFlow.Location = new System.Drawing.Point(6, 32);
            this.WaterFlow.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.WaterFlow.Name = "WaterFlow";
            this.WaterFlow.Size = new System.Drawing.Size(120, 20);
            this.WaterFlow.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "In Flow (m3/s)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Drain);
            this.groupBox1.Controls.Add(this.FlowSpeed);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.WaterFlow);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 64);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // Drain
            // 
            this.Drain.AutoSize = true;
            this.Drain.Location = new System.Drawing.Point(258, 33);
            this.Drain.Name = "Drain";
            this.Drain.Size = new System.Drawing.Size(51, 17);
            this.Drain.TabIndex = 5;
            this.Drain.Text = "Drain";
            this.Drain.UseVisualStyleBackColor = true;
            // 
            // FlowSpeed
            // 
            this.FlowSpeed.DecimalPlaces = 1;
            this.FlowSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.FlowSpeed.Location = new System.Drawing.Point(132, 32);
            this.FlowSpeed.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.FlowSpeed.Name = "FlowSpeed";
            this.FlowSpeed.Size = new System.Drawing.Size(120, 20);
            this.FlowSpeed.TabIndex = 4;
            this.FlowSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(129, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Flow speed (m/s)";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.Display);
            this.panel1.Location = new System.Drawing.Point(12, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 635);
            this.panel1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Overland Flow";
            ((System.ComponentModel.ISupportInitialize)(this.Display)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WaterFlow)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FlowSpeed)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Display;
        private System.Windows.Forms.NumericUpDown WaterFlow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown FlowSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox Drain;
        private System.Windows.Forms.Panel panel1;
    }
}

