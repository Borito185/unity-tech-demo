using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class LerpNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase input;
        
        public Vector2 _in  = new Vector2(0, 1);
        public Vector2 _out = new Vector2(0, 1);
        
        public override void Prepare(int seed)
        {
            input = GetInputValue(nameof(input), input);
        }

        public override float Sample(Vector3 pos)
        {
            float f = input.Sample(pos);
            f = Mathf.InverseLerp(_in.x, _in.y, f);
            f = Mathf.Lerp(_out.x, _out.y, f);
            return f;
        }
    }
}