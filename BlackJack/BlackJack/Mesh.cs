// <summary> Create and manipulates the mesh for a provided model. </summary>

namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OpenTK;
    
    /// <summary>
    /// Creates usable object data from a file.
    /// </summary>
    public class Mesh
    {
        /// <summary> A list of the vertexes from the model. </summary>
        private List<Vector3> vertexList = new List<Vector3>();

        /// <summary> A list of the texture coordinates from the model. </summary>
        private List<Vector2> textureList = new List<Vector2>();

        /// <summary> A list of the normal vertexes from the model. </summary>
        private List<Vector3> normalList = new List<Vector3>();

        /// <summary> A list of the faces in the format [vertex, texture, normal]. </summary>
        private List<int[]> faceList = new List<int[]>();

        /// <summary> A list of vertexes as gathered from the loaded model data. </summary>
        private List<Vertex> verticies;

        /// <summary> A list of integers that represents the order of vertexes to draw. </summary>
        private List<short> indicies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        public Mesh(string filename)
        {
            string line;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader stream = new StreamReader(fs, System.Text.Encoding.UTF8, true, 128))
                {
                    while ((line = stream.ReadLine()) != null)
                    {
                        this.ParseLine(line);
                    }
                }
            }

            // Add a Placeholder Texture Vector if the file didn't contain one.
            if (this.textureList.Count == 0)
            {
                this.textureList.Add(Vector2.Zero);
            }

            // Add a Placeholder Normal Vector if the file didn't contain one.
            if (this.normalList.Count == 0)
            {
                this.normalList.Add(Vector3.Zero);
            }

            this.GenerateMeshData();
        }

        /// <summary>
        /// Gets a list of vertexes as gathered from the loaded model data.
        /// </summary>
        public Vertex[] Verticies
        {
            get
            {
                return this.verticies.ToArray();
            }
        }

        /// <summary>
        /// Gets a list of indexes that represents the order of vertexes to draw.
        /// </summary>
        public short[] Indicies
        {
            get
            {
                return this.indicies.ToArray();
            }
        }

        /// <summary>
        /// Parse each line and assign it's value to it's respective variable.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        private void ParseLine(string line)
        {
            if (line != string.Empty)
            {
                string type = line.Substring(0, 2);
                string data = line.Substring(2);
                switch (type)
                {
                    case "o ":
                        {
                            Console.WriteLine("Loading Object " + data + "...");
                            break;
                        }

                    case "v ":
                        {
                            Vector3 newVector = new Vector3();
                            float.TryParse(data.Split(' ')[0], out newVector.X);
                            float.TryParse(data.Split(' ')[1], out newVector.Y);
                            float.TryParse(data.Split(' ')[2], out newVector.Z);
                            this.vertexList.Add(newVector);
                            break;
                        }

                    case "vt":
                        {
                            Vector2 newTexVert = new Vector2();
                            float.TryParse(data.Split(' ')[1], out newTexVert.X);
                            float.TryParse(data.Split(' ')[2], out newTexVert.Y);
                            newTexVert.Y = 1 - newTexVert.Y; // Why? Look into why blender flips this.
                            this.textureList.Add(newTexVert);
                            break;
                        }

                    case "vn":
                        {
                            Vector3 newNormal = new Vector3();
                            float.TryParse(data.Split(' ')[0], out newNormal.X);
                            float.TryParse(data.Split(' ')[1], out newNormal.Y);
                            float.TryParse(data.Split(' ')[2], out newNormal.Z);
                            this.normalList.Add(newNormal);
                            break;
                        }

                    case "f ":
                        {
                            // Find out how much data is present in each face group
                            switch (data.Split(' ')[0].Split('/').Length)
                            {
                                case 1:
                                    {
                                        // The file only provides verticies
                                        foreach (string subData in data.Split(' '))
                                        {
                                            int vertFace, texFace, normFace;
                                            int.TryParse(subData, out vertFace);
                                            vertFace--;
                                            texFace = 0;
                                            normFace = 0;
                                            this.faceList.Add(new int[] { vertFace, texFace, normFace });
                                        }

                                        break;
                                    }

                                case 2:
                                    {
                                        // The file provides Verticies and Texture Coordinates.
                                        foreach (string subData in data.Split(' '))
                                        {
                                            int vertFace, texFace, normFace;
                                            int.TryParse(subData.Split('/')[0], out vertFace);
                                            int.TryParse(subData.Split('/')[1], out texFace);
                                            vertFace--;
                                            texFace--;
                                            normFace = 0;
                                            this.faceList.Add(new int[] { vertFace, texFace, normFace });
                                        }

                                        break;
                                    }

                                case 3:
                                    {
                                        // The file provides Verticies, Texture Coordinates, and Normal Vectors.
                                        foreach (string subData in data.Split(' '))
                                        {
                                            int vertFace, texFace, normFace;
                                            int.TryParse(subData.Split('/')[0], out vertFace);
                                            int.TryParse(subData.Split('/')[1], out texFace);
                                            int.TryParse(subData.Split('/')[2], out normFace);
                                            vertFace--;
                                            // There can be a case where the texture attribute is empty, in which case keep this 0.
                                            if (texFace > 0)
                                            {
                                                texFace--;
                                            }
                                            normFace--;
                                            this.faceList.Add(new int[] { vertFace, texFace, normFace });
                                        }

                                        break;
                                    }
                            }
                            
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Generates the Mesh Data from the current model data.
        /// </summary>
        private void GenerateMeshData()
        {
            this.verticies = new List<Vertex>();
            this.indicies = new List<short>();

            Dictionary<string, short> vertIndex = new Dictionary<string, short>();

            foreach (int[] face in this.faceList)
            {
                string faceID = string.Format("{0}/{1}/{2}", face[0], face[1], face[2]);

                if (!vertIndex.ContainsKey(faceID))
                {
                    vertIndex.Add(faceID, (short)vertIndex.Keys.Count);
                    this.verticies.Add(new Vertex(this.vertexList[face[0]], this.textureList[face[1]], this.normalList[face[2]]));
                }

                this.indicies.Add(vertIndex[faceID]);
            }
        }
    }
}