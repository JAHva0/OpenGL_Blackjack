// <summary> Static class for initializing and manipulating the camera. </summary>

namespace BlackJack
{
    using System;
    using System.Drawing;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    
    /// <summary>
    /// Static class for controlling the camera.
    /// </summary>
    public static class Camera
    {
        /// <summary> The three dimensional coordinates of the camera's eye. </summary>
        private static Vector3 cameraeyelocation;

        /// <summary> The three dimensional coordinates of the center of the camera's vision. </summary>
        private static Vector3 cameraeyetarget;

        /// <summary> The Uniform Buffer Object handle for the camera. Lets us save the location of the data so that we can modify it later if needed. </summary>
        private static int globalCameraUBO = -1;

        /// <summary> The Binding Index for the camera, so that all objects which need it can refer to it using this handle. </summary>
        private static int globalCameraBindingIndex = 1;

        /// <summary> Stores the matrix data for the camera. </summary>
        private static GlobalMatrix matricies;

        /// <summary> Gets the Global Binding Index for the camera. </summary>
        /// <value> The global binding index for the camera. Should always be 1. </value>
        public static int GlobalUBO 
        { 
            get 
            { 
                return globalCameraBindingIndex; 
            } 
        }

        /// <summary>
        /// Initializes the camera's matrices and Uniform Buffer.
        /// </summary>
        /// <param name="window">The dimensions of the window being used. Creates the aspect ratio. </param>
        /// <param name="z_Near">The location of the near clipping plane.</param>
        /// <param name="z_Far">The location of the far clipping plane.</param>
        /// <param name="location">The three dimensional coordinates for the camera eye.</param>
        /// <param name="target">The three dimensional coordinates where the camera is looking.</param>
        public static void Initialize(Size window, float z_Near, float z_Far, Vector3 location, Vector3 target)
        {
            cameraeyelocation = location;
            cameraeyetarget = target;

            matricies.Perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)window.Width / (float)window.Height, z_Near, z_Far);
            matricies.View = Matrix4.LookAt(location, target, new Vector3(0.0f, 1.0f, 0.0f));

            GL.GenBuffers(1, out globalCameraUBO);
            GL.BindBuffer(BufferTarget.UniformBuffer, globalCameraUBO);
            GL.BufferData(BufferTarget.UniformBuffer, GlobalMatrix.SizeInBytes, ref matricies, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, globalCameraBindingIndex, globalCameraUBO, IntPtr.Zero, GlobalMatrix.SizeInBytes);
        }

        /// <summary>
        /// Moves the camera in the X or Y direction the specified amount.
        /// </summary>
        /// <param name="move_distance">The vector to be added to the Camera's current position.</param>
        public static void Pan(Vector2 move_distance)
        {
            cameraeyelocation.X += move_distance.X;
            cameraeyelocation.Y += move_distance.Y;
            UpdateCameraInfo();
        }

        /// <summary>
        /// Moves the camera in the Z direction the specified amount.
        /// </summary>
        /// <param name="distance">The distance to be added to the Camera's current Z location.</param>
        public static void Zoom(float distance)
        {
            cameraeyelocation.Z += distance;
            UpdateCameraInfo();
        }

        /// <summary>
        /// Updates the Uniform Buffer with new camera information.
        /// </summary>
        private static void UpdateCameraInfo()
        {
            matricies.View = Matrix4.LookAt(cameraeyelocation, cameraeyetarget, new Vector3(0.0f, 1.0f, 0.0f));

            GL.BindBuffer(BufferTarget.UniformBuffer, globalCameraUBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, GlobalMatrix.SizeInBytes, ref matricies);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        /// <summary>
        /// Struct for holding the raw camera matrices.
        /// </summary>
        private struct GlobalMatrix
        {
            /// <summary> The view matrix. </summary>
            public Matrix4 View;

            /// <summary> The perspective matrix. </summary>
            public Matrix4 Perspective;

            /// <summary> Gets an integer representation of the size of the Global Matrices in bytes. </summary>
            /// <value> The size of the Global Matrices in bytes. </value>
            public static IntPtr SizeInBytes
            {
                get
                {
                    return (IntPtr)(sizeof(float) * 16 * 2);
                }
            }
        }
    }
}
