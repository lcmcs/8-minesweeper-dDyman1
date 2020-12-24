namespace Minesweeper
{
    internal class Location
    {
        private int r;
        private int c;

        public Location(int c, int r)
        {
            this.r = r;
            this.c = c;
        }

        public int getRow()
        {
            return r;
        }
        public int getColumn()
        {
            return c;
        }
    }
}