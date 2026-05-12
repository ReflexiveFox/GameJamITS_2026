using GameJam;
using UnityEngine;

namespace GameJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Stats")]
        [SerializeField] private int targetLostLives = 10;

        [Header("Debug, don't touch")]
        [SerializeField] private int _savedLives;
        [SerializeField] private int currentLostLives = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Entity.OnEntitiesCollided += HandleEntitiesAccident;
        }

        private void Start()
        {
            _savedLives = 0;
            currentLostLives = 0;
        }

        private void OnDestroy()
        {
            Entity.OnEntitiesCollided -= HandleEntitiesAccident;
        }

        private void HandleEntitiesAccident(int entityLives)
        {
            currentLostLives += entityLives;
            if(currentLostLives >= targetLostLives)
            {
                Debug.Log("Game Over!");
                // Implement game over logic here (e.g., show game over screen, restart level, etc.)
            }
        }

        public void RegisterSavedEntity(Entity entity)
        {
            _savedLives += entity.Lives;
            Debug.Log($"Vite salvate: {_savedLives}");
        }
    }
}