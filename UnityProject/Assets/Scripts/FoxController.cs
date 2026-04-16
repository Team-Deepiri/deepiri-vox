using UnityEngine;
using System.Collections;

namespace FoxRocketArcade
{
    public class FoxController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 8f;
        public Vector2 moveBounds = new Vector2(7f, 4f);

        [Header("Combat")]
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float fireCooldown = 0.15f;

        [Header("Visuals")]
        public SpriteRenderer bodySprite;
        public SpriteRenderer headSprite;
        public SpriteRenderer tailSprite;
        public SpriteRenderer backpackSprite;
        public SpriteRenderer shirtSprite;

        private Rigidbody2D rb;
        private Vector2 velocity;
        private float lastFireTime;
        private bool isAlive = true;

        private RocketController currentRocket;
        private GameManager gameManager;

        public bool IsAlive => isAlive;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            gameManager = FindObjectOfType<GameManager>();
        }

        void Update()
        {
            if (!isAlive || gameManager?.GameState != GameState.Playing) return;

            HandleInput();
            UpdateAnimation();
        }

        void HandleInput()
        {
            velocity = Vector2.zero;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                velocity.x = -1f;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                velocity.x = 1f;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                velocity.y = 1f;
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                velocity.y = -1f;

            velocity = velocity.normalized * moveSpeed;

            if (Input.GetKey(KeyCode.Space) && Time.time - lastFireTime >= fireCooldown)
            {
                Fire();
            }
        }

        void Fire()
        {
            if (bulletPrefab && firePoint)
            {
                Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                lastFireTime = Time.time;
            }
        }

        void UpdateAnimation()
        {
            if (velocity.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (velocity.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }

        void FixedUpdate()
        {
            if (!isAlive) return;

            rb.velocity = velocity;

            Vector2 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -moveBounds.x, moveBounds.x);
            pos.y = Mathf.Clamp(pos.y, -moveBounds.y + 1f, moveBounds.y);
            transform.position = pos;
        }

        public void MountRocket(RocketController rocket)
        {
            currentRocket = rocket;
            transform.SetParent(rocket.transform);
            transform.localPosition = new Vector3(0f, 0.8f, 0f);
        }

        public void DismountRocket()
        {
            transform.SetParent(null);
            currentRocket = null;
        }

        public void Die()
        {
            isAlive = false;
            rb.velocity = Vector2.zero;
            
            GetComponent<SpriteRenderer>().enabled = false;
            
            if (gameManager)
                gameManager.GameOver();
        }

        public void Revive()
        {
            isAlive = true;
            GetComponent<SpriteRenderer>().enabled = true;
            lastFireTime = 0;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
            {
                Die();
            }
        }
    }
}