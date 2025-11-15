using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class YLimitNode : NoiseNodeBase
    {
        public float yLimit;
        
        public override void Prepare(int seed)
        {
            
        }

        public override float Sample(Vector3 pos)
        {
            return pos.y > yLimit ? 0 : 1;
        }
    }
}