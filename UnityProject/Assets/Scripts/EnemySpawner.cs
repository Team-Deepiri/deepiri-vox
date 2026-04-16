using UnityEngine;
using System.Collections;

namespace FoxRocketArcade
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject[] enemyPrefabs;
        public float baseSpawnInterval = 2f;
        public float spawnIntervalDecrease = 0.02f;
        public float minSpawnInterval = 0.5f;
        public float spawnBoundX = 8f;
        public float spawnY = 7f;
        public float startDelay = 2f;

        [Header("Difficulty Scaling")]
        public float difficultyIncreaseRate = 0.1f;
        public int maxEnemies = 20;

        private float currentSpawnInterval;
        private float lastSpawnTime;
        private float difficulty = 1f;
        private bool isSpawning = false;
        private int activeEnemies = 0;

        void Start()
        {
            currentSpawnInterval = baseSpawnInterval;
            lastSpawnTime = Time.time + startDelay;
        }

        void Update()
        {
            if (!isSpawning) return;

            if (Time.time - lastSpawnTime >= currentSpawnInterval)
            {
                SpawnEnemy();
                lastSpawnTime = Time.time;
            }

            difficulty += difficultyIncreaseRate * Time.deltaTime;
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval, 
                baseSpawnInterval - (difficulty * spawnIntervalDecrease)
            );
        }

        public void StartSpawning()
        {
            isSpawning = true;
            difficulty = 1f;
            currentSpawnInterval = baseSpawnInterval;
            lastSpawnTime = Time.time;
        }

        public void StopSpawning()
        {
            isSpawning = false;
        }

        void SpawnEnemy()
        {
            if (activeEnemies >= maxEnemies) return;

            int enemyIndex = GetWeightedRandom();
            if (enemyIndex < 0 || enemyIndex >= enemyPrefabs.Length) return;

            float xPos = Random.Range(-spawnBoundX, spawnBoundX);
            Vector2 spawnPos = new Vector2(xPos, spawnY);
            
            GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPos, Quaternion.identity);
            activeEnemies++;
            
            var controller = enemy.GetComponent<EnemyController>();
            if (controller)
            {
                controller.health += Mathf.FloorToInt(difficulty / 5);
            }
        }

        int GetWeightedRandom()
        {
            float r = Random.value;
            
            if (difficulty < 3)
            {
                if (r < 0.7f) return 0;
                if (r < 0.95f) return 1;
                return 2;
            }
            else if (difficulty < 6)
            {
                if (r < 0.5f) return 0;
                if (r < 0.85f) return 1;
                return 2;
            }
            else
            {
                if (r < 0.4f) return 0;
                if (r < 0.7f) return 1;
                if (r < 0.9f) return 2;
                return 2;
            }
        }

        public void OnEnemyDestroyed()
        {
            activeEnemies--;
        }
    }
}