// <summary> Contains all of the classes and functions related to compiling shaders. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    
    /// <summary>
    /// Static class for handling GLSL file loading and compiling.
    /// </summary>
    public static class Shaders
    {
        /// <summary> Static list of GLSL files included in this project. </summary>
        private static Dictionary<string, int> shaderlist = new Dictionary<string, int>();

        /// <summary> Gets the list of GLSL files in this project. </summary>
        /// <value> A dictionary of program handles, indexed by file name. </value>
        public static Dictionary<string, int> ShaderList
        {
            get
            {
                return shaderlist;
            }
        }
        
        /// <summary>
        /// Loads all of the files located in the folder and adds them to the public list.
        /// </summary>
        public static void Load()
        {
            // Locate the shader folder in this project.
            string shaderfolder = Program.CurrentDirectory + @"BlackJack\BlackJack\Shaders";

            foreach (string shaderfile in Directory.GetFiles(shaderfolder, "*.glsl"))
            {
                if (shaderfile.Contains("Vertex"))
                {
                    shaderlist.Add(Path.GetFileNameWithoutExtension(shaderfile), new Shader(ShaderType.VertexShader, shaderfile).Handle);
                }
                else if (shaderfile.Contains("Fragment"))
                {
                    shaderlist.Add(Path.GetFileNameWithoutExtension(shaderfile), new Shader(ShaderType.FragmentShader, shaderfile).Handle);
                }
                else
                {
                    throw new Exception("Shader must be contain it's type in the filename");
                }
            }
        }

        /// <summary>
        /// Represents an individual GLSL object.
        /// </summary>
        public class Shader
        {
            /// <summary> The ID of the program we are making. Cannot be modified outside of the constructor. </summary>
            private int handle;

            /// <summary>
            /// Initializes a new instance of the <see cref="Shader"/> class.
            /// </summary>
            /// <param name="type">The Type to create.</param>
            /// <param name="sourcefile">The text file containing the source code.</param>
            public Shader(ShaderType type, string sourcefile)
            {
                // Assign the id of the shader to the next available.
                this.handle = GL.CreateShader(type);

                // Load the source code from the file and assign it to this shader.
                GL.ShaderSource(this.Handle, File.ReadAllText(sourcefile));

                // Compile the shader
                GL.CompileShader(this.Handle);

                // Catch any errors that arose (doesn't seem to work?)
                string infolog = GL.GetShaderInfoLog(this.handle);
                if (infolog.Length > 0)
                {
                    throw new Exception(type.ToString() + " Compile Error: " + infolog);
                }
            }

            /// <summary> Gets the ID of the program. </summary>
            /// <value>The program Handle.</value>
            public int Handle
            {
                get
                {
                    return this.handle;
                }
            }
        }
    }
}
