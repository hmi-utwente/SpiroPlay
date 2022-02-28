using UnityEngine;

namespace M8_Discobowlen
{
    
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Rens van der Werff - r.vanderwerff@student.utwente.nl
    /// 
    /// Bowlingball_Behavior class:
    /// Controls the bowlingball
    /// </summary>
    
    public class Bowlingball_Behavior : MonoBehaviour
    {
        #pragma warning disable 649
        
        [SerializeField] private Animator ani_person;
        [SerializeField] private Animator ani_bar;
        [SerializeField] private Animator ani_beam;
        [SerializeField] private Animator ani_disco;
        private Animator ani_ball;

        [SerializeField] private bool landed;
        private AudioSource rolling;
        [SerializeField] private AudioClip[] sounds;
        [SerializeField] private Transform otherBall;

        [HideInInspector] public float drawProgress;
        [HideInInspector] public bool thrown;

        private float throwStart;
        private float ballStart;
        
        private bool t = true;
        private bool y = true;

        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;
        [HideInInspector] public Vector3 position;

        #pragma warning restore 649
        
        private void Start()
        {
            ani_ball = GetComponent<Animator>();
            rolling = GetComponent<AudioSource>();
            rolling.clip = sounds[0];
            position = transform.position;
        }

        private void Update()
        {
            ani_person.SetFloat("DrawProgress", drawProgress);
            ani_ball.SetFloat("DrawProgress", drawProgress);
            ani_bar.SetFloat("DrawProgress", drawProgress);

            if (thrown && t)
            {
                Throw();
                t = false;
            }

            if (landed && y)
            {
                rolling.clip = sounds[1];
                rolling.loop = true;
                rolling.Play();
                y = false;
            }
            
            if (otherBall.position.y > 2f)
            {
                rolling.enabled = false;
            }
        }

        private void Throw()
        {
            // Calculate proper start of animation
            throwStart = (1 - drawProgress) / 1.5f;
            ballStart = (1 - drawProgress) / 6.8f;
            ani_person.Play("Player_Throw", 0 , throwStart);
            ani_ball.Play("Ball_Throw", 0 , ballStart);
            ani_beam.Play("Beam_Loop");
            ani_disco.Play("Discoball_Loop");
        }
        public void EndLoop()
        {
            position.x += speed;
            transform.position = position;
            transform.Rotate(0,0, -rotationSpeed);
        }
        
        public void End()
        {
            ani_ball.enabled = false;
            ani_beam.enabled = false;
            ani_disco.enabled = false;
        }

        public void Switch()
        {
            speed = 0f;
            ani_ball.enabled = false;
        }
        
    }

}

