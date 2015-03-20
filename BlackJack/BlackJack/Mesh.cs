namespace BlackJack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using OpenTK;

    
    public class Mesh
    {
        private List<Vector3> vertexList = new List<Vector3>();
        private List<Vector2> textureList = new List<Vector2>();
        private List<Vector3> normalList = new List<Vector3>();

        private List<int[]> faceList = new List<int[]>();

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
        }

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
                            foreach (string subData in data.Split(' '))
                            {
                                int vertFace, texFace, normFace;
                                int.TryParse(subData.Split('/')[0], out vertFace);
                                int.TryParse(subData.Split('/')[1], out texFace);
                                int.TryParse(subData.Split('/')[2], out normFace);
                                vertFace--;
                                texFace--;
                                normFace--;
                                this.faceList.Add(new int[] { vertFace, texFace, normFace });
                            }
                            break;
                        }
                }
            }
        }
    }
}