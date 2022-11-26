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
        // Header Controller
        private bool draggingForm = false;
        private int startX = 0,startY = 0;

        // Side Panel Move Controller
        Panel currentPanel;
        Panel affectedPanel;
        private bool panelResizer = false;
        private string moveFor = "both";
        private int panelX = 0, panelY = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            draggingForm = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingForm)
            {
                this.Location = new Point(Cursor.Position.X - startX, Cursor.Position.Y - startY);
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            initiateData();
            RunCode();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            initiateData();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                RunCode();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            draggingForm = true;
            startX = Cursor.Position.X - this.Left;
            startY = Cursor.Position.Y - this.Top;
        }

        private void panel5_MouseDown(object sender, MouseEventArgs e)
        {
            panelResizingINIT("x", sidePanel,editorPanel);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // move
            
        }

        private void panel5_MouseUp(object sender, MouseEventArgs e)
        {
            panelResizer = false;
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            panelResizingINIT("y", helperPanel, textPanel);
        }

        private void panelResizingINIT(string dir, Panel panel,Panel afPanel)
        {
            panelResizer = true;
            currentPanel = panel;
            affectedPanel = afPanel;
            moveFor = dir;
            panelX = Cursor.Position.X - this.Left;
            panelY = Cursor.Position.Y - this.Top;
        }

        private void panel4_MouseUp(object sender, MouseEventArgs e)
        {
            panelResizer = false;
        }

        private void ResizePanels(object sender, MouseEventArgs e)
        {
            if (panelResizer)
            {
                if (moveFor == "x")
                {
                    currentPanel.Width = (Cursor.Position.X - this.Left);
                    affectedPanel.Width = flowLayoutPanel2.Width - currentPanel.Width;
                }
                else if (moveFor == "y")
                {
                    currentPanel.Height = -(Cursor.Position.Y - this.Bottom);
                    affectedPanel.Height = flowLayoutPanel2.Height - currentPanel.Height;
                }
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void initiateData()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            CompilerControll.scanner.tokens.Clear();
            CompilerControll.scanner.ERRs.Clear();
        }
        private void RunCode()
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
    }
}
