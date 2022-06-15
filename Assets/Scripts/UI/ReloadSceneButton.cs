using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReloadSceneButton : MonoBehaviour
{
    private Button _targetButton;
    private Scene _scene;
    private PlayerStorage _playerStorage;
    private ChunkStorage _chunkStorage;

    private void Start()
    {
        _targetButton = gameObject.GetComponent<Button>();
        _targetButton.onClick.AddListener(HandleButtonClick);
        _scene = SceneManager.GetActiveScene();
        _playerStorage = new PlayerStorage();
        _chunkStorage = new ChunkStorage();
    }

    private void OnDestroy()
    {
        if (_targetButton != null) _targetButton.onClick.RemoveListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        _playerStorage.Cleare();
        _chunkStorage.Cleare();
        SceneManager.LoadScene(_scene.buildIndex);
    }
}