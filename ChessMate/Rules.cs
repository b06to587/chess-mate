using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMate
{
    class Rules
    {
        public static bool validMove(Piece p, Position to)
        {
            if (p.getPosition().Equals(to)) return false;
            String type = p.getType();
            switch (type)
            {
                case "pawn":
                    return pawnValid(p, p.getPosition(), to);
                case "knight":
                    return knightValid(p, p.getPosition(), to);
                case "bishop":
                    return bishopValid(p, p.getPosition(), to);
                case "rook":
                    return rookValid(p, p.getPosition(), to);
                case "queen":
                    return queenValid(p, p.getPosition(), to);
                case "king":
                    return kingValid(p, p.getPosition(), to);
                default:
                    return false;
            }
        }

        private static bool kingValid(Piece p, Position from, Position to)
        {
            int xDist = to.x - from.x;
            int yDist = to.y - from.y;

            //Check for valid castling
            if (p.numberOfMoves == 0 && yDist == 0 && (xDist == 2 || xDist == -2))
            {
                int direction = xDist == 2 ? 1 : -2;
                Piece p2 = Chess.board.at(new Position(to.x + direction, from.y));
                if (Chess.IsSame(p, p2.getPosition()) && p2.getType() == "rook" && p2.numberOfMoves == 0 && !Chess.IsChecked(p.getColor()))
                {
                    return rookValid(p, from, to);
                }
            }

            if (!((xDist * xDist) <= 1 && (yDist * yDist) <= 1))
            {
                return false;
            }
            return !Chess.IsSame(p, to);
        }

        private static bool queenValid(Piece p, Position from, Position to)
        {
            return rookValid(p, from, to) || bishopValid(p, from, to);
        }

        private static bool rookValid(Piece p, Position from, Position to)
        {
            int xDist = to.x - from.x;
            int yDist = to.y - from.y;

            if (!(xDist == 0 || yDist == 0))
            {
                return false;
            }

            int yDirection = yDist == 0 ? 0 : yDist < 0 ? -1 : 1;
            int xDirection = xDist == 0 ? 0 : xDist < 0 ? -1 : 1;

            int x = from.x + xDirection;
            int y = from.y + yDirection;
            Position current = new Position(x, y);

            while (!current.Equals(to))
            {
                if (!Chess.IsEmpty(current))
                {
                    return false;
                }
                current.x += xDirection;
                current.y += yDirection;
            }

            return !Chess.IsSame(p, to);
        }

        private static bool bishopValid(Piece p, Position from, Position to)
        {
            int xDist = to.x - from.x;
            int yDist = to.y - from.y;

            if (xDist * xDist != yDist * yDist || xDist == 0)
            {
                return false;
            }

            int yDirection = yDist < 0 ? -1 : 1;
            int xDirection = xDist < 0 ? -1 : 1;

            int x = from.x + xDirection;
            int y = from.y + yDirection;
            Position current = new Position(x, y);

            while (!current.Equals(to))
            {
                if (!Chess.IsEmpty(current))
                {
                    return false;
                }
                current.x += xDirection;
                current.y += yDirection;
            }

            return !Chess.IsSame(p, to);
        }

        private static bool knightValid(Piece p, Position from, Position to)
        {
            int xDist = from.x - to.x;
            int yDist = from.y - to.y;

            return !Chess.IsSame(p, to) && xDist * xDist + yDist * yDist == 5;
        }

        private static bool pawnValid(Piece p, Position from, Position to)
        {
            int xDist = from.x - to.x;
            int yDist = from.y - to.y;

            int direction = p.getColor() == "white" ? -1 : 1;

            if (from.y + direction > 7 || from.y + direction < 0) return false;

            if (from.y + direction != to.y && ((from.y + 2 * direction != to.y && p.getNumberOfMoves() == 0) || !Chess.IsEmpty(new Position(from.x, from.y + direction)) || p.getNumberOfMoves() > 0))
            {
                return false;
            }
            if (xDist < -1 || xDist > 1)
            {
                return false;
            }
            if (xDist == 0 && !Chess.IsEmpty(to))
            {
                return false;
            }
            if (xDist * xDist == 1 && yDist * yDist != 1)
            {
                return false;
            }
            if (xDist != 0 && (Chess.IsEmpty(to) || Chess.IsSame(p, to)))
            {
                return false;
            }

            return true;
        }
    }
}
