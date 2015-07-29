using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace DatOptimize
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (currentFile != null)
                {
                    currentFile.Dispose();
                    currentFile = null;
                }
                currentFile = new DatFile2(dialog.FileName);
                currentFile.CheckJumpTables();
                map = currentFile.CreateMap();
                InitScrollBar();
                UpdateImage();
            }
        }

        private void InitScrollBar()
        {
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = (int)(currentFile.Size / 4096);
            vScrollBar1.SmallChange = 1;
            vScrollBar1.LargeChange = 1024;
        }

        private void UpdateImage()
        {
#if DEBUG
            int offset = vScrollBar1.Value * 1024;
            Bitmap image = new Bitmap(1024, 1024, PixelFormat.Format24bppRgb);
            BitmapData data = image.LockBits(new Rectangle(new Point(0, 0), image.Size), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = data.Stride;
            IntPtr scan0 = data.Scan0;
            unsafe
            {
                byte* ptr = (byte*)scan0;
                for (int y = 0; y < 1024; y++)
                {
                    for (int x = 0; x < 1024; x++)
                    {
                        if (offset < map.Length)
                        {
                            byte r = (byte)((map[offset] & 1) << 7);
                            byte g = (byte)((map[offset] & 2) << 6);
                            byte b = (byte)((map[offset] & 4) << 5);
                            *(ptr + y * stride + x * 3) = r;
                            *(ptr + y * stride + x * 3 + 1) = g;
                            *(ptr + y * stride + x * 3 + 2) = b;
                            offset++;
                        }
                    }
                }
            }
            image.UnlockBits(data);
            pictureBox1.Image = image;
#endif
        }

        DatFile2 currentFile;
        byte[] map;

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (map == null) return;
            StringBuilder message = new StringBuilder();
            message.Append("Gaps: ");
            int start = -1;
            for (int i = 0; i < map.Length; i++)
            {
                if (map[i] == 0)
                {
                    if (start == -1)
                        start = i;
                }
                else
                {
                    if (start != -1)
                    {
                        message.AppendFormat("{0}-{1},", start, i - 1);
                        start = -1;
                    }
                }
            }
            if (start != -1)
            {
                message.AppendFormat("{0}-{1},", start, map.Length - 1);
            }
            message.Length = message.Length - 1;
            MessageBox.Show(message.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenFileDialog dialog2 = new OpenFileDialog();
                if (dialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DatFile2 file = new DatFile2(dialog.FileName);
                    file.CheckJumpTables(false);
                    DatFile2 file2 = new DatFile2(dialog2.FileName);
                    file2.CheckJumpTables(false);
                    try
                    {
#if DEBUG
                        file.CompareDetails(file2);
#endif
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }

        }
    }
}
