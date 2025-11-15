using System.Threading;
using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class MapNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase input;

        public AnimationCurve map;
        public int            cacheSize = 256;
        
        private float[] cache;

        public override void Prepare(int seed)
        {
            input = GetInputValue(nameof(input), input);
            cache = GenerateCurveArray(map);
        }

        public sealed override float Sample(Vector3 pos)
        {
            float f = input.Sample(pos);
            if (f <= 0) return cache[0];
            if (f >= 1) return cache[^1];
            
            f *= cacheSize-1;
            int i = Mathf.RoundToInt(f);
            return cache[i];
        }
        
        public float[] GenerateCurveArray(AnimationCurve self)
        {
            float[] arr = new float[cacheSize];
            for (int j = 0; j < cacheSize; j++) 
                arr[j] = self.Evaluate((float)j / (cacheSize - 1));
            return arr;
        }
    }
}