using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum EGameState
{
    GracePeriod,
    Combat,
    Paused,
    GameOver,
    Victory,
    Endless
}

public class GameState : MonoBehaviour
{
    [Header("Settings")]
    [Range(30, 120)]
    [SerializeField] private int frameRate = 60;
    public int maxEnemiesAlive = 400;
    public int currentEnemiesAlive = 0;
    public int currentNumKills = 0;
    public TMP_Text numKillText;

    [Header("Events")]

    public UnityEvent OnPause;
    public UnityEvent OnResume;
    public UnityEvent OnCombatStart;
    public UnityEvent OnGracePeriod;
    public UnityEvent OnDefeat;
    public UnityEvent OnVictory;
    public UnityEvent OnEndlessMode;

    public EGameState CurrentGameState { get; private set; } = EGameState.GracePeriod;

    private static GameState _instance;
    public static GameState Instance { get { return _instance; } }

    private bool isPaused = false;

    private EGameState previousGameState;

    [Header("DEBUGGING")]
    public TMP_Text gameStateText;

    public GameObject endScreen;

    

    private void Awake()
    {
        Application.targetFrameRate = frameRate;

        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Multiple instances of GameState detected. Destroying the new instance. ");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        maxEnemiesAlive = 400;
        currentEnemiesAlive = 0;
        currentNumKills = 0;

        SetGameState(EGameState.GracePeriod);
    }

    private void Update()
    {
        gameStateText.text = $"{CurrentGameState}";
        numKillText.text = currentNumKills.ToString();
    }

    public bool CanSpawnMoreEnemies()
    {
        return currentEnemiesAlive < maxEnemiesAlive;
    }

    public void SetGameState(EGameState newState)
    {
        CurrentGameState = newState;
        Debug.Log($"Game state changed to: {CurrentGameState}");
    }

    public void StartGracePeriod()
    {
        SetGameState(EGameState.GracePeriod);
    }

    public void StartCombat()
    {
        SetGameState(EGameState.Combat);
    }

    public void PauseGame()
    {
        if (isPaused == false)
        {
            OnPause?.Invoke(); // Trigger pause event

            previousGameState = CurrentGameState; // Save the current state before pausing

            SetGameState(EGameState.Paused);

            Time.timeScale = 0f; // Pause the game time

            isPaused = true;
        }
        else
        {
            OnResume?.Invoke(); // Trigger resume event

            Time.timeScale = 1f; // Resume the game time

            SetGameState(previousGameState); // Restore the previous state

            isPaused = false;
        }
    }

    public void EndGame()
    {
        endScreen.SetActive(true);

        SetGameState(EGameState.GameOver);
    }

    public void Victory()
    {
        SetGameState(EGameState.Victory);
    }

    public void StartEndlessMode()
    {
        SetGameState(EGameState.Endless);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
