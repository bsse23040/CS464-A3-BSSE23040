using UnityEngine;

/// <summary>
/// A coin. The collider on the prefab has Is Trigger ticked, so the player walks through it
/// and OnTriggerEnter fires (the player's Rigidbody is what makes that happen).
/// </summary>
public class Coin : MonoBehaviour
{
    [SerializeField] int value = 1;

    void OnTriggerEnter(Collider other)
    {
        // logged before the if, so when a pickup does nothing the Console shows whether the trigger fired at all
        Debug.Log($"Coin touched by {other.name}");

        if (!other.CompareTag("Player")) return;          // tags are case-sensitive: the built-in Player tag
        GameManager.Instance.AddScore(value);             // the coin never touches the score itself
        Destroy(gameObject);                              // gameObject, not this: Destroy(this) would remove only the script
    }
}
