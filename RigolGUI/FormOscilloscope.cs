using FFTWSharp;
using RigolLib;
using SharpGL;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RigolGUI
{
    public partial class FormOscilloscope : Form
    {
        private readonly Oscilloscope oscilloscope;

        private volatile Waveform waveform = Waveform.EMPTY;
        private readonly object waveformLock = new object();

        private volatile Waveform fftWaveform = Waveform.EMPTY;
        private readonly object fftWaveformLock = new object();

        private double fps = 0;

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

                DateTime start = DateTime.UtcNow;

                Waveform newWaveform;
                try
                {
                    newWaveform = oscilloscope.QueryWaveform();
                }
                catch
                {
                    continue;
                }

                lock (waveformLock)
                {
                    this.waveform = newWaveform;
                }

                PerformFFT();

                fps = 1000D / (DateTime.UtcNow - start).TotalMilliseconds;
            }
        }

        double[] fftIn, fftOut;
        GCHandle h_fftIn, h_fftOut;
        IntPtr fftPlan = IntPtr.Zero;

        private void InitFFT(int size)
        {
            fftIn = new double[size * 2];
            fftOut = new double[size * 2];
            h_fftIn = GCHandle.Alloc(fftIn, GCHandleType.Pinned);
            h_fftOut = GCHandle.Alloc(fftOut, GCHandleType.Pinned);
            fftPlan = fftw.dft_1d(size, h_fftIn.AddrOfPinnedObject(), h_fftOut.AddrOfPinnedObject(), fftw_direction.Forward, fftw_flags.Measure);
        }

        public new void Dispose()
        {
            base.Dispose();

            fftw.destroy_plan(fftPlan);

            h_fftIn.Free();
            h_fftOut.Free();
        }

        private void PerformFFT()
        {
            if (fftPlan == IntPtr.Zero)
                InitFFT(1200);

            Waveform.Point[] points = this.waveform.Points;

            if (points.Length != 1200)
                return;

            lock(fftWaveformLock)
            {
                for (int x = 0; x < 1200; x++)
                {
                    fftIn[x * 2 + 1] = 0;
                    fftIn[x * 2] = points[x].Y;
                }

                fftw.execute(fftPlan);

                Waveform.Point[] fftPoints = new Waveform.Point[1200];

                for (int x = 0; x < 1200; x++)
                {
                    double yR = fftOut[x * 2];
                    double yI = fftOut[x * 2 + 1];
                    double y = Math.Sqrt(yR * yR + yI * yI);
                    fftPoints[x] = new Waveform.Point(x, y, x, y);
                }

                fftWaveform = new Waveform("Hz", "V", fftPoints);
            }
        }

        private void RenderWaveform(Waveform waveform, OpenGLControl openGLControl)
        {
            OpenGL gl = openGLControl.OpenGL;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            Waveform.Point[] points = waveform.Points;

            gl.Ortho2D(0, points.Length, 0, 255);
            gl.Viewport(0, 0, mainGraph.Width, mainGraph.Height);

            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Color(1.0f, 0.0f, 0.0f);

            foreach (Waveform.Point point in points)
            {
                gl.Vertex(point.RawX, point.RawY);
            }

            gl.End();

            gl.Flush();
        }

        private void mainGraph_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            RenderWaveform(waveform, mainGraph);
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshWaveformData();
        }

        private void tmRefresh_Tick(object sender, EventArgs e)
        {
            this.Text = "Oscilloscope (" + oscilloscope.Idendity + ") [" + fps + " fps]";
        }

        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWaveformData();
        }

        private void fttGraph_OpenGLDraw(object sender, RenderEventArgs args)
        {
            RenderWaveform(fftWaveform, fftGraph);
        }
    }
}
