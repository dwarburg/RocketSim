using System;

namespace RocketSim;

public static class EntryPoint
{
    [STAThread]
    private static void Main()
    {
        using var game = new RocketSimGame();
        game.Run();
    }
}