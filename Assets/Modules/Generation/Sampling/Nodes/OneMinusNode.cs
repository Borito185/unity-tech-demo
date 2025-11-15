using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class OneMinusNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase input;
        
        public override void Prepare(int seed)
        {
            input = GetInputValue(nameof(input), input);
        }

        public override float Sample(Vector3 pos)
        {
            float f = input.Sample(pos);
            f = 1 - f;
            return f;
        }
    }
}