using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static GameObject player;

    private List<Player> _playerPrefabs;
    private Vector3 _spawnPosition;
    private PlayerStorage _playerStorage;


    private Vector3 _movePosition;
    private Camera _cam;
    private bool _positionChanged = false;

    public PlayerManager(List<Player> playerPrefabs, Vector3 spawnPosition, Camera camera)
    {
        _playerPrefabs = playerPrefabs;
        _spawnPosition = spawnPosition;
        _playerStorage = new PlayerStorage();
        _cam = camera;
    }

    public void Spawn()
    {
        foreach(var prefab in _playerPrefabs)
        {
            Player player = Instantiate(prefab, _spawnPosition.GetRandomPointInRange(2), Quaternion.identity);
            _playerStorage.AddPlayer(player);
        }
        player = _playerStorage.FirstOrDefault().gameObject;
    }

    public void Update()
    {
        Movement();
    }


    bool one_click = false;
    float timer_for_double_click;
    float delay = 0.5f;

    public void Movement()
    {
        bool isDoubleClick = false;
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(1))
        {
            if (!one_click)
            {
                timer_for_double_click = Time.time;
                one_click = true;
            }
            else
            {
                if ((Time.time - timer_for_double_click) > delay)
                {
                    timer_for_double_click = Time.time;
                    one_click = false;
                }
                else
                {
                    isDoubleClick = true;
                    one_click = false;
                }
            }
            RaycastHit hit;
            Ray castPoint = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                _movePosition = hit.point;
                _positionChanged = true;
            }
        }
        else
            return;

#else
        if (Input.touchCount >= 1)
        {
            
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray castPoint = _cam.ScreenPointToRay(touch.position);
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
                {
                    _movePosition = hit.point;
                    _positionChanged = true;
                }
            }
            if (Input.GetTouch(0).tapCount >= 2)
                isDoubleClick = true;
        }
        else
            return;
#endif
        if (_positionChanged)
        {
            _positionChanged = false;
            foreach(var player in _playerStorage.GetPlayers())
            {
                var position = _movePosition.GetRandomPointInRange(_playerStorage.Count / 2.0f);
                player.Move(position, isDoubleClick);
            }
        }
    }
}
