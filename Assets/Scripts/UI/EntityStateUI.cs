using UnityEngine;
using UnityEngine.UI;

namespace GameJam
{
    [RequireComponent(typeof(Image))]
    public class EntityStateUI : MonoBehaviour
    {
        [SerializeField] private Entity connectedEntity;
        private Image stateImage;

        private void Awake()
        {
            stateImage = GetComponent<Image>();
            if(connectedEntity == null)
            {
                Debug.LogError($"Connected Entity is not assigned into inspector!", this);
            }
            else
            {
                connectedEntity.OnEntityTimeStateChanged += OnConnectedEntityTimeStateChanged;
            }
        }

        private void OnDestroy()
        {
            connectedEntity.OnEntityTimeStateChanged -= OnConnectedEntityTimeStateChanged;
        }

        private void OnConnectedEntityTimeStateChanged(Entity entity)
        {
            switch (entity.CurrentTimeState)
            {
                case TimeState.TimeStateEnum.Normal:
                    stateImage.color = Color.white;
                    break;
                case TimeState.TimeStateEnum.Slow:
                    stateImage.color = Color.blue;
                    break;
                case TimeState.TimeStateEnum.Fast:
                    stateImage.color = Color.red;
                    break;
            }
        }
    }
}