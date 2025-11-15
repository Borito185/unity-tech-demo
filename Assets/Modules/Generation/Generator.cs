using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Generation.Details;
using Generation.Noise.Chunking;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation.Noise
{
    public class Generator : MonoBehaviour
    {
        private static readonly SemaphoreSlim pool = new(8);
        
        private int seed = Random.Range(-100_000, 100_000);
        
        public NoiseGraph   graph;
        
        public bool         includeDetails = true;
        public List<Detail> detailOptions;

        internal readonly List<SampleOverride> overrides = new();

        public bool isTesting;
        public bool useTetrahedron;

        public async Awaitable<ChunkData> Generate(ChunkData cd)
        {
            cd.seed = seed;
            await Awaitable.BackgroundThreadAsync();
            await pool.WaitAsync();
            try
            {
                Threaded_Generate(ref cd);
            }
            finally
            {
                pool.Release();
            }
            
            await Awaitable.MainThreadAsync();
            if (!cd.firstSpawn) return cd;
            cd.firstSpawn = false;

            if (includeDetails && (Utils.IsTesting() || isTesting))
            {
                foreach (Detail detail in detailOptions) 
                    detail.TrySpawn(ref cd);
            }
            
            return cd;
        }
        
        private void Threaded_Generate(ref ChunkData cd)
        {
            try
            {
                // Sample
                ISampler.SamplePoints(ref cd, graph);
            
                // Apply overrides
                SampleOverride.ApplyOverrides(ref cd, overrides);
            
                // MarchingCubes
                if (useTetrahedron)
                    MarchingTetrahedron.CreateMesh(ref cd);
                else
                    MarchingCubes.CreateMesh(ref cd);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}