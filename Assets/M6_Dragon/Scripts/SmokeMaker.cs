using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeMaker : MonoBehaviour
{
    private ParticleSystem ps = null;
    private List<ParticleCollisionEvent> events = null;

    [SerializeField] private string tagToCheck = "";
    [SerializeField] GameObject smokeObject = null;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        events = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int count = ps.GetSafeCollisionEventSize();

        if (count > events.Count) {
            events = new List<ParticleCollisionEvent>();
        }

        int numEvents = ps.GetCollisionEvents(other, events);

        if (other.CompareTag(tagToCheck))
        {
            
            for (int i = 0; i < numEvents; i++)
            {
                if (Random.value > 0.3f) continue;
                Vector3 intersectPoint = events[i].intersection;
                GameObject spawnedSmoke = Instantiate(smokeObject, intersectPoint, smokeObject.transform.rotation);
            }
        }
    }
}
