
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttons = new Button[Height, Width];
            this.mines = new bool[Height, Width];

            createMines();
            calculateValues();
            createDifficultySelect();

            for (int c = 0; c < Height; c++)
                for (int r = 0; r < Width; r++)
                    this.buttons[c, r] = new System.Windows.Forms.Button();

            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();

            //
            //Split Container
            //
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Vertical;

            //
            //Split panel 1
            //

            this.splitContainer.Panel1.BackColor = System.Drawing.Color.Navy;
            this.splitContainer.Panel1.Controls.Add(this.toolStrip);
            this.splitContainer.TabIndex = 0;

            //
            //Split panel 2
            //
            this.splitContainer.Panel1.AutoScroll = true;
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanel1);


            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = Height;

            for (int c = 0; c < Height; c++)
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F / Height));

            for (int r = 0; r < Width; r++)
                for (int c = 0; c < Height; c++)
                {
                    this.tableLayoutPanel1.Controls.Add(this.buttons[c, r], c, r);
                }

            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = Width;
            for (int r = 0; r < Width; r++)
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F / Width));

            this.tableLayoutPanel1.Size = new System.Drawing.Size(1392, 1213);
            this.tableLayoutPanel1.TabIndex = 0;


            for (int c = 0; c < Height; c++)
            {
                for (int r = 0; r < Width; r++)
                {
                    this.buttons[c, r].Dock = System.Windows.Forms.DockStyle.Fill;
                    this.buttons[c, r].Location = new System.Drawing.Point(3, 3);
                    this.buttons[c, r].Name = "button2";
                    this.buttons[c, r].Size = new System.Drawing.Size(315, 600);
                    this.buttons[c, r].TabIndex = 1;
                    this.buttons[c, r].UseVisualStyleBackColor = true;
                    this.buttons[c, r].Tag = new Location(c, r);
                    this.buttons[c, r].MouseDown += new MouseEventHandler(this.button2_Click);
                    this.buttons[c, r].IsAccessible = true;

                }
            }

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1492, 1313);
            this.Controls.Add(this.splitContainer);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void calculateValues()
        {
            values = new int[Height, Width];
            ForEachCell((i, j) =>
            {
                var value = 0;
                ForEachNeighbor(i, j, (i1, j1) =>
                {
                    if (mines[i1, j1])
                        value++;
                });
                values[i, j] = value;
            });
        }
        public void ForEachCell(Action<int, int> action)
        {
            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                    action(i, j);
        }

        public void ForEachNeighbor(int i, int j, Action<int, int> action)
        {
            for (var i1 = i - 1; i1 <= i + 1; i1++)
            {
                for (var j1 = j - 1; j1 <= j + 1; j1++)
                {
                    if (InBounds(j1, i1) && !(i1 == i && j1 == j))
                    {
                        action(i1, j1);
                    }
                }
            }
        }

        private bool InBounds(int x, int y)
        {
            return y >= 0 && y < Height && x >= 0 && x < Width;
        }

        private void createMines()
        {
            int numOfMines = mineCount;
            mines = new bool[Height, Width];
            
            var rnd = new Random();
            while (numOfMines > 0)
            {
                var x = rnd.Next(Width);
                var y = rnd.Next(Height);
                if (!mines[y, x])
                {
                    mines[y, x] = true;
                    numOfMines--;
                }
            }
        }

        #endregion
        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanel1;
        private ToolStrip toolStrip;
        private ToolStripDropDownButton difficultyButton;
        private ToolStripButton easy, medium, difficult;
    }
}