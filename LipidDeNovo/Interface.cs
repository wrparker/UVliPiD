using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LipidDeNovo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int continueAlgorithm = 1;

            //Sanitize Input
            foreach (Control x in this.Controls)
            {
                if (x is TextBox)
                {
                    TextBox textBox = x as TextBox;
                    Console.WriteLine("Checking " + textBox.Name);
                    if (textBox.Text == string.Empty && textBox.Enabled)
                    {
                        continueAlgorithm = -1;
                    }
                }
                else if (x is GroupBox)
                {
                    foreach (Control y in x.Controls)
                    {
                        if (y is TextBox)
                        {
                            TextBox textBox = y as TextBox;
                            Console.WriteLine("Checking " + textBox.Name);
                            if (textBox.Text == string.Empty && textBox.Enabled)
                            {
                                continueAlgorithm = -1;
                            }
                        }
                    }
                }
            }
            //End Sanitization

            if (continueAlgorithm == -1)
            {
                MessageBox.Show("Please fill in values for all required textboxes!");
            }

            else if (radioButton1.Checked)
            {
                Console.WriteLine("Doing Method 1");
                    Lipid_Calculation f = new Lipid_Calculation();
                    f.Method1(Convert.ToDouble(textBox13.Text), Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), textBox6.Text, textBox5.Text, textBox7.Text, textBox8.Text);
            }

            //Method 2
            else if (radioButton2.Checked)
            {
                Lipid_Calculation f = new Lipid_Calculation();
                f.Method2(Convert.ToDouble(textBox13.Text), Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), textBox6.Text, textBox5.Text, textBox7.Text, textBox8.Text, Convert.ToDouble(textBox3.Text), Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox12.Text));
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
