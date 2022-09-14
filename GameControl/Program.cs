using System;

namespace GameControl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(144, "map.tmx", "tilemap.png");
            game.Run();
        }
    }
}
