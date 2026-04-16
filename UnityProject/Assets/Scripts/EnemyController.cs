using UnityEngine;
using System.Collections;

namespace FoxRocketArcade
{
    public enum EnemyType
    {
        Drone,
        Fighter,
        Mother
    }

    public class EnemyController : MonoBehaviour
    {
        [Header("Enemy Type")]
        public EnemyType enemyType;

        [Header("Stats")]
        public int health = 1;
        public int scoreValue = 100;
        public float moveSpeed = 2f;
        public float fireRate = 1f;
        public float detectionRange = 5f;

        [Header("Combat")]
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float bulletSpeed = 5f;

        [Header("AI Behavior")]
        public bool doZigzag = false;
        public float zigzagAmount = 2f;
        public float zigzagSpeed = 3f;
        public bool doChase = false;
        public bool doShoot = true;

        private Transform playerTransform;
        private float lastFireTime;
        private float zigzagOffset;
        private SpriteRenderer sr;
        private bool isDead = false;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            zigzagOffset = Random.Range(0f, Mathf.PI * 2f);
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        void Start()
        {
            SetupEnemy();
        }

        void SetupEnemy()
        {
            switch (enemyType)
            {
                case EnemyType.Drone:
                    health = 1;
                    scoreValue = 100;
                    moveSpeed = 3f;
                    fireRate = 0.5f;
                    doZigzag = true;
                    doShoot = true;
                    break;
                case EnemyType.Fighter:
                    health = 2;
                    scoreValue = 250;
                    moveSpeed = 4f;
                    fireRate = 0.8f;
                    doChase = true;
                    doShoot = true;
                    break;
                case EnemyType.Mother:
                    health = 10;
                    scoreValue = 1000;
                    moveSpeed = 1f;
                    fireRate = 1.5f;
                    doShoot = true;
                    break;
            }
        }

        void Update()
        {
            if (isDead || !playerTransform) return;

            if (IsOutOfBounds())
            {
                Destroy(gameObject);
                return;
            }

            if (doShoot && Time.time - lastFireTime >= fireRate)
            {
                Shoot();
            }
        }

        void FixedUpdate()
        {
            if (isDead || !playerTransform) return;

            Vector2 targetPos = transform.position;

            if (doChase && playerTransform)
            {
                targetPos = playerTransform.position;
            }
            else
            {
                targetPos.y -= moveSpeed * Time.fixedDeltaTime;
            }

            if (doZigzag)
            {
                float zigzag = Mathf.Sin(Time.time * zigzagSpeed + zigzagOffset) * zigzagAmount;
                targetPos.x += zigzag * Time.fixedDeltaTime;
            }

            transform.position = Vector2.Lerp(transform.position, targetPos, 0.1f);
        }

        void Shoot()
        {
            if (!bulletPrefab || !firePoint || !playerTransform) return;

            Vector2 direction = (playerTransform.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            lastFireTime = Time.time;
        }

        bool IsOutOfBounds()
        {
            return transform.position.y < -8f || transform.position.x < -12f || transform.position.x > 12f;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            isDead = true;
            FindObjectOfType<GameManager>()?.AddScore(scoreValue);
            
            if (enemyType == EnemyType.Mother)
            {
                SpawnPowerup();
            }
            
            Destroy(gameObject);
        }

        void SpawnPowerup()
        {
            if (Random.value < 0.3f)
            {
                var powerup = Resources.Load<GameObject>("Prefabs/Powerup");
                if (powerup)
                    Instantiate(powerup, transform.position, Quaternion.identity);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PlayerBullet"))
            {
                TakeDamage(1);
                Destroy(other.gameObject);
            }
        }
    }
}