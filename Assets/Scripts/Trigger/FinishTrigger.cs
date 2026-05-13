using UnityEngine;

namespace GameJam
{
    public class FinishTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Entity>(out Entity entity))
            {
                GameManager.Instance.RegisterSavedEntity(entity);
                entity.DestroyEntity();
            }
        }
    }
}