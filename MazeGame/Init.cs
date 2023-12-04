using MazeGame;
using System;

namespace MazeGameMain
{
    // main program to run the game
    public static class Init
    {
        [STAThread]
        static void Main()
        {
            var game = new MazeGameProgram();
            game.Run();
        }
    }
}
