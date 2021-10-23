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

    public class SpiroResultSimple
    {
        #region variables

        //general data
        public float fvc;
        public float fev1;
        public float pef;
        public string metaphor;
        public bool crit1a;
        public bool crit1b;
        public bool crit2a;
        public bool crit2b;
        public bool crit2c;
        public bool crit3;

        #endregion

        //constructor; initialize new spirometry test result
        public SpiroResultSimple(float fvc, float fev1, float pef, string metaphor, bool crit1a, bool crit1b, bool crit2a, bool crit2b, bool crit2c, bool crit3)
        {
            this.fvc = fvc;
            this.fev1 = fev1;
            this.pef = pef;
            this.metaphor = metaphor;
            this.crit1a = crit1a;
            this.crit1b = crit1b;
            this.crit2a = crit2a;
            this.crit2b = crit2b;
            this.crit2c = crit2c;
            this.crit3 = crit3;
        }

        public void Debug()
        {
            var output = "SimpleSpiroResult loaded with following parameters::: ";
            output += "fvc: " + fvc + " fev1: " + fev1 + " pef: " + pef + " metaphor: " + metaphor + " crit1a: " + crit1a + " crit1b: " + crit1b;
            output += " crit2a: " + crit2a + " crit2b: " + crit2b + " crit2c: " + crit2c + " crit3: " + crit3;
            UnityEngine.Debug.Log(output);
        }
    }
}
