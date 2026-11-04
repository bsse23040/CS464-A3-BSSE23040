using UnityEngine;

/// <summary>
/// A bounce pad: the force for Part A. It throws the player straight up with one kick.
/// ForceMode.VelocityChange ignores the mass, so the height depends only on the number: v = 9 m/s rises
/// v squared over 2g = 4.1 m. The kick is up, because the controller rewrites the sideways speed every step.
/// </summary>
public class BouncePad : MonoBehaviour
{
    [SerializeField] float bounceSpeed = 9f;     // metres per second, straight up

    void OnTriggerEnter(Collider other)          // Enter, not Stay: one kick per landing, not one per physics step
    {
        if (!other.CompareTag("Player")) return;
        Rigidbody body = other.attachedRigidbody;
        body.linearVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);   // so a bounce on the way down is not short
        body.AddForce(Vector3.up * bounceSpeed, ForceMode.VelocityChange);
    }
}
