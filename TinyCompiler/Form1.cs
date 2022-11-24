using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyCompiler
{
    public partial class Form1 : Form
    {
        private bool draggingForm = false;
        private int startX = 0,startY = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompilerControll.start(textBox1.Text);
            // Show Tokens
            for (int i = 0; i < CompilerControll.scanner.tokens.Count; i++)
            {
                dataGridView1.Rows.Add(CompilerControll.scanner.tokens.ElementAt(i).lex, CompilerControll.scanner.tokens.ElementAt(i).tok);
            }
            // Show Errors
            for (int i = 0; i < CompilerControll.scanner.ERRs.Count; i++)
            {
                dataGridView2.Rows.Add(CompilerControll.scanner.ERRs.ElementAt(i));
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            draggingForm = true;
            startX = Cursor.Position.X - this.Left;
            startY = Cursor.Position.Y - this.Top;
        }
    }
}
