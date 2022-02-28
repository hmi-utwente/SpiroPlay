using UnityEngine;

namespace M6_Dragon.Scripts
{
    public class RoetvlekBehaviour : MonoBehaviour
    {
        public int lifetime;
        
        // Start is called before the first frame update
        private void Start()
        {
            //randomize roetvlek appearance
            var image = gameObject.GetComponent<SpriteRenderer>();
            image.flipX = Random.value > 0.5f;
            Destroy(gameObject, lifetime);
        }
    }
}
