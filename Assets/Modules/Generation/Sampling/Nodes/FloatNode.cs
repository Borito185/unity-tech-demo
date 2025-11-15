using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class FloatNode : NoiseNodeBase
    {
        public float value;

        public override void Prepare(int seed)
        {
            
        }

        public override float Sample(Vector3 pos)
        {
            return value;
        }
    }
}