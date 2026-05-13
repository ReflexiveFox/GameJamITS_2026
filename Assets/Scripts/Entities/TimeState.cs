using System;
using UnityEngine;

namespace GameJam
{
    public class TimeState : MonoBehaviour
    {
        public static TimeState Instance;

        [Serializable]
        public enum TimeStateEnum
        {
            Slow = 0,
            Normal = 1,
            Fast = 2
        }
        [SerializeField] private float Slow = 0.5f;
        [SerializeField] private float Normal = 1f;
        [SerializeField] private float Fast = 2f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public float GetTimeFactor(TimeStateEnum timeState)
        {
            return timeState switch
            {
                TimeStateEnum.Slow => Slow,
                TimeStateEnum.Normal => Normal,
                TimeStateEnum.Fast => Fast,
                _ => Normal
            };
        }
    }
}
    
