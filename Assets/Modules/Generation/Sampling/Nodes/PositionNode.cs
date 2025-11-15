using System;
using UnityEngine;

namespace Generation.Noise.Nodes
{
    public class PositionNode : NoiseNodeBase
    {
        public enum Options
        {
            X,
            Y,
            Z
        }

        public Options component = Options.Y; 
        
        public override void Prepare(int seed)
        {
        }

        public override float Sample(Vector3 pos)
        {
            return component switch
            {
                Options.X => pos.x,
                Options.Y => pos.y,
                Options.Z => pos.z,
                _         => throw new ArgumentOutOfRangeException()
            };
        }
    }
}