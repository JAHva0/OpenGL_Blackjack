﻿// <summary> Creates, modifies and displays 2D text </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    
    /// <summary>
    /// Class for displaying text on the window.
    /// </summary>
    public class Text
    {
        /// <summary> The fonts which have been loaded so far. </summary>
        private static List<Font> loadedFonts = new List<Font>();

        private Font font;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class.
        /// Referenced by other constructors to ensure that all fonts are loaded as soon as one is called for.
        /// </summary>
        public Text()
        { 
            // If there are no fonts, load them
            if (loadedFonts.Count == 0)
            {
                string fontFolder = Program.CurrentDirectory + @"BlackJack\BlackJack\Font\";
                loadedFonts.Add(new Font(fontFolder + "FontData_Arial.csv", fontFolder + "Font_Arial.bmp"));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class.
        /// </summary>
        /// <param name="displaytext">The text to display.</param>
        public Text(string displaytext, string font)
            : this()
        {
            // Make the provided font the one we will use
            this.font = loadedFonts.Where(x => x.Name == font).SingleOrDefault();

            // If we could not find a font with the given name in the loaded list, throw an error
            if (this.font == null)
            {
                throw new Exception(string.Format("Font '{0}' not found.", font));
            }

            // Create a group of quads based on the letters in the displaytext string and their attributes in the Font.
            foreach (char c in displaytext)
            {
                this.font.GetLetterCoordinates((int)c);

            }
        }

        /// <summary> Gets a list of the loaded fonts. </summary>
        /// <value> A list of loaded fonts. </value>
        public static List<Font> LoadedFont
        {
            get
            {
                return loadedFonts;
            }
        }

        /// <summary>
        /// Loads a new font from provided data and texture files.
        /// </summary>
        public class Font
        {
            /// <summary> The size of the image texture. </summary>
            private Size imageSize;

            /// <summary> The size of an individual cell on the texture. </summary>
            private Size cellSize;

            /// <summary> The name of this font. </summary>
            private string name;

            /// <summary> The first char value in the texture. </summary>
            private int startingChar;

            /// <summary> A dictionary that allows reference to the loaded letter information. </summary>
            private Dictionary<int, Letter> letterLookup = new Dictionary<int, Letter>();
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Font"/> class.
            /// </summary>
            /// <param name="data">The data comma separated values that describe how to read the texture.</param>
            /// <param name="texture">The image that contains the font.</param>
            public Font(string data, string texture)
            {
                // Check to be sure that both files are present in the file structure, and throw an exception if not.
                if (!File.Exists(data))
                {
                    throw new Exception("Unable to locate Font data file: " + data);
                }

                if (!File.Exists(texture))
                {
                    throw new Exception("Unable to locate Font texture file: " + texture);
                }

                for (int i = 0; i <= 255; i++)
                {
                    this.letterLookup.Add(i, new Letter(i));
                }

                // Read the Data file and store the relevant data
                string line;
                using (FileStream fs = new FileStream(data, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader stream = new StreamReader(fs, System.Text.Encoding.UTF8, true, 128))
                    {
                        while ((line = stream.ReadLine()) != null)
                        {
                            this.ParseDataLine(line);
                        }
                    }
                }
            }

            /// <summary> The name of this font. </summary>
            /// <value> The name of the font. </value>
            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public Rectangle GetLetterCoordinates(int id)
            {
                Letter l = this.letterLookup[id];

                int cellsPerRow = this.imageSize.Width / this.cellSize.Width;
                int cellIndex = id - this.startingChar;

                Point index = new Point((cellIndex % cellsPerRow) + 1, (cellIndex / cellsPerRow) + 1);

                Rectangle coords = new Rectangle();
                coords.Location = new Point(index.X * this.cellSize.Width, index.Y * this.cellSize.Height);
                coords.Height = this.cellSize.Height;
                coords.Width = l.BaseWidth;

                return new Rectangle();
            }

            /// <summary>
            /// Parse a line of data read from the data file.
            /// </summary>
            /// <param name="line">The line we are currently reading.</param>
            private void ParseDataLine(string line)
            {
                // General data about how to read the texture.
                if (line.Contains("Image Width"))
                {
                    int w;
                    int.TryParse(line.Split(',')[1], out w);
                    this.imageSize.Width = w;
                }
                else if (line.Contains("Image Height"))
                {
                    int h;
                    int.TryParse(line.Split(',')[1], out h);
                    this.imageSize.Height = h;
                }
                else if (line.Contains("Cell Width"))
                {
                    int w;
                    int.TryParse(line.Split(',')[1], out w);
                    this.cellSize.Width = w;
                }
                else if (line.Contains("Cell Height"))
                {
                    int h;
                    int.TryParse(line.Split(',')[1], out h);
                    this.cellSize.Height = h;
                }
                else if (line.Contains("Font Name"))
                {
                    this.name = line.Split(',')[1];
                }
                else if (line.Contains("Start Char"))
                {
                    int c;
                    int.TryParse(line.Split(',')[1], out c);
                    this.startingChar = c;
                }
                else if (line.Contains("Base Width"))
                {
                    int charID, baseWidth;
                    int.TryParse(line.Split(' ')[1], out charID);
                    int.TryParse(line.Split(',')[1], out baseWidth);

                    this.letterLookup[charID].BaseWidth = baseWidth;
                }
                else if (line.Contains("Width Offset"))
                {
                    int charID, widthOffset;
                    int.TryParse(line.Split(' ')[1], out charID);
                    int.TryParse(line.Split(',')[1], out widthOffset);

                    this.letterLookup[charID].WidthOffset = widthOffset;
                }
                else if (line.Contains("Y Offset"))
                {
                    int charID, yOffset;
                    int.TryParse(line.Split(' ')[1], out charID);
                    int.TryParse(line.Split(',')[1], out yOffset);

                    this.letterLookup[charID].YOffset = yOffset;
                }
                else if (line.Contains("XOffset"))
                {
                    int charID, xOffset;
                    int.TryParse(line.Split(' ')[1], out charID);
                    int.TryParse(line.Split(',')[1], out xOffset);

                    this.letterLookup[charID].XOffset = xOffset;
                }
            }

            /// <summary>
            /// Represents a letter within the font class. Stores location information loaded from the data file.
            /// </summary>
            private class Letter
            {
                /// <summary> The ANSCII ID of this letter. </summary>
                private int id;

                /// <summary> The width of the letter. </summary>
                private int baseWidth;

                /// <summary> How far the left side of the width is from the side of the cell. </summary>
                private int widthOfffset;

                /// <summary> The location of the letter within the cell. </summary>
                private Point offset;

                /// <summary>
                /// Initializes a new instance of the <see cref="Letter"/> class.
                /// </summary>
                /// <param name="id">The char ID of this letter.</param>
                public Letter(int id)
                {
                    this.id = id;
                    this.baseWidth = 0;
                    this.widthOfffset = 0;
                    this.offset = Point.Empty;
                }

                /// <summary>
                /// Gets the ASCII id of this letter.
                /// </summary>
                public int ID
                {
                    get
                    {
                        return this.id;
                    }
                }

                /// <summary>
                /// Gets or sets the base width of this letter.
                /// </summary>
                public int BaseWidth
                {
                    get
                    {
                        return this.baseWidth;
                    }

                    set
                    {
                        this.baseWidth = value;
                    }
                }

                /// <summary>
                /// Gets or sets the width offset of this letter.
                /// </summary>
                public int WidthOffset
                {
                    get
                    {
                        return this.widthOfffset;
                    }

                    set
                    {
                        this.widthOfffset = value;
                    }
                }

                /// <summary>
                /// Gets or sets the X offset of this letter.
                /// </summary>
                public int XOffset
                {
                    get
                    {
                        return this.offset.X;
                    }

                    set
                    {
                        this.offset.X = value;
                    }
                }

                /// <summary>
                /// Gets or sets the y offset of this letter.
                /// </summary>
                public int YOffset
                {
                    get
                    {
                        return this.offset.Y;
                    }

                    set
                    {
                        this.offset.Y = value;
                    }
                }
            }
        }
    }
}