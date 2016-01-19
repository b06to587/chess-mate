using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ChessMate
{
    class DataHandler
    {
        public static String lastFile;
        public static void checkForChanges()
        {
            string currentFile = File.ReadAllText(Chess.currentGameFile);

            if (lastFile != null && lastFile != currentFile)
            {
                Chess.loadGame();
                lastFile = currentFile;
            }
        }



        static bool failedToWrite = false;
        public static void writeToJSON()
        {
            var query = from piece in Chess.board.getBoard()
                        where piece.Value.getType() != "blank"
                        select piece.Value;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"chess\": {\"pieces\": [");
            foreach (var piece in query)
            {
                sb.Append("{\"position\": {\"x\": " + piece.getPosition().x + ", \"y\": " + piece.getPosition().y + "}, \"type\": \"" + piece.getType() + "\", \"moves\": " + piece.getNumberOfMoves() + ", \"color\": \"" + piece.getColor() + "\"}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("], \"turn\": \"" + Chess.WhosTurn() + "\"}}");

            DateTime dt = DateTime.Now;
            if (Chess.currentGameFile == "")
            {
                Chess.currentGameFile = Chess.gamePath + "\\chess_" + dt.ToString("yyyy_MM_dd_HH_mm") + ".json";
            }

            try
            {
                using (StreamWriter sw = File.CreateText(Chess.currentGameFile))
                {
                    sw.Write(sb.ToString());
                    failedToWrite = false;
                }
            }
            catch
            {
                failedToWrite = true;
            }

            lastFile = sb.ToString();
        }


        public static void loadFromJSON()
        {
            if (failedToWrite)
            {
                return;
            }
            Chess.board.clear();

            string jsonString = File.ReadAllText(Chess.currentGameFile);

            var json = Json.Decode(jsonString);

            var listOfPieces = json["chess"]["pieces"];

            foreach (var piece in listOfPieces)
            {
                String color = piece.color;
                String type = piece.type;
                Position position = new Position(piece.Position.x, piece.Position.y);
                int moves = piece.moves;

                Piece p = new Piece(type, color, position, moves);

                Chess.board.set(position, p);
            }
            Chess.turn = json["chess"]["turn"];
        }
    }
}
