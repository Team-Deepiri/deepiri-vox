using UnityEngine;

namespace FoxRocketArcade
{
    public enum PowerupType
    {
        RapidFire,
        Shield,
        Multiplier
    }

    public class PowerupController : MonoBehaviour
    {
        [Header("Settings")]
        public PowerupType powerupType;
        public float fallSpeed = 3f;
        public float rotationSpeed = 100f;

        [Header("Effects")]
        public float duration = 10f;
        public float pickupRadius = 1f;

        private GameManager gameManager;
        private Transform player;

        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        void Start()
        {
            GetComponent<SpriteRenderer>().color = GetColor();
        }

        Color GetColor()
        {
            switch (powerupType)
            {
                case PowerupType.RapidFire: return Color.red;
                case PowerupType.Shield: return Color.cyan;
                case PowerupType.Multiplier: return Color.yellow;
                default: return Color.white;
            }
        }

        void Update()
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            if (transform.position.y < -7f)
            {
                Destroy(gameObject);
            }

            if (player)
            {
                float dist = Vector2.Distance(transform.position, player.position);
                if (dist < pickupRadius)
                {
                    Collect();
                }
            }
        }

        void Collect()
        {
            if (!gameManager) return;

            var fox = player.GetComponent<FoxController>();
            if (fox)
            {
                switch (powerupType)
                {
                    case PowerupType.RapidFire:
                        fox.fireCooldown = 0.05f;
                        break;
                    case PowerupType.Shield:
                        break;
                    case PowerupType.Multiplier:
                        gameManager.currentScoreMultiplier = 2;
                        break;
                }
            }

            gameManager.AddScore(50);
            Destroy(gameObject);
        }
    }
}