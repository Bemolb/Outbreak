using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour
{
    public List<Chunk> ChunkPrefabs;

    public List<Player> PlayerPrefabs;

    public Camera Camera;

    public float Dificult = 1.0f;

    private LocationGenerator _locationGenerator;
    private PlayerManager _playerManager;

    private void Awake()
    {
        _locationGenerator = new LocationGenerator(ChunkPrefabs, Dificult);
        _locationGenerator.GenerateLocation();
        GetComponent<NavMeshSurface>().BuildNavMesh();
        Vector3 playerSpawn = _locationGenerator.GetStartPosition();
        _playerManager = new PlayerManager(PlayerPrefabs, playerSpawn, Camera);
        _playerManager.Spawn();
    }

    private void Start()
    {
        Camera.GetComponent<CameraMovment>().Init(PlayerManager.player.transform);
    }

    private void Update()
    {
        _playerManager.Update();
    }

}
