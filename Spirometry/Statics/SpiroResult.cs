using System;
using System.Collections.Generic;
using Spirometry.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;

namespace Spirometry.Statics
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// SpiroResult class:
    /// Data container for a complete spirometry test including relevant parameters, raw data, user info, timestamp and more metadata
    /// </summary>

    public class SpiroResult
    {
        #region variables

        //general data
        public UserStorage.User User { get; private set; }
        public System.DateTime TimeOfTest { get; }
        public int IndexInSession { get; }
        public string Metaphor { get; }
        public float PredictedFVC { get; }
        public float PredictedPEF { get; }
        public string FeedbackText { get; }
        public int SessionID { get; }
        public AudioClip FeedbackAudio { get; }

        //errors
        public Validation Validation { get; }

        #endregion

        //constructor; initialize new spirometry test result
        public SpiroResult(List<SpiroData> rawFunction, UserStorage.User user, string metaphor, int indexInSession, Settings settings)
        {
            //general stuff
            User = user;
            Metaphor = metaphor;
            PredictedFVC = user.predictedFvc;
            PredictedPEF = user.predictedPef;
            SessionID = user.currentSessionID;
            TimeOfTest = System.DateTime.Now;
            IndexInSession = indexInSession;

            //calculate errors
            Validation = new Validation(rawFunction);
            FeedbackText = GenerateFeedbackText(Validation, settings.Feedback);
            FeedbackAudio = GenerateFeedbackAudio(Validation, settings.Feedback);
        }

        private static string GenerateFeedbackText(Validation validation, FeedbackConfig config)
        {
            //if there are no errors
            if (validation.NoErrors) return config.noErrors;

            //generate feedback for every error
            if (validation.UnsatisfactoryStart && !validation.PrematureEnd && !validation.CoughDetected)
                return config.badStartOfExpiration;
            if (!validation.UnsatisfactoryStart && !validation.PrematureEnd && validation.CoughDetected)
                return config.coughDetected;
            if (!validation.UnsatisfactoryStart && validation.PrematureEnd && !validation.CoughDetected)
                return config.badEndOfExpiration;
            if (validation.UnsatisfactoryStart && validation.PrematureEnd && !validation.CoughDetected)
                return config.badStartAndBadEnd;
            if (validation.UnsatisfactoryStart && !validation.PrematureEnd && validation.CoughDetected)
                return config.badStartAndCough;
            if (!validation.UnsatisfactoryStart && validation.PrematureEnd && validation.CoughDetected)
                return config.badEndAndCough;
            if (validation.UnsatisfactoryStart && validation.PrematureEnd && validation.CoughDetected)
                return config.badStartAndBadEndAndCough;

            //fallback
            return config.noErrors;
        }
        
        private static AudioClip GenerateFeedbackAudio(Validation validation, FeedbackConfig config)
        {
            //if there are no errors
            if (validation.NoErrors) return config.noErrorsAudio;

            //generate feedback for every error
            if (validation.UnsatisfactoryStart && !validation.PrematureEnd && !validation.CoughDetected)
                return config.badStartOfExpirationAudio;
            if (!validation.UnsatisfactoryStart && !validation.PrematureEnd && validation.CoughDetected)
                return config.coughDetectedAudio;
            if (!validation.UnsatisfactoryStart && validation.PrematureEnd && !validation.CoughDetected)
                return config.badEndOfExpirationAudio;
            if (validation.UnsatisfactoryStart && validation.PrematureEnd && !validation.CoughDetected)
                return config.badStartAndBadEndAudio;
            if (validation.UnsatisfactoryStart && !validation.PrematureEnd && validation.CoughDetected)
                return config.badStartAndCoughAudio;
            if (!validation.UnsatisfactoryStart && validation.PrematureEnd && validation.CoughDetected)
                return config.badEndAndCoughAudio;
            if (validation.UnsatisfactoryStart && validation.PrematureEnd && validation.CoughDetected)
                return config.badStartAndBadEndAndCoughAudio;

            //fallback
            return config.noErrorsAudio;
        }

        public SpiroResultSimple SimpleVersion
        {
            get
            {
                var v = Validation;
                return new SpiroResultSimple(v.FVC, v.FEV1, v.PEF, Metaphor, v.Crit1A, v.Crit1B, v.Crit2A, v.Crit2B, v.Crit2C, v.Crit1A);
            }
        }
    }
}
