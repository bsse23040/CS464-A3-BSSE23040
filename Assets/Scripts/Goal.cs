using UnityEngine;

/// <summary>
/// The goal pad at the end of a level. It asks the GameManager, which decides whether the player has enough coins.
/// </summary>
public class Goal : MonoBehaviour
{
    // Stay, not Enter: if the last coin is picked up while standing on the pad, it still counts
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.GoalReached();
    }
}
