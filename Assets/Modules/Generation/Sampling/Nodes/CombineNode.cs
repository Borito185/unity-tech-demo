using System;
using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class CombineNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase[] inputs;

        public Strat combineStrategy;

        public enum Strat
        {
            Sum,
            Multiply,
            Average,
            Min,
            Max
        }

        public override void Prepare(int seed)
        {
            inputs = GetInputValues(nameof(inputs), inputs);
        }

        public override float Sample(Vector3 pos)
        {
            if (inputs == null || inputs.Length == 0)
                return 1f;

            // use span to avoid allocation
            Span<float> f = stackalloc float[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                f[i] = inputs[i].Sample(pos);

            return combineStrategy switch
            {
                Strat.Sum      => Sum(in f),
                Strat.Multiply => Product(in f),
                Strat.Average  => Average(in f),
                Strat.Min      => Min(in f),
                Strat.Max      => Max(in f),
                _              => throw new ArgumentOutOfRangeException()
            };
        }

        private float Sum(in Span<float> f)
        {
            float sum = 0f;
            for (int i = 0; i < f.Length; i++)
                sum += f[i];

            return sum;
        }

        private float Product(in Span<float> f)
        {
            float prod = 1f;
            for (int i = 0; i < f.Length; i++)
                prod *= f[i];

            return prod;
        }

        private float Average(in Span<float> f)
        {
            float sum = 0f;
            for (int i = 0; i < f.Length; i++)
                sum += f[i];

            return sum / inputs.Length;
        }

        private float Min(in Span<float> f)
        {
            float min = float.MaxValue;
            for (int i = 0; i < f.Length; i++)
                min = Mathf.Min(f[i], min);

            return min;
        }

        private float Max(in Span<float> f)
        {
            float max = float.MinValue;
            for (int i = 0; i < f.Length; i++)
                max = Mathf.Max(f[i], max);

            return max;
        }
    }
}