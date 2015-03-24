// <summary> Creates and renders an OpenGL window, handles input and loads resources. </summary>

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
        private BaseGLObject obj;

        /// <summary> A Generic Light. </summary>
        private Light testLight;

        /// <summary> A text object. </summary>
        private Text textText;
        
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

            Camera.Initialize(this.Size, 0.1f, 100f, new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero);
            Shaders.Load();
            this.testLight = new Light("Main", new Vector3(0.0f, 0.0f, 5.0f), new Vector3(0.0f, 0.3f, 1.0f));

            this.obj = new BaseGLObject();
            this.textText = new Text("Hello World");
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
        }

        /// <summary>
        /// Called when the window is showing the next frame. Render calls go here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(this.obj.ShaderProgram);
            this.obj.Render();
            GL.UseProgram(0);

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
