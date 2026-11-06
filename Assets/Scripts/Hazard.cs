using System.Collections;
using UnityEngine;

/// <summary>
/// A trigger that hurts. One script is every hazard: spikes, a guard and a crusher.
/// With a restTime above 0 it becomes a timed hazard: rest, warn, strike, and round again.
/// </summary>
public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 8f;       // the size of the impulse, in newton-seconds
    [SerializeField] float knockTime = 0.25f;    // seconds the player cannot steer, so the push is not wiped

    [Header("Timed hazards (restTime 0 = always on)")]
    [SerializeField] float restTime = 0f;
    [SerializeField] float warnTime = 0.7f;      // reaction time is about 0.25 s, so 0.7 s is fair
    [SerializeField] float strikeTime = 0.4f;
    [SerializeField] float startDelay = 0f;      // so two crushers do not strike together
    [SerializeField] GameObject warning;         // lit during the warning
    [SerializeField] GameObject slab;            // shown during the strike, so the strike can be seen

    IEnumerator Start()
    {
        if (restTime <= 0f) yield break;
        Collider zone = GetComponent<Collider>();
        zone.enabled = false;
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            zone.enabled = false;
            Show(slab, false);
            yield return new WaitForSeconds(restTime);
            Show(warning, true);
            yield return new WaitForSeconds(warnTime);
            Show(warning, false);
            zone.enabled = true;
            Show(slab, true);
            yield return new WaitForSeconds(strikeTime);
        }
    }

    static void Show(GameObject thing, bool on)
    {
        if (thing != null) thing.SetActive(on);
    }

    // OnTriggerStay, not Enter: standing in spikes keeps hurting. Health decides how often (its i-frames).
    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health)) return;
        if (!health.TakeDamage(damage)) return;              // only a hit that lands throws the player

        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        Vector3 away = body.position - transform.position;
        away.y = 0f;                                         // along the floor, never up into the air
        Vector3 push = away.normalized * knockback;

        if (other.TryGetComponent(out PlayerController player))
            player.Knockback(push, knockTime);               // the controller keeps the push
        else
            body.AddForce(push, ForceMode.Impulse);
    }
}
