// <summary> Creates a point of light in the scene. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    
    public class Light
    {
        private int globalLightUBO = -1;

        private Info lightInfo;

        private int globalBindingIndex = -1;

        private static Dictionary<string, Light> lightsInScene = new Dictionary<string, Light>();

        public static Dictionary<string, Light> LightsInScene
        {
            get
            {
                return lightsInScene;
            }
        }

        public Vector3 Location
        {
            get
            {
                return this.lightInfo.location;
            }
        }

        public Vector3 Color
        {
            get
            {
                return this.lightInfo.color;
            }
        }

        public int GlobalBindingIndex
        {
            get
            {
                return this.globalBindingIndex;
            }
        }
        
        public Light(string name, Vector3 location, Vector3 color)
        {
            this.lightInfo.location = location;
            this.lightInfo.color = color;

            this.globalBindingIndex = LightsInScene.Count + 2;

            GL.GenBuffers(1, out this.globalLightUBO);
            GL.BindBuffer(BufferTarget.UniformBuffer, this.globalLightUBO);
            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)(Vector3.SizeInBytes * 2), ref lightInfo, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, this.globalBindingIndex, this.globalLightUBO, IntPtr.Zero, (IntPtr)(Vector3.SizeInBytes * 2));

            LightsInScene.Add(name, this);
        }

        private struct Info
        {
            public Vector3 location;
            public Vector3 color;
        }
    }
}
