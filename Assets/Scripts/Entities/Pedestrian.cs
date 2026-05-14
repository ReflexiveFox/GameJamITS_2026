using UnityEngine;

namespace GameJam
{
    public class Pedestrian : Entity
    {
        [Header("Pedestrian Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private float waitTime = 1.0f;
        protected override void Awake()
        {
            base.Awake();
            if(animator == null)
            {
                Debug.LogError($"Animator is missing in this pedestrian!", this);
            }
        }

        public override void DestroyEntity()
        {
            Debug.Log("Pedestrian destroy");
            Invoke(nameof(DisableObject), waitTime);
        }

        private void DisableObject()
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 2f);
        }

        public void PlayDeathAnimation()
        {
            animator.SetTrigger("Die");
        }
    }
}