using RigolLib;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace RigolGUI
{
    public partial class FormOscilloscope : Form
    {
        private readonly Oscilloscope oscilloscope;
        private volatile Waveform waveform = Waveform.EMPTY;
        private readonly object waveformLock = new object();

        public FormOscilloscope(Oscilloscope oscilloscope)
        {
            this.oscilloscope = oscilloscope;

            InitializeComponent();

            this.cbChannel.SelectedIndex = 0;

            this.Text = "Oscilloscope (" + oscilloscope.Idendity + ")";

            RefreshWaveformData(false);

            new Thread(WaveformThread).Start();
        }

        private void RefreshWaveformData(bool fast = true)
        {
            oscilloscope.SetWaveformConfig(cbChannel.Text, false, false, fast);
        }

        private void WaveformThread()
        {
            while (!this.IsDisposed)
            {
                if (!this.Visible)
                {
                    continue;
                }

                Waveform newWaveform = oscilloscope.QueryWaveform();

                lock (waveformLock)
                {
                    this.waveform = newWaveform;
                }
            }
        }

        private void mainGraph_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            OpenGL gl = mainGraph.OpenGL;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            Waveform.Point[] points;
            lock (waveformLock)
            {
                points = waveform.Points;
            }

            gl.Ortho2D(0, points.Length, 0, 255);
            gl.Viewport(0, 0, mainGraph.Width, mainGraph.Height);

            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Color(1.0f, 0.0f, 0.0f);

            foreach(Waveform.Point point in points)
            {
                gl.Vertex(point.RawX, point.RawY);
            }

            gl.End();

            gl.Flush();
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshWaveformData();
        }

        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWaveformData();
        }
    }
}
