using UnityEngine;

/// <summary>
/// One wide trigger under the level. Falling is a mistake, not damage: it skips the hearts and costs a life.
/// </summary>
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.PlayerDied();
    }
}
