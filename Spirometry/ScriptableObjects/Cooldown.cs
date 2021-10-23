using System.Collections;
using UnityEngine;

namespace Spirometry.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Cooldown", menuName = "Spiro-Play/Cooldown", order = 3)]
    public class Cooldown : ScriptableObject
    {
        public bool IsActive { get; private set; }
    
        private IEnumerator StartCooldown(float duration)
        {
            IsActive = true;
            yield return new WaitForSeconds(duration);
            IsActive = false;
            yield return null;
        }
    }
}
