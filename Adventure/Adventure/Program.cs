using System;

namespace Adventure
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AdventureGame game = new AdventureGame())
            {
                game.Run();
            }
        }
    }
#endif
}

