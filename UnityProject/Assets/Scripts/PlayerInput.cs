using UnityEngine;

namespace FoxRocketArcade
{
    public class PlayerInput : MonoBehaviour
    {
        private GameManager gameManager;
        private FoxController player;
        private DirectionController directionController;

        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = FindObjectOfType<FoxController>();
            directionController = FindObjectOfType<DirectionController>();
        }

        void Update()
        {
            if (!gameManager || gameManager.GameState != GameState.Playing) return;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (gameManager.HasNewRocket)
                {
                    gameManager.HopToRocket();
                }
            }
        }
    }
}