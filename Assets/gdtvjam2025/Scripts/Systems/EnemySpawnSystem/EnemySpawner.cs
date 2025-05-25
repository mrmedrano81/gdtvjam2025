using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("DEBUGGING")]
    public bool showSpawnArea = true;
    public bool continuousSpawning = true;
    public float spawnInterval = 2f;

    [Header("Settings")]
    public int numberOfEnemies = 5;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public Transform mapCenterPoint;
    public GameObject enemyPrefab;


    private InputManager inputManager;
    private ObjectPool enemyPool;

    private float lastSpawnTime;
    private bool spawningEnemies = false;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        enemyPool = GetComponent<ObjectPool>();
        spawningEnemies = continuousSpawning;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        if (GameState.Instance.CurrentGameState != EGameState.Combat) return;

        if (spawningEnemies && Time.time - lastSpawnTime > spawnInterval)
        {
            if (!GameState.Instance.CanSpawnMoreEnemies())
            {
                return;
            }
            SpawnEnemies();
            lastSpawnTime = Time.time;
        }
    }

    //private void OnEnable()
    //{
    //    inputManager.OnSpace += ExecuteSpawningLogic;
    //}

    //private void OnDisable()
    //{
    //    inputManager.OnSpace -= ExecuteSpawningLogic;
    //}


    private void ExecuteSpawningLogic()
    {
        if (continuousSpawning)
        {
            spawningEnemies = !spawningEnemies;
        }
        else
        {
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        int randEnemyCount = Random.Range(1, numberOfEnemies);

        for (int i = 0; i < randEnemyCount; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0f,
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
            ) + transform.position;

            //GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            //enemy.GetComponent<EnemyAIController>().mapCenterPoint = mapCenterPoint;

            enemyPool.SpawnObjectAt(spawnPosition);

            GameState.Instance.currentEnemiesAlive++;
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnArea)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y));
        }
    }
}
