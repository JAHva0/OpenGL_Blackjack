// <summary> Base class to handle OpenGL functions and methods common to all displayed objects. </summary>

namespace BlackJack
{
    using System;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    
    /// <summary>
    /// Basic class to handle common OpenGL methods.
    /// </summary>
    public class BaseGLObject
    {
        /// <summary> The handle for the vertex array object. </summary>
        private int vertexArrayObject = -1;
        
        /// <summary> The handle for the vertex buffer data. </summary>
        private int vertexBufferObject = -1;

        /// <summary> The handle for the index buffer data. </summary>
        private int indexBufferObject = -1;

        /// <summary> The handle for the shader program used by this object. </summary>
        private int shaderProgram = -1;

        /// <summary> The number of points in this object. Used by the render method. </summary>
        private int indexCount;

        //// -----------------------Uniforms-----------------------

        /// <summary> The Uniform Block reference for the camera matrices. </summary>
        private int globalCameraMatrix = -1;

        private int U_location = -1;
        private int U_rotation = -1;
        private int U_scale = -1;

        //// -----------------------Transforms-----------------------

        private Vector3 location = new Vector3(0.0f, 0.0f, 0.0f);
        private Matrix4 rotation = Matrix4.Identity;
        private Matrix4 scale = Matrix4.Identity;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGLObject"/> class. 
        /// The blank constructor just creates a basic card for testing.
        /// </summary>
        public BaseGLObject()
        {
            // Load the basic box model
            Mesh model = new Mesh(Program.CurrentDirectory + @"BlackJack\BlackJack\Models\Box.obj");

            this.indexCount = model.Indicies.Length;

            // If no shader program called Basic exists, create one
            if (!Shaders.ProgramList.ContainsKey("Basic"))
            {
                Shaders.CreateNewProgram("Basic", Shaders.ShaderList["VertexShader"], Shaders.ShaderList["FragmentShader"]);
            }

            // Store the program handle.
            this.shaderProgram = Shaders.ProgramList["Basic"];

            // Get the Block index for the camera.
            this.globalCameraMatrix = GL.GetUniformBlockIndex(this.shaderProgram, "GlobalCamera");

            // Get the location of the rest of the uniforms.
            this.U_location = GL.GetUniformLocation(this.shaderProgram, "location");
            this.U_rotation = GL.GetUniformLocation(this.shaderProgram, "rotation");
            this.U_scale = GL.GetUniformLocation(this.shaderProgram, "scale");

            GL.UseProgram(this.shaderProgram);
            GL.UniformBlockBinding(this.shaderProgram, this.globalCameraMatrix, Camera.GlobalUBO);
            GL.Uniform3(this.U_location, this.location);
            GL.UseProgram(0);

            this.InitializeBufferObjects(model);

            this.InitialzeVertexObject();
        }

        public void RotateX(float angle)
        {
            this.rotation *= Matrix4.CreateRotationX(angle);
        }

        public void RotateY(float angle)
        {
            this.rotation *= Matrix4.CreateRotationY(angle);
        }

        public void RotateZ(float angle)
        {
            this.rotation *= Matrix4.CreateRotationZ(angle);
        }

        public void Scale(float scale)
        {
            this.scale *= Matrix4.CreateScale(scale);
        }

        public void SetPosition(Vector3 position)
        {
            this.location = position;
        }

        /// <summary>
        /// Render the object.
        /// </summary>
        public void Render()
        {
            GL.UseProgram(this.shaderProgram);

            // Set the uniforms
            GL.UniformMatrix4(this.U_rotation, false, ref this.rotation);
            GL.UniformMatrix4(this.U_scale, false, ref this.scale);
            GL.Uniform3(this.U_location, ref this.location);

            // Draw the object
            GL.BindVertexArray(this.vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedShort, 0);

            // Reset 
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        /// <summary>
        /// Initialize the Vertex and Index Buffers for this object.
        /// </summary>
        /// <param name="model">The model information for the object.</param>
        private void InitializeBufferObjects(Mesh model)
        {
            // Create the vertex buffer
            GL.GenBuffers(1, out this.vertexBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(model.Verticies.Length * (int)Vertex.SizeInBytes), model.Verticies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Create the index Buffer
            GL.GenBuffers(1, out this.indexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(this.indexCount * sizeof(short)), model.Indicies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        /// <summary>
        /// Initializes the Vertex Array for this object.
        /// </summary>
        private void InitialzeVertexObject()
        {
            // Create the Vertex Array Buffer and initialize it.
            GL.GenVertexArrays(1, out this.vertexArrayObject);
            GL.BindVertexArray(this.vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.TextureOffset);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.NormalOffset);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);

            GL.BindVertexArray(0);
        }
    }
}
