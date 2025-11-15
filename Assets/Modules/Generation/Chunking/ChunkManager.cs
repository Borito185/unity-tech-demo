using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generation.Noise.Chunking
{
    [RequireComponent(typeof(Generator))]
    public class ChunkManager : ChunkManagerBase
    {
        private Generator mesh;
        
        public List<Transform> trackingTransforms = new();
        public int             radius;
        
        [SerializeField]
        private bool isLoaded;

        private void Start()
        {
            StartCoroutine(CheckChunks());
        }

        private IEnumerator CheckChunks()
        {
            while (true)
            {
                if (isActiveAndEnabled)
                {
                    StartUpdateChunks();
                }

                yield return new WaitForSeconds(0.2f);
            }
        }

        /// <summary>
        /// Loads/unloads chunks in vicinity of tracked transforms.
        /// </summary>
        private void StartUpdateChunks()
        {
            HashSet<Vector3Int> vicinity = new((radius * 2) ^ 2);
            foreach (Transform t in trackingTransforms) 
                FindChunksInVicinity(t.position, vicinity);

            foreach (Vector3Int c in loadedChunks.Keys.ToList())
            {
                if (vicinity.Contains(c)) continue;

                DespawnChunk(c);
            }

            foreach (Vector3Int c in vicinity)
            {
                if (loadedChunks.ContainsKey(c)) continue;

                _ = LoadChunk(c);
            }
        }

        #region Positional

        private void FindChunksInVicinity(Vector3 pos, HashSet<Vector3Int> set)
        {
            Vector3Int chunkPos = WorldPositionToChunkPosition(pos);
            for (int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
            for (int z = -radius; z <= radius; z++)
            {
                Vector3Int offset = new(x, y, z);
                if (offset.sqrMagnitude <= radius * radius)
                    set.Add(chunkPos + offset);
            }
        }
        
        private Vector3Int WorldPositionToChunkPosition(Vector3 pos)
        {
            Vector3 relativePos    = pos - transform.position;
            Vector3 scaledToChunks = relativePos / chunkSize;
            return Vector3Int.FloorToInt(scaledToChunks);
        }

        private Vector3 ChunkPositionToWorldPosition(Vector3Int chunkPos, bool center = true)
        {
            Vector3 local = chunkPos * chunkSize;
            if (center)
                local += 0.5f * chunkSize * Vector3.one;
            return transform.position + local;
        }

        #endregion
    }
}