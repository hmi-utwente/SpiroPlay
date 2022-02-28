using System.Collections;
using System.Collections.Generic;
using Spirometry.ScriptableObjects;
using Spirometry.Statics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spirometry.Debugging
{
    
    /// <summary>
    /// Spiro-Play project by University of Twente, MST and Deventer ziekenhuis
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// Garbage collector class:
    /// This class is responsible for clearing datastreams periodically for performance reasons
    /// </summary>
    
    public class GarbageCollector : MonoBehaviour
    {

        #region variables
        #pragma warning disable 649
    
        [FormerlySerializedAs("AllowanceReference")]
        [Header("Settings")]
        [Tooltip("Check this data to see if collection is allowed")]
        [SerializeField] private Datastream Reference;
        [SerializeField] private Datastream[] ListsToClear;
        [SerializeField] private float CollectionInterval = 5f;
    
        #pragma warning restore 649
        #endregion
    
        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(GarbageCollection());
        }

        //initiate clearing of data when idle signal is detected
        private IEnumerator GarbageCollection()
        {
            while (Application.isPlaying)
            {
                if (Reference.Values.Count > 100)
                {
                    bool collect = true;
                    for (int i = Reference.Values.Count - 99; i < Reference.Values.Count - 1; i++)
                    {
                        if (Reference.Values[i].Flow < -0.3 || Reference.Values[i].Flow > 0.3)
                            collect = false;
                    }

                    if (collect)
                        ClearRTData();
                }
                yield return new WaitForSeconds(CollectionInterval);
            }
            yield return null;
        }

        //actually clear scriptable objects containing datastreams
        private void ClearRTData()
        {
            Debug.Log("Garbage Collector: Clearing data...");
            foreach (var list in ListsToClear)
            {
                list.Values = new List<SpiroData>();
                list.Clear();
                list.Values = new List<SpiroData>();
                list.Clear();
            }
        }
    }
}
