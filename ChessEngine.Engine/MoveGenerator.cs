﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessEngine.Engine
{
    public class MoveGenerator
    {
        private Board _moveBoard;
        private byte _cPlayer;
        private List<IMove> normalMoves, captureMoves; 

        public List<IMove> GetAllMovesForPlayer(Board board, byte player)
        {
            _cPlayer = player;
            _moveBoard = board;
            List<IMove> oldMoves = new List<IMove>();
            normalMoves = new List<IMove>();
            captureMoves = new List<IMove>();
            if (_moveBoard.Mate != 0)
            {
                Console.WriteLine("hit mate");
                return normalMoves;
            }
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    if ((Board.Game.Tiles[(byte)(16*h + w)] & 0x88) == _cPlayer)
                    {
                        oldMoves.AddRange(GetLegalMovements((byte)(16*h + w)));
                    }
                }
            }
            List<IMove> allMoves = new List<IMove>();
            allMoves.AddRange(captureMoves);
            allMoves.AddRange(normalMoves);
            return allMoves;
        }

        public List<IMove> GetLegalMovements(byte origin)
        {
            List<IMove> moves = new List<IMove>();
            byte piece = _moveBoard.Tiles[origin];

            switch (piece)
            {
                    //pawn
                case 0x01: case 0x09:
                    moves.AddRange(GetPawnMoves(origin));
                    break;
                    //knight
                case 0x02: case 0x0A:
                    moves.AddRange(GetAbsoluteMoves(origin));
                    break;
                    //king
                case 0x03: case 0x0B:
                    moves.AddRange(GetAbsoluteMoves(origin));
                    break;
                    //bishop
                case 0x05: case 0x0D:
                    moves.AddRange(GetDiagonalMoves(origin));
                    break;
                    //rook
                case 0x06: case 0x0E:
                    moves.AddRange(GetStraightMoves(origin));
                    break;
                    //queen
                case 0x07: case 0x0F:
                    moves.AddRange(GetStraightMoves(origin));
                    moves.AddRange(GetDiagonalMoves(origin));
                    break;
            }
            return moves;
        }

        public List<IMove> GetStraightMoves(byte origin)
        {
            Move newMove;
            List<IMove> straightMoves = new List<IMove>();
            Boolean upperBreak, lowerBreak, leftBreak, rightBreak;
            upperBreak = lowerBreak = leftBreak = rightBreak = false;
            for (int v = 1; v < 8; v++)
            {
                byte target = 0x00;
                //upper
                target = (byte)(16*v + origin);
                if ((0x88 & target) == 0 && !upperBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        straightMoves.Add(newMove);
                        upperBreak = true;
                    }
                    else
                    {
                        upperBreak = true;
                    }
                }
                //lower
                target = (byte)(-16*v + origin);
                if ((0x88 & target) == 0 && !lowerBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        straightMoves.Add(newMove);
                        lowerBreak = true;
                    }
                    else
                    {
                        lowerBreak = true;
                    }
                }
                //Left
                target = (byte)(origin + v);
                if ((0x88 & target) == 0 && !leftBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        straightMoves.Add(newMove);

                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        straightMoves.Add(newMove);
                        leftBreak = true;
                    }
                    else
                    {
                        leftBreak = true;
                    }
                }
                //Right
                target = (byte)(origin - v);
                if ((0x88 & target) == 0 && !rightBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        straightMoves.Add(newMove);
                        rightBreak = true;
                    }
                    else
                    {
                        rightBreak = true;
                    }
                }
            }
            return straightMoves;
        }

        public List<IMove> GetDiagonalMoves(byte origin)
        {
            Move newMove = null;
            List<IMove> diagonalMoves = new List<IMove>();
            Boolean upperRight, upperLeft, lowerRight, lowerLeft;
            upperRight = upperLeft = lowerRight = lowerLeft = false;
            for (int v = 1; v < 8; v++)
            {
                //UpperRight
                byte target = (byte) (origin + 16*v + v);
                if (!upperRight && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                        upperRight = true;
                    }
                    else
                    {
                        upperRight = true;
                    }
                }
                //UpperLeft
                target = (byte) (origin + 16*v - v);
                if (!upperLeft && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                        upperLeft = true;
                    }
                    else
                    {
                        upperLeft = true;
                    }
                }
                //LowerRight
                target = (byte) (origin - 16*v + v);
                if (!lowerRight && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                        lowerRight = true;
                    }
                    else
                    {
                        lowerRight = true;
                    }
                }
                //LowerLeft
                target = (byte) (origin - 16*v - v);
                if (!lowerLeft && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        diagonalMoves.Add(newMove);
                        lowerLeft = true;
                    }
                    else
                    {
                        lowerLeft = true;
                    }
                }
            }
            return diagonalMoves;
        }


        public List<IMove> GetAbsoluteMoves(byte origin)
        {
            string[] moves = null;
            byte piece = _moveBoard.Tiles[origin];
            var absMoves = new List<IMove>();
            switch (piece & 0x07)
            {
                case 0x02:
                    moves = new[] { "2,1", "1,2", "2,-1", "1,-2", "-2,1", "-1,2", "-2,-1", "-1,-2" };
                    break;
                case 0x03:
                    moves = new[] { "1,0", "-1,0", "0,1", "0,-1", "1,1", "1,-1", "-1,1", "-1,-1" };
//                    absMoves.AddRange(GenerateCastling(piece / Math.Abs(piece)));
                    break;
            }
            foreach (string s in moves)
            {
                string[] m = s.Split(new [] { ',' }, 2);
                byte target = (byte)(origin + 16*int.Parse(m[0]) + int.Parse(m[1]));
                if ((0x88 & target) == 0)
                {
                    Move newMove = new Move(origin, piece) {Target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        normalMoves.Add(newMove);
                        absMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        captureMoves.Add(newMove);
                        absMoves.Add(newMove);
                    }
                }
            }
            return absMoves;
        }

        public List<IMove> GetPawnMoves(byte origin)
        {
            byte piece = _moveBoard.Tiles[origin];
            byte target;
            List<IMove> pawnMoves = new List<IMove>();
            int direction;
            if ((piece & 0x08) == 0x08 && _cPlayer == Logic.BlackPlayer)
            {
                direction = -16;
            }
            else 
            {
                direction = 16;
            }
            Move newMove;

            //Move one space
            target = (byte)(direction + origin);
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                newMove.Target = target;
                if (_moveBoard.Tiles[target] == 0)
                {
                    normalMoves.Add(newMove);
                    CheckForPromotion(newMove);
                    pawnMoves.Add(newMove);

                    //Move ahead two spaces from start
                    int rank = (origin & 0x70)/16;
                    if ((rank == 6 && (piece & 0x08) == _cPlayer) || (rank == 1 && (piece & 0x00) == _cPlayer))
                    {
                        target = (byte) (2*direction + origin);
                        if ((target & 0x88) == 0 && _moveBoard.Tiles[target] == 0)
                        {
                            newMove = new Move(origin, piece);
                            newMove.Target = target;
                            normalMoves.Add(newMove);
                            pawnMoves.Add(newMove);
                        }
                    }
                }
            }
            //Kill piece right
            target = (byte) (((0x70 & origin) + direction) + ((0x07 & origin) + 1));
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                newMove.Target = target;
                if (CheckForKill(newMove))
                {
                    captureMoves.Add(newMove);
                    pawnMoves.Add(newMove);
                }
                CheckForPromotion(newMove);
            }
            //Kill piece left
            target = (byte)(((0x70 & origin) + direction) + ((0x07 & origin) - 1));
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                newMove.Target = target;
                if (CheckForKill(newMove))
                {
                    captureMoves.Add(newMove);
                    pawnMoves.Add(newMove);
                }
                CheckForPromotion(newMove);
            }

            //EnPassant
            if (_moveBoard.EnPassant != 0)
            {
                if ((_moveBoard.Tiles[_moveBoard.EnPassant] & 0x08) != _cPlayer && (origin & 0x70) == 4 * direction && ((origin & 0x07) == (_moveBoard.EnPassant & 0x07) + 1 || (origin & 0x07) == (_moveBoard.EnPassant & 0x07) - 1))
                {
                    EnPassant ep = new EnPassant(origin, _moveBoard.EnPassant);
                    captureMoves.Add(ep);
                    pawnMoves.Add(ep);
                }
            }    
            return pawnMoves;
        }

        public Move GenerateSimpleMove(byte origin, byte target)
        {
            Move newMove = new Move(origin, _moveBoard.Tiles[origin]);

            if (_moveBoard.Tiles[target] == 0)
            {
                return newMove;
            }
            if (CheckForKill(newMove))
            {
                return newMove;
            }

            return null;
        }

        
        public List<IMove> GenerateCastling(byte player)
        {
            List<IMove> castling = new List<IMove>();
            Castling castle;
            byte king;
            int y;
            Boolean leftCastle, rightCastle;
            if (player == Logic.BlackPlayer)
            {
                y = 7;
                king = 0x0B;
                leftCastle = _moveBoard.WhiteQueenCastle;
                rightCastle = _moveBoard.WhiteKingCastle;
            }
            else
            {
                y = 0;
                king = 0x03;
                leftCastle = _moveBoard.WhiteQueenCastle;
                rightCastle = _moveBoard.WhiteKingCastle;
            }
            /*
            if (leftCastle && (_moveBoard.Tiles[16*y + 0] & 0x07) == 0x02 && _moveBoard.Tiles[16*y + 1] == 0 && _moveBoard.Tiles[16*y + 2] == 0 && _moveBoard.Tiles[16*y + 3] == 0 && _moveBoard.Tiles[16*y + 4] == king)
            {
                if (!Board.CheckForCheck(_moveBoard, player, new[] { y, 2 }) && !Board.CheckForCheck(_moveBoard, player, new[] { y, 3 }))
                {
                    castle = new Castling(king, y);
                    castling.Add(castle);
                }
            }
            if (rightCastle && _moveBoard.Tiles[y, 7] == 2 * player && _moveBoard.Tiles[y, 6] == 0 && _moveBoard.Tiles[y, 5] == 0 && _moveBoard.Tiles[y, 4] == 6)
            {
                if (!Board.CheckForCheck(_moveBoard, player, new[] { y, 5 }) && !Board.CheckForCheck(_moveBoard, player, new[] { y, 6 }))
                {
                    castle = new Castling(king, y);
                    castling.Add(castle);
                }
            }
         
            
            else
            {
                if (moveBoard.WhiteQueenCastle && moveBoard.Tiles[0, 1] == 0 && moveBoard.Tiles[0, 2] == 0 && moveBoard.Tiles[0, 3] == 0)
                {
                    castle = new Castling(king, 0);
                    castling.Add(castle);
                }
                if (moveBoard.WhiteKingCastle && moveBoard.Tiles[7, 6] == 0 && moveBoard.Tiles[7, 5] == 0)
                {
                    castle = new Castling(king, 7);
                    castling.Add(castle);
                }

            }*/
            return castling;
        }
        

        
        public static void CheckForPromotion(Move move)
        {
            if ((move.Piece & 0x08) != 0 && (move.Target & 0x70)/16 == 7)
            {
                move.Piece = 0x0F;
            }
            else if ((move.Piece & 0x08) == 0 && (move.Target & 0x70) / 16 == 0)
            {
                move.Piece = 0x07;
            }
        }

        public Boolean CheckForKill(Move move)
        {
            byte piece = _moveBoard.Tiles[move.Target];
            if (piece == 0 || ((piece & 0x08) == (move.Piece & 0x08))) return false;
            move.Kill = piece;
            return true;
        }

        public List<IMove> SacrificeCheck(Board board)
        {
            byte position = board.LastMovedPiece;
            byte piece = board.Tiles[position];
            List<IMove> sacrificeMoves = new List<IMove>();
            foreach (byte captureOrigin in SelfCaptureCheck(board, position, piece))
            {
                Move captureMove = new Move(captureOrigin, board.Tiles[captureOrigin]);
                captureMove.Kill = position;
                sacrificeMoves.Add(captureMove);
            }
            return sacrificeMoves;
        }

        public List<byte> SelfCaptureCheck(Board board, byte origin, byte piece)
        {
            byte target;
            byte[] pieces = { 0x02, 0x05, 0x06 };
            List<byte> killMoves = new List<byte>();

            foreach (byte p in pieces)
            {
//                if (p == 0x01)
//                {
//                    int direction;
//                    if ((piece & 0x08) == 0x08 && _cPlayer == Logic.BlackPlayer)
//                    {
//                        direction = -16;
//                    }
//                    else
//                    {
//                        direction = 16;
//                    }
//                    target = (byte)(((0x70 & origin) + direction) + ((0x07 & origin) + 1));
//                    if ((0x88 & target) == 0 && (piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && (board.Tiles[target] == 0x01 || board.Tiles[target] == 0x09)))
//                    {
//                        killMoves.Add(target);
//                    }
//                    target = (byte)(((0x70 & origin) + direction) + ((0x07 & origin) - 1));
//                    if ((0x88 & target) == 0 && (piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && (board.Tiles[target] == 0x01 || board.Tiles[target] == 0x09)))
//                    {
//                        killMoves.Add(target);
//                    } 
//                }
                if (p == 0x02)
                {
                    string[] moves = { "2,1", "1,2", "2,-1", "1,-2", "-2,1", "-1,2", "-2,-1", "-1,-2" };
                    foreach (string[] m in moves.Select(s => s.Split(new[] {','}, 2)))
                    {
                        target = (byte) (origin + 16*int.Parse(m[0]) + int.Parse(m[1]));
                        if ((0x88 & target) == 0 && (piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && (board.Tiles[target] == 0x02 || board.Tiles[target] == 0x0A)))
                        {
                            killMoves.Add(target);
                        }
                    }
                }
                if (p == 0x05)
                {
                    Boolean upperRight, upperLeft, lowerRight, lowerLeft;
                    upperRight = upperLeft = lowerRight = lowerLeft = false;
                    for (int v = 1; v < 8; v++)
                    {
                        //UpperRight
                        target = (byte)(origin + 16 * v + v);
                        if (!upperRight && (0x88 & target) == 0)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x05 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                upperRight = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                upperRight = true;
                            }
                        }
                        //UpperLeft
                        target = (byte)(origin + 16 * v - v);
                        if (!upperLeft && (0x88 & target) == 0)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x05 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                upperLeft = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                upperLeft = true;
                            }
                        }
                        //LowerRight
                        target = (byte)(origin - 16 * v + v);
                        if (!lowerRight && (0x88 & target) == 0)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x05 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                lowerRight = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                lowerRight = true;
                            }
                        }
                        //LowerLeft
                        target = (byte)(origin - 16 * v - v);
                        if (!lowerLeft && (0x88 & target) == 0)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x05 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                lowerLeft = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                lowerLeft = true;
                            }
                        }
                    }
                }
                if (p == 0x06)
                {
                    Boolean upperBreak, lowerBreak, leftBreak, rightBreak;
                    upperBreak = lowerBreak = leftBreak = rightBreak = false;
                    for (int v = 1; v < 8; v++)
                    {
                        //upper
                        target = (byte)(16 * v + origin);
                        if ((0x88 & target) == 0 && !upperBreak)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x06 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                upperBreak = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                upperBreak = true;
                            }
                            
                        }
                        //lower
                        target = (byte)(-16 * v + origin);
                        if ((0x88 & target) == 0 && !lowerBreak)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x06 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                lowerBreak = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                lowerBreak = true;
                            }
                        }
                        //Left
                        target = (byte)(origin + v);
                        if ((0x88 & target) == 0 && !leftBreak)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x06 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                leftBreak = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                leftBreak = true;
                            }
                        }
                        //Right
                        target = (byte)(origin - v);
                        if ((0x88 & target) == 0 && !rightBreak)
                        {
                            if ((piece & 0x07) == (board.Tiles[target] & 0x07) && ((piece & board.Tiles[target] & 0x08) == (piece & 0x08) && ((board.Tiles[target] & 0x07) == 0x06 || ((board.Tiles[target] & 0x07) == 0x07))))
                            {
                                killMoves.Add(target);
                                rightBreak = true;
                            }
                            if (board.Tiles[target] != 0x00)
                            {
                                rightBreak = true;
                            }
                        }
                    }
                }
            }
            return killMoves;
        }
    }
}
