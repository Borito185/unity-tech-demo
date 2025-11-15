using Generation.Noise;
using Generation.Noise.Chunking;
using UnityEngine;

namespace Generation.Details
{
    [CreateAssetMenu(menuName = "Cave/Details/Noisy Oriented")]
    public class NoisyOrientedDetail : Detail
    {
        public NoiseGraph noise;
        [Tooltip("values higher than noise cutoff are considered as possible spawn location")]
        public float      noiseCutoff = 0.99f;
        
        [Tooltip("This detail can only be placed on triangles with a similar normal")]
        public Vector3 normal = Vector3.up;
        [Tooltip("Defines the range the dot product of the normals should be in.")]
        public Vector2 dotRange = new(-1,1);
        [Tooltip("Defines the min and max size of the triangle this detail is placed on.")]
        public Vector2 sizeRange = new (0, 5);

        [Tooltip("The detail is spawned on the triangle + offset * normal")]
        public float offset = 0.2f;
        
        public override void TrySpawn(ref ChunkData cd)
        {
            noise.Prepare(cd.seed);
            foreach (Triangle t in IterateTriangles(cd))
            {
                Vector3 tCenter = t.GetCenter();
                Vector3 tNormal = t.GetNormal();
                
                float normalDot = Vector3.Dot(normal, tNormal);
                if (normalDot < dotRange.x || normalDot > dotRange.y)
                    continue;
                
                float noiseValue = noise.Sample(tCenter);
                if (noiseValue < noiseCutoff)
                    continue;
                
                float size       = t.GetSize();
                if (size < sizeRange.x || size > sizeRange.y)
                    continue;

                var go = Spawn(cd);
                go.transform.position = tCenter + tNormal * offset;
            }
        }
    }
}