using System;

namespace ShipShape
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ShmupGame game = new ShmupGame())
            {
                game.Run();
            }
        }
    }
}

