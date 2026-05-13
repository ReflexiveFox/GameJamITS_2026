using UnityEngine;

namespace GameJam
{
    public class GenericSpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        [SerializeField] private Entity prefab;
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

        private void Start()
        {
            UpdateSpawnInterval(true);
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= currentSpawnInterval)
            {
                Spawn();
                _timer = 0f;
                UpdateSpawnInterval(false);
            }
        }

        private void UpdateSpawnInterval(bool isStart)
        {
            currentSpawnInterval = Random.Range(isStart ? minStartSpawnInterval : minSpawnInterval, isStart ? maxStartSpawnInterval : maxSpawnInterval);
        }

        private void Spawn()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 randomOffset = new(Random.Range(minXSpawnOffset, maxXSpawnOffset), 0f, Random.Range(minZSpawnOffset, maxZSpawnOffset));
                Instantiate(prefab, transform.position + randomOffset, transform.rotation);
            }
        }
    }
}