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

    public Button ReloadButton;

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
        Button btn = ReloadButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    private void Update()
    {
        _playerManager.Update();
    }


    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
