﻿using FFTWSharp;
using RigolLib;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RigolGUI
{
    public partial class FormOscilloscope : Form
    {
        private readonly Oscilloscope oscilloscope;

        private volatile IEnumerable<Waveform> waveforms = new Waveform[] { };

        private volatile IEnumerable<Waveform> fftWaveforms = new Waveform[] { };

        private double fps = 0;

        public FormOscilloscope(Oscilloscope oscilloscope)
        {
            this.oscilloscope = oscilloscope;

            InitializeComponent();

            cbChannel.SelectedIndex = 0;

            Text = "Oscilloscope (" + oscilloscope.Idendity + ")";

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

                try
                {
                    List<Waveform> tmpWaveforms = new List<Waveform>();
                    if (cbChan1.Checked)
                    {
                        tmpWaveforms.Add(oscilloscope.QueryWaveform("CHAN1"));
                    }
                    if (cbChan2.Checked)
                    {
                        tmpWaveforms.Add(oscilloscope.QueryWaveform("CHAN2"));
                    }
                    if (cbChan3.Checked)
                    {
                        tmpWaveforms.Add(oscilloscope.QueryWaveform("CHAN3"));
                    }
                    if (cbChan4.Checked)
                    {
                        tmpWaveforms.Add(oscilloscope.QueryWaveform("CHAN4"));
                    }
                    if (cbMath.Checked)
                    {
                        tmpWaveforms.Add(oscilloscope.QueryWaveform("MATH"));
                    }
                    waveforms = tmpWaveforms;
                }
                catch
                {
                    continue;
                }

                PerformFFT();

                double millis = (DateTime.UtcNow - start).TotalMilliseconds;
                fps = 1000D / millis;
                if (fps > 60)
                {
                    fps = 60;
                    Thread.Sleep((int)((1000D / 60D) - millis));
                }
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
            {
                InitFFT(1200);
            }

            List<Waveform> fftTmp = new List<Waveform>();
            foreach (Waveform waveform in waveforms)
            {
                Waveform.Point[] points = waveform.Points;

                if (points.Length != 1200)
                {
                    continue;
                }

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

                fftTmp.Add(new Waveform("Hz", "V", fftPoints, waveform.Channel));
            }

            fftWaveforms = fftTmp;
        }

        private void RenderWaveform(IEnumerable<Waveform> waveforms, OpenGLControl openGLControl)
        {
            OpenGL gl = openGLControl.OpenGL;

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            int maxPointsLength = 0;
            foreach (Waveform waveform in waveforms)
            {
                if (waveform.Points.Length > maxPointsLength)
                {
                    maxPointsLength = waveform.Points.Length;
                }
            }

            gl.Ortho2D(0, maxPointsLength, 0, 255);
            gl.Viewport(0, 0, mainGraph.Width, mainGraph.Height);

            foreach (Waveform waveform in waveforms)
            {
                gl.Begin(OpenGL.GL_LINE_STRIP);
                gl.Color(waveform.R, waveform.G, waveform.B, 0.9f);

                foreach (Waveform.Point point in waveform.Points)
                {
                    gl.Vertex(point.RawX, point.RawY);
                }

                gl.End();
            }

            gl.Flush();
        }

        private void mainGraph_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            RenderWaveform(waveforms, mainGraph);
        }

        private void btnRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshWaveformData();
        }

        private void tmRefresh_Tick(object sender, EventArgs e)
        {
            lblWaveformFPS.Text =  fps + " fps";
        }

        private void FormOscilloscope_Load(object sender, EventArgs e)
        {

        }

        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWaveformData();
        }

        private void fttGraph_OpenGLDraw(object sender, RenderEventArgs args)
        {
            RenderWaveform(fftWaveforms, fftGraph);
        }
    }
}
