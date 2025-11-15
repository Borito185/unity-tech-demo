using System.Collections.Generic;
using Generation.Noise;
using Generation.Noise.Chunking;
using UnityEngine;

namespace Generation.Details
{
    public abstract class Detail : ScriptableObject
    {
        public GameObject prefab;
        public abstract void TrySpawn(ref ChunkData cd);

        protected GameObject Spawn(ChunkData cd)
        {
            GameObject go = Instantiate(prefab, cd.parent.detailContainer);
            return go;
        }

        protected IEnumerable<Triangle> IterateTriangles(ChunkData cd)
        {
            Vector3[] v = cd.vertices;
            int[]     t = cd.triangles;
            
            for (int i = 0; i < t.Length; i += 3)
            {
                Vector3 a = v[t[i]], b = v[t[i+1]], c = v[t[i+2]];

                a += cd.chunkWorldPos;
                b += cd.chunkWorldPos;
                c += cd.chunkWorldPos;
                
                yield return new Triangle(a,b,c);
            }
        }
        
        protected readonly struct Triangle
        {
            public readonly Vector3 a;
            public readonly Vector3 b;
            public readonly Vector3 c;

            public Vector3 GetCenter()
            {
                return (a + b + c) / 3;
            }

            public Vector3 GetNormal()
            {
                return Vector3.Cross(b - a, c - a).normalized;
            }

            public float GetSize()
            {
                return Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            }

            public Triangle(Vector3 a, Vector3 b, Vector3 c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }
        }
    }
}