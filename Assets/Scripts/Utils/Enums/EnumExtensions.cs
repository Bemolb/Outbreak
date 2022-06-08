using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public static class EnumExtensions
{
    private static Random _random = new Random();
    public static T RandomEnum<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(_random.Next(values.Length));
    }
    public static List<T> GetAsCollection<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }
}
