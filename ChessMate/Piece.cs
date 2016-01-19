using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMate
{
    public class Piece
    {
        private static Dictionary<string, Char> symbols = new Dictionary<string, Char>(){
            {"king", '\u2654'},
            {"queen", '\u2655'},
            {"rook", '\u2656'},
            {"bishop", '\u2657'},
            {"knight", '\u2658'},
            {"pawn", '\u2659'}
        };

        private Position position;
        private string type, color;
        public int numberOfMoves;
        /*
         * Create a new chess piece.
         * Types available are: King, Queen, Rook, Bishop, Knight and Pawn
         * Colors available are: White, Black
         */
        public Piece(string type, string color, Position position, int numberOfMoves = 0)
        {
            this.type = type;
            this.color = color;
            this.numberOfMoves = numberOfMoves;
            this.position = position;
        }

        public Piece(Piece piece)
        {
            this.type = piece.getType();
            this.color = piece.getColor();
            this.position = piece.getPosition();
            this.numberOfMoves = piece.numberOfMoves;
        }

        public Position getPosition()
        {
            return position;
        }

        public string visual()
        {
            if (this.type == "blank")
            {
                return "";
            }

            Char piece = symbols[this.type];
            if (this.color == "black") { ++piece; ++piece; ++piece; ++piece; ++piece; ++piece; }

            return piece.ToString();
        }

        public string getType()
        {
            return this.type;
        }

        public string getColor()
        {
            return this.color;
        }

        public bool free()
        {
            return this.type == "blank";
        }

        public void delete()
        {
            this.type = "blank";
            this.color = "blank";
        }

        public int getNumberOfMoves()
        {
            return numberOfMoves;
        }

        public int getValue()
        {
            switch (this.type)
            {
                case "king":
                    return 4;
                case "queen":
                    return 60;
                case "rook":
                    return 30;
                case "bishop":
                case "knight":
                    return 20;
                case "pawn":
                    return 5 + numberOfMoves * numberOfMoves / 3;
                default:
                    return 0;

            }
        }

        public void changeColor()
        {
            this.color = this.color == "white" ? "black" : "white";
        }

        public void setPosition(Position position)
        {
            this.position = position;
        }


        public bool Equals(Piece other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.type == this.type && other.position.Equals(this.position) && other.color == this.color && other.numberOfMoves == this.numberOfMoves;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Piece)) return false;
            return Equals((Piece)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = type.GetHashCode();
                result += 397 * result + color.GetHashCode();
                result += 397 * result + position.GetHashCode();
                result += 397 * result + numberOfMoves.GetHashCode();
                return result;
            }
        }

        internal void debug()
        {
            Debug.WriteLine(color + " " + type + " (" + position.x + "," + position.y + ") : " + numberOfMoves);
        }
    }
}
