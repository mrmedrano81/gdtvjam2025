using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("DEBUGGING")]
    public bool showSpawnArea = true;

    [Header("Settings")]
    public int numberOfEnemies = 5;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    public Transform mapCenterPoint;
    public GameObject enemyPrefab;


    private InputManager inputManager;
    private ObjectPool enemyPool;


    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        enemyPool = GetComponent<ObjectPool>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        inputManager.OnSpace += SpawnEnemies;
    }

    private void OnDisable()
    {
        inputManager.OnSpace -= SpawnEnemies;
    }


    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0f,
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
            ) + transform.position;

            //GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            //enemy.GetComponent<EnemyAIController>().mapCenterPoint = mapCenterPoint;

            enemyPool.SpawnObjectAt(spawnPosition);
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
