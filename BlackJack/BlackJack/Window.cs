﻿// <summary> Creates and renders an OpenGL window, handles input and loads resources. </summary>

namespace BlackJack
{
    using System;
    using System.Drawing;
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
        private BaseGLObject obj;

        private Light testLight;
        
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

            GL.ClearColor(Color.CornflowerBlue);

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

            Camera.Initialize(this.Size, 0.1f, 100f, new Vector3(0.0f, 0.0f, 15.0f), Vector3.Zero);
            Shaders.Load();
            this.testLight = new Light("Main", new Vector3(0.0f, 0.0f, 10.0f), new Vector3(1.0f, 1.0f, 1.0f));

            this.obj = new BaseGLObject();
            //this.obj.RotateX(45);
            //this.obj.RotateY(45);
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

            //this.obj.RotateY(.5f);
        }

        /// <summary>
        /// Called when the window is showing the next frame. Render calls go here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            this.obj.Render();

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
                case Key.R:
                    {
                        // Move the object to a random location
                        Random r = new Random();
                        Vector3 newLocation = new Vector3(r.Next(-5, 5), r.Next(-5, 5), r.Next(-5, 5));
                        this.obj.SetPosition(newLocation);
                        break;
                    }
                case Key.Up:
                    {
                        this.obj.Scale(1.1f);
                        break;
                    }
                case Key.Down:
                    {
                        this.obj.Scale(0.9f);
                        break;
                    }
                default: break;
            }
        }
    }
}
