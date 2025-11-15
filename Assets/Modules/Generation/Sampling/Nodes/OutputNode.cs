using UnityEngine;
using XNode;

namespace Generation.Noise.Nodes
{
    public class OutputNode : Node, ISampler
    {
        [Input] public NoiseNodeBase output;

        public void Prepare(int seed)
        {
            output = GetInputValue(nameof(output), output);
        }

        public float Sample(Vector3 pos)
        {
            float f = output.Sample(pos);
            f = Mathf.Clamp01(f);
            return f;
        }
    }
}