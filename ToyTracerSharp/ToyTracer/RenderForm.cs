using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using ToyTracerLibrary;

namespace ToyTracer
{
    public partial class RenderForm : Form
    {
        public RenderForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.CheckFileExists = true;
            d.CheckPathExists = true;
            d.DefaultExt = "tal";
            if (d.ShowDialog() == DialogResult.OK)
            {
                filenameTextBox.Text = d.FileName;
            }
        }

        private delegate void DrawPixDel(PixelMap map, int toDraw, int completed);

        private void DrawPix(PixelMap map, int toDraw, int completed)
        {
            DrawPixRunWrapArgs wrap = new DrawPixRunWrapArgs();
            wrap.map = map;
            wrap.toDraw = toDraw;
            wrap.completed = completed;
            ThreadPool.QueueUserWorkItem(new WaitCallback(DrawPixRunWrap), wrap);
        }

        private class DrawPixRunWrapArgs
        {
            public PixelMap map;
            public int toDraw;
            public int completed;
        }
        private void DrawPixRunWrap(object o)
        {
            DrawPixRunWrapArgs args = (DrawPixRunWrapArgs)o;
            DrawPixRun(args.map, args.toDraw, args.completed);
        }
        object lockObj = new object();

        private void DrawPixRun(PixelMap map, int toDraw, int completed)
        {
            if (this.InvokeRequired)
            {
                lock (lockObj)
                {
                    try
                    {
                        this.Invoke(new DrawPixDel(DrawPixRun), map, toDraw, completed);
                        Thread.Sleep(10);
                    }
                    catch
                    {
                    }
                }
                return;
            }
            Bitmap img;
            if (this.outputPictureBox.Image is Bitmap)
            {
                img = this.outputPictureBox.Image as Bitmap;
                if (img.Width != map.Width || img.Height != map.Height)
                    img = new Bitmap(map.Width, map.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            else
                img = new Bitmap(map.Width, map.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            if (toDraw <= 0)
            {
                /*
                for (int i = 0; i < map.Width; i++)
                {
                    for (int j = 0; j < map.Height; j++)
                    {
                        img.SetPixel(i, j, System.Drawing.Color.White);
                    }
                }
                 * */
            }
            else
            {
                System.Drawing.Imaging.BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, img.PixelFormat);
                int j = toDraw - 1;
                unsafe
                {
                    byte* pixels = (byte*)data.Scan0.ToPointer();
                    pixels += data.Stride * (toDraw - 1);
                    for (int i = 0; i < map.Width; i++)
                    {
                        *(pixels) = map.Pix[(j * map.Width + i)].B;
                        *(pixels+1) = map.Pix[(j * map.Width + i)].G;
                        *(pixels+2) = map.Pix[(j * map.Width + i)].R;
                        pixels += 3;
                    }
                }
                img.UnlockBits(data);
                /*
                for (int i = 0; i < map.Width; i++)
                {
                    byte r = map.Pix[(j * map.Width + i)].R;
                    byte g = map.Pix[(j * map.Width + i)].G;
                    byte b = map.Pix[(j * map.Width + i)].B;

                    img.SetPixel(i, j, System.Drawing.Color.FromArgb(r, g, b));
                }*/
                if (toDraw == map.Height)
                {
                    img.Save("Autosave" + DateTime.UtcNow.ToString("yyyyMMddHHmmss'Z'") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            if (this.progressBar.Maximum != map.Height)
                this.progressBar.Maximum = map.Height;
            if (this.progressBar1.Maximum != map.Height * map.Width)
                this.progressBar1.Maximum = map.Height * map.Width;
            this.progressBar.Value = toDraw;
            if (completed != -1)
                this.progressBar1.Value = completed;
            this.outputPictureBox.Image = img;
            if (toDraw % 10 == 0)
                this.outputPictureBox.Refresh();
        }

        private delegate void DisplayExceptionDel(Exception e);
        private void DisplayException(Exception e)
        {
            MessageBox.Show(e.ToString());
        }
        ToyTracerLibrary.ToyTracer lastRendered;

        private void worker()
        {
            TalReader tr = new TalReader();
            try
            {
                tr.Read(talfile);
                ToyTracerLibrary.ToyTracer tt = tr.CreateSceneRenderer();
                lastRendered = tt;
                tt.OnProgress += new ProgressEventHandler(DrawPix);
                PixelMap map = tt.Trace();
                DrawPix(map, map.Height, -1);
            }
            catch (Exception e)
            {
                this.Invoke(new DisplayExceptionDel(DisplayException), e);
                return;
            }
            finally
            {
                working = false;
            }
        }

        private string talfile;

        private bool working;

        private void renderButton_Click(object sender, EventArgs e)
        {
            if (working)
                return;
            talfile = this.filenameTextBox.Text;
            Thread workerThread = new Thread(new ThreadStart(worker));
            working = true;
            workerThread.IsBackground = true;
            workerThread.Start();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = "png";
            dialog.Filter = "PNG Graphics (*.png)|*.png";
            dialog.FilterIndex = 0;
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.outputPictureBox.Image.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void outputPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (lastRendered != null)
            {
#if DEBUG
                GatherInfo gathered = lastRendered.Gather(e.X, e.Y);
                DebugForm form = new DebugForm();
                form.SetImage((Bitmap)this.outputPictureBox.Image);
                form.SetTracer(lastRendered);
                form.SetGather(gathered);
                form.Show();
#endif
            }
        }
    }
}