using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event Action<int> OnTargetLostLivesUpdated = delegate { };
        public static event Action<int> OnSavedLivesUpdated = delegate { };
        public static event Action<int> OnGameOver = delegate { };

        [Header("Stats")]
        [SerializeField] private int targetLostLives = 10;

        [Header("Debug, don't touch")]
        [SerializeField] private int currentSavedLives;
        [SerializeField] private int currentLostLives = 0;

        public int CurrentLostLives
        {
            get => currentLostLives;
            private set
            {
                currentLostLives = value;
                OnTargetLostLivesUpdated?.Invoke(currentLostLives);
            }
        }

        public int TargetLostLives => targetLostLives;

        public int CurrentSavedLives 
        {
            get => currentSavedLives;
            set
            {
                currentSavedLives = value;
                OnSavedLivesUpdated?.Invoke(currentSavedLives);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Entity.OnEntitiesCollided += HandleEntitiesAccident;
            SceneManager.sceneLoaded += ResetTimeScale;
            SceneManager.sceneUnloaded += ResetTimeScale;
        }       

        private void Start()
        {
            CurrentSavedLives = 0;
            CurrentLostLives = 0;
        }

        private void OnDestroy()
        {
            Entity.OnEntitiesCollided -= HandleEntitiesAccident;
            SceneManager.sceneLoaded -= ResetTimeScale;
            SceneManager.sceneUnloaded -= ResetTimeScale;
        }

        private void HandleEntitiesAccident(int entityLives)
        {
            CurrentLostLives += entityLives;
            if(CurrentLostLives >= TargetLostLives)
            {
                Time.timeScale = 0f;
                OnGameOver?.Invoke(CurrentSavedLives);
            }
        }
        private void ResetTimeScale(Scene arg0)
        {
            Time.timeScale = 1f;
        }

        private void ResetTimeScale(Scene arg0, LoadSceneMode arg1)
        {
            Time.timeScale = 1f;
        }

        public void RegisterSavedEntity(Entity entity)
        {
            CurrentSavedLives += entity.Lives;
            Debug.Log($"Vite salvate: {CurrentSavedLives}");
        }
    }
}