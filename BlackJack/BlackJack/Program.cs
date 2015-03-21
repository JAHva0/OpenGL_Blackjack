// <summary> Program entry point. </summary>

namespace BlackJack
{
    using System.IO;
    
    /// <summary>
    /// Class to hold the entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Gets the path of the current directory, to allow relative paths to files.
        /// </summary>
        public static string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("BlackJack"));
            }
        }

        /// <summary>
        /// Creates a window that will run until the user closes it.
        /// </summary>
        /// <param name="args">Parameter not used.</param>
        public static void Main(string[] args)
        {
            using (Window win = new Window(new System.Drawing.Size(1600, 900), "OpenGL Blackjack"))
            {
                win.Run();
            }
        }
    }
}
