using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FoxRocketArcade
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        RocketChange
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        public GameState GameState => gameState;
        public int Score => score;
        public bool HasNewRocket => hasNewRocket;

        [Header("References")]
        public FoxController player;
        public RocketController currentRocket;
        public DirectionController directionController;
        public BackgroundManager backgroundManager;
        public EnemySpawner enemySpawner;

        [Header("UI")]
        public Text scoreText;
        public Text timerText;
        public Text directionText;
        public GameObject rocketWarningPanel;
        public GameObject startPanel;
        public GameObject gameOverPanel;
        public Text finalScoreText;

        [Header("Rocket Settings")]
        public GameObject newRocketPrefab;
        public float rocketSpawnTime = 3f;
        public bool hasNewRocket = false;
        public Vector2 rocketSpawnBounds = new Vector2(7f, 3f);

        [Header("Scoring")]
        public int currentScoreMultiplier = 1;

        private GameState gameState;
        private int score = 0;
        private float gameTime = 0;
        private GameObject spawnedNewRocket;
        private float rocketChangeTimer = 0;

        void Awake()
        {
            gameState = GameState.Menu;
            score = 0;
            
            if (startPanel)
                startPanel.SetActive(true);
            if (gameOverPanel)
                gameOverPanel.SetActive(false);
            if (rocketWarningPanel)
                rocketWarningPanel.SetActive(false);
        }

        void Start()
        {
            if (player)
                player.gameObject.SetActive(false);
            if (currentRocket)
                currentRocket.gameObject.SetActive(false);
        }

        void Update()
        {
            if (gameState != GameState.Playing) return;

            gameTime += Time.deltaTime;

            UpdateUI();

            if (hasNewRocket && rocketChangeTimer > 0)
            {
                rocketChangeTimer -= Time.deltaTime;
                if (rocketChangeTimer <= 0)
                {
                    MissedRocket();
                }
            }
        }

        public void StartGame()
        {
            gameState = GameState.Playing;
            score = 0;
            gameTime = 0;
            hasNewRocket = false;
            rocketChangeTimer = 0;

            if (startPanel)
                startPanel.SetActive(false);
            if (gameOverPanel)
                gameOverPanel.SetActive(false);

            if (player)
            {
                player.gameObject.SetActive(true);
                player.Revive();
            }

            if (currentRocket)
            {
                currentRocket.gameObject.SetActive(true);
                currentRocket.Activate();
            }

            if (enemySpawner)
                enemySpawner.StartSpawning();

            if (directionController)
                directionController.Reset();

            if (backgroundManager)
                backgroundManager.SetBackground(BackgroundType.Space);
        }

        public void GameOver()
        {
            gameState = GameState.GameOver;
            
            if (gameOverPanel)
            {
                finalScoreText.text = $"SCORE: {score}";
                gameOverPanel.SetActive(true);
            }
            
            if (enemySpawner)
                enemySpawner.StopSpawning();
        }

        public void OnRocketExploded()
        {
            hasNewRocket = true;
            rocketChangeTimer = rocketSpawnTime;
            
            if (rocketWarningPanel)
                rocketWarningPanel.SetActive(true);

            SpawnNewRocket();
        }

        void SpawnNewRocket()
        {
            if (newRocketPrefab)
            {
                Vector2 spawnPos = new Vector2(
                    Random.Range(-rocketSpawnBounds.x, rocketSpawnBounds.x),
                    Random.Range(-rocketSpawnBounds.y, rocketSpawnBounds.y)
                );
                
                spawnedNewRocket = Instantiate(newRocketPrefab, spawnPos, Quaternion.identity);
                currentRocket = spawnedNewRocket.GetComponent<RocketController>();
            }
        }

        public void HopToRocket()
        {
            if (!hasNewRocket || !spawnedNewRocket || !player) return;

            if (currentRocket)
                Destroy(currentRocket.gameObject);

            currentRocket = spawnedNewRocket.GetComponent<RocketController>();
            currentRocket.Activate();
            
            player.MountRocket(currentRocket);
            
            hasNewRocket = false;
            spawnedNewRocket = null;
            rocketChangeTimer = 0;
            
            currentScoreMultiplier = 1;
            
            if (rocketWarningPanel)
                rocketWarningPanel.SetActive(false);
            
            if (backgroundManager)
                backgroundManager.SetBackground(backgroundManager.GetNextBackground());
        }

        void MissedRocket()
        {
            if (spawnedNewRocket)
                Destroy(spawnedNewRocket);
            
            GameOver();
        }

        public void AddScore(int amount)
        {
            score += amount * currentScoreMultiplier;
        }

        void UpdateUI()
        {
            if (scoreText)
                scoreText.text = score.ToString();
            
            if (timerText && currentRocket)
                timerText.text = Mathf.Ceil(currentRocket.currentTimer).ToString();
            
            if (directionText && directionController)
                directionText.text = directionController.GetDirectionName();
        }

        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        void OnDestroy()
        {
            if (spawnedNewRocket)
                Destroy(spawnedNewRocket);
        }
    }
}