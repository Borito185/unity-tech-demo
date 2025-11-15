using System;
using System.Collections.Generic;
using Generation.Noise.Chunking;
using UnityEngine;
using UnityEngine.LowLevelPhysics;

namespace Generation.Noise
{
    [Serializable]
    public struct SampleOverride : IEquatable<SampleOverride>
    {
        [HideInInspector]
        public int     seed;
        [HideInInspector]
        public Vector3 pos;
        public float   size;
        public float   frequency;
        public float   roughness;
        public bool    isAdditive;

        public Bounds GetBounds()
        {
            Vector3 min = pos - Vector3.one * (size * 2);
            Vector3 max = pos + Vector3.one * (size * 2);

            Bounds b = new Bounds();
            b.SetMinMax(min, max);
            return b;
        }

        public void Prepare(FastNoiseLite noise)
        {
            noise.SetSeed(seed);
            noise.SetFrequency(frequency);
        }
        
        public float Sample(Vector3 pos, FastNoiseLite noise, float old)
        {
            float distance = (pos - this.pos).magnitude;

            float t = distance / (size * 2);
            t += noise.GetNoise(pos.x * frequency, pos.y * frequency, pos.z * frequency) * roughness;
            t =  Mathf.Clamp01(t);
            if (isAdditive)
            {
                t = 1 - t;
                t = Mathf.Max(old, t);
                return t;
            }

            t = Mathf.Min(old, t);
            return t;
        }

        public static void ApplyOverrides(ref ChunkData cd, List<SampleOverride> overrides)
        {
            int size = cd.size + 1;

            Bounds cdBounds = cd.GetBounds();

            int Loc(Vector3Int pos)
            {
                return pos.z * size * size + pos.y * size + pos.x;
            }

            FastNoiseLite noise = new FastNoiseLite();
            for (int i = 0; i < overrides.Count; i++)
            {
                var o = overrides[i];
                var b = o.GetBounds();
                if (!cdBounds.Intersects(b))
                    continue;

                Vector3Int min = Vector3Int.Max(Vector3Int.FloorToInt(b.min - cd.chunkWorldPos), Vector3Int.zero);
                Vector3Int max = Vector3Int.Min(Vector3Int.CeilToInt(b.max - cd.chunkWorldPos), Vector3Int.one * (size-1)); 
                o.Prepare(noise);
                for (int x = min.x; x <= max.x; x++)
                for (int y = min.y; y <= max.y; y++)
                for (int z = min.z; z <= max.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    
                    int   loc = Loc(pos);
                    float f   = o.Sample(pos + cd.chunkWorldPos, noise, cd.points[loc]);
                    cd.points[loc] = f;
                }
            }
        }

        public bool Equals(SampleOverride other)
        {
            return seed == other.seed && pos.Equals(other.pos) && size.Equals(other.size) && frequency.Equals(other.frequency) && roughness.Equals(other.roughness) && isAdditive == other.isAdditive;
        }

        public override bool Equals(object obj)
        {
            return obj is SampleOverride other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(seed, pos, size, frequency, roughness, isAdditive);
        }
    }
}