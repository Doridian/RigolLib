﻿namespace RigolGUI
{
    partial class FormOscilloscope
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
            this.components = new System.ComponentModel.Container();
            this.mainGraph = new SharpGL.OpenGLControl();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.fftGraph = new SharpGL.OpenGLControl();
            this.tmRefresh = new System.Windows.Forms.Timer(this.components);
            this.lblWaveformFPS = new System.Windows.Forms.Label();
            this.cbChan1 = new System.Windows.Forms.CheckBox();
            this.cbChan2 = new System.Windows.Forms.CheckBox();
            this.cbChan3 = new System.Windows.Forms.CheckBox();
            this.cbChan4 = new System.Windows.Forms.CheckBox();
            this.cbMath = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainGraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // mainGraph
            // 
            this.mainGraph.DrawFPS = false;
            this.mainGraph.FrameRate = 60;
            this.mainGraph.Location = new System.Drawing.Point(12, 55);
            this.mainGraph.Name = "mainGraph";
            this.mainGraph.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.mainGraph.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.mainGraph.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.mainGraph.Size = new System.Drawing.Size(616, 237);
            this.mainGraph.TabIndex = 0;
            this.mainGraph.TabStop = false;
            this.mainGraph.OpenGLDraw += new SharpGL.RenderEventHandler(this.mainGraph_OpenGLDraw);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(154, 37);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh waveform settings";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cbChannel
            // 
            this.cbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.Items.AddRange(new object[] {
            "CHAN1",
            "CHAN2",
            "CHAN3",
            "CHAN4",
            "MATH"});
            this.cbChannel.Location = new System.Drawing.Point(172, 21);
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.Size = new System.Drawing.Size(185, 21);
            this.cbChannel.TabIndex = 4;
            this.cbChannel.SelectedIndexChanged += new System.EventHandler(this.cbChannel_SelectedIndexChanged);
            // 
            // fftGraph
            // 
            this.fftGraph.DrawFPS = false;
            this.fftGraph.FrameRate = 60;
            this.fftGraph.Location = new System.Drawing.Point(12, 298);
            this.fftGraph.Name = "fftGraph";
            this.fftGraph.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.fftGraph.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.fftGraph.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.fftGraph.Size = new System.Drawing.Size(616, 237);
            this.fftGraph.TabIndex = 5;
            this.fftGraph.TabStop = false;
            this.fftGraph.OpenGLDraw += new SharpGL.RenderEventHandler(this.fttGraph_OpenGLDraw);
            // 
            // tmRefresh
            // 
            this.tmRefresh.Enabled = true;
            this.tmRefresh.Tick += new System.EventHandler(this.tmRefresh_Tick);
            // 
            // lblWaveformFPS
            // 
            this.lblWaveformFPS.AutoSize = true;
            this.lblWaveformFPS.Location = new System.Drawing.Point(634, 55);
            this.lblWaveformFPS.Name = "lblWaveformFPS";
            this.lblWaveformFPS.Size = new System.Drawing.Size(35, 13);
            this.lblWaveformFPS.TabIndex = 6;
            this.lblWaveformFPS.Text = "label1";
            // 
            // cbChan1
            // 
            this.cbChan1.AutoSize = true;
            this.cbChan1.Location = new System.Drawing.Point(637, 85);
            this.cbChan1.Name = "cbChan1";
            this.cbChan1.Size = new System.Drawing.Size(74, 17);
            this.cbChan1.TabIndex = 7;
            this.cbChan1.Text = "Channel 1";
            this.cbChan1.UseVisualStyleBackColor = true;
            // 
            // cbChan2
            // 
            this.cbChan2.AutoSize = true;
            this.cbChan2.Location = new System.Drawing.Point(635, 109);
            this.cbChan2.Name = "cbChan2";
            this.cbChan2.Size = new System.Drawing.Size(74, 17);
            this.cbChan2.TabIndex = 8;
            this.cbChan2.Text = "Channel 2";
            this.cbChan2.UseVisualStyleBackColor = true;
            // 
            // cbChan3
            // 
            this.cbChan3.AutoSize = true;
            this.cbChan3.Location = new System.Drawing.Point(635, 133);
            this.cbChan3.Name = "cbChan3";
            this.cbChan3.Size = new System.Drawing.Size(74, 17);
            this.cbChan3.TabIndex = 9;
            this.cbChan3.Text = "Channel 3";
            this.cbChan3.UseVisualStyleBackColor = true;
            // 
            // cbChan4
            // 
            this.cbChan4.AutoSize = true;
            this.cbChan4.Location = new System.Drawing.Point(635, 157);
            this.cbChan4.Name = "cbChan4";
            this.cbChan4.Size = new System.Drawing.Size(74, 17);
            this.cbChan4.TabIndex = 10;
            this.cbChan4.Text = "Channel 4";
            this.cbChan4.UseVisualStyleBackColor = true;
            // 
            // cbMath
            // 
            this.cbMath.AutoSize = true;
            this.cbMath.Location = new System.Drawing.Point(634, 180);
            this.cbMath.Name = "cbMath";
            this.cbMath.Size = new System.Drawing.Size(50, 17);
            this.cbMath.TabIndex = 11;
            this.cbMath.Text = "Math";
            this.cbMath.UseVisualStyleBackColor = true;
            // 
            // FormOscilloscope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 591);
            this.Controls.Add(this.cbMath);
            this.Controls.Add(this.cbChan4);
            this.Controls.Add(this.cbChan3);
            this.Controls.Add(this.cbChan2);
            this.Controls.Add(this.cbChan1);
            this.Controls.Add(this.lblWaveformFPS);
            this.Controls.Add(this.fftGraph);
            this.Controls.Add(this.cbChannel);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.mainGraph);
            this.Name = "FormOscilloscope";
            this.Text = "Oscilloscope";
            this.Load += new System.EventHandler(this.FormOscilloscope_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mainGraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fftGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpGL.OpenGLControl mainGraph;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cbChannel;
        private SharpGL.OpenGLControl fftGraph;
        private System.Windows.Forms.Timer tmRefresh;
        private System.Windows.Forms.Label lblWaveformFPS;
        private System.Windows.Forms.CheckBox cbChan1;
        private System.Windows.Forms.CheckBox cbChan2;
        private System.Windows.Forms.CheckBox cbChan3;
        private System.Windows.Forms.CheckBox cbChan4;
        private System.Windows.Forms.CheckBox cbMath;
    }
}

