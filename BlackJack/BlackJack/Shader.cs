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
        private static Dictionary<string, Shader> shaderlist = new Dictionary<string, Shader>();

        /// <summary> Static list of the Shader Programs created in this project. </summary>
        private static Dictionary<string, int> programlist = new Dictionary<string, int>();

        /// <summary> Gets the list of GLSL files in this project. </summary>
        /// <value> A dictionary of program handles, indexed by file name. </value>
        public static Dictionary<string, Shader> ShaderList
        {
            get
            {
                return shaderlist;
            }
        }

        /// <summary> Gets the list of Compiled Shader Programs in this project. </summary>
        /// <value> A dictionary of program handles, indexed by assigned name. </value>
        public static Dictionary<string, int> ProgramList
        {
            get
            {
                return programlist;
            }
        }
        
        /// <summary>
        /// Loads all of the files located in the folder and adds them to the public list.
        /// </summary>
        public static void Load()
        {
            // Locate the shader folder in this project.
            string shaderfolder = Entry.CurrentDirectory + @"BlackJack\BlackJack\Shaders";

            foreach (string shaderfile in Directory.GetFiles(shaderfolder, "*.glsl"))
            {
                if (shaderfile.Contains("Vertex"))
                {
                    shaderlist.Add(Path.GetFileNameWithoutExtension(shaderfile), new Shader(ShaderType.VertexShader, shaderfile));
                }
                else if (shaderfile.Contains("Fragment"))
                {
                    shaderlist.Add(Path.GetFileNameWithoutExtension(shaderfile), new Shader(ShaderType.FragmentShader, shaderfile));
                }
                else
                {
                    throw new Exception("Shader must be contain it's type in the filename");
                }
            }
        }

        /// <summary>
        /// Create a new shader program from the provided Shaders.
        /// </summary>
        /// <param name="name">The name to assign this program.</param>
        /// <param name="vertexShader">The Vertex Shader to compile.</param>
        /// <param name="fragmentShader">The Fragment Shader to compile.</param>
        public static void CreateNewProgram(string name, Shader vertexShader, Shader fragmentShader)
        {
            // Check to be sure we have appropriate types of shaders
            if (vertexShader.Type != ShaderType.VertexShader || fragmentShader.Type != ShaderType.FragmentShader)
            {
                throw new Exception("Invalid Shader Types.");
            }
            
            int programHandle = GL.CreateProgram();
            GL.AttachShader(programHandle, vertexShader.Handle);
            GL.AttachShader(programHandle, fragmentShader.Handle);
            GL.LinkProgram(programHandle);

            programlist.Add(name, programHandle);
        }

        /// <summary>
        /// Represents an individual GLSL object.
        /// </summary>
        public class Shader
        {
            /// <summary> The ID of the program we are making. Cannot be modified outside of the constructor. </summary>
            private int handle;

            /// <summary> The type of this shader. </summary>
            private ShaderType type;

            /// <summary>
            /// Initializes a new instance of the <see cref="Shader"/> class.
            /// </summary>
            /// <param name="type">The Type to create.</param>
            /// <param name="sourcefile">The text file containing the source code.</param>
            public Shader(ShaderType type, string sourcefile)
            {
                // Assign the id of the shader to the next available.
                this.handle = GL.CreateShader(type);

                // Store the type of shader we are creating.
                this.type = type;

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

            /// <summary> Gets the Type of the program. </summary>
            /// <value>The program type.</value>
            public ShaderType Type
            {
                get
                {
                    return this.type;
                }
            }
        }
    }
}
