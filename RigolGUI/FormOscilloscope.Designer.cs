namespace RigolGUI
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
            this.mainGraph = new SharpGL.OpenGLControl();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mainGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // mainGraph
            // 
            this.mainGraph.DrawFPS = false;
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
            // FormOscilloscope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 516);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.mainGraph);
            this.Name = "FormOscilloscope";
            this.Text = "Oscilloscope";
            ((System.ComponentModel.ISupportInitialize)(this.mainGraph)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpGL.OpenGLControl mainGraph;
        private System.Windows.Forms.Button btnRefresh;
    }
}

