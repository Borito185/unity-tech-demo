using System;
using Generation.Noise.Nodes;
using UnityEngine;
using XNode;

namespace Generation.Noise
{
    [CreateAssetMenu]
    public class NoiseGraph : NodeGraph, ISampler
    {
        private OutputNode outNode;

        public void Prepare(int seed)
        {
            outNode = nodes.Find(n => n is OutputNode) as OutputNode;
            if (outNode == null)
                throw new ArgumentException($"Graph '{name}' was not set up correctly");
            
            outNode.Prepare(seed);
            foreach (Node node in nodes)
            {
                if (node is NoiseNodeBase b)
                    b.Prepare(seed);
            }
        }

        public float Sample(Vector3 pos)
        {
            return outNode.Sample(pos);
        }
    }
}