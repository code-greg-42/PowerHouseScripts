using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // game progress variables
    private int waveNumber = 0;
    private int totalPoints = 0;

    [Header("Enemies")]
    public GameObject[] enemyPrefabs;

    [Header("UI Components")]
    public Button startButton;
    public Button restartButton;
    public GameObject startMenu;
    public GameObject gameOverMenu;
    public TextMeshProUGUI waveNumberText;
    public TextMeshProUGUI waveTimerText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI gameOverPointsText;
    public TextMeshProUGUI gameOverWavesDefeatedText;

    [Header("Script Attachments")]
    public PlayerAttack playerAttack;

    // spawn location limits
    private readonly float spawnMinX = -11.0f;
    private readonly float spawnMaxX = 11.0f;
    private readonly float spawnMinY = 1.0f;
    private readonly float spawnMaxY = 9.0f;
    private readonly float spawnMinZ = 0.0f;
    private readonly float spawnMaxZ = 13.0f;

    // wave timer variables
    private int waveTimer = 20;
    private readonly int waveTimerMax = 20;
    private readonly int waveBonusMultiplier = 10;
    private Coroutine waveTimerCoroutine;

    [HideInInspector]
    public bool isGameActive;

    // Start is called before the first frame update
    void Start()
    {
        // setup listeners for UI buttons
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive)
        {
            int enemyCount = FindObjectsOfType<Enemy>().Length;

            if (enemyCount == 0)
            {
                // calculate wave bonus points and add to point total
                int waveBonusPoints = waveNumber * waveTimer * waveBonusMultiplier;
                AddPoints(waveBonusPoints);

                // increase wave number and update wave number UI element
                waveNumber++;
                waveNumberText.text = "Wave: " + waveNumber;
                // reset wave timer and update wave timer UI element
                waveTimer = waveTimerMax;
                waveTimerText.text = "Timer: " + waveTimer;

                // restart wave timer coroutine if exists
                if (waveTimerCoroutine != null)
                {
                    StopCoroutine(waveTimerCoroutine);
                }
                waveTimerCoroutine = StartCoroutine(StartWaveTimer());

                SpawnNewWave(waveNumber);
            }
        }
    }

    void SpawnNewWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // randomize spawn locations within limits
            float spawnPosX = Random.Range(spawnMinX, spawnMaxX);
            float spawnPosY = Random.Range(spawnMinY, spawnMaxY);
            float spawnPosZ = Random.Range(spawnMinZ, spawnMaxZ);

            // choose random enemy from enemyprefabs array
            Vector3 spawnPos = new(spawnPosX, spawnPosY, spawnPosZ);
            int enemyIndex = Random.Range(0, enemyPrefabs.Length);

            // spawn enemy
            Instantiate(enemyPrefabs[enemyIndex], spawnPos, Quaternion.identity);
        }
    }

    void StartGame()
    {
        isGameActive = true;
        startMenu.SetActive(false);
    }

    IEnumerator StartWaveTimer()
    {
        while (waveTimer >= 0)
        {
            waveTimerText.text = "Timer: " + waveTimer;
            yield return new WaitForSeconds(1);
            waveTimer--;
        }
        EndGame();
    }

    void EndGame()
    {
        isGameActive = false;

        gameOverPointsText.text = "Points: " + totalPoints;
        gameOverWavesDefeatedText.text = "Waves Defeated: " + (waveNumber - 1);

        gameOverMenu.SetActive(true);

        // find all enemies left in play
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (enemy.TryGetComponent(out Rigidbody rb)) 
            {
                // stop any current healing coroutine
                enemy.StopHeal();

                if (rb != null)
                {
                    // freeze the position
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddPoints(int pointsToAdd)
    {
        totalPoints += pointsToAdd;
        pointsText.text = "Points: " + totalPoints;
    }
}
