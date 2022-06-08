using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;


public class LocationGenerator : MonoBehaviour
{
    private List<Chunk> _chunkPrefabs;
    private List<Chunk> _startChunks;
    private List<Chunk> _borderChunks;
    private List<Chunk> _locationChunks;
    private List<Chunk> _obstacleChunks;

    private ChunkStorage _chunkStorage;

    private Random _random = new Random();

    [NonSerialized]
    public static Vector2 _matrixSize;

    private Vector3 _posToSpawn;
    private float _dificult;

    public LocationGenerator(List<Chunk> chunkPrefabs, float dificult)
    {
        _chunkPrefabs = chunkPrefabs;
        _dificult = dificult;
        _chunkStorage = new ChunkStorage();
    }

    public void GenerateLocation()
    {
        GenerateMatrix();
        _startChunks = _chunkPrefabs.Where(c => c.Type == ChunkType.LocationStart).ToList();
        _borderChunks = _chunkPrefabs.Where(c => c.Type == ChunkType.LoactionBorder).ToList();
        _locationChunks = _chunkPrefabs.Where(c => c.Type == ChunkType.LocationPart).ToList();
        _obstacleChunks = _chunkPrefabs.Where(c => c.Type == ChunkType.LocationObstacle).ToList();
        _chunkPrefabs.Clear();
        Chunk startChunk = null;
        int starChunkX = _random.Next(0, (int)_matrixSize.x);
        int starChunkY = _random.Next(0, (int)_matrixSize.y);
        int id = 0;
        for (int x = 0; x < _matrixSize.x; x++)
        {
            for(int y = 0; y < _matrixSize.y; y++)
            {
                Chunk newChunk;
                int index = 0;
                if (x != starChunkX || y != starChunkY)
                {
                    index = _random.Next(0, _locationChunks.Count);
                    newChunk = Instantiate(_locationChunks[index], new Vector3(0, 0, 0), Quaternion.identity);
                }
                else
                {

                    index = _random.Next(0, _startChunks.Count);
                    newChunk = Instantiate(_startChunks[index], new Vector3(0, 0, 0), Quaternion.identity);
                    startChunk = newChunk;
                }
                Chunk previusChunk = _chunkStorage.GetLast(_matrixSize);
                _chunkStorage.AddChunk(newChunk);
                newChunk.positionInMatrix = new Vector2(x, y);
                if(previusChunk != null)
                    newChunk.SetChunkPosition(previusChunk);
                if (newChunk.Type == ChunkType.LocationStart)
                    _posToSpawn = newChunk.PlayerSpawnPosition.position;
                newChunk.Id = id;
                id++;
            }
        }
        GenerateLocationBorder();
        GenerateLocationObstacle();
        CheckAvailability();
        /*var r = FixDedendChunk(startChunk, null);
        Debug.Log(r.Count());
        foreach (var t in r)
            t.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;*/

    }

    public Vector3 GetStartPosition() => _posToSpawn;

    private void CheckAvailability()
    {
        List<Chunk> notBorderChunks = _chunkStorage.Where(c => c.positionInMatrix.x != 0 && c.positionInMatrix.y != 0).ToList();
        Lookup<float, Chunk> groups = (Lookup<float, Chunk>)_chunkStorage.Where(c => c.positionInMatrix.x != 0).ToLookup(c => c.positionInMatrix.x);
        int clearedCount = 0;
        foreach(var chunks in groups)
        {
            if(chunks.All(c => c.GetBlockedSides().Contains(ChunkSide.Bottom)))
            {
                int index = _random.Next(0, chunks.Count());
                Chunk toUnlock = chunks.ElementAt(index);
                var obstacle = toUnlock.GetObstacle(ChunkSide.Bottom);
                var list = _chunkStorage.Where(x => x.HasObstacle(obstacle.Object)).ToList();
                list.ForEach(c => c.UnblockSide(obstacle));
                toUnlock.UnblockSide(obstacle);
                clearedCount += 1;
            }
        }
        groups = (Lookup<float, Chunk>)_chunkStorage.Where(c => c.positionInMatrix.y != 0).ToLookup(c => c.positionInMatrix.y);
        foreach (var chunks in groups)
        {
            if (chunks.All(c => c.GetBlockedSides().Contains(ChunkSide.Left)))
            {
                int index = _random.Next(0, chunks.Count());
                Chunk toUnlock = chunks.ElementAt(index);
                var obstacle = toUnlock.GetObstacle(ChunkSide.Left);
                var list = _chunkStorage.Where(x => x.HasObstacle(obstacle.Object)).ToList();
                list.ForEach(c => c.UnblockSide(obstacle));
                toUnlock.UnblockSide(obstacle);
                clearedCount += 1;
            }
        }
        if(clearedCount > 0)
        {
            notBorderChunks.Shuffle();
            var temp = notBorderChunks.Where(c => c.GetBlockedSides().Count == 0).ToList();
            var toAddObstacle = temp.Count >= clearedCount ? temp.Take(clearedCount) : null;
            if (!(toAddObstacle is null))
            {
                foreach(var chunk in toAddObstacle)
                {
                    ChunkSide side = EnumExtensions.RandomEnum<ChunkSide>();
                    Vector3 chunkOrientation = new Vector3();
                    switch (side)
                    {
                        case ChunkSide.Top:
                            chunkOrientation = ChunckOrientation.Down;
                            break;
                        case ChunkSide.Right:
                            chunkOrientation = ChunckOrientation.Left;
                            break;
                        case ChunkSide.Bottom:
                            chunkOrientation = ChunckOrientation.Top;
                            break;
                        case ChunkSide.Left:
                            chunkOrientation = ChunckOrientation.Right;
                            break;
                    }
                    Chunk newChunk = GetObstacleChunk(chunkOrientation);
                    newChunk.SetChunkPosition(chunk);
                    ChunkObstacle obstacle = new ChunkObstacle(side, newChunk.gameObject);
                    chunk.BlockSide(obstacle);
                }
            }
        }
    }

    private IEnumerable<Chunk> FixDedendChunk(Chunk curChunk, Chunk enterFrom)
    {
        var freeSide = EnumExtensions.GetAsCollection<ChunkSide>().Except(curChunk.GetBlockedSides()).ToList();
        List<Chunk> nextChunks = new List<Chunk>();
        Task.Delay(10).GetAwaiter().GetResult();
        foreach(var side in freeSide)
        {
            Chunk nextChunk = null;
            switch(side)
            {
                case ChunkSide.Top:
                    nextChunk = _chunkStorage.GetChunk(curChunk.positionInMatrix + Vector2.up);
                    break;
                case ChunkSide.Right:
                    nextChunk = _chunkStorage.GetChunk(curChunk.positionInMatrix + Vector2.right);
                    break;
                case ChunkSide.Bottom:
                    nextChunk = _chunkStorage.GetChunk(curChunk.positionInMatrix + Vector2.down);
                    break;
                case ChunkSide.Left:
                    nextChunk = _chunkStorage.GetChunk(curChunk.positionInMatrix + Vector2.left);
                    break;
            }
            nextChunk = enterFrom?.Id == nextChunk?.Id ? null : nextChunk;
            if (!(nextChunk is null) && nextChunk.Checked != true)
            {
                nextChunks.Add(nextChunk);
                nextChunk.Checked = true;
            }
                    
        }
        if (nextChunks.Count == 0)
            yield return curChunk;
        else
        {
            foreach (var c in nextChunks)
                foreach (var r in FixDedendChunk(c, curChunk))
                    yield return r;
        }
    }

    private void GenerateLocationObstacle()
    {
        double modifire = Mathf.Sqrt(_chunkStorage.Count) / 100.0;
        PlaceObstacle(modifire);
    }

    private void PlaceObstacle(double modifire)
    {
        foreach (var chunk in _chunkStorage.GetChunks())
        {
            int maxBlock = 2;
            if (chunk.IsBorderChunk)
                maxBlock = 3;
            int blockSideCount = _random.Next(0, maxBlock + 1) - chunk.GetBlockedSides().Count;
            for (int i = 0; i < blockSideCount; i++)
            {
                if (_random.NextDouble() + modifire <= 0.25)
                    continue;
                var notBlocked = EnumExtensions.GetAsCollection<ChunkSide>().Except(chunk.GetBlockedSides()).ToList();
                int index = _random.Next(0, notBlocked.Count);
                var side = notBlocked[index];
                ChunkSide oppSide = ChunkSide.Top;
                Vector3 chunkOrientation = new Vector3();
                Vector2 neighboringPos = new Vector2();
                switch (side)
                {
                    case ChunkSide.Top:
                        oppSide = ChunkSide.Bottom;
                        chunkOrientation = ChunckOrientation.Down;
                        neighboringPos = chunk.positionInMatrix + new Vector2(1, 0);
                        break;
                    case ChunkSide.Right:
                        oppSide = ChunkSide.Left;
                        chunkOrientation = ChunckOrientation.Left;
                        neighboringPos = chunk.positionInMatrix + new Vector2(0, 1);
                        break;
                    case ChunkSide.Bottom:
                        oppSide = ChunkSide.Top;
                        chunkOrientation = ChunckOrientation.Top;
                        neighboringPos = chunk.positionInMatrix + new Vector2(-1, 0);
                        break;
                    case ChunkSide.Left:
                        oppSide = ChunkSide.Right;
                        chunkOrientation = ChunckOrientation.Right;
                        neighboringPos = chunk.positionInMatrix + new Vector2(0, -1);
                        break;
                }
                Chunk newChunk = GetObstacleChunk(chunkOrientation);
                newChunk.SetChunkPosition(chunk);
                ChunkObstacle obstacle = new ChunkObstacle(side, newChunk.gameObject);
                if (BlockNeighboring(neighboringPos, oppSide, obstacle.Object))
                {
                    chunk.BlockSide(obstacle);
                }
                else
                {
                    UnityEngine.Object.Destroy(obstacle.Object);
                }
            }
        }
    }

    private bool BlockNeighboring(Vector2 position, ChunkSide side, GameObject gameObject)
    {
        var chunk = _chunkStorage.FirstOrDefault(c => c.positionInMatrix == position);
        if (chunk is null)
            return true;
        if (!chunk.CanBlocked)
            return false;
        ChunkObstacle obstacle = new ChunkObstacle(side, gameObject);
        return chunk.BlockSide(obstacle);
    }

    private void GenerateLocationBorder()
    {
        foreach(var chunk in _chunkStorage.GetChunks())
        {
            if (chunk.positionInMatrix.x == 0)
            {
                Chunk newChunk = GetBorderChunk(ChunckOrientation.Top);
                newChunk.SetChunkPosition(chunk);
                ChunkObstacle obstacle = new ChunkObstacle(ChunkSide.Bottom, newChunk.gameObject);
                chunk.BlockSide(obstacle);
            }
            if (chunk.positionInMatrix.y == 0)
            {
                Chunk newChunk = GetBorderChunk(ChunckOrientation.Right);
                newChunk.SetChunkPosition(chunk);
                ChunkObstacle obstacle = new ChunkObstacle(ChunkSide.Left, newChunk.gameObject);
                chunk.BlockSide(obstacle);
            }
            if (chunk.positionInMatrix.x == _matrixSize.x - 1)
            {
                Chunk newChunk = GetBorderChunk(ChunckOrientation.Down);
                newChunk.SetChunkPosition(chunk);
                ChunkObstacle obstacle = new ChunkObstacle(ChunkSide.Top, newChunk.gameObject);
                chunk.BlockSide(obstacle);
            }
            if (chunk.positionInMatrix.y == _matrixSize.y - 1)
            {
                Chunk newChunk = GetBorderChunk(ChunckOrientation.Left);
                newChunk.SetChunkPosition(chunk);
                ChunkObstacle obstacle = new ChunkObstacle(ChunkSide.Right, newChunk.gameObject);
                chunk.BlockSide(obstacle);
            }
        }
    }

    private Chunk GetObstacleChunk(Vector3 chunkOrientation)
    {
        int index = _random.Next(0, _obstacleChunks.Count);
        Chunk newChunk = UnityEngine.Object.Instantiate(_obstacleChunks[index], new Vector3(0, 0, 0), Quaternion.identity);
        newChunk.Orientation = chunkOrientation;
        return newChunk;
    }

    private Chunk GetBorderChunk(Vector3 chunkOrientation)
    {

        int index = _random.Next(0, _borderChunks.Count);
        Chunk newChunk = UnityEngine.Object.Instantiate(_borderChunks[index], new Vector3(0, 0, 0), Quaternion.identity);
        newChunk.Orientation = chunkOrientation;
        return newChunk;
    }

    private void GenerateMatrix()
    {
        int sizeX = 1;
        int sizeY = 1;
        int totalSize = Mathf.RoundToInt(3.0f * _dificult);
        int rotation = _random.Next(0, 2);
        do
        {
            if(totalSize <= 5 && rotation == 0)
                sizeX = _random.Next(1, totalSize + 1);
            else
                sizeY = _random.Next(1, totalSize + 1);
            if(totalSize > 6)
            {
                int min = (int)Mathf.Sqrt(totalSize);
                int max = min * 2;
                sizeX = _random.Next(min, max + 1);
                sizeY = totalSize / sizeX;
            }
        }
        while (sizeX * sizeY != totalSize);
        _matrixSize = new Vector2(sizeX, sizeY);
    }
}
