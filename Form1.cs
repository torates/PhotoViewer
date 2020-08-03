using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;



namespace PhotoViewer
{
    public partial class Form1 : Form
    {
        public Form1(string[] args)
        {
            InitializeComponent();
            this.info.Visible = false;
            this.info.Enabled = false;
            this.pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            ControlExtension.Draggable(pictureBox1, true);
            this.AllowDrop = true;
            this.pictureBox1.AllowDrop = true;
            try
            {
                if (args.Length > 0)
                {
                    dragPath = args[0];
                    hasDragged = true;
                }
                if (hasDragged == true)
                {
                    string img;
                    img = dragPath;
                    path = img;
                    pictureBox1.Image = Image.FromFile(img);
                    pictureBox1.Image = checkImgSize(path);
                    this.pictureBox1.Width = this.pictureBox1.Image.Width;
                    this.pictureBox1.Height = this.pictureBox1.Image.Height;
                    Size finalSize = new Size(this.pictureBox1.Image.Width + 17, this.pictureBox1.Image.Height + 67);
                    Point zero = new Point(0, 24);
                    this.Size = finalSize;
                    this.MinimumSize = new Size(this.pictureBox1.Width, this.pictureBox1.Height + 66);
                    pictureBox1.Location = zero;
                    filename = Path.GetFileName(img);
                    this.Text = filename;
                }
            }
            catch (IndexOutOfRangeException)
            {
                hasDragged = false;
                MessageBox.Show("IndexOutOfRangeException");
            }

        }
        OpenFileDialog abrir = new OpenFileDialog();
        string path;
        string dragPath;
        bool hasDragged;
        string filename;
        int counter;
        int desiredWidth;
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Image img = Image.FromFile(path);
            decimal width = img.Width / 20;
            decimal height = img.Height / 20;
            int minimumSize = pictureBox1.Image.Width + pictureBox1.Image.Height / 2;
            if (e.Delta > 0)
            {
                pictureBox1.Width = pictureBox1.Width + Convert.ToInt32(width);
                pictureBox1.Height = pictureBox1.Height + Convert.ToInt32(height);
                Bitmap output = new Bitmap(img, pictureBox1.Width + Convert.ToInt32(width), pictureBox1.Height + Convert.ToInt32(height));
                Image outputf = (Image)output;
                pictureBox1.Image = outputf;
                counter -= 1;


            }
            else if (e.Delta < 0)
            {
                if (counter != 8)
                {
                    pictureBox1.Width = pictureBox1.Width - Convert.ToInt32(width);
                    pictureBox1.Height = pictureBox1.Height - Convert.ToInt32(height);
                    Bitmap output = new Bitmap(img, pictureBox1.Width - Convert.ToInt32(width), pictureBox1.Height - Convert.ToInt32(height));
                    Image outputf = (Image)output;
                    pictureBox1.Image = outputf;
                    counter += 1;
                }

            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string img;
            try
            {
                if (abrir.ShowDialog() == DialogResult.OK)
                {
                    counter = 0;
                    img = abrir.FileName;
                    path = img;
                    pictureBox1.Image = Image.FromFile(img);
                    pictureBox1.Image = checkImgSize(path);
                    this.pictureBox1.Width = this.pictureBox1.Image.Width;
                    this.pictureBox1.Height = this.pictureBox1.Image.Height;
                    Size finalSize = new Size(this.pictureBox1.Image.Width + 17, this.pictureBox1.Image.Height + 67);
                    Point zero = new Point(0, 24);
                    this.Size = finalSize;
                    this.MinimumSize = new Size(this.pictureBox1.Width, this.pictureBox1.Height + 66);
                    pictureBox1.Location = zero;
                    filename = Path.GetFileName(img);
                    this.Text = filename;
                }
                else if (hasDragged == true)
                {
                }
            }
            catch
            {
                string msg = "Only input pictures or gifs!";
                string caption = "ERROR, WRONG FILE EXTENSION";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(msg, caption, buttons);
                if (result == DialogResult.OK)
                {
                    openToolStripMenuItem_Click(sender, e);
                }
            }
        }
        Label dropToLabel;
        public Image checkImgSize(string path)
        {
            Size desiredSize = new Size(1366, 768);
            Image img = Image.FromFile(path);
            desiredWidth = desiredSize.Width;
            decimal ratio = Convert.ToDecimal(img.Width) / Convert.ToDecimal(img.Height);
            if (pictureBox1.Image.Width > desiredSize.Width)
            {
                decimal width = desiredSize.Height * ratio;
                Bitmap output = new Bitmap(img, Convert.ToInt32(width), desiredSize.Height);
                Image outputf = (Image)output;
                return outputf;
            }
            else if (pictureBox1.Image.Height > desiredSize.Height)
            {
                decimal height = desiredSize.Height * ratio;
                Bitmap output = new Bitmap(img, Convert.ToInt32(height), desiredSize.Height);
                Image outputf = (Image)output;
                return outputf;
            }
            return img;

        }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Process.Start("explorer.exe", "/select, " + path);
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                Image img = Image.FromFile(files[0]);
                path = files[0];
                pictureBox1.Image = img;
                pictureBox1.Image = checkImgSize(files[0]);
                this.Controls.Remove(dropToLabel);
                dropToLabel.Enabled = false;
                dropToLabel.Visible = false;
            }
            catch
            {
                string msg = "Only input pictures or gifs!";
                string caption = "ERROR, WRONG FILE EXTENSION";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(msg, caption, buttons);
                if (result == DialogResult.OK)
                {
                    dropToLabel.Enabled = false;
                    dropToLabel.Visible = false;
                    return;
                }
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            dropToLabel = new Label();
            dropToLabel.Text = "Drop here to open the file";
            dropToLabel.Font = new Font(FontFamily.GenericMonospace, 20);
            dropToLabel.AutoSize = true;
            dropToLabel.Location = new Point(this.Width / 2, this.Height / 2);

            this.Controls.Add(dropToLabel);
            dropToLabel.BringToFront();
        }

        private void pictureBox1_DragLeave(object sender, EventArgs e)
        {
            this.Controls.Remove(dropToLabel);
            dropToLabel.Enabled = false;
            dropToLabel.Visible = false;
        }
    }
}
