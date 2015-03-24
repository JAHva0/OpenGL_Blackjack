﻿// <summary> Creates and renders an OpenGL window, handles input and loads resources. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    /// <summary>
    /// The class that manages the Window for us.
    /// </summary>
    public class Window : GameWindow
    {
        /// <summary>
        /// Testing object.
        /// </summary>
        private List<BaseGLObject> obj = new List<BaseGLObject>();

        private Light testLight;
        private float lightAngle = 0;
        private float lightrotationSpeed = 0.05f;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="windowDimensions">The dimensions of the window in pixels.</param>
        /// <param name="title">The title to appear at the top of the window.</param>
        public Window(Size windowDimensions, string title)
        {
            this.Location = new Point(10, 10);
            this.Title = title;
            this.WindowBorder = OpenTK.WindowBorder.Fixed;
            this.ClientSize = windowDimensions;
        }

        /// <summary>
        /// Runs once, as soon as the window is opened. Initializations go here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.Black);

            // Tells OpenGL to cull faces that appear in the back, and that our front faces will be winding clockwise.
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);

            // Enable depth testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthRange(0.0f, 1.0f);

            // Enable Textures
            GL.Enable(EnableCap.Texture2D);

            this.KeyDown += this.Window_KeyDown;

            Camera.Initialize(this.Size, 0.1f, 100f, new Vector3(35.0f, 0.0f, 35.0f), Vector3.Zero);
            Shaders.Load();
            this.testLight = new Light("Main", new Vector3(10.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.3f, 1.0f));

            float numMonkeys = 10f;
            float monkeyStep = 2f;

            for (float x = -numMonkeys; x <= numMonkeys; x += monkeyStep)
            {
                for (float y = -numMonkeys; y <= numMonkeys; y += monkeyStep)
                {
                    for (float z = -numMonkeys; z <= numMonkeys; z += monkeyStep)
                    {
                        BaseGLObject newobj = new BaseGLObject();
                        Vector3 newLocation = new Vector3(x, y, z);
                        Random r = new Random();
                        newobj.RotateX(r.Next(0, 180));
                        newobj.RotateY(r.Next(0, 180));
                        newobj.RotateZ(r.Next(0, 180));
                        newobj.SetPosition(newLocation);
                        this.obj.Add(newobj);
                    }
                }
            }
        }

        /// <summary>
        /// Called whenever the window is set to a different size, either programmatically or by the user.
        /// Maintain the aspect ratio of the window here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, this.Width, this.Height);
        }

        /// <summary>
        /// Called just before the next frame is shown.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            foreach (BaseGLObject o in this.obj)
            {
                o.RotateY(0.5f);
            }

            Light.LightsInScene["Main"].SetPosition(
                new Vector3(
                    (float)(5 * Math.Sin(this.lightAngle)), 
                    0f, 
                    (float)(5 * Math.Cos(this.lightAngle))));

            this.lightAngle += this.lightrotationSpeed;
            if (Math.Abs(this.lightAngle) > 2 * Math.PI)
            {
                Random r = new Random();
                this.lightrotationSpeed = (float)r.Next(3, 10) / 100;
                if (r.Next(0, 2) == 1)
                {
                    this.lightrotationSpeed = -this.lightrotationSpeed;
                }
                this.lightAngle = 0;
            }
        }

        /// <summary>
        /// Called when the window is showing the next frame. Render calls go here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (int shaderProgram in Shaders.ProgramList.Values)
            {
                var items = this.obj.Where(x => x.ShaderProgram == shaderProgram);
                
                GL.UseProgram(shaderProgram);
                foreach (BaseGLObject o in items)
                {
                    o.Render();
                }
                GL.UseProgram(0);

            }

            this.SwapBuffers();
        }

        /// <summary>
        /// Handles user input from the keyboard.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                
                default: break;
            }
        }
    }
}
