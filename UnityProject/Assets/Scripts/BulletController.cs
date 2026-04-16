using UnityEngine;

namespace FoxRocketArcade
{
    public class BulletController : MonoBehaviour
    {
        [Header("Settings")]
        public float speed = 15f;
        public int damage = 1;
        public float lifetime = 3f;
        public bool isEnemy = false;

        private Rigidbody2D rb;
        private float spawnTime;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            spawnTime = Time.time;
            
            if (!isEnemy)
            {
                rb.velocity = Vector2.up * speed;
            }
        }

        void Update()
        {
            if (Time.time - spawnTime > lifetime)
            {
                Destroy(gameObject);
                return;
            }

            if (transform.position.y > 10f || transform.position.y < -10f ||
                transform.position.x < -12f || transform.position.x > 12f)
            {
                Destroy(gameObject);
            }
        }

        public void SetDirection(Vector2 dir)
        {
            rb.velocity = dir.normalized * speed;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (isEnemy)
            {
                if (other.CompareTag("Player"))
                {
                    var fox = other.GetComponent<FoxController>();
                    if (fox)
                        fox.Die();
                    Destroy(gameObject);
                }
            }
            else
            {
                if (other.CompareTag("Enemy"))
                {
                    var enemy = other.GetComponent<EnemyController>();
                    if (enemy)
                        enemy.TakeDamage(damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}