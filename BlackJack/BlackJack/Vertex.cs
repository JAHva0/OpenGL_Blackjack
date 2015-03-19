// <summary> Vertex Struct for simplifying data storage. </summary>

namespace BlackJack
{
    using System;
    using OpenTK;

    /// <summary>
    /// Data structure for holding information about a specific point on an object.
    /// </summary>
    public struct Vertex
    {
        /// <summary> The position of the vertex in 3d space. </summary>
        private Vector3 position;

        /// <summary> The position U and V coordinates for this vertex on the texture. </summary>
        private Vector2 texture;

        /// <summary> The normal vector for this point. </summary>
        private Vector3 normal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="positiondata">The X,Y, and Z coordinates for the point.</param>
        /// <param name="texturedata">The T and U coordinates of the texture for this point.</param>
        /// <param name="normaldata">The normal vector for this point.</param>
        public Vertex(Vector3 positiondata, Vector2 texturedata, Vector3 normaldata)
        {
            this.position = positiondata;
            this.texture = texturedata;
            this.normal = normaldata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="positiondata">The X,Y, and Z coordinates for the point.</param>
        /// <param name="texturedata">The T and U coordinates of the texture for this point.</param>
        public Vertex(Vector3 positiondata, Vector2 texturedata)
            : this(positiondata, texturedata, Vector3.Zero)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct.
        /// </summary>
        /// <param name="positiondata">The X,Y, and Z coordinates for the point.</param>
        public Vertex(Vector3 positiondata)
            : this(positiondata, Vector2.Zero, Vector3.Zero)
        { 
        }

        /// <summary> Gets an integer representation of a vertex point in bytes. </summary>
        /// <value> The size in bytes of the Vertex structure. </value>
        public static IntPtr SizeInBytes
        {
            get
            {
                return (IntPtr)(Vector3.SizeInBytes + Vector2.SizeInBytes + Vector3.SizeInBytes);
            }
        }

        /// <summary> Gets an integer representation of a texture offset in bytes. </summary>
        /// <value> The size in bytes of the texture offset. </value>
        public static IntPtr TextureOffset
        {
            get
            {
                return (IntPtr)Vector3.SizeInBytes;
            }
        }

        /// <summary> Gets an integer representation of a normal offset in bytes. </summary>
        /// <value> The size in bytes of the normal offset. </value>
        public static IntPtr NormalOffset
        {
            get
            {
                return (IntPtr)(Vector3.SizeInBytes + Vector2.SizeInBytes);
            }
        }
    }
}