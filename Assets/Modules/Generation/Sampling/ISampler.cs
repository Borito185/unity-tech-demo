using Generation.Noise.Chunking;
using UnityEngine;

namespace Generation.Noise
{
    public interface ISampler
    {
        public float Sample(Vector3 pos);
        internal static void SamplePoints(ref ChunkData data, ISampler sampler)
        {
            int size = data.size + 1;
            
            float[] points = new float[size * size * size];

            int i = 0;
            for (int z = 0; z < size; z++)
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                Vector3Int offset = new(x, y, z);
                
                Vector3 v = data.chunkWorldPos+ offset;
                float   f = sampler.Sample(v);
                f = Mathf.Clamp01(f);
                
                points[i++] = f;
            }

            data.points = points;
        }
    }
}