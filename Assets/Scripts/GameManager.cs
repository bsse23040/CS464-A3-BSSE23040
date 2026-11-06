using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// One GameManager per level. It owns the score, the lives and the three states of the game:
/// playing, won and lost. The prefab keeps only the numbers; each level gives it its own texts, panels,
/// player and respawn point.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win")]
    [SerializeField] int targetScore = 6;             // coins needed before the goal counts

    [Header("Lives")]
    [SerializeField] int lives = 3;
    [SerializeField] float respawnDelay = 0.6f;       // a beat to see what happened
    [SerializeField] float spawnProtection = 1.5f;    // seconds nothing can hurt the new life

    [Header("Set in each level")]
    [SerializeField] Health player;
    [SerializeField] Transform respawnPoint;
    [SerializeField] TMP_Text scoreText, livesText, hintText, boostText;
    [SerializeField] GameObject winPanel, losePanel;

    int score;
    bool respawning, ended;
    int shownBoost = -1;
    Coroutine hint;
    PlayerController controller;

    public bool IsRespawning => respawning;

    void Awake() => Instance = this;

    void Start()
    {
        controller = player.GetComponent<PlayerController>();
        UpdateUI();
    }

    // the boost text changes about ten times a second while a boost runs, and not at all otherwise
    void Update()
    {
        int tenths = Mathf.CeilToInt(controller.BoostLeft * 10f);
        if (tenths == shownBoost) return;
        shownBoost = tenths;
        boostText.text = tenths > 0 ? $"Speed boost: {tenths / 10f:0.0} s" : "";
    }

    // ---------------------------------------------------------------- score and win
    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();                                   // the text changes when the score changes, not every frame
    }

    // The goal calls this. It wins only with enough coins; otherwise it tells the player how many are missing.
    public void GoalReached()
    {
        if (ended) return;
        if (score >= targetScore) Win();
        else ShowHint($"Collect {targetScore - score} more coin{(targetScore - score == 1 ? "" : "s")} first!");
    }

    public void UpdateUI()
    {
        scoreText.text = $"Coins: {score} / {targetScore}";
        livesText.text = $"Lives: {lives}   Health: {Mathf.Max(player.Current, 0)} / {player.Max}";   // never show -1 hearts
    }

    void ShowHint(string message)
    {
        if (hint != null) StopCoroutine(hint);
        hint = StartCoroutine(Hint(message));
    }

    IEnumerator Hint(string message)
    {
        hintText.text = message;
        yield return new WaitForSeconds(2f);
        hintText.text = "";
    }

    // ---------------------------------------------------------------- losing a life
    public void PlayerDied()
    {
        if (ended || respawning) return;              // a fall and a hit can both kill in the same moment
        lives--;
        UpdateUI();
        if (lives <= 0) Lose();
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        respawning = true;
        // wait a beat first: the knockback of the hit that killed us is still on its way, and it would slide the new life away
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody body = player.GetComponent<Rigidbody>();
        body.position = respawnPoint.position;
        body.linearVelocity = Vector3.zero;           // the new life starts standing still
        player.Restore(spawnProtection);
        respawning = false;
    }

    // a checkpoint moves the respawn point: we come back here from now on
    public void SetCheckpoint(Transform point)
    {
        respawnPoint = point;
        ShowHint("Checkpoint!");
    }

    // ---------------------------------------------------------------- the end of a level
    void Win()
    {
        ended = true;
        winPanel.SetActive(true);
        Time.timeScale = 0f;                          // stops physics, the guards and everything that uses Time.deltaTime
    }

    void Lose()
    {
        ended = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // wired to the buttons on the win and lose panels
    public void Restart()
    {
        Time.timeScale = 1f;                          // timeScale survives a scene load, so put it back first
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);   // after the last level, back to the first
    }
}
