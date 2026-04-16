using UnityEngine;
using System.Collections;

namespace FoxRocketArcade
{
    public class RocketController : MonoBehaviour
    {
        [Header("Rocket Stats")]
        public float maxDuration = 25f;
        public float currentTimer;
        public bool isExploding = false;

        [Header("Visuals")]
        public SpriteRenderer bodySprite;
        public ParticleSystem flameParticles;
        public ParticleSystem explosionPrefab;
        public Transform flamePoint;

        [Header("Movement")]
        public float hoverAmount = 0.1f;
        public float hoverSpeed = 3f;

        private float startY;
        private SpriteRenderer sr;
        private GameManager gameManager;
        private bool isActive = true;

        public bool IsActive => isActive;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            startY = transform.position.y;
            currentTimer = maxDuration;
        }

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        void Update()
        {
            if (!isActive || !gameManager || gameManager.GameState != GameState.Playing) return;

            if (!isExploding)
            {
                currentTimer -= Time.deltaTime;
                
                float hover = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
                transform.position = new Vector3(transform.position.x, startY + hover, transform.position.z);

                if (currentTimer <= 0)
                {
                    Explode();
                }
            }
            
            UpdateFlame();
        }

        void UpdateFlame()
        {
            if (flameParticles && !isExploding)
            {
                var main = flameParticles.main;
                main.startSize = 0.3f + Mathf.Sin(Time.time * 15) * 0.1f;
            }
        }

        public void Explode()
        {
            isExploding = true;
            
            if (explosionPrefab)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            
            if (flameParticles)
                flameParticles.Stop();
            
            sr.enabled = false;
            
            gameManager?.OnRocketExploded();
        }

        public void Activate()
        {
            isActive = true;
            isExploding = false;
            currentTimer = maxDuration;
            sr.enabled = true;
            
            if (flameParticles)
                flameParticles.Play();
        }

        public void Deactivate()
        {
            isActive = false;
            if (flameParticles)
                flameParticles.Stop();
        }

        public float GetTimerPercent() => currentTimer / maxDuration;
    }
}