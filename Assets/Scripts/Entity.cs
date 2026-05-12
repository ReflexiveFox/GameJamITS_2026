using UnityEngine;

namespace GameJam
{
    public abstract class Entity : SelectableEntity
    {
        [SerializeField] private float timeFactor;

        private enum TimeState 
        {
             Slow = 0,
             Normal = 1,
             Fast = 2
        }

        public void ReduceTimeFactor()
        {

        }

        public void IncreaseTimeFactor()
        {

        }
    }
}