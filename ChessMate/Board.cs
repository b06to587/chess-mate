using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMate
{
    class Board
    {
        private Dictionary<Position, Piece> board;
        public Board()
        {
            board = new Dictionary<Position, Piece>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; ++x)
                {
                    Position p = new Position(x, y);
                    board[p] = new Piece("blank", "none", p);
                }
            }

            string color = "black";
            for (int y = 1; y < 7; y += 5)
            {
                for (int x = 0; x < 8; ++x)
                {
                    Position p = new Position(x, y);
                    board[p] = new Piece("pawn", color, p);
                }
                color = "white";
            }

            color = "black";
            for (int y = 0; y < 8; y += 7)
            {

                Position p = new Position(0, y);
                board[p] = new Piece("rook", color, p);

                p = new Position(1, y);
                board[p] = new Piece("knight", color, p);

                p = new Position(2, y);
                board[p] = new Piece("bishop", color, p);

                p = new Position(3, y);
                board[p] = new Piece("queen", color, p);

                p = new Position(4, y);
                board[p] = new Piece("king", color, p);

                p = new Position(5, y);
                board[p] = new Piece("bishop", color, p);

                p = new Position(6, y);
                board[p] = new Piece("knight", color, p);

                p = new Position(7, y);
                board[p] = new Piece("rook", color, p);

                color = "white";
            }

        }
        public Piece at(Position p)
        {
            return board[p];
        }

        public void clear()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; ++x)
                {
                    Position p = new Position(x, y);
                    board[p] = new Piece("blank", "none", p);
                }
            }
        }

        public void set(Position position, Piece piece)
        {
            board[position] = piece;
        }

        public Position findKing(string color)
        {
            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    Piece current = at(new Position(x, y));
                    if (current.getType() == "king" && current.getColor() == color)
                    {
                        return new Position(x, y);
                    }
                }
            }
            return null;
        }

        public Dictionary<Position, Piece> getBoard()
        {
            return board;
        }
    }
}
