using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public ChunkType Type;

    [NonSerialized]
    public int Id;

    public Transform RightBorder;
    public Transform LeftBorder;
    public Transform TopBorder;
    public Transform BottomBorder;

    [NonSerialized]
    public Vector2 positionInMatrix;
    [NonSerialized]
    public bool Checked = false;

    public Transform PlayerSpawnPosition;

    public Transform ConnectionPosition;

    [NonSerialized]
    public Vector3 Orientation;

    public bool IsBorderChunk => CheckIsBorderChunk();
    public bool CanBlocked => CheckCanBlocked();

    private List<ChunkObstacle> _blokedSides = new List<ChunkObstacle>();

    private bool CheckCanBlocked()
    {
        return IsBorderChunk ? _blokedSides.Count < 3 : _blokedSides.Count < 2;
    }

    private bool CheckIsBorderChunk()
    {
        return positionInMatrix.x == 0 || positionInMatrix.x == LocationGenerator._matrixSize.x - 1 || positionInMatrix.y == 0 || positionInMatrix.y == LocationGenerator._matrixSize.y - 1;
    }

    public List<ChunkSide> GetBlockedSides()
    {
        return _blokedSides.Select(x => x.Side).ToList();
    }

    public bool HasObstacle(GameObject gameObject)
    {
        return _blokedSides.Any(o => o.Object.Equals(gameObject));
    }

    public ChunkObstacle GetObstacle(ChunkSide side)
    {
        return _blokedSides.FirstOrDefault(o => o.Side == side);
    }

    public void SetChunkPosition(Chunk otherChunk)
    {
        if(Type != ChunkType.LoactionBorder && Type != ChunkType.LocationObstacle)
        {
            if (otherChunk.positionInMatrix.x == positionInMatrix.x)
            {

                transform.position = otherChunk.RightBorder.position - LeftBorder.position;
                return;
            }
            else
            {
                transform.position = otherChunk.TopBorder.position - BottomBorder.position;
                return;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(Orientation);
            if (Orientation == ChunckOrientation.Top)
            {
                transform.position = otherChunk.BottomBorder.position - ConnectionPosition.position; return;
            }
            if (Orientation == ChunckOrientation.Right)
            {
                transform.position = otherChunk.LeftBorder.position - ConnectionPosition.position; return;
            }
            if (Orientation == ChunckOrientation.Down)
            {
                transform.position = otherChunk.TopBorder.position - ConnectionPosition.position; return;
            }
            if (Orientation == ChunckOrientation.Left)
            {
                transform.position = otherChunk.RightBorder.position - ConnectionPosition.position; return;
            }
        }
    }

    public bool BlockSide(ChunkObstacle obstacle)
    {
        if (_blokedSides.Any(s => s.Side == obstacle.Side))
            return false;
        _blokedSides.Add(obstacle);
        return true;
    }

    public void UnblockSide(ChunkObstacle obstacle)
    {
        _blokedSides.Remove(obstacle);
        DestroyImmediate(obstacle.Object);
    }
}
