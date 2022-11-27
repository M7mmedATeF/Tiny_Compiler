using System;
using System.Runtime.InteropServices;
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
        private bool minimizing = false;
        private int initialMainHeight = 0;
        private int startX = 0,startY = 0;

        // Side Panel Move Controller
        Panel currentPanel;
        Panel affectedPanel;
        private bool panelResizer = false;
        private string moveFor = "both";
        private int panelX = 0, panelY = 0;

        // Rounded Borders
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public Form1()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
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
            Application.Exit();
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
            richTextBox1.Text = "";
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
                    affectedPanel.Width = mainPanel.Width - ((currentPanel.Width > currentPanel.MinimumSize.Width)? currentPanel.Width : currentPanel.MinimumSize.Width);
                }
                else if (moveFor == "y")
                {
                    currentPanel.Height = -(Cursor.Position.Y - this.Bottom);
                    affectedPanel.Height = mainPanel.Height - currentPanel.Height;
                }
            }
        }
        private void iconButton5_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                initialMainHeight = mainPanel.Height;
            }
            else
            {
                minimizing = true;
                this.WindowState = FormWindowState.Normal;
            }
            //formResizingINIT();
        }
        private void formResizingINIT()
        {
            if (minimizing)
            {
                mainPanel.Height = initialMainHeight;
                panel1.Height = initialMainHeight;
                minimizing = false;
            }
            else
            {
                panel1.Height = this.Height - header.Height;
            }
            editorPanel.Width = mainPanel.Width - sidePanel.Width;
            editorPanel.Height = mainPanel.Height;
            sidePanel.Height = mainPanel.Height;
        }
        private void initiateData()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            CompilerControll.scanner.tokens.Clear();
            CompilerControll.scanner.editorColorizer.Clear();
            CompilerControll.scanner.ERRs.Clear();
        }
        private void RunCode()
        {
            CompilerControll.start(richTextBox1.Text);
            initializeTextBoxTheme();
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
        private void initializeTextBoxTheme()
        {
            richTextBox1.Clear();
            for (int i = 0; i < CompilerControll.scanner.editorColorizer.Count; i++)
            {
                Token tk = CompilerControll.scanner.editorColorizer.ElementAt(i);
                if (tk.tok == Token_Class.IDENTIFIER)
                {
                    richTextBox1.SelectionColor = Color.Wheat;
                }else if (tk.tok == Token_Class.INT || tk.tok == Token_Class.STRING || tk.tok == Token_Class.FLOAT || tk.tok == Token_Class.MAIN_FN || tk.tok == Token_Class.IF || tk.tok == Token_Class.READ || tk.tok == Token_Class.WRITE || tk.tok == Token_Class.UNTIL || tk.tok == Token_Class.ELSE || tk.tok == Token_Class.ELSEIF || tk.tok == Token_Class.END || tk.tok == Token_Class.ENDL || tk.tok == Token_Class.REPEAT || tk.tok == Token_Class.RETURN || tk.tok == Token_Class.THEN)
                {
                    richTextBox1.SelectionColor = Color.LightSkyBlue;
                }
                else if (tk.tok == Token_Class.INTEGER_NUM || tk.tok == Token_Class.FLOAT_NUM)
                {
                    richTextBox1.SelectionColor = Color.Wheat;
                }
                else if (tk.tok == Token_Class.STRING_VAL)
                {
                    richTextBox1.SelectionColor = Color.Orange;
                }
                else if (tk.tok == Token_Class.COMMENT)
                {
                    richTextBox1.SelectionColor = Color.LightGreen;
                }
                else
                {
                    richTextBox1.SelectionColor = Color.White;
                }
                richTextBox1.SelectedText = tk.lex;
            }
        }
    }
}
