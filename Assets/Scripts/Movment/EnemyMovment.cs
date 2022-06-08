using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment
{
    private NavMeshAgent _agent;
    private LineRenderer _line;
    private Enemy _enemy;
    private float _stopRange;

    private PlayerStorage _playerStorage;

    public EnemyMovment(Enemy enemy)
    {
        _enemy = enemy;
        _line = _enemy.GetComponent<LineRenderer>();
        _agent = _enemy.GetComponent<NavMeshAgent>();
        _agent.autoRepath = true;
        _agent.autoBraking = true;
        _agent.speed = _enemy.Speed;
        _stopRange = _enemy.StopRange;
        _playerStorage = new PlayerStorage();
    }

    public void Move()
    {
        GameObject clothestPlayer = _playerStorage.GetClothest(_enemy.transform.position);
        if (_agent.hasPath)
            DrawPath(_agent.path);
        else
            _line.positionCount = 0;
        if (clothestPlayer is null)
            return;
        float distance = Vector3.Distance(_enemy.transform.position, clothestPlayer.transform.position);
        if (distance <= 20 && distance >= _stopRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(clothestPlayer.transform.position);
        }
        else if(_agent.hasPath)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
        _enemy.Attack(clothestPlayer);
    }

    private void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2)
        {
            if(path.corners.Length != 0)
                _line.positionCount = 0;
            return;
        }
        _line.positionCount = path.corners.Length;
        _line.SetPosition(0, new Vector3(_enemy.transform.position.x, 0, _enemy.transform.position.z));
        for (var i = 1; i < path.corners.Length; i++)
        {
            _line.SetPosition(i, path.corners[i]);
        }
    }
}
