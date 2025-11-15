using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Generation.Noise.Chunking
{
    [DisallowMultipleComponent, RequireComponent(typeof(Generator))]
    public abstract class ChunkManagerBase : MonoBehaviour
    {
        protected Generator generator;

        public int chunkSize;

        public Transform chunkContainer;
        public Transform detailContainer;
        
        public Chunk chunkPrefab;
        
        protected readonly Dictionary<Vector3Int, ChunkData> loadedChunks = new();

        private readonly HashSet<SampleOverride> overrides = new();
        
        public void Awake()
        {
            generator = GetComponent<Generator>();
            if (chunkPrefab == null) 
                Debug.LogError($"{name} - No chunk prefab was found on this instance.");

            if (chunkContainer == null) 
                chunkContainer = transform;
            if (detailContainer == null) 
                detailContainer = transform;
        }
        

        protected async Awaitable LoadChunk(Vector3Int pos, bool forceReload = false, bool forceRespawnDetails = false)
        {
            bool exists = loadedChunks.TryGetValue(pos, out ChunkData cd);
            if (exists && !forceReload) return;
            if (!exists || !cd.chunk)
                cd = CreateChunk(pos);
            
            if (forceRespawnDetails)
                cd.firstSpawn = true;
            
            cd = await generator.Generate(cd);
            
            cd.chunk.SetChunkData(cd);
        }
        
        private ChunkData CreateChunk(Vector3Int pos)
        {
            Chunk     chunk     = Instantiate(chunkPrefab, chunkContainer);
            ChunkData cd = chunk.GetChunkData(this, pos);
            loadedChunks[pos] = cd; 
            return cd;
        } 
        
        protected void DespawnChunk(Vector3Int pos)
        {
            bool exists = loadedChunks.Remove(pos, out ChunkData value);
            if (!exists) return;
            
            #if UNITY_EDITOR
            if (Application.isEditor && Application.isPlaying == false)
                DestroyImmediate(value.chunk.gameObject);
            else
                Destroy(value.chunk.gameObject);
            #else
            Destroy(value.chunk.gameObject);
            #endif
        }
        
        public void Clear(bool clearChunks = true, bool clearDetails = true)
        {
            if (clearChunks)
            {
                loadedChunks.Clear();
            
                for (int i = chunkContainer.childCount - 1; i >= 0; i--)
                {
                    var t = chunkContainer.GetChild(i);
                
                    #if UNITY_EDITOR
                    if (Application.isEditor && !Application.isPlaying)
                        DestroyImmediate(t.gameObject, false);
                    else
                        Destroy(t.gameObject);
                    #else
                        Destroy(t.gameObject);
                    #endif
                }

                try
                {
                    if (Utils.IsTesting())
                    {
                        overrides.Clear();
                        generator.overrides.Clear();
                    }
                    else
                    {
                        overrides.Clear();
                        generator.overrides.Clear();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (clearDetails)
            {
                for (int i = detailContainer.childCount - 1; i >= 0; i--)
                {
                    var t = detailContainer.GetChild(i);
                    #if UNITY_EDITOR
                    if (Application.isEditor && !Application.isPlaying)
                        DestroyImmediate(t.gameObject, false);
                    else
                        Destroy(t.gameObject);
                    #else
                        Destroy(t.gameObject);
                    #endif
                }
            }
        }
    }
}