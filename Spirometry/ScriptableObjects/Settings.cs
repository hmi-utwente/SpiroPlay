using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Spirometry.ScriptableObjects
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// Settings class:
    /// Settings file to store important configurations
    /// </summary>

    [CreateAssetMenu(fileName = "Settings", menuName = "Spiro-Play/Settings Object", order = 0)]
    public class Settings : ScriptableObject
    {
        [Header("Input Window Limits")]
        [Tooltip("This is the minimum number that the user is able to put in the textfield")]
        public float MinimumFVC;

        [Tooltip("This is the maximum number that the user is able to put in the textfield")]
        public float MaximumFVC;

        [Tooltip("This is the minimum number that the user is able to put in the textfield")]
        public float MinimumPEF;

        [Tooltip("This is the maximum number that the user is able to put in the textfield")]
        public float MaximumPEF;

        [Header("Default values")] public float DefaultPeakFlow;

        [Tooltip("If lung volume cannot be calculated from previous tests or personal data, fall back to this value")]
        public float DefaultPredictedFVC;

        [Tooltip("If PEF cannot be calculated from previous tests or personal data, fall back to this value")]
        public float DefaultPredictedPEF;
        [FormerlySerializedAs("defaultCoinAmount")] [Tooltip("This is the amount of coins new users start with")]
        public int startingCoinAmount = 10;
        [Tooltip("This is the amount of coins users win by completing a session")]
        public int coinWinAmountSuccessfull = 3;
        [Tooltip("This is the amount of coins users win by completing a session")]
        public int coinWinAmountUnsuccessfull = 2;
        [Tooltip("For every metaphor (12 as of writing), indicate whether it should be unlocked by default")]
        //public bool[] defaultUnlockStatus;
        public MetaphorMetadata[] MetaphorInfo;
        [Tooltip("How many session users are able to complete per day while still earning coins")]
        public int sessionLimitPerDay;

        [Header("Error Detection")]
        [Tooltip("Maximum age to be deemed young enough for adjusted maxFVC values")]
        public float ageThreshold;
        
        [Header("Realtime detection config")]
        [Tooltip("Multiply goal volume with this factor to increase motivation for users")]
        public float GoalModifier = 1f;

        public float DelayAfterEndDetected = 3f;
        public float StartOfTestFlowThreshold;
        public float SwitchToExpirationFlowThreshold;
        public float EndOfTestFlowThreshold;
        public FeedbackConfig Feedback;

        [System.Serializable]
        public struct MetaphorMetadata
        {
            public int index;
            public string id;
            public string displayName;
            public bool unlockedPerDefault;
        }

        public string GetDisplayName(string metaphorID)
        {
            foreach (var metaphor in MetaphorInfo)
            {
                if (metaphor.id == metaphorID) return metaphor.displayName;
            }
            return metaphorID;
        }
    }
}
