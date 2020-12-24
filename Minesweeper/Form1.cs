using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private Button[,] buttons;
        private Image redFlag = Image.FromFile(@"C:\Users\dovdm\Downloads\redFlag2.png");
        bool[,] mines;
        public int[,] values;
        private Image bomb = Image.FromFile(@"C:\Users\dovdm\Downloads\bomb.png");
        private bool firstClick = false;
        int mineCount;
        int flagCount;
        int minesFlagged;

        public int Height { get; set; }
        public int Width { get; set; }
        public Form1()
        {
            Height = 8;
            Width = 10;
            mineCount = (int)(0.125 * Height * Width);
            flagCount = mineCount;

            InitializeComponent();
        }



        private void button2_Click(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            Location l = (Location)b.Tag;

            if (b.IsAccessible)
            {
                if (!firstClick || values[l.getColumn(), l.getRow()] == 0) {
                    flipFirstPanels(l);
                    b.BackColor = Color.LightSlateGray;
                    firstClick = true;
                    b.IsAccessible = false;
                }
                else
                {
                    buttonAction(b, l, e);
                }
            }           

        }

        private void flipFirstPanels(Location l)
        {
            ForEachNeighbor(l.getColumn(), l.getRow(), (i1, j1)=>{
                int v = values[i1, j1];
                Button b = this.buttons[i1, j1];
                if ( v < 1)
                {
                    if (b.IsAccessible)
                    {
                        b.BackColor = Color.LightSlateGray;
                        b.IsAccessible = false;
                        flipFirstPanels(new Location(i1, j1));
                    }
                }
                if (v <= 2 && v > 0 )
                {
                    if (b.IsAccessible)
                    {
                        b.BackColor = Color.LightSlateGray;
                        b.ForeColor = getNumberColor(v);
                        b.Text = v.ToString();
                        b.IsAccessible = false;
                    }
                }
            });
        }

        //re-enable button click to unFlag a button 
        //create flag press count as well as count if mine is flagged
        //if all mines flagged enter gameWinMessage method
        private void buttonAction(Button b, Location l, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (!isMine(l.getRow(), l.getColumn()))
                {
                    int val = values[l.getColumn(), l.getRow()];
                    b.ForeColor = getNumberColor(val);
                    b.Text = val.ToString();
                    b.BackColor = Color.LightSlateGray;
                }
                else
                {

                    showAllBombs();
                    endGameMessage();

                }
            }
            else
            {
                b.BackgroundImage = new Bitmap(redFlag, b.Size);
            }
        }

        private void endGameMessage()
        {
            DialogResult result = MessageBox.Show("New Game?", "Game Over", MessageBoxButtons.YesNo);
           
            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0);
            }
            else if (result == DialogResult.No)
            {
                Application.Exit();
            }
            else
            {
                Application.Exit();
            }
        }

        private void showAllBombs()
        {
            ForEachCell((i, j) =>
            {
                buttons[i, j].IsAccessible = false;
                if (mines[i, j])
                {
                    buttons[i, j].BackgroundImage = new Bitmap(bomb, buttons[i, j].Size);
                }
            });
        }

        private Color getNumberColor(int val)
        {
            Color c = Color.DarkGray;
            switch (val)
            {
                case 0:
                    c =  Color.DarkGray;
                    break;
                case 1:
                    c = Color.DarkBlue;
                    break;
                case 2:
                    c = Color.DarkGreen;
                    break;
                case 3:
                    c = Color.DarkRed;
                    break;
            }
            return c;
        }

        public bool isMine(int width, int height)
        {

            return mines[height,width];
        }
    }

    


}
