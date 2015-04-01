// <summary> Base class to handle OpenGL functions and methods common to all displayed objects. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
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

        /// <summary> The handle for the texture buffer. </summary>
        private int textureBufferObject = -1;

        /// <summary> The handle for the shader program used by this object. </summary>
        private int shaderProgram = -1;

        /// <summary> The number of points in this object. Used by the render method. </summary>
        private int indexCount;

        /// <summary> Holds the data for the collision model, if needed. </summary>
        private CollisionModel collision;

        //// -----------------------Uniforms-----------------------

        /// <summary> The Uniform Block reference for the camera matrices. </summary>
        private int globalCameraMatrix = -1;

        /// <summary> The Uniform Block reference for the light vectors. </summary>
        private int globalLight = -1;

        /// <summary> The Location of the model matrix uniform. </summary>
        private int uModelMatrix = -1;

        //// -----------------------Transforms-----------------------

        /// <summary> The value to pass to the location uniform. </summary>
        private Matrix4 location = Matrix4.Identity;

        /// <summary> The value to pass to the rotation uniform. </summary>
        private Matrix4 rotation = Matrix4.Identity;

        /// <summary> The value to pass to the scale uniform. </summary>
        private Matrix4 scale = Matrix4.Identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGLObject"/> class. 
        /// Intended for use by derived classes which provide their own model data. 
        /// </summary>
        /// <param name="shaderProgramName">The name of the shader program to use/create.</param>
        /// <param name="vertShader">The Vertex Shader to use.</param>
        /// <param name="fragShader">The Fragment Shader to use.</param>
        public BaseGLObject(string shaderProgramName, string vertShader, string fragShader)
        {
            // If no shader program called Basic exists, create one
            if (!Shaders.ProgramList.ContainsKey(shaderProgramName))
            {
                Shaders.CreateNewProgram(shaderProgramName, Shaders.ShaderList[vertShader], Shaders.ShaderList[fragShader]);
            }

            // Store the program handle.
            this.shaderProgram = Shaders.ProgramList[shaderProgramName];

            // Get the Block index for the camera.
            this.globalCameraMatrix = GL.GetUniformBlockIndex(this.shaderProgram, "GlobalCamera");

            // Get the Block index for the light
            this.globalLight = GL.GetUniformBlockIndex(this.shaderProgram, "Light");

            // Get the location of the model matrix uniform.
            this.uModelMatrix = GL.GetUniformLocation(this.shaderProgram, "modelMatrix");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGLObject"/> class.
        /// </summary>
        /// <param name="model"> The model to use.</param>
        /// <param name="textureFile"> The texture to apply to the model.</param>
        /// <param name="shaderProgramName">The name of the shader program to use/create.</param>
        /// <param name="VertShader">The Vertex Shader to use.</param>
        /// <param name="FragShader">The Fragment Shader to use.</param>
        public BaseGLObject(Mesh model, string textureFile, string shaderProgramName, string VertShader, string FragShader)
            : this(shaderProgramName, VertShader, FragShader)
        {
            GL.UseProgram(this.shaderProgram);
            GL.UniformBlockBinding(this.shaderProgram, this.globalCameraMatrix, Camera.GlobalUBO);
            GL.UniformBlockBinding(this.shaderProgram, this.globalLight, Light.LightsInScene["Main"].GlobalBindingIndex);
            GL.UseProgram(0);

            this.InitializeBufferObjects(model);

            this.CreateTexture(textureFile);

            this.InitialzeVertexObject();

            this.collision = new CollisionModel(model.VertexFaceList, model.VertexList);
        }

        /// <summary> Gets the handle for the shader program used by this object. </summary>
        /// <value>The shader program for this object.</value>
        public int ShaderProgram
        {
            get
            {
                return this.shaderProgram;
            }
        }

        /// <summary>
        /// Checks the collision model for an intersection with the provided vector.
        /// </summary>
        /// <param name="vec">The vector to check along.</param>
        /// <returns>True if the vector hit part of the collision model.</returns>
        public bool CheckForCollision(Vector3 vec)
        {
            if (this.collision != null)
            {
                return this.collision.CheckForCollision(new Vector3(vec));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Rotate the object along the X axis.
        /// </summary>
        /// <param name="angle">The amount to rotate.</param>
        public void RotateX(float angle)
        {
            this.rotation *= Matrix4.CreateRotationX((float)(Math.PI / 180) * angle);
        }

        /// <summary>
        /// Rotate the object along the Y axis.
        /// </summary>
        /// <param name="angle">The amount to rotate.</param>
        public void RotateY(float angle)
        {
            this.rotation *= Matrix4.CreateRotationY((float)(Math.PI / 180) * angle);
        }

        /// <summary>
        /// Rotate the object along the Z axis.
        /// </summary>
        /// <param name="angle">The amount to rotate.</param>
        public void RotateZ(float angle)
        {
            this.rotation *= Matrix4.CreateRotationZ((float)(Math.PI / 180) * angle);
        }

        /// <summary>
        /// Scale the object.
        /// </summary>
        /// <param name="scale">The amount to grow or shrink.</param>
        public void Scale(float scale)
        {
            this.scale *= Matrix4.CreateScale(scale);
        }

        /// <summary>
        /// Set the position of the center of the object to a particular set of coordinates.
        /// </summary>
        /// <param name="position">Where the object should be drawn.</param>
        public void SetPosition(Vector3 position)
        {
            this.location = Matrix4.CreateTranslation(position);
        }

        /// <summary>
        /// Render the object.
        /// </summary>
        public virtual void Render()
        {
            // Set the model matrix uniform, if we need to. (2D objects have no need)
            if (this.uModelMatrix != -1)
            {
                Matrix4 modelMatrix = this.scale * this.rotation * this.location; // The order of the multiplication is important.
                GL.UniformMatrix4(this.uModelMatrix, false, ref modelMatrix);
            }

            // Assign the proper texture for the buffer
            GL.BindTexture(TextureTarget.Texture2D, this.textureBufferObject);

            // Draw the object
            GL.BindVertexArray(this.vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedShort, 0);

            // Reset 
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Initialize the Vertex and Index Buffers for this object.
        /// </summary>
        /// <param name="model">The model information for the object.</param>
        internal void InitializeBufferObjects(Mesh model)
        {
            // Create the vertex buffer
            GL.GenBuffers(1, out this.vertexBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(model.Verticies.Length * (int)Vertex.SizeInBytes), model.Verticies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Create the index Buffer
            GL.GenBuffers(1, out this.indexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(model.Indicies.Length * sizeof(short)), model.Indicies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Save the number of indicies for the render function.
            this.indexCount = model.Indicies.Length;
        }

        /// <summary>
        /// Updates the stored buffer objects with new data.
        /// </summary>
        /// <param name="model">The new model data to load to the buffer.</param>
        internal void UpdateBufferObjects(Mesh model)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(model.Verticies.Length * (int)Vertex.SizeInBytes), model.Verticies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(model.Indicies.Length * sizeof(short)), model.Indicies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this.indexCount = model.Indicies.Length;
        }

        /// <summary>
        /// Initializes the Vertex Array for this object.
        /// </summary>
        internal void InitialzeVertexObject()
        {
            // Create the Vertex Array Buffer and initialize it.
            GL.GenVertexArrays(1, out this.vertexArrayObject);
            GL.BindVertexArray(this.vertexArrayObject);

            GL.BindTexture(TextureTarget.Texture2D, this.textureBufferObject);

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

        /// <summary>
        /// Loads a texture from a file.
        /// </summary>
        /// <param name="textureFile">The file to load.</param>
        internal void CreateTexture(string textureFile)
        {
            if (!System.IO.File.Exists(textureFile))
            {
                throw new Exception("Texture File Not Found: " + textureFile);
            }
            
            Bitmap bmp = (Bitmap)Image.FromFile(textureFile);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.GenTextures(1, out this.textureBufferObject);
            GL.BindTexture(TextureTarget.Texture2D, this.textureBufferObject);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, this.textureBufferObject);
        }

        /// <summary>
        /// Holds list of indexed Vectors that represent the collision model for this object.
        /// </summary>
        private class CollisionModel
        {
            /// <summary> The face index order for the collision model. </summary>
            private List<int> faceList;

            /// <summary>  </summary>
            private List<Vector3> vertexList;
            
            public CollisionModel(List<int> faces, List<Vector3> verticies)
            {
                this.faceList = faces;
                this.vertexList = verticies;
            }

            public bool CheckForCollision(Vector3 vec)
            {
                List<int[]> hitFaces = new List<int[]>();
                
                Vector3 D = vec;
                Vector3 O = Camera.CurrentLocation; // Camera Eye
                for (int f = 0; f < this.faceList.Count; f += 3)
                {
                    Vector3 V1 = vertexList[this.faceList[0 + f]];
                    Vector3 V2 = vertexList[this.faceList[1 + f]];
                    Vector3 V3 = vertexList[this.faceList[2 + f]];

                    if (this.TriangleIntersection(V1, V2, V3, O, D))
                    {
                        hitFaces.Add(new int[] { this.faceList[0 + f], this.faceList[1 + f], this.faceList[2 + f] });
                    }
                }

                Console.WriteLine("Hit: {0} faces", hitFaces.Count);
                if (hitFaces.Count > 0)
                {
                    return true;
                }
                else
                { 
                    return false;
                }
            }

            private bool TriangleIntersection(Vector3 V1, Vector3 V2, Vector3 V3, Vector3 O, Vector3 D)
            {
                Vector3 e1, e2;
                Vector3 P, Q, T;
                float det, inv_det, u, v;
                float t;

                // Find vectors for the two edges sharing V1
                Vector3.Subtract(ref V2, ref V1, out e1);
                Vector3.Subtract(ref V3, ref V1, out e2);

                // Calculate the determinant
                Vector3.Cross(ref D, ref e2, out P);
                // If the determinant is near zero, the ray lies in the plane of the triangle
                det = Vector3.Dot(e1, P);

                if (det > -float.Epsilon && det < float.Epsilon)
                {
                    return false;
                }

                inv_det = 1.0f / det;

                // Calculate the distance from V1 to the ray origin.
                Vector3.Subtract(ref O, ref V1, out T);

                // Calculate the u parameter and test bound
                u = Vector3.Dot(T, P) * inv_det;
                // The intersection lies outside of the triangle
                if (u < 0.0f || u > 1.0f)
                {
                    return false;
                }

                // Prepare to test the v parameter
                Vector3.Cross(ref T, ref e1, out Q);

                // Calculate the V parameter and test the bound
                v = Vector3.Dot(D, Q) * inv_det;
                // The intersection lies outside of the triangle
                if (v < 0.0f || u + v > 1.0f)
                {
                    return false;
                }

                t = Vector3.Dot(e2, Q) + inv_det;

                if (t > float.Epsilon)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
