using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMate
{
    class AI
    {

        public static ChessMove nextDraw()
        {
            ChessMove finalMove = new ChessMove(new Position(0, 0), new Position(0, 0));
            Random rnd = new Random();
            double best = -10000;

            List<Piece> blackPieces = Chess.blackPieces();
            foreach (Piece piece in blackPieces)
            {
                Piece p = blackPieces.Find(x => x.Equals(piece));
                Dictionary<Position, bool?> moves = Chess.validMoves(p);
                foreach (KeyValuePair<Position, bool?> move in moves)
                {
                    double value = 0;
                    if (move.Value != false)
                    {
                        value = Chess.board.at(move.Key).getValue() + rnd.NextDouble()*4;
                        if(isThreatened(p.getPosition())){
                            value += p.getValue();

                            if (isProtected(p.getPosition()))
                            {
                                int low = lowestThreathener(p.getPosition(), "white");
                                int val = p.getValue();
                                value -= low < val ? low : val;
                            }
                        }

                        int previousValue = p.getValue();
                        Chess.tempMove(p.getPosition(), move.Key);
                        value += p.getValue() - previousValue;

                        if (isThreatened(p.getPosition()))
                        {
                            value -= p.getValue();
                            if (isProtected(p.getPosition()))
                            {
                                int low = lowestThreathener(p.getPosition(), "white");
                                int val = p.getValue();
                                value += low < val ? low : val;
                            }
                        }

                        if (Chess.IsChecked("white"))
                        {
                            value += rnd.NextDouble() * 5;
                        }
                        Chess.undo();
                    }
                    else if (move.Value == false)
                    {
                        value = -10000;
                    }

                    if (value > best)
                    {
                        best = value;
                        finalMove = new ChessMove(piece.getPosition(), move.Key);
                    }
                }                
            }

            return finalMove;
        }

        private static int lowestThreathener(Position position, string color)
        {
            List<Piece> pieces;

            if (color == "white")
            {
                pieces = Chess.whitePieces();
            }
            else
            {
                pieces = Chess.blackPieces();
            }

            int value = 123456789;
            Chess.checkingOwn = true;
            foreach (Piece p in pieces)
            {
                if (Rules.validMove(p, position))
                {
                    if (p.getValue() < value)
                    {
                        value = p.getValue();
                    }
                }
            }
            Chess.checkingOwn = false;
            return value;
        }

        private static bool isThreatened(Position position)
        {
            return canMoveTo(position, "white");
        }

        private static bool isProtected(Position position)
        {
            return canMoveTo(position, "black");
        }
        private static bool canMoveTo(Position position, string color){
            List<Piece> pieces;

            if (color == "white")
            {
                pieces = Chess.whitePieces();
            }
            else
            {
                pieces = Chess.blackPieces();
            }

            Chess.checkingOwn = true;
            foreach (Piece p in pieces)
            {
                if (Rules.validMove(p, position))
                {
                    Chess.checkingOwn = false;
                    return true;
                }
            }
            Chess.checkingOwn = false;
            return false;
            
        }
        
    }
}

