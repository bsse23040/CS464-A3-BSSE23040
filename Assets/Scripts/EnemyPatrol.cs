using UnityEngine;

public enum PatrolMode { Loop, PingPong }

/// <summary>
/// A guard. It walks a list of waypoints (Patrol), runs at the player when they come close (Chase),
/// and when they get far enough away it gives up and walks back to its route (Return).
/// The guard needs a kinematic Rigidbody, a trigger collider and a Hazard on the same object.
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    enum State { Patrol, Chase, Return }

    [Header("Route")]
    [SerializeField] Transform[] points;
    [SerializeField] PatrolMode mode = PatrolMode.Loop;
    [SerializeField] float patrolSpeed = 2f;           // metres per second: the player runs at 5
    [SerializeField] float waitTime = 0.3f;            // a short pause at each point
    [SerializeField] float arriveDistance = 0.1f;      // bigger than one step (2 m/s x 0.017 s = 3 cm), so it cannot be skipped

    [Header("Chase")]
    [SerializeField] float chaseSpeed = 3.5f;          // slower than the player, so a guard can be outrun
    [SerializeField] float chaseRange = 6f;            // it starts chasing inside this
    [SerializeField] float giveUpRange = 9f;           // and only gives up outside this, so it cannot flicker at one edge
    [SerializeField] float turnSpeed = 360f;           // degrees per second

    State state = State.Patrol;
    Transform player;
    int index;
    int step = 1;
    float waitUntil;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;   // found here, so one prefab fits every level
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (InRange(chaseRange)) state = State.Chase;
                break;

            case State.Chase:
                MoveTowards(player.position, chaseSpeed);
                if (!InRange(giveUpRange)) state = State.Return;
                break;

            case State.Return:
                MoveTowards(points[index].position, patrolSpeed);
                if (InRange(chaseRange)) state = State.Chase;
                else if (Arrived(points[index].position)) state = State.Patrol;
                break;
        }
    }

    void Patrol()
    {
        if (Time.time < waitUntil) return;
        Vector3 target = points[index].position;
        MoveTowards(target, patrolSpeed);
        if (Arrived(target))
        {
            index = NextIndex();
            waitUntil = Time.time + waitTime;
        }
    }

    int NextIndex()
    {
        if (mode == PatrolMode.Loop)
            return (index + 1) % points.Length;

        if (index + step < 0 || index + step >= points.Length) step = -step;   // turn round at either end
        return index + step;
    }

    // Moves flat along the floor at the guard's own height, and turns to face the way it walks.
    void MoveTowards(Vector3 target, float speed)
    {
        target.y = transform.position.y;
        Vector3 toTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (toTarget != Vector3.zero)                  // LookRotation of a zero vector logs a warning
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(toTarget), turnSpeed * Time.deltaTime);
    }

    // arrival by distance, not ==: close enough counts, as long as it is more than one step
    bool Arrived(Vector3 target)
    {
        target.y = transform.position.y;
        return Vector3.Distance(transform.position, target) < arriveDistance;
    }

    // Distance along the floor. A player on another floor (a platform above, a pit below) is out of range.
    bool InRange(float range)
    {
        Vector3 flat = player.position - transform.position;
        if (Mathf.Abs(flat.y) > 2.5f) return false;
        flat.y = 0f;
        return flat.magnitude < range;
    }

    // select the guard to see its two ranges: yellow where it starts chasing, red where it gives up
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, giveUpRange);
    }
}
