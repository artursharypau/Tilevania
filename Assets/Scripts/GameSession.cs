using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    private static readonly WaitForSecondsRealtime RestartingDelay = new(1f);
    private static GameSession _instance;

    private PlayerController _player;

    public static GameSession Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindFirstObjectByType<GameSession>();

                if (!_instance)
                {
                    GameObject go = new(nameof(GameSession));
                    _instance = go.AddComponent<GameSession>();
                }
            }

            return _instance;
        }
    }

    public uint CollectedCoins { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LevelExit.LevelExited.AddListener(OnLevelExited);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LevelExit.LevelExited.RemoveListener(OnLevelExited);

        if (_player)
        {
            _player.Died.RemoveListener(OnPlayerDied);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterPlayer();

        print("The scene " + scene.name + " has been loaded");
    }

    private void RegisterPlayer()
    {
        PlayerController newPlayer = FindFirstObjectByType<PlayerController>();

        if (_player != null)
        {
            _player.Died.RemoveListener(OnPlayerDied);
        }

        _player = newPlayer;
        _player.Died.AddListener(OnPlayerDied);
    }

    private void OnPlayerDied()
    {
        _player.Died.RemoveListener(OnPlayerDied);

        StartCoroutine(RestartRoutine());
    }

    private static IEnumerator RestartRoutine()
    {
        yield return RestartingDelay;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnLevelExited()
    {
        CollectedCoins += _player.Coins;
    }
}
