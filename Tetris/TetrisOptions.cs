using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Tetris
{
    public class TetrisOptions
    {
        static int score = 0;

        public static Vec256 board =
        new Vec256(1, 24, 128, 1, 24, 128, 1, 24, 128, 1, 24, 128, 1, 24, 128, 1, 24,
        128, 1, 24, 128, 1, 24, 128, 1, 24, 128, 1, 24, 128, 255, 15);

        static readonly Vec256 checkLineMask = new Vec256(0, 4, 64, 0, 4, 64, 0, 4, 64, 0, 4, 64,
        0, 4, 64, 0, 4, 64, 0, 4, 64, 0, 4, 64,
        0, 4, 64, 0, 4, 64, 0, 0);

        static readonly Vec256 fullLine = new Vec256(0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 255, 15);

        static bool Intersects(Vec256 figure) => (figure & board) != 0;
        public static readonly int width = 12;
        static readonly Vec256 lineToStringShuffler = new Vec256(0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1);
        static readonly Vec256 lineToStringValue = new Vec256(1, 2, 4, 8, 16, 32, 64, 128, 1, 2, 4, 8);
        static readonly Vec256 tile = new Vec256(' ' & 255, '*' & 255);
        static readonly Vec256 emptyLine = new Vec256(1, 8);

        static bool MoveDown() => Shift(width);
        static bool MoveRight() => Shift(1);
        static bool MoveLeft() => Shift(-1);

        static bool RotateLeft() => Rotate(1);
        static bool RotateRight() => Rotate(-1);


        public static Vec256[][] figure = new Vec256[][] //Types of shapes
        {
        new[] {
            new Vec256(240, 0),
            new Vec256(64, 0, 4, 64, 0, 4),
        },
        new[] {
            new Vec256(96, 0, 6),
        },
        new[] {
            new Vec256(96, 0, 12),
            new Vec256(64, 0, 6, 32),
        },
        new[] {
            new Vec256(192, 0, 6),
            new Vec256(32, 0, 6, 64),
        },
        new[] {
            new Vec256(112, 0, 2),
            new Vec256(64, 0, 6, 64),
            new Vec256(32, 0, 7),
            new Vec256(32, 0, 6, 32),
        },
        new[] {
            new Vec256(112, 0, 4),
            new Vec256(64, 0, 4, 96),
            new Vec256(32, 0, 14),
            new Vec256(96, 0, 2, 32),
        },
        new[] {
            new Vec256(224, 0, 2),
            new Vec256(96, 0, 4, 64),
            new Vec256(64, 0, 7),
            new Vec256(32, 0, 2, 96),
        }
        };

        static Vec256[] fallingPiece = new Vec256[4];

        static int rotationsCount;

        static int fallingPieceRotation;

        static Vec256 FallingPiece
        {
            get
            {
                return fallingPiece[fallingPieceRotation % rotationsCount];
            }
        }

        static Random r = new Random(12345);

        static void SelectNextPiece()
        {
            SelectPiece(r.Next(0, figure.Length)); //Choosing a shape randomly, thanks to the figure array
        }

        static void SelectPiece(int i)
        {
            fallingPieceRotation = 0; //Rotation position
            rotationsCount = figure[i].Length; //Passing the shape to the global varriable
            Array.Copy(figure[i], fallingPiece, rotationsCount); //Writing the object template array to the falling shape array
        }

        public static void RemoveFullLines(ref Vec256 board, ref int score)
        {
            Vec256 test = board;

            test &= (test << 5);
            test &= (test << 2);
            test &= (test << 1);
            test &= (test << 1);

            if ((test & checkLineMask) == 0)
            {
                return;
            }

            int scoreBoost = 1;
            Vec256 lineMask = fullLine >> width;
            Vec256 underLineMask = fullLine;

            while (lineMask != 0)
            {
                while ((board & lineMask) == lineMask)
                {
                    score += scoreBoost;
                    scoreBoost <<= 1;
                    board = (board & underLineMask) | ((board & ~underLineMask & ~lineMask) << width) | emptyLine;
                }

                underLineMask |= lineMask;
                lineMask >>= width;
            }
        }

        public static void Replay(string script)
        {
            foreach (string item in script.Split(' '))
            {
                SelectNextPiece();
                DisplayField();

                Thread.Sleep(100);

                int rotation = int.Parse(item.Split(':')[0]);
                int move = int.Parse(item.Split(':')[1]);

                for (int i = 0; i < rotation; i++)
                {
                    RotateLeft();
                    DisplayField();
                    Thread.Sleep(100);
                }
                DisplayField();
                Thread.Sleep(200);
                int moveDelta = Math.Sign(move);

                for (int i = 0; i < Math.Abs(move); i++)
                {
                    Shift(moveDelta);
                    DisplayField();
                    Thread.Sleep(100);
                }

                DisplayField();
                Thread.Sleep(200);

                while (MoveDown())
                {
                    DisplayField();
                    Thread.Sleep(40);
                }

                AddPieceToBoard();
                RemoveFullLines();
                DisplayField();
                Thread.Sleep(100);
            }
        }

        static void RemoveFullLines()
        {
            RemoveFullLines(ref board, ref score);
        }

        static void AddPieceToBoard()
        {
            board |= FallingPiece;
        }

        static bool Rotate(int count)
        {
            while (count < 0)
            {
                count += rotationsCount;
            }

            fallingPieceRotation += count;

            if (!Intersects(FallingPiece))
                return true;
            fallingPieceRotation -= count;

            return false;
        }

        static bool Shift(int count)
        {
            if (count > 0)
            {
                if (Intersects(FallingPiece << count))
                {
                    return false;
                }

                for (int i = 0; i < rotationsCount; i++)
                {
                    fallingPiece[i] <<= count;
                }
            }
            else
            {
                if (Intersects(FallingPiece >> -count))
                {
                    return false;
                }

                for (int i = 0; i < rotationsCount; i++)
                {
                    fallingPiece[i] >>= -count;
                }
            }

            return true;
        }

        static string GetLine(Vec256 field)
        {
            Vec256 line = field.Shuffle(lineToStringShuffler) & lineToStringValue;

            line = line.Min(Vec256.ONE);
            line = tile.Shuffle(line);

            return Encoding.ASCII.GetString(line.ToArray<byte>());
        }

        static void DisplayField()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            DisplaySystemInfo();

            Console.SetCursorPosition(0, 2);

            Console.ForegroundColor = ConsoleColor.Black;

            Vec256 field = board | FallingPiece;

            while (field != 0)
            {
                Console.WriteLine(GetLine(field));
                field >>= width;
            }

            Console.WriteLine($"\n\n Score: {score} \n");
        }

        static void DisplaySystemInfo()
        {
            Console.SetCursorPosition(16, 2);

            Console.SetCursorPosition(16, 5);
            Console.WriteLine($"Board {board}");

            Console.SetCursorPosition(16, 7);
            Console.WriteLine($"Piece {FallingPiece}");

            Console.SetCursorPosition(16, 9);
            Console.WriteLine($"p:000 {fallingPiece[0]}");

            Console.SetCursorPosition(16, 10);
            Console.WriteLine($"p:090 {fallingPiece[1]}");

            Console.SetCursorPosition(16, 11);
            Console.WriteLine($"p:180 {fallingPiece[2]}");

            Console.SetCursorPosition(16, 12);
            Console.WriteLine($"p:270 {fallingPiece[3]}");
        }

        static void Control()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(false);

                if (key.Key == ConsoleKey.UpArrow)
                {
                    RotateLeft();
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    RotateRight();
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    MoveLeft();
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    MoveRight();
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    while (MoveDown()) ;
                }

                while (Console.KeyAvailable)
                {
                    Console.ReadKey();
                }

                DisplayField();
            }
        }

        public static void PlayGame()
        {
            SelectNextPiece();

            while (!Intersects(FallingPiece))
            {
                DisplayField();

                for (int i = 0; i < 10; i++)
                {
                    Control();

                    Thread.Sleep(100);
                }

                if (!MoveDown())
                {
                    AddPieceToBoard();
                    RemoveFullLines();
                    SelectNextPiece();

                    continue;
                }
            }

            DisplayField();
        }
    }
}
