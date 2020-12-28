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
        private bool firstClick = true;
        int mineCount;
        int flagCount;
        int minesFlagged;
        int difficulty;
        //Time
        int seconds = 0;
        int minutes = 0;
        //Interaction with table
        bool canInteract = false;

        public int Height { get; set; }
        public int Width { get; set; }
        public Form1(int difficulty = 0)
        {
            this.difficulty = difficulty;
            SetHeightWidth();
            mineCount = (int)(0.125 * Height * Width);
            flagCount = mineCount;
            minesFlagged = 0;
            InitializeComponent();
            tableLayoutPanel1.Enabled = true;
        }

        private void SetHeightWidth()
        {
            switch (difficulty)
            {
                case 0:
                    Height = 8;
                    Width = 10;
                    break;
                case 1:
                    Height = 14;
                    Width = 18;
                    break;
                case 2:
                    Height = 20;
                    Width = 24;
                    break;
                default:
                    Height = 8;
                    Width = 10;
                    break;
            }
        }
        private void CreateTimer()
        {
            // 
            // timer
            // 
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_handler);

            //
            //timerBox
            //

            timerBox.IsAccessible = false;
            timerBox.Location = new Point(140, 125);
            timerBox.Name = "timerBox";
            timerBox.Size = new Size(150, 200);
            timerBox.Font = new Font("Arial", 15, FontStyle.Regular);
            timerBox.RightToLeft = RightToLeft.Yes;
            timerBox.BorderStyle = BorderStyle.None;
            timerBox.BackColor = Color.MediumSeaGreen;
            timerBox.ForeColor = Color.Black;
            //this.timerBox.TabIndex = 17;
            // 
            // timeLabel
            // 
            time.AutoSize = true;
            time.Location = new System.Drawing.Point(0, 125);
            time.Name = "time";
            time.AutoSize = true;
            time.Font = new Font("Arial", 15, FontStyle.Regular);
            time.TabIndex = 18;
            time.Text = "Timer:";
        }

        private void button1_click(object sender, EventArgs e)
        {
            ToolStripButton b = (ToolStripButton)sender;
            int tag = (int)b.Tag;
            if (difficulty != tag)
            {
                Form1 nextForm = new Form1(tag);
                Hide();
                nextForm.ShowDialog();
                Close();
            }
        }

        private void button2_Click(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            Location l = (Location)b.Tag;

            if (!canInteract)
            {
                MessageBox.Show("You must start the timer in order to play!");
            }
            else if (e.Button == MouseButtons.Right)
            {
                MarkAsFlag(b, l);
            }
            else if (b.IsAccessible)
            {

                if (firstClick)
                {
                    FlipFirstPanels(b, l, e);
                    firstClick = false;
                }
                else
                {
                    if (b.BackgroundImage != null)
                    {
                        //Do nothing because a flagged space cannot be interacted with, but is also not "inaccessible" as a space
                    }
                    else
                    {
                        FlipPanel(b, l, e);
                    }
                }
            }
        }

        private void FlipFirstPanels(Button b, Location l, MouseEventArgs e)
        {
            if (IsMine(l.getColumn(), l.getRow()))
            {
                CreateMines();
                CalculateValues();
                FlipFirstPanels(b, l, e);
            }

            //Make panel inaccessible / "flipped"
            buttons[l.getColumn(), l.getRow()].BackColor = Color.LightSlateGray;
            buttons[l.getColumn(), l.getRow()].IsAccessible = false;
            FlipNeighboringPanels(l);
            firstClick = false;
        }

        private void MarkAsFlag(Button b, Location l)
        {
            if (b.BackgroundImage != null)
            {
                b.BackgroundImage = null;
                flagCount++;
                flagsLeft.Text = "" + flagCount;
            }
            else
            {
                if (flagCount < 1)
                {
                    //Do nothing because there are no more flags to be placed
                }
                else
                {
                    b.BackgroundImage = new Bitmap(redFlag, b.Size); //Mark as a flag
                    flagCount--;
                    CheckEndGameThroughFlags();
                    flagsLeft.Text = "" + flagCount;
                    if (IsMine(l.getColumn(), l.getRow()))
                    {
                        minesFlagged++;
                        CheckEndGameThroughFlags();
                    }
                }
            }
        }

        private void FlipPanel(Button b, Location l, MouseEventArgs e)
        {
            if (b.IsAccessible)
            {
                if (IsMine(l.getColumn(), l.getRow()))
                {
                    Explosion();
                }
                else
                {
                    int val = values[l.getColumn(), l.getRow()];
                    //Make panel inaccessible / "flipped"
                    buttons[l.getColumn(), l.getRow()].ForeColor = GetNumberColor(val);
                    buttons[l.getColumn(), l.getRow()].Text = val.ToString();
                    buttons[l.getColumn(), l.getRow()].BackColor = Color.LightSlateGray;
                    buttons[l.getColumn(), l.getRow()].IsAccessible = false;
                    CheckEndGameThroughPanels();
                }
            }
        }

        private void FlipNeighboringPanels(Location l)
        {
            int val;

            //Flip panels that are buttons neighbors through recursion
            ForEachNeighbor(l.getColumn(), l.getRow(), (i1, j1) => {
                Button b = buttons[i1, j1];
                val = values[i1, j1];
                if (!IsMine(i1, j1) && b.IsAccessible && val <= 2)
                {
                    if (val == 0)
                    {
                        buttons[i1, j1].BackColor = Color.LightSlateGray;
                        buttons[i1, j1].IsAccessible = false;
                        FlipNeighboringPanels(new Location(i1, j1));
                    }
                    else
                    {
                        buttons[i1, j1].ForeColor = GetNumberColor(val);
                        buttons[i1, j1].Text = val.ToString();
                        buttons[i1, j1].BackColor = Color.LightSlateGray;
                        buttons[i1, j1].IsAccessible = false;   
                    }
                }
            });
        }
        /*
         *Check if all flags that are placed cover all bombs
         */
        private bool AllFlagsProper()
        {
            return mineCount == minesFlagged;
        }

        public void CheckEndGameThroughFlags()
        {
            if (AllFlagsProper())
                WinGame();
        }

        public void CheckEndGameThroughPanels()
        {
            if (AllSpacesClear())
                WinGame();
        }

        private bool AllSpacesClear()
        {
            bool SpacesClear = true;
            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                    if (buttons[i, j].IsAccessible || IsMine(i, j))
                    {
                        SpacesClear = false;
                    }
            return SpacesClear;

        }
        public void Explosion()
        {
            ShowAllBombs();
            EndGameMessage();
        }

        private void WinGame()
        {
            WinMessage();
            EndGameMessage();
        }

        private void WinMessage()
        {
            MessageBox.Show("YOU WIN!");
        }

        private void EndGameMessage()
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

        private void ShowAllBombs()
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

        private Color GetNumberColor(int val)
        {
            Color c = Color.DarkGray;
            switch (val)
            {
                case 0:
                    c = Color.DarkGray;
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

        public bool IsMine(int height, int width)
        {

            return mines[height, width];
        }

        private void CreateStartGame()
        {
            startButton.Location = new Point(0, 225);
            startButton.Text = "Start";
            startButton.Size = new Size(100, 100);
            startButton.BackColor = Color.BlueViolet;
            startButton.Click += new EventHandler(start_click);
        }

        private void FlagLabels()
        {
            flagsLabel.Location = new Point(0, 325);
            flagsLeft.Location = new Point(300, 325);
            flagsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            flagsLeft.Font = new Font("Arial", 12, FontStyle.Bold);
            flagsLabel.AutoSize = true;
            flagsLeft.AutoSize = true;
            flagsLabel.Text = "Number of Flags:";
            flagsLeft.Text = " " + flagCount;
        }

        private void start_click(object sender, EventArgs e)
        {
            timer.Start();
            seconds = 0;
            minutes = 0;
            canInteract = true;
        }

        private void CreateDifficultySelect()
        {
            toolStrip = new ToolStrip();
            difficultyButton = new ToolStripDropDownButton();
            ToolStripDropDown dropDown = new ToolStripDropDown();
            difficultyButton.Text = "Difficulty";
            difficultyButton.Name = "Difficulty";
            difficultyButton.DropDown = dropDown;
            difficultyButton.DropDownDirection = ToolStripDropDownDirection.BelowRight;
            difficultyButton.ShowDropDownArrow = true;

            // Declare three buttons, set their foreground color and text, 
            // and add the buttons to the drop-down.
            easy = new ToolStripButton();
            easy.Text = "Easy";
            easy.Tag = 0;

            medium = new ToolStripButton();
            medium.Text = "Medium";
            medium.Tag = 1;

            difficult = new ToolStripButton();
            difficult.Text = "Hard";
            difficult.Tag = 2;

            difficult.Click += new EventHandler(button1_click);
            medium.Click += new EventHandler(button1_click);
            easy.Click += new EventHandler(button1_click);


            difficultyButton.DropDownItems.AddRange(new ToolStripButton[]
                {difficult ,medium, easy });

            toolStrip.Items.Add(difficultyButton);
        }

        private void timer_handler(object sender, EventArgs e)
        {
            seconds++;

            if (seconds == 60)
            {
                minutes++;
                seconds = 0;
            }
            string min = minutes.ToString();
            string sec = seconds.ToString();
            if (min.Length < 2)
            {
                min = "0" + min;
            }
            if (sec.Length < 2)
            {
                sec = "0" + sec;
            }

            timerBox.Text = min + ":" + sec;
        }


    }

}