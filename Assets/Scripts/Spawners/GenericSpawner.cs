using UnityEngine;

namespace GameJam
{
    public class GenericSpawner : MonoBehaviour
    {
        [SerializeField] private Entity prefab;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int spawnCount = 1;

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
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                Instantiate(prefab, transform.position + randomOffset, transform.rotation);
            }
        }
    }
}