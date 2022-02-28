using UnityEngine;

namespace M9_Soccer
{
    public class AudienceBehaviour : MonoBehaviour
    {

        #pragma  warning disable 649
    
        [Header("Configuration")]
        [SerializeField] private Animator[] animators = null;
        [SerializeField] private AudioSource cheerAudio = null;
    
        private static readonly int FadeIsCheering = Animator.StringToHash("IsCheering");

        public bool IsCheering
        {
            set
            {
                //enable or disable audience audio
                switch (value)
                {
                    case true when !cheerAudio.isPlaying:
                        cheerAudio.Play();
                        break;
                    case false when cheerAudio.isPlaying:
                        cheerAudio.Stop();
                        break;
                }

                //trigger animation
                foreach (var animator in animators)
                    animator.SetBool(FadeIsCheering, value);
            }
        }
    
        #pragma warning restore 649

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsCheering = true;
            }
        }
    }
}
