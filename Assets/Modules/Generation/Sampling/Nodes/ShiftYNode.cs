using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class ShiftYNode : NoiseNodeBase
    {
        [Input] public NoiseNodeBase shiftAmount;
        public         Vector3       shiftDirection = Vector3.up;
        [Input] public NoiseNodeBase input;

        private Vector3 normalized;
        
        public override void Prepare(int seed)
        {
            input       = GetInputValue(nameof(input), input);
            shiftAmount = GetInputValue(nameof(shiftAmount), shiftAmount);
            normalized  = shiftDirection.normalized;
        }

        public override float Sample(Vector3 pos)
        {
            float shift = shiftAmount.Sample(pos);

            pos += normalized * shift;
            return input.Sample(pos);
        }
    }
}