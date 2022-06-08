using UnityEngine;

public class ChunkObstacle
{
    public ChunkSide Side { get; }
    public GameObject Object { get; }

    public ChunkObstacle(ChunkSide chunkSide, GameObject gameObject)
    {
        Side = chunkSide;
        Object = gameObject;
    }
}
