using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [Header("References")]
    //    [SerializeField] private Transform turretRotationPoint // Only needed if I had a rotating gun
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 3f; // Targeting Range of the Turret
    [SerializeField] private float bps = 1f; // Bullets per Seconds

    // [SerializeField] private float rotationSpeed = 200f;


    private Transform target;
    private float timeUntilFire;
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }

    private void FindTarget()
    {
        // Our origin, Distance/Range, Direction (Position in Vector2), ?, LayerMask, so it won't aim at our tiles
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);


        if (hits.Length > 0) {
            target = hits[0].transform;
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }

    private bool CheckTargetIsInRange() {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    /* // Only needed if I had a rotating gun
    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg + (-90f);
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation,targetRotation, rotationSpeed * Time.deltaTime);
    }
    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        //        RotateTowardsTarget(); // Not needed without a rotating Turret gun
        if (!CheckTargetIsInRange())
        { // If ENEMY out of range, it targets the next enemy
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;
            if (timeUntilFire >= 1f / bps)
            {
                Shoot();
                timeUntilFire = 0f;
            }
       }
    }
}
