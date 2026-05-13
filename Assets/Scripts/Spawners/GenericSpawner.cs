using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameJam
{
    public class GenericSpawner : MonoBehaviour
    {
        [Serializable]
        public struct SpawningEntity
        {
            public Entity prefabToSpawn;
            public int amountToSpawn;
        }

        [Header("Spawner Settings")]
        [SerializeField] private List<SpawningEntity> entitiesToSpawn;
        private List<SpawningEntity> currentEntitiesToSpawn;
        [Header("Normal Spawn values")]
        [SerializeField, Min(0f)] private float minSpawnInterval = 3f;
        [SerializeField, Min(0f)] private float maxSpawnInterval = 5f;
        [Header("Starting Spawn values")]
        [SerializeField, Min(0f)] private float minStartSpawnInterval = 0f;
        [SerializeField, Min(0f)] private float maxStartSpawnInterval = 3f;
        [Space]
        [SerializeField] private int spawnCount = 1;
        [Header("Spawn Area Settings")]
        [Header("Horizontal Spawn Offset")]
        [SerializeField] private float minXSpawnOffset = -.5f;
        [SerializeField] private float maxXSpawnOffset = .5f;
        [Header("Vertical Spawn Offset")]
        [SerializeField] private float minZSpawnOffset = -.5f;
        [SerializeField] private float maxZSpawnOffset = .5f;

        private float _timer;
        private float currentSpawnInterval;
        private bool canSpawn;

        private bool IsListEmpty
        {
            get
            {
                foreach(var entity in currentEntitiesToSpawn)
                {
                    if (entity.amountToSpawn != 0)
                        return false;
                }
                return true;
            }
        }

        private void Awake()
        {
            GameManager.OnGameStarted += StartSpawning;
        }

        private void Start()
        {
            currentEntitiesToSpawn = entitiesToSpawn;
            canSpawn = false;
        }

        private void Update()
        {
            if(GameManager.Instance.IsPaused || !canSpawn) return;

            _timer += Time.deltaTime;

            if (_timer >= currentSpawnInterval)
            {
                Spawn();
                _timer = 0f;
                SetSpawnInterval(false);
            }
        }

        private void OnDestroy()
        {
            GameManager.OnGameStarted -= StartSpawning;
        }

        private void StartSpawning()
        {
            canSpawn = true;
            SetSpawnInterval(true);
        }

        private void SetSpawnInterval(bool isStart)
        {
            currentSpawnInterval = Random.Range(isStart ? minStartSpawnInterval : minSpawnInterval, isStart ? maxStartSpawnInterval : maxSpawnInterval);
        }

        private void Spawn()
        {
            if(!canSpawn) return;

            for (int i = 0; i < spawnCount;)
            {
                if (IsListEmpty)
                {
                    // Refill list
                    currentEntitiesToSpawn = entitiesToSpawn;
                }
                SpawningEntity spawningEntity = currentEntitiesToSpawn[Random.Range(0, entitiesToSpawn.Count)];
                Vector3 randomOffset = new(Random.Range(minXSpawnOffset, maxXSpawnOffset), 0f, Random.Range(minZSpawnOffset, maxZSpawnOffset));
                
                if (spawningEntity.amountToSpawn > 0)
                {
                    Instantiate(spawningEntity.prefabToSpawn, transform.position + randomOffset, transform.rotation);

                    if (currentEntitiesToSpawn.Count > 1)
                    {
                        spawningEntity.amountToSpawn--;
                    }
                    i++;
                }
            }
        }
    }
}