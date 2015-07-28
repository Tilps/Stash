using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace PWIRecover
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Thread process = new Thread(new ParameterizedThreadStart(processFile));
                process.Start(dialog.FileName);
            }
        }

        private void processFile(object fileName)
        {
            byte curly = (byte)'{';
            string dialogFileName = (string)fileName;
            using (FileStream stream = File.OpenRead(dialogFileName))
            {
                int next;
                while (((next = stream.ReadByte()) != -1))
                {
                    if (next == 0x41)
                    {
                        next = stream.ReadByte();
                        if (next == 0x00)
                        {
                            ReadParagraph(stream);
                        }
                        else if (next == 0x41 || next == curly)
                        {
                            stream.Seek(-1, SeekOrigin.Current);
                        }
                    }
                    else if (next == curly)
                    {
                        next = stream.ReadByte();
                        if (next == (byte)'\\')
                        {
                            byte[] name = new byte[3];
                            stream.Read(name, 0, 3);
                            if (name[0] == (byte)'p')
                                if (name[1] == (byte)'w')
                                    if (name[2] == (byte)'i')
                                        this.Invoke(new ParameterizedThreadStart(safeTextUpdate), "NEW PWI FILE DETECTED");
                            stream.Seek(-4, SeekOrigin.Current);
                        }
                        else if (next == 0x41 || next == curly)
                        {
                            stream.Seek(-1, SeekOrigin.Current);
                        }
                    }
                }
            }
        }

        private void ReadParagraph(FileStream stream)
        {
            long posSave = stream.Position;
            try
            {
                StringBuilder para = new StringBuilder();
                byte[] header = new byte[4];
                stream.Read(header, 0, 4);
                int unlikely = 0;
                if (header[3] != 0)
                    unlikely++;
                BinaryReader reader = new BinaryReader(stream);
                {
                    int plainLength = reader.ReadInt16();
                    if (plainLength < 0)
                        return;
                    int length = reader.ReadInt16();
                    if (length < 0)
                        return;
                    if (plainLength > length)
                        return;
                    byte[] data = new byte[0x14];
                    stream.Read(data, 0, 0x14);
                    if (data[0] > 10)
                        unlikely += 2;
                    if (data[1] != 0)
                        unlikely++;
                    if (data[19] != 0)
                        unlikely++;
                    if (data[18] != 0)
                        unlikely++;
                    //                    stream.Seek(0x14, SeekOrigin.Current);
                    for (int i = 0; i < length; i++)
                    {
                        byte read = reader.ReadByte();
                        if (read == 0)
                            continue;
                        if (read < 0x80)
                        {
                            para.Append((char)read);
                        }
                        else
                        {
                            switch (read)
                            {
                                case 0xe5:
                                case 0xe6:
                                case 0xe7:
                                case 0xe8:
                                case 0xc2:
                                case 0xc5:
                                    stream.Seek(2, SeekOrigin.Current);
                                    i += 2;
                                    break;
                                case 0xe9:
                                case 0xea:
                                case 0xeb:
                                case 0xec:
                                    stream.Seek(1, SeekOrigin.Current);
                                    i += 1;
                                    break;
                                case 0xef:
                                    stream.Seek(3, SeekOrigin.Current);
                                    i += 3;
                                    break;
                                case 0xf1:
                                    stream.Seek(7, SeekOrigin.Current);
                                    i += 7;
                                    break;
                                case 0xc1:
                                    byte readExtended = reader.ReadByte();
                                    i++;
                                    para.Append((char)readExtended);
                                    break;
                                case 0xc4:
                                    byte code = reader.ReadByte();
                                    i++;
                                    switch (code)
                                    {
                                        case 0x04:
                                            para.Append("\t");
                                            break;
                                        case 0x19:
                                            stream.Seek(2, SeekOrigin.Current);
                                            i += 2;
                                            break;
                                        case 0x1b:
                                            stream.Seek(6, SeekOrigin.Current);
                                            i += 6;
                                            break;

                                    }
                                    break;
                                default:
                                    byte lower = read;
                                    byte upper = reader.ReadByte();
                                    i++;
                                    int fullCode = upper << 6 | (lower & ~0xc0);
                                    para.Append((char)fullCode);
                                    break;
                            }
                        }
                    }
                }
                if (unlikely >= 3)
                    return;//this.Invoke(new ParameterizedThreadStart(safeTextUpdate), "Totally Bogus:" + para.ToString());
                else if (unlikely >= 2)
                    this.Invoke(new ParameterizedThreadStart(safeTextUpdate), unlikely.ToString()+"Bogus:"+para.ToString());
                else
                    this.Invoke(new ParameterizedThreadStart(safeTextUpdate), para.ToString());
                this.BeginInvoke(new ParameterizedThreadStart(safeLabelUpdate), posSave);
                System.Threading.Thread.Sleep(10);
            }
            catch
            {
                // Ignore errors.
            }
            finally
            {
                stream.Position = posSave;
            }
        }
        private void safeTextUpdate(object para)
        {
            string paraString = (string)para;
            textBox1.Text += paraString + "\r\n";
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
        private void safeLabelUpdate(object pos)
        {
            label1.Text = pos.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, textBox1.Text);
            }
        }
    }
}