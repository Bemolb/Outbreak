using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStorage
{
    private static List<Player> _allPlayers;

    public int Count { get { return _allPlayers.Count; } }

    static PlayerStorage()
    {
        _allPlayers = new List<Player>();
    }

    public void AddPlayer(Player player) => _allPlayers.Add(player);

    public List<Player> GetPlayers() => _allPlayers;

    public void RemovePlayer(Player player) => _allPlayers.Remove(player);

    public Player FirstOrDefault(Func<Player, bool> expression) => _allPlayers.FirstOrDefault(expression);

    public Player FirstOrDefault() => _allPlayers.FirstOrDefault();

    public List<Player> OrderBy(Func<Player, float> expression) => _allPlayers.OrderBy(expression).ToList();

    public GameObject GetClothest(Vector3 fromCord)
    {
        if (Count == 0)
            return null;
        GameObject clothestPlayer = _allPlayers.OrderBy(p => Vector3.Distance(fromCord, p.transform.position)).First().gameObject;
        return clothestPlayer;
    }
}
