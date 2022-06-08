using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;

public static class Extensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while(n > 1)
        {
            byte[] box = new byte[1];
            do
                provider.GetBytes(box);
            while (!(box[0] < n * (byte.MaxValue / n)));
            int i = (box[0] % n);
            n--;
            T value = list[i];
            list[i] = list[n];
            list[n] = value;
        }
    }

    public static Vector3 GetRandomPointInRange(this Vector3 position, float range)
    {
        Vector2 centerPoint = new Vector2(position.x, position.z);
        Vector2 randomPoint = centerPoint + UnityEngine.Random.insideUnitCircle * range;
        return new Vector3(randomPoint.x, 0, randomPoint.y);
    }
}
