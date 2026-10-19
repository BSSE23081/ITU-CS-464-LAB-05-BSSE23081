
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int targetScore = 10;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject winPanel;

    [Header("Red Coin")]
    [SerializeField] private GameObject redCoinPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnArea = 13f;
    [SerializeField] private float coinHeight = 0.5f;

    [Header("Blue Bonus Coins")]
    [SerializeField] private GameObject blueCoinPrefab;
    [SerializeField] private TMP_Text bonusText;
    [SerializeField] private int bonusCoinTotal = 50;
    [SerializeField] private float bonusRadius = 5f;

    private int score;
    private int bonusCoins;
    private int redCoinSpawnScore;

    private bool redCoinSpawned;
    private bool bonusStarted;
    private bool bonusCompleted;
    private bool isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Choose a random score from 1 to 10, inclusive.
        redCoinSpawnScore = Random.Range(1, 11);
    }

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(false);
            bonusText.text = $"BonusCoins: 0/{bonusCoinTotal}";
        }

        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        if (isGameOver)
            return;

        score += amount;
        UpdateScoreUI();

        // Spawn only one Red Coin when the chosen score is reached.
        if (!redCoinSpawned && score >= redCoinSpawnScore)
        {
            SpawnRedCoin();
        }

        CheckWin();
    }

    private void SpawnRedCoin()
    {
        if (redCoinSpawned || redCoinPrefab == null || player == null)
            return;

        redCoinSpawned = true;

        Vector3 position = GetRandomPosition();

        Instantiate(redCoinPrefab, position, Quaternion.identity);

        Debug.Log("The Red Coin has spawned!");
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 position = player.position;

        // Try to avoid spawning the coin right beside the player.
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-spawnArea, spawnArea);
            float z = Random.Range(-spawnArea, spawnArea);

            position = new Vector3(x, coinHeight, z);

            if (Vector3.Distance(position, player.position) >= 2f)
                break;
        }

        return position;
    }

    public void CollectRedCoin()
    {
        if (isGameOver || bonusStarted)
            return;

        bonusStarted = true;
        bonusCoins = 0;

        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(true);
            UpdateBonusUI();
        }

        SpawnBlueCoins();

        Debug.Log("Red Coin collected! Bonus round started.");
    }

    private void SpawnBlueCoins()
    {
        if (blueCoinPrefab == null || player == null)
        {
            Debug.LogError(
                "Assign Blue Coin Prefab and Player in GameManager."
            );
            return;
        }

        // Spawn the coins in a circular ring around the player.
        for (int i = 0; i < bonusCoinTotal; i++)
        {
            float angle = i * Mathf.PI * 2f / bonusCoinTotal;

            float x = player.position.x
                    + Mathf.Cos(angle) * bonusRadius;

            float z = player.position.z
                    + Mathf.Sin(angle) * bonusRadius;

            Vector3 position = new Vector3(x, coinHeight, z);

            Instantiate(
                blueCoinPrefab,
                position,
                Quaternion.identity
            );
        }

        Debug.Log($"{bonusCoinTotal} Blue Coins spawned!");
    }

    
public void CollectBlueCoin()
{
    if (isGameOver || !bonusStarted || bonusCompleted)
        return;

    bonusCoins++;

    if (bonusCoins > bonusCoinTotal)
        bonusCoins = bonusCoinTotal;

    UpdateBonusUI();

    if (bonusCoins >= bonusCoinTotal)
    {
        bonusCompleted = true;
        Debug.Log("All 50 Blue Coins collected! Game Over!");
        Win(); // End the game immediately
    }
}


    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score} / {targetScore}";
    }

    private void UpdateBonusUI()
    {
        if (bonusText != null)
        {
            bonusText.text =
                $"BonusCoins: {bonusCoins}/{bonusCoinTotal}";
        }
    }

    private void CheckWin()
    {
        if (isGameOver || score < targetScore)
            return;

        // If the Red Coin has spawned, finish the bonus round first.
        if ((redCoinSpawned && !bonusCompleted) || score == targetScore)
            return;

        Win();
    }

    private void Win()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    
    public void Restart()
    {
       // Resume the game before reloading the scene.
        Time.timeScale = 1f;

        SceneManager.LoadScene(
        SceneManager.GetActiveScene().buildIndex
        );
    }

}
