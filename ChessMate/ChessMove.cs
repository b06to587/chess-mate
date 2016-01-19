using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMate
{
    public class ChessMove
    {
        public ChessMove(Position from, Position to, int value = 0)
        {
            this.from = from;
            this.to = to;
            this.value = value;
        }

        public Position from;
        public Position to;
        public int value;

        internal void print()
        {
            Debug.WriteLine("Moving (" + from.x + "," + from.y + ") to (" + to.x + "," + to.y + ")");
        }
    }
}
