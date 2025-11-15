using System.Collections.Generic;
using Generation.Details;
using UnityEngine;

namespace Generation.Noise.Chunking
{
    public struct ChunkData
    {
        public ChunkManagerBase parent;
        public Chunk            chunk;

        public int  seed;
        public bool firstSpawn;
        public int  size;
        
        public Vector3Int chunkPos;
        public Vector3    chunkWorldPos;
        
        public Vector3[] vertices;
        public int[]     triangles;

        public float[] points;
        

        public Mesh ToMesh()
        {
            Mesh m = new()
            {
                vertices  = vertices,
                triangles = triangles
            };
            m.RecalculateNormals();
            m.RecalculateTangents();
            return m;
        }

        public Bounds GetBounds()
        {
            Bounds b = new();
            b.SetMinMax(chunkWorldPos, chunkWorldPos + Vector3.one * (size+1));
            return b;
        }
    }
}