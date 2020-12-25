using System;
using System.Globalization;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;

namespace Tetris
{
    public class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            if(args.Length == 0)
            {
                Console.BackgroundColor = ConsoleColor.Blue;

                Console.Clear();
                TetrisOptions.PlayGame();

                return;
            }
            else if(args[0] == "-solve")
            {
                SolveTetris.Solve(int.Parse(args[1]));
                return;
            }
            else if(args[0] == "-replay")
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                TetrisOptions.Replay(args[1]);
                return;
            }
            else
            {
                Console.WriteLine("Usage: \ntetris\ntetris -solve max-depth\ntetris -replay \"script\"");
            }
        }
    }
}
