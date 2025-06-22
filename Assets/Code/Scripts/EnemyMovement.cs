using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb; // This will allow us to move our enemy

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target; // Point we want to move to
    private int pathIndex = 0;

    private void Start()
    {
        target = LevelManager.main.path[pathIndex];
    }

    private void Awake() // Recommendation from ChatGPT. Should be deleted if it causes problems
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;
            if (pathIndex == LevelManager.main.path.Length)
            {
                /*//*/
                EnemySpawner.onEnemyDestroy.Invoke(); // It will call the function from EnemySpawner
                Destroy(gameObject); // Wenn Enemy reaches the End of the Path
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed; // linearVelocity before. ChatGPT recommended to change that
    }
}



