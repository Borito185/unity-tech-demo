using System.Collections.Generic;
using UnityEngine;

namespace Generation.Noise
{
    public static class MeshSimplification
    {
        public static void SimplifyMesh(Mesh mesh, float threshold)
        {
            /*Vector3[] verts = mesh.vertices;
            int[]     tris  = mesh.triangles;

            float                sqrThresh = threshold * threshold;
            Dictionary<int, int> remap     = new();

            int count = 0;
            
            // Merge close vertices
            for (int i = 0; i < verts.Length; i++)
            {
                if (remap.ContainsKey(i)) continue;

                count++;
                for (int j = i + 1; j < verts.Length; j++)
                {
                    if ((verts[i] - verts[j]).sqrMagnitude < sqrThresh)
                        remap[j] = i;
                }
            }

            // Create new vertex list
            List<Vector3> newVerts = new();
            int[]         map      = new int[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                int root = i;
                if (remap.TryGetValue(i, out int r)) 
                    root = r;

                if (!map.Contains(root))
                {
                    map[i] = newVerts.Count;
                    newVerts.Add(verts[root]);
                }
                else
                    map[i] = map[root];
            }

            // Remap triangles
            for (int i = 0; i < tris.Length; i++)
                tris[i] = map[tris[i]];

            mesh.vertices  = newVerts.ToArray();
            mesh.triangles = tris;
            mesh.RecalculateNormals();*/
        }
    }
}