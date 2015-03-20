// <summary> Base class to handle OpenGL functions and methods common to all displayed objects. </summary>

namespace BlackJack
{
    /// <summary>
    /// Basic class to handle common OpenGL methods.
    /// </summary>
    public class BaseGLObject
    {
        /// <summary> The handle for the vertex buffer data. </summary>
        private int vertexBufferObject = -1;

        /// <summary> The handle for the index buffer data. </summary>
        private int indexBufferObject = -1;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGLObject"/> class. 
        /// The blank constructor just creates a basic card for testing.
        /// </summary>
        public BaseGLObject()
        {
        }
    }
}
