using UnityEngine;

public class ChunckOrientation
{
    private Vector3 Value;

    public static Vector3 Top { get { return new ChunckOrientation(0).Value; } }

    public static Vector3 Right { get { return new ChunckOrientation(1).Value; } }

    public static Vector3 Down { get { return new ChunckOrientation(2).Value; } }

    public static Vector3 Left { get { return new ChunckOrientation(3).Value; } }

    private ChunckOrientation(int type)
    {
        switch(type)
        {
            case 0:
                Value = new Vector3(0, 0, 0);
                break;
            case 1:
                Value = new Vector3(0, 90, 0);
                break;
            case 2:
                Value = new Vector3(0, 180, 0);
                break;
            case 3:
                Value = new Vector3(0, 270, 0);
                break;
        }
    }
}
