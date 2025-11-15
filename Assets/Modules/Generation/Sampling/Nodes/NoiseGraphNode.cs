using UnityEngine;

namespace Generation.Noise.Nodes
{
    /// <summary>
    ///     GraphNode for adding a subgraph. It essentially enables you to create reusable functions.
    /// </summary>
    public class NoiseGraphNode : NoiseNodeBase
    {
        public NoiseGraph subGraph;

        public override void Prepare(int seed)
        {
            if(subGraph)
                subGraph.Prepare(seed);
        }

        public override float Sample(Vector3 pos)
        {
            return subGraph.Sample(pos);
        }
    }
}