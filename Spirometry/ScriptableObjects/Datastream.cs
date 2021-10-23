using System.Collections.Generic;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Spirometry.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Datastream", menuName = "Spiro-Play/Datastream", order = 2)]
    public class Datastream : ScriptableObject
    {
        public enum Type { FlowOverTime, FlowOverVolume, VolumeOverTime }
        public Type CurveType;
        public UnityEvent newValueReceived = new UnityEvent();

        public int Count { get { return Values.Count; } }
        [FormerlySerializedAs("Data")] public List<SpiroData> Values = new List<SpiroData>();

        public void Add (SpiroData spirodata)
        {
            Values.Add(spirodata);
            newValueReceived.Invoke();
        }

        public void Clear()
        {
            if (Values.Count > 0)
                Values.Clear();
        }
    }
}
