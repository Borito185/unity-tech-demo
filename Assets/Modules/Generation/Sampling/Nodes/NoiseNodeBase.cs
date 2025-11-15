using UnityEngine;
using XNode;

namespace Generation.Noise.Nodes
{
    /// <summary>
    ///     General parent for nodes to ensure that the data flows nicely.
    /// </summary>
    public abstract class NoiseNodeBase : Node, ISampler
    {
        [Output] public NoiseNodeBase output;

        /// <summary>
        ///     Called before generation. Should be used for initializing required inputs using the example below.
        ///     <code>myValue = GetInputValue(nameof(myValue), myValue)</code>
        /// </summary>
        public abstract void Prepare(int seed);

        /// <summary>
        ///     Fetches the value associated for the given position.
        ///     This value will be used to generate the cave where 0 -> space 1 -> wall.
        /// </summary>
        /// <param name="pos">The position to query.</param>
        /// <returns>The value associated with this position.</returns>
        public abstract float Sample(Vector3 pos);

        /// <summary>
        ///     Ensures data flows nicely.
        ///     Assumes the only use for CaveNode will be <see cref="Sample" /> as it only allows the default output.
        /// </summary>
        /// <returns>The reference to this instance.</returns>
        public sealed override object GetValue(NodePort _)
        {
            return this;
        }
    }
}