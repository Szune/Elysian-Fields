using System;

namespace Elysian_Fields_Map_Editor
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new ElysianMapEditor())
                game.Run();
        }
    }
#endif
}
