﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Engine;

namespace ChessEngine.CommandLine
{
    
    class Winboard
    {
        private readonly Logic _ai = new Logic();

        public Winboard()
        {
            //Board board = Board.Game;
        }

        public void ProcessCmd(String input)
        {
            if (input.Length >= 4
                && string.Format("abcdefgh").IndexOf(input.Substring(0, 1)) >= 0
                && string.Format("12345678").IndexOf(input.Substring(1, 1)) >= 0
                && string.Format("abcdefgh").IndexOf(input.Substring(2, 1)) >= 0
                && string.Format("12345678").IndexOf(input.Substring(3, 1)) >= 0)
            {
                input = "usermove " + input;
            }

            string command = input;
            string argument = "";

            int spaceIdx = input.IndexOf(' ');
            if (spaceIdx >= 0)
            {
                command = input.Substring(0, spaceIdx);
                argument = input.Substring(spaceIdx + 1);
            }
            command = command.ToLower();

            
            switch (command)
            {
                case "protover":
                    Console.WriteLine("feature usermove=1");
                    Console.WriteLine("feature setboard=1");
                    Console.WriteLine("feature analyze=1");
                    Console.WriteLine("feature done=1");
                    break;
                case "new":
                    //_board.FENCurrent = new FEN(FEN.FENStart);
                    Board.Game.ResetGame();
                    Logic.Player = Logic.BlackPlayer;
                    break;
                case "force":
                    Logic.Player = 0xFF;
                    break;
                case "go":
                    Logic.Player = Board.Game.WhosTurn;
                    StartThinking();
                    break;
                case "time":
                    /*_timeLeft = TimeSpan.FromMilliseconds(int.Parse(argument) * 10);*/
                    break;
                case "usermove":
                    //Console.WriteLine("move e7e5");
                    IMove move = Decode(argument);
                    move.Execute();
                    Program.Logger.WriteLine(Board.Game.PrintBoard());
                    StartThinking();
                    break;
                case "?":
                    //implement later
                    //player.ForceMove();
                    //Move now. If your engine is thinking, it should move immediately; otherwise, the command should be ignored (treated as a no-op).
                    break;
                case "draw":
                    //The engine's opponent offers the engine a draw. To accept the draw, send "offer draw". To decline, ignore the offer (that is, send nothing). 
                    //If you're playing on ICS, it's possible for the draw offer to have been withdrawn by the time you accept it, so don't assume the game is over because you accept a draw offer. Continue playing until xboard tells you the game is over. See also "offer draw" below.
                    break;
                case "setboard":
                    //_board.FENCurrent = new FEN(argument);
                    SetFEN(argument);
                    break;
                case "undo":
                    /*_board.MoveUndo();*/
                    break;
                case "remove":
                    /*_board.MoveUndo();
                    _board.MoveUndo();*/
                    break;
                case "level":
                    /*_timeControl = TimeControl.Parse(argument);*/
                    break;
                case "analyze":
                    /*_player.YourTurn(_board, new TimeControl(TimeSpan.FromDays(365), TimeSpan.FromDays(1), 0), TimeSpan.FromDays(365));*/
                    break;
                case "exit":
                    /*_player.TurnStop();*/
                    break;
                //custom stuff for debugging
                case "setpos1":
                    /*_board.FENCurrent = new FEN("rr3bk1/pppq1p2/1nn1pB1p/3bP3/P2P1Q2/1P1B1P2/4N1PP/R4RK1 b - - 4 18 ");*/
                    break;
                case "eval":
                    /*Evaluator eval = new Evaluator();
                    int e = eval.EvalFor(_board, _board.WhosTurn);*/
                    break;
            }
        }

        private void StartThinking()
        {
            IMove move = _ai.GetBestMove();
//            Move test = (Move) move;
//            Program.Logger.WriteLine(test.origin + ":" + test.target);
            String algebraicMove = Encode(move);
            move.Execute();
            Program.Logger.WriteLine(algebraicMove);
            Console.WriteLine("move " + algebraicMove);
        }

        public String Encode(IMove move)
        {
            String moveNotation = "";
            if (move is Move || move is EnPassant)
            {
                Move moving = (Move)move;
                moveNotation += ChessConverter.AlgStrings[moving.origin];
//                if (moving.kill != null)
//                {
//                    moveNotation += "x";
//                }
                moveNotation += ChessConverter.AlgStrings[moving.target];
//                if (moving.IsCheck())
//                {
//                    moveNotation += "+";
//                }
            }
            else if (move is Castling)
            {
                Castling castling = (Castling)move;
                moveNotation = castling.GetSide() == "king" ? "O-O" : "O-O-O";
            }
            return moveNotation;
        }

        public IMove Decode(String input)
        {
            IMove move;
            if (input.Contains("O"))
            {
                move = input == "O-O" ? new Castling(0, 0) : new Castling(0, 7);
            }
            else
            {
                Program.Logger.WriteLine(input + " rank: " + input.Substring(0, 2) + " file: " + input.Substring(2, 2));
                move = new UserMove(input.Substring(0,2), input.Substring(2,2));
            }
            return move;
        }
        public void SetFEN(string fen)
        {
            Board board = Board.Game;
            board.ResetGame();
            string[] info = fen.Split(new [] {'/', ' '});
            for (int rank = 0; rank < 8; rank++)
            {
                Program.Logger.WriteLine(info[rank]);
                int file = 0;
                foreach (char piece in info[rank])
                {
                    if (EmptyTiles.Contains(piece))
                    {
                        int e = (int) Char.GetNumericValue(piece);
                        //Program.Logger.WriteLine("space of " + e);
                        for (int f = file; f < (e + file); f++)
                        {
                            board.Tiles[16 * rank + f] = 0;
                        }
                        file += e;
                    }
                    else
                    {
                        board.Tiles[16* rank + file] = ChessConverter.GetPiece(piece);
                        //Program.Logger.WriteLine("at " + rank + " " + file + " equals " + (16 * rank + file) + " piece: " + ChessConverter.GetPiece(piece));
                        file++;
                    }
                }
            }
            _ai.Turn = info[8];
            foreach (char c in info[9])
            {
                if (c.Equals('-'))
                {
                    break;
                }
                switch (c)
                {
                    case 'K':
                        board.WhiteKingCastle = true;
                        break;
                    case 'Q':
                        board.WhiteQueenCastle = true;
                        break;
                    case 'k':
                        board.BlackKingCastle = true;
                        break;
                    case 'q':
                        board.BlackQueenCastle = true;
                        break;
                }
            }
            if (!info[10].Contains("-"))
            {
                board.EnPassant = (byte)Array.IndexOf(ChessConverter.AlgStrings, info[10]);
            }
            board.FiftyMove = int.Parse(info[11]);
            board.MoveCount = int.Parse(info[12]);
        }

        public static readonly char[] EmptyTiles = {'1', '2', '3', '4', '5', '6', '7', '8'};
    }
}
