using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace AppCatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1_Click(this, EventArgs.Empty);
            CleanupOld();
            t = new System.Threading.Timer(CaptureLoop);
            t.Change(0, 100);
        }
        System.Threading.Timer t;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

       private static readonly string lotroFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "The Lord of the Rings Online");
 
        private void CleanupOld()
        {
            if (Directory.Exists(Path.Combine(lotroFolder, "Plugins\\TMD\\AppDisplay\\Resources")))
            {
                try
                {
                    foreach (string fileName in Directory.GetFiles(Path.Combine(lotroFolder, "Plugins\\TMD\\AppDisplay\\Resources"), "Frame*.jpg"))
                    {
                        try
                        {
                            File.Delete(fileName);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }

        void CaptureLoop(object state)
        {
            if (Directory.Exists(Path.Combine(lotroFolder, "Plugins\\TMD\\AppDisplay\\Resources")))
            {
                long frameNumber = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 2));
                long oldFrame = frameNumber - 6;
                string fileName = Path.Combine(Path.Combine(lotroFolder, "Plugins\\TMD\\AppDisplay\\Resources"), "Frame" + frameNumber.ToString() + ".jpg");
                string oldFileName = Path.Combine(Path.Combine(lotroFolder, "Plugins\\TMD\\AppDisplay\\Resources"), "Frame" + oldFrame.ToString() + ".jpg");
                try
                {
                    File.Delete(oldFileName);
                }
                catch
                {
                }
                if (File.Exists(fileName))
                    return;
                SaveToFile(fileName);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            EnumWindows(new EnumWindowsProc(UpdateList), IntPtr.Zero);
        }

        bool UpdateList(IntPtr hWnd, IntPtr param)
        {
            if (GetParent(hWnd) != IntPtr.Zero)
                return true;
            RECT windowRect;
            GetWindowRect(hWnd, out windowRect);
            if (windowRect.Height == 0 || windowRect.Width == 0)
                return true;
            StringBuilder builder = new StringBuilder(1024);
            GetWindowText(hWnd, builder, builder.Capacity);
            if (string.IsNullOrEmpty(builder.ToString()))
                return true;
            StringBuilder builder2 = new StringBuilder(1024);
            GetClassName(hWnd, builder2, builder2.Capacity);
            listBox1.Items.Add(new WindowEntry() { HWnd=hWnd, Name=builder.ToString()});
            return true;
        }


        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth,
           int nHeight);
        [DllImport("gdi32.dll", SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteDC(IntPtr hdc);

        private void button2_Click(object sender, EventArgs e)
        {
            object o = listBox1.SelectedItem;
            if (o == null)
                return;
            WindowEntry entry = (WindowEntry)o;
            currentHWnd = entry.HWnd;

        }

        IntPtr currentHWnd = IntPtr.Zero;
        private void SaveToFile(string fileName)
        {
            
            RECT windowRect;
            if (!GetWindowRect(currentHWnd, out windowRect))
                return;
            IntPtr dc = IntPtr.Zero;
            Graphics orig = Graphics.FromHwnd(currentHWnd);
            try
            {
                dc = orig.GetHdc();
                IntPtr dc2 = IntPtr.Zero;
                try
                {
                    dc2 = CreateCompatibleDC(dc);

                    IntPtr bitmapHandle = IntPtr.Zero;
                    try
                    {
                        bitmapHandle = CreateCompatibleBitmap(dc, windowRect.Width, windowRect.Height);
                        SelectObject(dc2, bitmapHandle);
                        PrintWindow(currentHWnd, dc2, 0);
                        Bitmap b = Bitmap.FromHbitmap(bitmapHandle);
                        try
                        {
                            using (FileStream stream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                            {
                                b.Save(stream, ImageFormat.Jpeg);
                            }
                        }
                        catch
                        {
                        }
                        b.Dispose();
                    }
                    finally
                    {
                        if (bitmapHandle != IntPtr.Zero)
                        {
                            DeleteObject(bitmapHandle);
                        }
                    }
                }
                finally
                {
                    if (dc2 != IntPtr.Zero)
                    {
                        DeleteDC(dc2);
                    }
                }
            }
            finally
            {
                if (dc != IntPtr.Zero)
                {
                    orig.ReleaseHdc(dc);
                }
            }
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Height
        {
            get { return Bottom - Top; }
        }
        public int Width
        {
            get { return Right - Left; }
        }

    }
}
