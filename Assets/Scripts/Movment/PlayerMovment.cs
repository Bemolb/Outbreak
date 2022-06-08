using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovment
{
    /*private Player _player;

    public PlayerMovment(Player player)
    {
        _player = player;
        _cam = Camera.main;
    }

    public void Move()
    {

        
        List<GameObject> otherPlayers = PlayerManager.Players;
        if (otherPlayers.Count != 0 && _positionChanged)
        {
            _positionChanged = false;
            otherPlayers.Remove(_player.gameObject);
            foreach (var oPlayer in otherPlayers)
            {
                if (Vector3.Distance(_movePosition, oPlayer.GetComponent<NavMeshAgent>().destination) < 3)
                {
                    var scaledForward = _player.transform.forward * 3f;
                    var leftRotated = Quaternion.AngleAxis(-15f, _player.transform.up) * scaledForward;
                    _movePosition += leftRotated;
                }
            }
            
        }
    }
    */
    
}