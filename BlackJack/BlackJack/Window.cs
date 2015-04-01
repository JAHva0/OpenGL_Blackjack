// <summary> Creates and renders an OpenGL window, handles input and loads resources. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Timers;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;

    /// <summary>
    /// The class that manages the Window for us.
    /// </summary>
    public class Window : GameWindow
    {
        private Timer FPSUpdate = new Timer(1000);
        private int frameCount = 0;
        private bool updateFPSText = false;
        
        /// <summary>
        /// Testing object.
        /// </summary>
        private List<BaseGLObject> objs = new List<BaseGLObject>();

        /// <summary> A Generic Light. </summary>
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

            this.KeyDown += this.Window_KeyDown;
            //this.MouseDown += Window_MouseDown;
            //this.MouseMove += Window_MouseMove;

            FPSUpdate.Elapsed += new ElapsedEventHandler(UpdateFPSCount);
        }

        void Window_MouseMove(object sender, MouseMoveEventArgs e)
        {
            foreach (BaseGLObject obj in this.objs)
            {
                obj.CheckForCollision(Camera.GetRaycast(e.X, e.Y));
            }
        }

        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<Vector3> Verticies = new List<Vector3>();

            foreach (BaseGLObject obj in this.objs)
            {
                obj.CheckForCollision(Camera.GetRaycast(e.X, e.Y));
            }
            

        }

        private void UpdateFPSCount(object source, ElapsedEventArgs e)
        {
            this.updateFPSText = true;
        }

        /// <summary>
        /// Runs once, as soon as the window is opened. Initializations go here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.FPSUpdate.Enabled = true;

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

            // Enable Textures - Does this do anything?
            // GL.Enable(EnableCap.Texture2D);

            // Enable Alpha and Blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Camera.Initialize(this.Size, 0.1f, 100f, new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero);
            Shaders.Load();
            this.testLight = new Light("Main", new Vector3(0.0f, 5.0f, 5.0f), new Vector3(1.0f, 1.0f, 1.0f));

            string modelFile = Entry.CurrentDirectory + @"\BlackJack\BlackJack\Models\Monkey.obj";
            string textureFile = Entry.CurrentDirectory + @"\BlackJack\BlackJack\Textures\monkey paint.png";

            this.objs.Add(new BaseGLObject(new Mesh(modelFile), textureFile, "Basic", "VertexShader", "FragmentShader"));

            Text newText = new Text("0 FPS", "Arial");
            newText.SetPosition(new Vector2(-750, 400));
            this.objs.Add(newText);
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

            if (this.updateFPSText)
            {
                // this.textText.SetText(this.frameCount.ToString() + " FPS");
                this.frameCount = 0;
                this.updateFPSText = false;
            }

        
                if (this.objs[0].CheckForCollision(Camera.GetRaycast(Mouse.X, Mouse.Y)))
                {
                    Light.LightsInScene["Main"].Color = new Vector3(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    Light.LightsInScene["Main"].Color = new Vector3(0.0f, 0.0f, 0.0f);
                }

            this.objs[0].RotateY(0.5f);
        }

        /// <summary>
        /// Called when the window is showing the next frame. Render calls go here.
        /// </summary>
        /// <param name="e">The parameter is not used.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            this.frameCount++;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (int shaderProgram in Shaders.ProgramList.Values)
            {
                var items = this.objs.Where(x => x.ShaderProgram == shaderProgram);

                GL.UseProgram(shaderProgram);
                foreach (BaseGLObject obj in items)
                {
                    obj.Render();
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
                case Key.Q:
                    {
                        Camera.SetPosition(new Vector3(10.0f, 10.0f, 10.0f)); break;
                    }
                case Key.R:
                    {
                        Camera.SetPosition(new Vector3(0.0f, 0.0f, 5.0f)); break;
                    }
                default: break;
            }
        }
    }
}
