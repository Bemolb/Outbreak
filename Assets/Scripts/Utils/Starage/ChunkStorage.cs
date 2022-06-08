using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkStorage
{
    private static List<Chunk> _allChunks;

    public int Count { get { return _allChunks.Count; } }

    static ChunkStorage()
    {
        _allChunks = new List<Chunk>();
    }

    public List<Chunk> GetChunks() => _allChunks;

    public void AddChunk(Chunk chunk) => _allChunks.Add(chunk);

    public void RemoveChunk(Chunk chunk) => _allChunks.Remove(chunk);

    public Chunk GetLast(Vector2 size)
    {
        Chunk lastChunk = _allChunks.LastOrDefault();
        if (lastChunk?.positionInMatrix.y == size.y - 1)
            lastChunk = _allChunks.FirstOrDefault(c => c.positionInMatrix == new Vector2(lastChunk.positionInMatrix.x, 0));
        return lastChunk;
    }

    public List<Chunk> Where(Func<Chunk, bool> expression) => _allChunks.Where(expression).ToList();

    public Chunk GetChunk(float x, float y) => GetChunk(new Vector2(x, y));

    public Chunk GetChunk(Vector2 position)
    {
        return _allChunks.FirstOrDefault(c => c.positionInMatrix == position);
    }

    public Chunk FirstOrDefault(Func<Chunk, bool> expression = null) => _allChunks.FirstOrDefault(expression);

}
