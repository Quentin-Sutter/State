using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance => instance;

    [Header("Scripts")]
    public MenuManager MenuManager { get; private set; }
    public WaveSpawner waveSpawner { get; private set; }
     public Player player { get; private set; }
    public UpgradeManager UpgradeManager { get; private set; }

    bool gameStarted = false;
    bool gameOver = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        //DontDestroyOnLoad(this.gameObject);

        // Initialisations
        MenuManager = FindFirstObjectByType<MenuManager>();
        MenuManager.Initialize();

        waveSpawner = FindFirstObjectByType<WaveSpawner>();
        waveSpawner.Initialize();

        player = FindFirstObjectByType<Player>();
        player.Initialialize();

        UpgradeManager = FindFirstObjectByType<UpgradeManager>();

        MenuManager.ShowStartMenu();
    }

    private void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartGame();
            }
        }

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RestartScene();
            }
        }
     
    }

    void StartGame ()
    {
        gameStarted = true;
        player.canControl = true;
        MenuManager.HideStartMenu();
        MenuManager.ShowGameMenu();
        waveSpawner.OnAllWavesFinished.AddListener(AllWavesFinished);
        MenuManager.SetMenuEventWaveSpawner(waveSpawner);
        waveSpawner.OnWaveFinished.AddListener(WaveFinished);
        StartCoroutine(waveSpawner.SpawnWave());
    }

    void OpenUpgradeSelectionMenu ()
    {
        player.RemoveControlPlayer();
        MenuManager.ShowUpgradePanel(UpgradeManager.GetUpgradeAfterWave(3));
    }

    public void UpgradeSelected (SO_Upgrade upgrade)
    {
        player.GiveControlPlayer();
        player.AddUpgrade(upgrade);
        MenuManager.HideUpgradeMenu();
        StartCoroutine(waveSpawner.SpawnWave());
    }

    void RestartScene ()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void GameLosed() 
    {
        gameOver = true;
        MenuManager.ShowLoseMenu();
    }

    public void GameWinned ()
    {
        gameOver = true;
        MenuManager.ShowWinMenu();
    }

    public void WaveFinished ()
    {
        if (!waveSpawner.IsLastWave())OpenUpgradeSelectionMenu();
    }

    public void AllWavesFinished ()
    {
        GameWinned();
    } 
}
