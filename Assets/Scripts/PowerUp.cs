using UnityEngine;

/// <summary>
/// The speed power-up. The pickup does nothing itself except ask the player: the boost and its timer
/// run on the player, so destroying the pickup cannot stop them.
/// </summary>
public class PowerUp : MonoBehaviour
{
    [SerializeField] float multiplier = 1.6f;
    [SerializeField] float duration = 5f;        // seconds
    [SerializeField] float spinSpeed = 90f;      // degrees per second

    void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerController>().BoostSpeed(multiplier, duration);
        Destroy(gameObject);
    }
}
