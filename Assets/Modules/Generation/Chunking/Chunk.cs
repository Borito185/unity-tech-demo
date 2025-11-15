using Generation.Noise.Chunking;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public     MeshFilter   mesh;
    public new MeshRenderer renderer;
    public new MeshCollider collider;

    private ChunkData? data;
    
    internal ChunkData GetChunkData(ChunkManagerBase parent, Vector3Int pos)
    {
        if (data.HasValue && data.Value.chunkPos == pos)
            return data.Value;
        
        ChunkData cd = new()
        {
            parent   = parent,
            chunk    = this,
            chunkPos = pos,

            size          = parent.chunkSize,
            chunkWorldPos = parent.transform.position + pos * parent.chunkSize,
            firstSpawn    = true
        };
        
        
        data = cd;
        return cd;
    }
    
    public void SetChunkData(ChunkData data)
    {
        this.data = data;

        Vector3Int pos = data.chunkPos;
        
        name                = $"chunk: {pos.x}, {pos.y}, {pos.z}";
        transform.position  = data.chunkWorldPos;
        mesh.sharedMesh     = data.ToMesh();
        collider.sharedMesh = mesh.sharedMesh;
    }
}