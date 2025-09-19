using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
            }

            return _instance;
        }
    }

    public MenuManager MenuManager { get; private set; }
    public WaveSpawner WaveSpawner { get; private set; }
    public Player Player { get; private set; }
    public UpgradeManager UpgradeManager { get; private set; }

    private bool _gameStarted;
    private bool _gameOver;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        MenuManager = FindFirstObjectByType<MenuManager>();
        WaveSpawner = FindFirstObjectByType<WaveSpawner>();
        Player = FindFirstObjectByType<Player>();
        UpgradeManager = FindFirstObjectByType<UpgradeManager>();

        if (MenuManager == null || WaveSpawner == null || Player == null || UpgradeManager == null)
        {
            Debug.LogError("GameManager could not find all required components in the scene.");
            enabled = false;
            return;
        }

        MenuManager.Initialize();
        MenuManager.SetMenuEventWaveSpawner(WaveSpawner);

        WaveSpawner.Initialize();
        Player.Initialize();

        MenuManager.ShowStartMenu();
    }

    private void OnDestroy()
    {
        if (WaveSpawner != null)
        {
            WaveSpawner.OnWaveFinished.RemoveListener(HandleWaveFinished);
            WaveSpawner.OnAllWavesFinished.RemoveListener(HandleAllWavesFinished);
        }
    }

    private void Update()
    {
        if (!_gameStarted && Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartGame();
            return;
        }

        if (_gameOver && Input.GetKeyDown(KeyCode.Mouse0))
        {
            RestartScene();
        }
    }

    private void StartGame()
    {
        if (_gameStarted)
        {
            return;
        }

        _gameStarted = true;
        Player.EnableControl();

        MenuManager.HideStartMenu();
        MenuManager.ShowGameMenu();

        WaveSpawner.OnWaveFinished.AddListener(HandleWaveFinished);
        WaveSpawner.OnAllWavesFinished.AddListener(HandleAllWavesFinished);

        StartCoroutine(WaveSpawner.SpawnWave());
    }

    private void OpenUpgradeSelectionMenu()
    {
        Player.DisableControl();
        MenuManager.ShowUpgradePanel(UpgradeManager.GetUpgradeAfterWave(3));
    }

    public void UpgradeSelected(SO_Upgrade upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogWarning("Trying to apply a null upgrade.");
            return;
        }

        Player.EnableControl();
        Player.AddUpgrade(upgrade);
        MenuManager.HideUpgradeMenu();

        StartCoroutine(WaveSpawner.SpawnWave());
    }

    private static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameLost()
    {
        if (_gameOver)
        {
            return;
        }

        _gameOver = true;
        MenuManager.ShowLoseMenu();
    }

    private void GameWon()
    {
        if (_gameOver)
        {
            return;
        }

        _gameOver = true;
        MenuManager.ShowWinMenu();
    }

    private void HandleWaveFinished()
    {
        if (!WaveSpawner.IsLastWave())
        {
            OpenUpgradeSelectionMenu();
        }
    }

    private void HandleAllWavesFinished()
    {
        GameWon();
    }
}
