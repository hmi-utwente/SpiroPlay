using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Spirometry.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Event", menuName = "Spiro-Play/Event", order = 3)]
    public class Event : ScriptableObject
    {
        public UnityEvent value;

        public void Test()
        {
            Debug.Log(this.name + " has trigered!");
        }
    }
}
