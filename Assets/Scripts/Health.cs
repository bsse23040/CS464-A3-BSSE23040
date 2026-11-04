using UnityEngine;

/// <summary>
/// The hearts. Health owns the hearts and the rule about how often they can be lost (the i-frames).
/// The GameManager owns the lives.
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float iFrames = 1f;      // seconds of safety after a hit

    int current;
    float invulnerableUntil;
    Renderer body;

    public int Current => current;
    public int Max => max;

    void Awake()
    {
        current = max;
        body = GetComponentInChildren<Renderer>();
    }

    // the player blinks while nothing can hurt them
    void Update()
    {
        bool hidden = Time.time < invulnerableUntil && Mathf.FloorToInt(Time.time * 10f) % 2 == 0;
        body.enabled = !hidden;
    }

    // Returns true when the hit landed. A hazard asks every physics step, so this is where the rule lives.
    public bool TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return false;     // down already, or still protected
        if (GameManager.Instance.IsRespawning) return false;

        invulnerableUntil = Time.time + iFrames;                              // start the clock
        current -= amount;
        GameManager.Instance.UpdateUI();
        if (current <= 0) GameManager.Instance.PlayerDied();                  // <= 0, not == 0: the crusher hits for 2
        return true;
    }

    // called by the GameManager after a respawn: full hearts and a moment of protection
    public void Restore(float protection)
    {
        current = max;
        invulnerableUntil = Time.time + protection;
        GameManager.Instance.UpdateUI();
    }
}
