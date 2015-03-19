// <summary> Program entry point. </summary>

namespace BlackJack
{
    /// <summary>
    /// Class to hold the entry point.
    /// </summary>
    public static class Program
    {
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
