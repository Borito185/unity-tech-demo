using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Generation.Noise.Chunking
{
    public class TestChunkManager : ChunkManagerBase
    {
        public Vector3Int loadArea = Vector3Int.one;
        public bool       repeat   = false;
        
        public async Task Generate()
        {
            Awake();
            bool firstPass = true;
            
            List<Awaitable> chunks = new();
            while (repeat || firstPass)
            {
                generator.graph.Prepare(42);
                
                base.Clear(clearChunks:false, clearDetails:true);
                
                firstPass = false;
                for (int x = 0; x < loadArea.x; x++)
                for (int y = 0; y < loadArea.y; y++)
                for (int z = 0; z < loadArea.z; z++)
                {
                    Vector3Int pos = new(x, y, z);

                    chunks.Add(LoadChunk(pos, forceReload:true, forceRespawnDetails:true));
                }
            
                foreach (Awaitable awaitable in chunks)
                {
                    await awaitable;
                }
                chunks.Clear();
            }
        }

        public void Clear()
        {
            base.Clear();
        }
    }
}