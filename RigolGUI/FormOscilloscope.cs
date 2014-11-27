using RigolLib;
using SharpGL;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace RigolGUI
{
    public partial class FormOscilloscope : Form
    {
        private readonly Oscilloscope oscilloscope;
        private volatile Waveform waveform;
        private readonly object waveformLock = new object();

        public FormOscilloscope(Oscilloscope oscilloscope)
        {
            this.oscilloscope = oscilloscope;

            RefreshWaveformData();

            InitializeComponent();

            new Thread(WaveformThread).Start();
        }

        private void RefreshWaveformData()
        {
            Waveform newWaveform = oscilloscope.GetWaveform(1, false, false);
            lock (waveformLock)
            {
                this.waveform = newWaveform;
            }
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

            gl.Ortho2D(0, points.Length, -3, 3);
            gl.Viewport(0, 0, mainGraph.Width, mainGraph.Height);

            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Color(1.0f, 0.0f, 0.0f);

            for (int x = 0; x < points.Length; x++)
            {
                Waveform.Point point = points[x];
                gl.Vertex(x, point.Y);
            }

            gl.End();

            gl.Flush();
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshWaveformData();
        }
    }
}
