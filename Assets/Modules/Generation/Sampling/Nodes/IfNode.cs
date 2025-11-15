using UnityEngine;
using UnityEngine.Serialization;

namespace Generation.Noise.Nodes
{
    public class IfNode : NoiseNodeBase
    {
        public float MoreThan  = 0.5f;
        public  float lerpRange = 0f;

        [Input] public NoiseNodeBase condition;
        [Input] public NoiseNodeBase trueValue;
        [Input] public NoiseNodeBase falseValue;

        private bool ConditionEqualsTrue;
        private bool ConditionEqualsFalse;
        
        public override void Prepare(int seed)
        {
            condition  = GetInputValue(nameof(condition), condition);
            trueValue  = GetInputValue(nameof(trueValue), trueValue);
            falseValue = GetInputValue(nameof(falseValue), falseValue);

            ConditionEqualsTrue  = condition == trueValue;
            ConditionEqualsFalse = condition == falseValue;
        }

        public override float Sample(Vector3 pos)
        {
            float sample = condition.Sample(pos);
            if (sample <= MoreThan - lerpRange)
                return ConditionEqualsFalse ? sample : falseValue.Sample(pos);
            if (sample >= MoreThan + lerpRange)
                return ConditionEqualsTrue ? sample : trueValue.Sample(pos);

            float t = Mathf.InverseLerp(MoreThan-lerpRange, MoreThan+lerpRange, sample);
            float a = ConditionEqualsFalse ? sample : falseValue.Sample(pos);
            float b = ConditionEqualsTrue ? sample : trueValue.Sample(pos);
            return Mathf.Lerp(a, b, t);
        }
    }
}