using UnityEngine;

namespace GameJam
{
    public class GenericSpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        [SerializeField] private Entity prefab;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int spawnCount = 1;
        [Header("Spawn Area Settings")]
        [Header("Horizontal Spawn Offset")]
        [SerializeField] private float minXSpawnOffset = -.5f;
        [SerializeField] private float maxXSpawnOffset = .5f;
        [Header("Vertical Spawn Offset")]
        [SerializeField] private float minZSpawnOffset = -.5f;
        [SerializeField] private float maxZSpawnOffset = .5f;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= spawnInterval)
            {
                Spawn();
                _timer = 0f;
            }
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