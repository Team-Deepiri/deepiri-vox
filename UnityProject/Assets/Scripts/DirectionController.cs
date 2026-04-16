using UnityEngine;
using System.Collections;

namespace FoxRocketArcade
{
    public class DirectionController : MonoBehaviour
    {
        [Header("Direction Settings")]
        public int currentDirection = 0;
        public int directionCount = 8;
        public float turnSpeed = 5f;
        public float turnCooldown = 0.3f;

        [Header("Pseudo-3D Settings")]
        public float perspectiveScaleMin = 0.8f;
        public float perspectiveScaleMax = 1.2f;
        public Camera mainCamera;
        public Transform worldContainer;

        [Header("Reference")]
        public BackgroundManager bgManager;

        private float targetAngle;
        private float currentAngle;
        private float lastTurnTime;
        private bool isTurning = false;

        public string[] directionNames = new string[] { 
            "NORTH", "NORTHEAST", "EAST", "SOUTHEAST", 
            "SOUTH", "SOUTHWEST", "WEST", "NORTHWEST" 
        };

        void Awake()
        {
            currentAngle = 0;
            targetAngle = 0;
        }

        void Start()
        {
            if (!mainCamera)
                mainCamera = Camera.main;
            if (!worldContainer)
                worldContainer = GameObject.Find("WorldContainer")?.transform ?? transform;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TurnLeft();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                TurnRight();
            }

            if (isTurning)
            {
                SmoothTurn();
            }
        }

        public void TurnLeft()
        {
            if (Time.time - lastTurnTime < turnCooldown) return;
            
            currentDirection = (currentDirection - 1 + directionCount) % directionCount;
            targetAngle = currentDirection * 45f;
            isTurning = true;
            lastTurnTime = Time.time;
            
            ApplyPseudo3D();
            CheckDirectionBonus();
        }

        public void TurnRight()
        {
            if (Time.time - lastTurnTime < turnCooldown) return;
            
            currentDirection = (currentDirection + 1) % directionCount;
            targetAngle = currentDirection * 45f;
            isTurning = true;
            lastTurnTime = Time.time;
            
            ApplyPseudo3D();
            CheckDirectionBonus();
        }

        void SmoothTurn()
        {
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, turnSpeed * Time.deltaTime);
            
            if (mainCamera)
            {
                mainCamera.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            }
            
            if (worldContainer)
            {
                worldContainer.rotation = Quaternion.Euler(0, 0, -currentAngle);
            }
            
            if (Mathf.Abs(currentAngle - targetAngle) < 0.5f)
            {
                currentAngle = targetAngle;
                isTurning = false;
            }
        }

        void ApplyPseudo3D()
        {
            float direction = currentDirection * 45f;
            float rad = direction * Mathf.Deg2Rad;
            
            float offsetX = Mathf.Sin(rad) * 0.5f;
            float offsetY = Mathf.Cos(rad) * 0.5f;
            
            float scale = Mathf.Lerp(perspectiveScaleMin, perspectiveScaleMax, (Mathf.Sin(rad) + 1) / 2);
            
            if (mainCamera)
            {
                Vector3 camPos = mainCamera.transform.position;
                camPos.x += offsetX;
                camPos.y += offsetY - 0.5f;
                mainCamera.transform.position = camPos;
            }
            
            if (bgManager)
            {
                if (currentDirection % 2 == 0)
                {
                    bgManager.SetBackground(bgManager.GetNextBackground());
                }
            }
        }

        void CheckDirectionBonus()
        {
            if (currentDirection % 2 == 0)
            {
                FindObjectOfType<GameManager>()?.AddScore(200);
            }
        }

        public string GetDirectionName()
        {
            return directionNames[currentDirection];
        }

        public float GetCurrentAngle()
        {
            return currentAngle;
        }

        public void Reset()
        {
            currentDirection = 0;
            targetAngle = 0;
            currentAngle = 0;
            isTurning = false;
            
            if (mainCamera)
            {
                mainCamera.transform.rotation = Quaternion.identity;
            }
            if (worldContainer)
            {
                worldContainer.rotation = Quaternion.identity;
            }
        }
    }
}