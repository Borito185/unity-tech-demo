using System;
using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class MathNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase input;
        
        public Strat combineStrategy;
        public float value;
        

        public enum Strat
        {
            Add,
            Multiply,
            Power,
            Min,
            Max,
            Abs
        }

        private Func<float, float> func;
        
        public override void Prepare(int seed)
        {
            input = GetInputValue(nameof(input), input);
            
            func = combineStrategy switch
            {
                Strat.Add                   => f => f + value,
                Strat.Multiply              => f => f * value,
                Strat.Power when value == 2 => f => f * f,
                Strat.Power                 => f => Mathf.Pow(f, value),
                Strat.Min                   => f => Mathf.Min(f, value),
                Strat.Max                   => f => Mathf.Max(f, value),
                Strat.Abs                   => Mathf.Abs,
                _                           => throw new ArgumentOutOfRangeException()
            };
        }

        public override float Sample(Vector3 pos)
        {
            float f = input.Sample(pos);
            f = func(f);
            return f;
        }
    }
}