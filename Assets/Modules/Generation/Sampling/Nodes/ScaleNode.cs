using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class ScaleNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase input;

        public Vector3 scalePos = Vector3.one;
        public float   scaleF   = 1f;

        public override void Prepare(int seed)
        {
            input = GetInputValue(nameof(input), input);
        }

        public override float Sample(Vector3 pos)
        {
            Vector3 scaled = Vector3.Scale(pos, scalePos);
            float   f      = input.Sample(scaled);
            f = f * scaleF;
            return f;
        }
    }
}