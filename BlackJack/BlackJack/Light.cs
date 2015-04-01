// <summary> Creates a point of light in the scene. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    
    /// <summary>
    /// Creates and manages the lights in a scene.
    /// </summary>
    public class Light
    {
        /// <summary> A Dictionary keyed by name of all lights in the scene. </summary>
        private static Dictionary<string, Light> lightsInScene = new Dictionary<string, Light>();
        
        /// <summary> The Uniform Buffer object reference for this light. Allows us to modify it's attributes. </summary>
        private int globalLightUBO = -1;

        /// <summary> The stored information for this light. </summary>
        private Info lightInfo;

        /// <summary> The global index of this light. Lets shaders reference it as needed. </summary>
        private int globalBindingIndex = -1;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        /// <param name="name">The name to give this particular light.</param>
        /// <param name="location">THe coordinate location for this light.</param>
        /// <param name="color">The color of the light.</param>
        public Light(string name, Vector3 location, Vector3 color)
        {
            this.lightInfo.Location = location;
            this.lightInfo.Color = color;

            this.globalBindingIndex = LightsInScene.Count + 2;

            GL.GenBuffers(1, out this.globalLightUBO);
            GL.BindBuffer(BufferTarget.UniformBuffer, this.globalLightUBO);
            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)(Vector3.SizeInBytes * 2), ref this.lightInfo, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, this.globalBindingIndex, this.globalLightUBO, IntPtr.Zero, (IntPtr)(Vector3.SizeInBytes * 2));

            LightsInScene.Add(name, this);
        }

        /// <summary> Gets a list of the lights in this scene. </summary>
        /// <value>A list of the lights present in this scene.</value>
        public static Dictionary<string, Light> LightsInScene
        {
            get
            {
                return lightsInScene;
            }
        }

        /// <summary> Gets the current location of this light. </summary>
        /// <value> The location of this light as a Vector3. </value>
        public Vector3 Location
        {
            get
            {
                return this.lightInfo.Location;
            }
            set
            {
                this.lightInfo.Location = value;
                this.UpdateLightInfo();
            }
        }

        /// <summary> Gets the current color of this light. </summary>
        /// <value> The color of this light as a Vector3. </value>
        public Vector3 Color
        {
            get
            {
                return this.lightInfo.Color;
            }
            set
            {
                this.lightInfo.Color = value;
                this.UpdateLightInfo();
            }
        }

        /// <summary> Gets the Global Binding Index for this light. </summary>
        /// <value> The Global Binding Index handle for this light. </value>
        public int GlobalBindingIndex
        {
            get
            {
                return this.globalBindingIndex;
            }
        }

        /// <summary>
        /// Moves the light an amount provided in the vector.
        /// </summary>
        /// <param name="amount">How far to move the light in the x, y, and z directions.</param>
        public void Move(Vector3 amount)
        {
            this.lightInfo.Location += amount;
            this.UpdateLightInfo();
        }

        /// <summary>
        /// Updates the Uniform Buffer with the current light information.
        /// </summary>
        private void UpdateLightInfo()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, this.globalLightUBO);
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, (IntPtr)(Vector3.SizeInBytes * 2), ref this.lightInfo);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        /// <summary>
        /// Stores the light information so it can be passed to the Uniform Buffer.
        /// </summary>
        private struct Info
        {
            /// <summary> The location of this light in three dimensional space. </summary>
            public Vector3 Location;

            /// <summary> The Color and Intensity of this light. </summary>
            public Vector3 Color;
        }
    }
}
