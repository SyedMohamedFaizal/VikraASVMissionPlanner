using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VikraASVMissionPlanner.Forms
{
    public sealed class SplashScreen : Form
    {
        private readonly Timer timer;
        private readonly PictureBox pictureBox;

        public SplashScreen()
        {
            timer = new Timer();
            pictureBox = new PictureBox();

            SuspendLayout();

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Width = 960;
            Height = 540;
            BackColor = Color.Black;
            ShowInTaskbar = false;
            DoubleBuffered = true;

            string iconPath = @"D:\Projects\Assets\mpdesktop.ico";
            if (File.Exists(iconPath))
            {
                Icon = new Icon(iconPath);
            }

            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.BackColor = Color.Black;

            string splashPath = @"D:\Projects\Assets\splashdark.jpg";
            if (File.Exists(splashPath))
            {
                using (Image image = Image.FromFile(splashPath))
                {
                    pictureBox.Image = new Bitmap(image);
                }
            }

            Controls.Add(pictureBox);

            timer.Interval = 1800;
            timer.Tick += Timer_Tick;

            Shown += SplashScreen_Shown;

            ResumeLayout(false);
        }

        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Hide();
            using (Form1 mainForm = new Form1())
            {
                mainForm.ShowDialog(this);
            }

            Close();
        }
    }
}
