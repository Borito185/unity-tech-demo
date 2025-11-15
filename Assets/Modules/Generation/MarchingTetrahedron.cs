using System;
using System.Collections.Generic;
using Generation.Noise.Chunking;
using UnityEngine;

namespace Generation.Noise
{
    public static class MarchingTetrahedron
    {
        public static void CreateMesh(ref ChunkData data)
        {
            int pointDim = data.size + 1;
            int size     = data.size;
            
            int bufferStartSize = pointDim * pointDim * pointDim * 2;

            Dictionary<Vector3, int> vertexMap = new(bufferStartSize);
            List<int>                indices   = new(bufferStartSize * 2);

            Span<Vector4> cube        = stackalloc Vector4[8];
            Span<Vector4> tetrahedron = stackalloc Vector4[4];
            for (int z = 0; z < size; z++)
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                Vector3Int pos = new(x, y, z);
                GetCube(data.points, pointDim, ref cube, pos);
                
                for (int i = 0; i < 6; i++)
                {
                    GetTetrahedron(in cube, ref tetrahedron, i);
                    int index = GetTetrahedronTypeIndex(0.5f, in tetrahedron);
                    
                    if (index == 0) continue;
                    
                    int[] triangles = triangulation[index];
                    for (int j = 0; triangles[j] != -1; j += 3)
                    {
                        Vector3 CreateVertex(in Span<Vector4> tetrahedron, int index)
                        {
                            Vector4 a = tetrahedron[TetrahedronEdgeConnection[triangles[index], 0]];
                            Vector4 b = tetrahedron[TetrahedronEdgeConnection[triangles[index], 1]];

                            Vector3 v = Interpolate(a, b, 0.5f);
                            return v;
                        }

                        Vector3 v0 = CreateVertex(in tetrahedron, j);
                        Vector3 v1 = CreateVertex(in tetrahedron, j+1);
                        Vector3 v2 = CreateVertex(in tetrahedron, j+2);
                    
                        indices.Add(vertexMap.AddVertex(v2));
                        indices.Add(vertexMap.AddVertex(v1));
                        indices.Add(vertexMap.AddVertex(v0));
                    }
                }
            }
            
            data.vertices  = vertexMap.ToVertexArray();
            data.triangles = indices.ToArray();
        }

        private static Vector3 Interpolate(Vector4 a, Vector4 b, float isoSurface)
        {
            float t = Mathf.InverseLerp(a.w, b.w, isoSurface);
            return Vector3.Lerp(a, b, t);
        }
        
        private static int GetTetrahedronTypeIndex(float isoSurface, in Span<Vector4> tetra)
        {
            int index = 0;
            index |= (tetra[0].w < isoSurface ? 1 : 0) << 0;
            index |= (tetra[1].w < isoSurface ? 1 : 0) << 1;
            index |= (tetra[2].w < isoSurface ? 1 : 0) << 2;
            index |= (tetra[3].w < isoSurface ? 1 : 0) << 3;
            return index;
        }
        
        private static void GetTetrahedron(in Span<Vector4> cube, ref Span<Vector4> tetrahedron, int index)
        {
            tetrahedron[0] = cube[TetrahedronsInACube[index, 0]];
            tetrahedron[1] = cube[TetrahedronsInACube[index, 1]];
            tetrahedron[2] = cube[TetrahedronsInACube[index, 2]];
            tetrahedron[3] = cube[TetrahedronsInACube[index, 3]];
        }
        
        private static void GetCube(float[] points, int space, ref Span<Vector4> cube, Vector3Int pos)
        {
            int stack = space * space;
            
            for (int i = 0; i < 8; i++)
            {
                Vector3Int v = pos + corners[i];
                cube[i] = new Vector4(v.x, v.y, v.z);
            }
            
            int v0 = Locate(space, pos);
            int v1 = v0 + 1;
            int v2 = v1 + stack;
            int v3 = v0 + stack;
            
            int v4 = v0 + space;
            int v5 = v1 + space;
            int v6 = v2 + space;
            int v7 = v3 + space;

            cube[0].w = points[v0];
            cube[1].w = points[v1];
            cube[2].w = points[v2];
            cube[3].w = points[v3];
            cube[4].w = points[v4];
            cube[5].w = points[v5];
            cube[6].w = points[v6];
            cube[7].w = points[v7];
        }
        
        private static int Locate(int space, Vector3Int pos)
        {
            return pos.z * space * space + pos.y * space + pos.x;
        }

        static readonly Vector3Int[] corners =
        {
            new(0,0,0), new(1,0,0), new(1,0,1), new(0,0,1),
            new(0,1,0), new(1,1,0), new(1,1,1), new(0,1,1)
        };
        
        /// <summary>
        /// TetrahedronEdgeConnection lists the index of verticies from a cube 
        /// that made up each of the six tetrahedrons within the cube.
        /// tetrahedronsInACube[6][4]
        /// </summary>
        private static readonly int[,] TetrahedronsInACube = new int[,]
        {
            {0,5,1,6},
            {0,1,2,6},
            {0,2,3,6},
            {0,3,7,6},
            {0,7,4,6},
            {0,4,5,6}
        };
        
        /// <summary>
        /// TetrahedronEdgeConnection lists the index of the endpoint vertices for each of the 6 edges of the tetrahedron.
        /// tetrahedronEdgeConnection[6][2]
        /// </summary>
        private static readonly int[,] TetrahedronEdgeConnection = new int[,]
        {
            {0,1},  {1,2},  {2,0},  {0,3},  {1,3},  {2,3}
        };
        
        /// <summary>
        /// For any edge, if one vertex is inside of the surface and the other is outside of 
        /// the surface then the edge intersects the surface
        /// For each of the 4 vertices of the tetrahedron can be two possible states, 
        /// either inside or outside of the surface
        /// For any tetrahedron the are 2^4=16 possible sets of vertex states.
        /// This table lists the edges intersected by the surface for all 16 possible vertex states.
        /// There are 6 edges.  For each entry in the table, if edge #n is intersected, then bit #n is set to 1.
        /// tetrahedronEdgeFlags[16]
        /// </summary>
        private static readonly int[] TetrahedronEdgeFlags = new int[]
        {
            0x00, 0x0d, 0x13, 0x1e, 0x26, 0x2b, 0x35, 0x38, 0x38, 0x35, 0x2b, 0x26, 0x1e, 0x13, 0x0d, 0x00
        };
        
        /// <summary>
        /// For each of the possible vertex states listed in tetrahedronEdgeFlags there
        /// is a specific triangulation of the edge intersection points.  
        /// TetrahedronTriangles lists all of them in the form of 0-2 edge triples 
        /// with the list terminated by the invalid value -1.
        /// tetrahedronTriangles[16][7]
        /// </summary>
        private static readonly int[][] triangulation = {
            new[]{-1, -1, -1, -1, -1, -1, -1},
            new[]{ 0,  3,  2, -1, -1, -1, -1},
            new[]{ 0,  1,  4, -1, -1, -1, -1},
            new[]{ 1,  4,  2,  2,  4,  3, -1},

            new[]{ 1,  2,  5, -1, -1, -1, -1},
            new[]{ 0,  3,  5,  0,  5,  1, -1},
            new[]{ 0,  2,  5,  0,  5,  4, -1},
            new[]{ 5,  4,  3, -1, -1, -1, -1},

            new[]{ 3,  4,  5, -1, -1, -1, -1},
            new[]{ 4,  5,  0,  5,  2,  0, -1},
            new[]{ 1,  5,  0,  5,  3,  0, -1},
            new[]{ 5,  2,  1, -1, -1, -1, -1},

            new[]{ 3,  4,  2,  2,  4,  1, -1},
            new[]{ 4,  1,  0, -1, -1, -1, -1},
            new[]{ 2,  3,  0, -1, -1, -1, -1},
            new[]{-1, -1, -1, -1, -1, -1, -1}
        };

    }
}