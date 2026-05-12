using GameJam;
using UnityEngine;

namespace GameJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int _savedLives;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RegisterSavedEntity(Entity entity)
        {
            _savedLives += entity.Lives;
            Debug.Log($"Vite salvate: {_savedLives}");
        }
    }
}