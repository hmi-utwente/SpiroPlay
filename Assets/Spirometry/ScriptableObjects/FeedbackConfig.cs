using UnityEngine;
using UnityEngine.Video;

namespace Spirometry.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Feedback Configuration", menuName = "Spiro-Play/Feedback Configuration")]
    public class FeedbackConfig : ScriptableObject
    {
        [Header("Instruction Video")] public VideoClip video;
        
        [TextArea]
        [Header("Preperation Instructions")]
        public string instructions;
        public AudioClip instructionsAudio;
        [Header("Completed without any detected errors")]
        public string noErrors;
        public AudioClip noErrorsAudio;
        [Header("Test is interrupted inexpectedly")]
        public string testInterrupted;
        public AudioClip testInterruptedAudio;
        [Header("Unstatisfactory start of expiration")]
        public string badStartOfExpiration;
        public AudioClip badStartOfExpirationAudio;
        [Header("Premature end of expiration")]
        public string badEndOfExpiration;
        public AudioClip badEndOfExpirationAudio;
        [Header("Cough or Glottic Closure detected")]
        public string coughDetected;
        public AudioClip coughDetectedAudio;
        [Header("Multiple Errors")]
        public string badStartAndBadEnd;
        public AudioClip badStartAndBadEndAudio;
        [Header("Multiple Errors")]
        public string badStartAndCough;
        public AudioClip badStartAndCoughAudio;
        [Header("Multiple Errors")]
        public string badEndAndCough;
        public AudioClip badEndAndCoughAudio;
        [Header("Multiple Errors")]
        public string badStartAndBadEndAndCough;
        public AudioClip badStartAndBadEndAndCoughAudio;
        [Header("Session check: Restart metaphor")]
        public string restartMetaphor;
        public AudioClip restartMetaphorAudio;
        [Header("Session check: Succes criteria reached")]
        public string endSessionSucces;
        public AudioClip endSessionSuccesAudio;
        [Header("Session check: Too many tests")]
        public string endSessionLimit;
        public AudioClip endSessionLimitAudio;
    }
}