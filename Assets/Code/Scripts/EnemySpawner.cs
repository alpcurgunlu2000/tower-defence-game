using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    /*
    [Header("References")]
    [SerializeField] private GameObject[] enemyPreFabs;
    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;   // 8 Enemies will be spawned
    [SerializeField] private float enemiesPerSecond = 0.5f;   // 8 Enemies will be spawned
    [SerializeField] private float timeBetweenWaves = 5f;   // 5 Seconds between the Waves
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private void Start()
    {
        Debug.Log("EnemySpawner Start called.");
        StartCoroutine(StartWave());

    }

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Update()
    {
        if (!isSpawning)
        {
            Debug.Log("Not spawning yet...");
            return;
        }
        Debug.Log("Spawning is active. Enemies left: " + enemiesLeftToSpawn);
        timeSinceLastSpawn += Time.deltaTime; // It makes the Rounds harder by Spawning faster every Round
         if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)

        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0)
        {
            EndWave();
        }
    }

    
    private void EnemyDestroyed()
    {
        enemiesAlive--;

    }

    private void SpawnEnemy()
    {
        if (enemyPreFabs.Length == 0 || enemyPreFabs[0] == null)
        {
            Debug.LogError("Kein Enemy Prefab zugewiesen!");
            return;
        }

        GameObject prefabToSpawn = enemyPreFabs[0];
        Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
    }

    private IEnumerator StartWave()
    {
        Debug.Log("Starte neue Welle in " + timeBetweenWaves + " Sekunden..."); // For Debugging
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true; 
        enemiesLeftToSpawn = EnemiesPerWave();
        Debug.Log("Welle gestartet! Gegner zu spawnen: " + enemiesLeftToSpawn); // For Debugging
    }


    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++; // nächste Welle!
        StartCoroutine(StartWave());
    }


    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
    */
}
