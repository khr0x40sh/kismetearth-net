using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KismetEarth.NET
{
    public partial class CoordinatesForm : Form
    {
        public double check1;
        public double check2;
        public double check3;
        public double check4;
        public bool exclude = false;
        Rectangle rect;
        bool good;
        Graphics g;
        Bitmap bm;

        public CoordinatesForm()
        {
            InitializeComponent();
        }
        private void CoordinatesForm_Load(object sender, EventArgs e)
        {
            //if ()
            //{
            //OriginalBmp_redraw();
            //}

            getDimensions();
        }
        private void getDimensions()
        {
            rect = pictureBox1.Bounds;

            load_bitmap();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //textBox1.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //textBox2.Clear();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //textBox3.Clear();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //textBox4.Clear();
        }

        private void OK_Btn_Click(object sender, EventArgs e)
        {
            checkinput();
            if (good == true)
            {
                try
                {
                    this.Close();
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Error Occured on 'OK Click'..." + e1);
                }
            }
        }

        private void checkinput()
        {
            try
            {
                good = true;
                try
                {
                    check1 = System.Convert.ToDouble(textBox1.Text);
                }
                catch
                {
                    MessageBox.Show("Value for Minimum Latitude is not in decimal degrees... (XX.YYYYYYYY)\n Please try Again!");
                    good = false;
                }
                try
                {
                    check2 = System.Convert.ToDouble(textBox2.Text);
                }
                catch
                {
                    MessageBox.Show("Value for Maximum Latitude is not in decimal degrees... (XX.YYYYYYYY)\n Please try Again!");
                    good = false;
                }
                try
                {
                    check3 = System.Convert.ToDouble(textBox3.Text);
                }
                catch
                {
                    MessageBox.Show("Value for Minimum Longitude is not in decimal degrees... (XX.YYYYYYYY)\n Please try Again!");
                    good = false;
                }
                try
                {
                    check4 = System.Convert.ToDouble(textBox4.Text);
                }
                catch
                {
                    MessageBox.Show("Value for Maximum Longitude is not in decimal degrees... (XX.YYYYYYYY)\n Please try Again!");
                    good = false;
                }
                if (textBox1.Text == null || textBox2 == null || textBox3 == null || textBox4 == null)
                {
                    MessageBox.Show("No coordinates specified, please try again!");
                    good = false;
                }
                if (check1 > 90 || check1 < -90)
                {
                    MessageBox.Show("Minimum Latitude contains an incorrect value!\n Please fix before submitting again!");
                    good = false;
                }
                if (check2 > 90 || check2 < -90)
                {
                    MessageBox.Show("Maximum Latitude contains an incorrect value!\n Please fix before submitting again!");
                    good = false;
                }
                if (check3 > 180 || check3 < -180)
                {
                    MessageBox.Show("Minimum Longitude contains an incorrect value!\n Please fix before submitting again!");
                    good = false;
                }
                if (check4 > 180 || check4 < -180)
                {
                    MessageBox.Show("Maximum Longitude contains an incorrect value!\n Please fix before submitting again!");
                    good = false;
                }
                if (check1 > check2)
                {
                    coord_switcher(check1, check2);
                }
                if (check3 > check4)
                {
                    coord_switcher(check3, check4);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Check Input Error... " + e);
            }
        }

        private void coord_switcher(double a, double b)
        {
            double temp = 0;
            temp = a;
            a = b;
            b = temp;
        }
        private void ExcludeBtn_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = false;
            exclude = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ExcludeBtn.Checked = false;
            exclude = false;
        }

        #region DrawLine / Bitmap
        private void drawhorizline(float width1, float height, string text_to_print)    //draws our Horiz lines for us
        {
            //===========================
            //  Clear Graphics First
            //===========================
            g = Graphics.FromImage(bm);
            g.Clear(SystemColors.Control);
            //===========================
            // RE-LOAD!
            //===========================
            load_bitmap();
            g = Graphics.FromImage(bm);
            //===========================
            //  Draw liek linez and stuffs
            //===========================
            g.DrawLine(new Pen(Color.Red, 5), width1, height, width1 + 284, height);
            g.DrawString(text_to_print, new Font("Courier New", 24, FontStyle.Bold), new SolidBrush(Color.Red), width1 + 20, height + 5);
            //redraw
            pictureBox1.Invalidate();
            //clean
            g.Dispose();
        }
        private void drawline(float width1, float height, string text_to_print, bool left_text)    //draws Vert. Lines with option for text left or right (left is true)
        {
            //===========================
            //  Clear Graphics First
            //===========================
            g = Graphics.FromImage(bm);
            g.Clear(SystemColors.Control);
            //===========================
            // RE-LOAD!
            //===========================
            load_bitmap();
            g = Graphics.FromImage(bm);
            //===========================
            //  Draw liek linez and stuffs
            //===========================
            g.DrawLine(new Pen(Color.Red, 5), width1, height, width1, height + 230);

            if (left_text == false)
            {
                g.DrawString(text_to_print, new Font("Courier New", 24, FontStyle.Bold), new SolidBrush(Color.Red), width1 + 10, height + 30);
            }
            else
            {
                g.DrawString(text_to_print, new Font("Courier New", 24, FontStyle.Bold), new SolidBrush(Color.Red), width1 - 210, height + 30);
            }
            //re-draw
            pictureBox1.Invalidate();
            //clean
            g.Dispose();
        }
        private void OriginalBmp_redraw()
        {
            double width = System.Convert.ToDouble(pictureBox1.Width) / 552;
            double height = System.Convert.ToDouble(pictureBox1.Height) / 302;
            Bitmap bm = (Bitmap)Image.FromFile("files/lat-lng-grid.gif");
            Bitmap resized = new Bitmap((int)((float)width * bm.Width), (int)((float)height * bm.Height));
            Graphics g = Graphics.FromImage(resized);

            g.DrawImage(bm, new Rectangle(0, 0, resized.Width, resized.Height), 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);
            g.Dispose();
            resized.Save("lat_lng_resized.gif");
        }
        private void load_bitmap()
        {
            bm = (Bitmap)Image.FromFile("lat_lng_resized.gif");
            pictureBox1.Image = bm;
        }
        #endregion

        #region Buttonz_click
        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            float width2 = rect.Width / 6;
            float height2 = rect.Height / 10 * 6 - 19;
            drawhorizline(width2, height2, "Min Lat");
            textBox2.Enabled = true;
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            float width2 = rect.Width / 6;
            float height2 = rect.Height / 10 * 3;
            drawhorizline(width2, height2, "Max Lat");
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            float width2 = (rect.Width / 6) + 44;
            float height2 = rect.Height / 12;
            drawline(width2, height2, "Min Long", false);
            textBox4.Enabled = true;
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
            float width2 = rect.Width / 6 * 5 - 5;
            float height2 = rect.Height / 12;
            drawline(width2, height2, "Max Long", true);
        }
        #endregion


    }
}
