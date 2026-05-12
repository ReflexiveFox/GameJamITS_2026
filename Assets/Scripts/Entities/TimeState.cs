using System;

namespace GameJam
{
    public static class TimeState
    {
        [Serializable]
        public enum TimeStateEnum
        {
            Slow = 0,
            Normal = 1,
            Fast = 2
        }
        public const float Slow = 0.5f;
        public const float Normal = 1f;
        public const float Fast = 2f;

        public static float GetTimeFactor(TimeStateEnum timeState)
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
    
