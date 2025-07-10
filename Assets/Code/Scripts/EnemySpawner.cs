using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
      // Code with Compiling Error
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

    private IEnumerator Start()
    {
        while (LevelManager.main.startPoint == null)
            yield return null;
        yield return null;
        // Debug.Log("EnemySpawner: Start point confirmed, starting waves...");
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
        // Debug.Log("EnemySpawner: Not spawning yet...");
            return;
        }
        // Debug.Log("EnemySpawner: Spawning is active. Enemies left: " + enemiesLeftToSpawn); // This gets spammed to infinity with the current broken code
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
        // Debug.Log($"EnemySpawner: Spawning enemy at {LevelManager.main.startPoint.position}");
        if (LevelManager.main.startPoint == null)
        {
            Debug.LogError("EnemySpawner: Start point not set! Cannot spawn.");
            return;
        }

        GameObject prefabToSpawn = enemyPreFabs[0];
        Vector3 spawnPosition = LevelManager.main.startPoint.position;
        spawnPosition.y += 0.5f; // if needed to visually center on cell
        // Debug.Log($"EnemySpawner: Spawning enemy at {spawnPosition}");
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);;
    }

    private IEnumerator StartWave()
    {
        // Debug.Log("EnemySpawner: Starting new wave in " + timeBetweenWaves + " seconds..."); // For Debugging
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true; 
        enemiesLeftToSpawn = EnemiesPerWave();
        // Debug.Log("EnemySpawner: Wave started! Enemies to spawn: " + enemiesLeftToSpawn); // For Debugging
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
    
}
