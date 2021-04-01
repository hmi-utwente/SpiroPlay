using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spirometry.Statics
{

    /// <summary>
    /// --Recognition Algorithm--
    /// Written by: Vivianne de With
    /// a.j.v.dewith@student.utwente.nl
    /// Mattienne van der Kamp
    /// m.vanderkamp@mst.nl
    ///
    /// Converted to C# by: Koen Vogel
    /// k.a.vogel@student.utwente.nl
    ///
    /// Written for the analysis of spirometry curves of school - aged children.
    /// Version 1.0
    ///
    /// Construction Date : April - December, 2019
    /// Conversion Date : May - December, 2019
    /// </summary>
    
    public class Validation
    {
        #region variables
        
        private const int FallOffLength = 10;

        //functions
        public List<SpiroData> FullFunction { get; private set; }
        public List<SpiroData> Inspiration { get; private set; }
        public List<SpiroData> Expiration { get; private set; }

        //parameters
        public float FVC { get; private set; }
        public float FIVC { get; private set; }
        public float FEV05 { get; private set; }
        public float FEV1 { get; private set; }
        public float TiffeneauIndex { get; private set; }
        public float NewTimeZero { get; private set; }
        public float FET { get; private set; }
        
        public float FIT { get; private set; }
        public float PEF { get; private set; }
        public float FEF2575 { get; private set; }
        public float EV { get; private set; }

        //Error Criteria
        public bool Crit1A { get; private set; }
        public bool Crit1B { get; private set; }
        public bool Crit2A { get; private set; }
        public bool Crit2B { get; private set; }
        public bool Crit2C { get; private set; }
        public bool Crit3 { get; private set; }
        public bool Crit4 { get; private set; }
        #endregion

        //combined errors
        public bool UnsatisfactoryStart { get; private set; }
        public bool PrematureEnd { get; private set; }
        public bool CoughDetected { get; private set; }
        public bool NoErrors { get; private set; }

        //Constructor, taking in a flow-timestamp function
        public Validation(List<SpiroData> rawFunction)
        {
            try
            {
                //functions
                FullFunction = GetVolume(rawFunction);
                Inspiration = Math.GetInspiration(rawFunction, 0);
                Expiration = Math.GetExpiration(rawFunction);
                Inspiration = GetVolume(Inspiration);
                Expiration = GetVolume(Expiration);

                //parameters
                FIVC = GetFVC(Inspiration);
                FVC = GetFVC(Expiration);
                PEF = GetPEF(Expiration);
                FEF2575 = GetFEF2575(Expiration, FVC);
                NewTimeZero = GetNewTimeZero(Expiration);
                FEV05 = GetFEV(Expiration, 0.5f + NewTimeZero);
                FEV1 = GetFEV(Expiration, 1f + NewTimeZero);
                FET = GetFET(Expiration, NewTimeZero);
                EV = GetEV(Expiration, NewTimeZero);
                TiffeneauIndex = FEV1 / FVC;

                //validating errors
                Crit1A = GetCriteria1A(FVC, EV);
                Crit1B = GetCriteria1B(Expiration);
                Crit2A = GetCriteria2A(Expiration);
                Crit2B = GetCriteria2B(FET);
                Crit2C = GetCriteria2C(Expiration);
                Crit3 = GetCriteria3(Expiration);
                Crit4 = GetCriteria4(rawFunction, Expiration);

                //combine criteria into errors
                UnsatisfactoryStart = !Crit1A || !Crit1B;
                PrematureEnd = !Crit2A || !Crit2B || !Crit2C;
                CoughDetected = !Crit3;

                //check if there are no errors at all
                NoErrors = !UnsatisfactoryStart && !CoughDetected && !PrematureEnd;
            }
            catch (Exception e)
            {
                Debug.Log("Error recognition failed, reason: " + e);
            }
            Debug.Log("Validation compiled, Errors recognized: " + "StartOfExpir-" + UnsatisfactoryStart + "  CoughDetected-" + CoughDetected + "  PrematureEnd-" + PrematureEnd);
        }

        #region parameters

        //calculate volume for a float - timestamp function
        private List<SpiroData> GetVolume(List<SpiroData> data)
        {
            float dt = 0.01f;
            var output = new List<SpiroData>();
            output.Add(new SpiroData(data[0].Timestamp, 0, data[0].Flow));

            for (var i = 1; i < data.Count; i++)
            {
                //dt = (float)((data[i].Timestamp - data[i - 1].Timestamp).TotalMilliseconds / 1000f);
                output.Add(new SpiroData(data[i].Timestamp, output[i - 1].Volume + data[i].Flow * dt, data[i].Flow));
            }

            return output;
        }

        //Forced vital capacity (FVC) is the volume of air that can forcibly be blown out after full inspiration,[10] measured in liters
        private static float GetFVC(List<SpiroData> expiration)
        {
            //find the highest x value (amount of liters)
            float highest = -99;
            foreach (var point in expiration)
            {
                //var absVol = Mathf.Abs(point.Volume);
                if (Mathf.Abs(point.Volume) > highest) highest = Mathf.Abs(point.Volume);
            }
            highest = expiration[expiration.Count - 1].Volume;
            return Mathf.Abs(highest);
        }

        //FEV1 is the volume of air that can forcibly be blown out in first 1 second, after full inspiration
        private static float GetFEV(List<SpiroData> expiration, float timestamp)
        {
            try
            {
                //calculate the timestamp you want the volume of
                var timestampIndex = Mathf.RoundToInt(timestamp * 100);
                
                //return volume of that timestamp
                if (timestampIndex <= expiration.Count - 1)
                {
                    var timestampVolume = expiration[timestampIndex].Volume;
                    return timestampVolume;
                }
                Debug.LogError("Validation failed to calculate FEV" + timestamp);
                return -999;
            }
            catch (Exception e)
            {
                Debug.LogError("Validation failed to calculate FEV" + timestamp + ", " + e.Message);
                return -999;
            }
        }

        //Forced Expiratory Time (FET) measures the length of the expiration in seconds.
        private static float GetFET(List<SpiroData> expiration, float newTimeZero)
        {
            try
            {
                //with a sample rate of 100, duration can be determined from amount of samples
                var duration =  expiration.Count / 100f;
                return duration - newTimeZero;        //subtract newtimezero for accuracy
            }
            catch (Exception e)
            {
                Debug.LogError("Validation failed to calculate FET, " + e.Message);
                return -999;
            }
        }

        //highest flow value
        private static float GetPEF(List<SpiroData> expiration)
        {
            try
            {
                float highest = -99;
                for (var i = 0; i < expiration.Count; i++)
                    if (expiration[i].Flow > highest)
                        highest = expiration[i].Flow;
                return highest;
            }
            catch (Exception e)
            {
                Debug.LogError("Validation failed to calculate PEF, " + e.Message);
                return -999;
            }
        }

        //average flow values of every point from .25 volume to .75 volume
        private static float GetFEF2575(List<SpiroData> expiration, float fvc)
        {
            try
            {
                var vol25 = 0.25f * fvc;
                var vol75 = 0.75f * fvc;

                int index25 = 0, index75 = 0;
                for (var i = 0; i < expiration.Count; i++)
                {
                    if (i + 1 >= expiration.Count) break;
                    if (vol25 > expiration[i].Volume && vol25 < expiration[i + 1].Volume)
                        index25 = i;
                    if (vol75 > expiration[i].Volume && vol75 < expiration[i + 1].Volume)
                        index75 = i;
                }

                //averaging all flow values
                var sum = 0f;
                float amountOfPoints = index75 - index25;
                for (var i = index25; i < index75; i++) sum += expiration[i].Flow;

                var avrg = sum / amountOfPoints;
                return avrg;
            }
            catch (Exception e)
            {
                Debug.Log("Validation failed to calculate FEF2575, " + e.Message);
                return -999;
            }
        }

        //New Time Zero parameter, means the intersection of the steepest slope in the curve, with the X-axis
        private static float GetNewTimeZero(List<SpiroData> expiration)
        {
            //with a range of 80ms, move along the function and make a line between the two points
            //return highest slopeValue
            try
            {
                //setup range of 80ms
                var rangeInSamples = 8;
                var slopes = new List<Slopestruct>();

                for (var i = 0; i < expiration.Count; i++)
                {
                    int fromIndex = i;
                    int toIndex = i + rangeInSamples;

                    //if out of range, break of loop
                    if (toIndex > expiration.Count - 1) break;

                    //find volumes
                    float fromVolume = expiration[fromIndex].Volume;
                    float toVolume = expiration[toIndex].Volume;

                    //determine volume time differences
                    var dv = toVolume - fromVolume;
                    var dt = (float) (rangeInSamples / 100f);

                    //construct slopeValue
                    var slopeValue = dv / dt;
                    var slope = new Slopestruct(slopeValue, fromIndex, toIndex, fromVolume, toVolume);
                    slopes.Add(slope);
                }

                /*
                //logging
                var text = "first 7 slopes: ";
                text += slopes[0].slopeValue + "   " + slopes[1].slopeValue + "   " + slopes[2].slopeValue + "   " +
                        slopes[3].slopeValue +
                        "   " + slopes[4].slopeValue + "   " + slopes[5].slopeValue + "   " + slopes[6].slopeValue +
                        "   " +
                        slopes[7].slopeValue + "   " + slopes[8].slopeValue + "   " + slopes[9].slopeValue;
                text += slopes[10].slopeValue + "   " + slopes[11].slopeValue + "   " + slopes[12].slopeValue + "   " +
                        slopes[13].slopeValue + "   " + slopes[14].slopeValue + "   " + slopes[15].slopeValue + "   " +
                        slopes[16].slopeValue + "   " + slopes[17].slopeValue;
                Debug.Log(text);
                */

                //go through all slopes to find highest
                var highestSlopeValue = -999f;
                var highestSlope = default(Slopestruct);
                foreach (var slope in slopes)
                    if (slope.slopeValue > highestSlopeValue)
                    {
                        highestSlopeValue = slope.slopeValue;
                        highestSlope = slope;
                    }

                if (highestSlope == default(Slopestruct)) return -999;
                //Debug.Log("Highest slopeValue: " + highestSlope.slopeValue + " with start-index-volume of " + highestSlope.startIndexVolume + " and start-index-time of " + highestSlope.startIndexTime);

                //Find intersection of slopeValue line with x axis
                //determine new time zero
                var j = highestSlope.slopeValue * (highestSlope.startIndexTime / 100f);
                float newTimeZero = (j - highestSlope.startIndexVolume) / highestSlope.slopeValue;
                //Debug.Log("New time zero: " + newTimeZero);
                return newTimeZero;
            }
            catch (Exception e)
            {
                Debug.Log("Failed to calculate New Time Zero, reason: " + e);
                return -999;
            }
        }
        
        private static float GetEV(List<SpiroData> expiration, float newTimeZero)
        {
            try
            {
                //calculate list of x-y values for time and volume (interpolated)
                const int interpolationFactor = 100;
                List<Vector2> interpExpir = new List<Vector2>();
                interpExpir = Math.Interpolate(Math.GetVolumeTime(expiration), interpolationFactor);
                
                //find the closest time value in function to newTimeZero and return volume of that point
                var minTimeDiff = 999f;
                var volumeAtMinTimeDiff = 999f;
                for (var i = 0; i < interpExpir.Count-1; i++)
                {
                    var diff = Mathf.Abs(newTimeZero - interpExpir[i].x);
                    if (!(diff < minTimeDiff)) continue;
                    minTimeDiff = diff;
                    volumeAtMinTimeDiff = interpExpir[i].y;
                }

                //if found, return
                if (minTimeDiff == 999f || volumeAtMinTimeDiff == 999f)
                    return -999;
                return volumeAtMinTimeDiff;
            }
            catch (Exception e)
            {
                Debug.Log("Validation failed to calculate EV, " + e.Message);
                return -999;
            }
        }
        #endregion

        #region Error Criteria

        private static bool GetCriteria1A(float FVC, float EV)
        {
            //Criteria 1a: (START OF TEST) EV must be <5% of the FVC or <150ml, whichever is greater
            //Miller, M.R., Hankinson, J.A.T.S., Brusasco, V., Burgos, F., Casaburi, R., Coates, A., ... &Jensen, R. (2005).Standardisation of spirometry. European respiratory journal, 26(2), 319 - 338.
            //Loeb, J.S., Blower, W.C., Feldstein, J.F., Koch, B.A., Munlin, A.L., &Hardie, W.D. (2008).Acceptability and repeatability of spirometry in children using updated ATS/ ERS criteria.Pediatric pulmonology, 43(10), 1020 - 1024.

            if (FVC == -999 || EV == -999)
                return true;
            
            try
            {
                bool output;
                if (FVC * 0.05 < 0.150)
                {
                    output = EV < 150;
                }
                else
                {
                    output = EV / 1000 < 0.05 * FVC;
                }

                return output;
            } catch (Exception e)
            {
                Debug.Log("Validation failed to calculate Criteria 1A, " + e.Message);
                return true;
            }
        }

        //detect if the first peak is also the maximum peak
        private static bool GetCriteria1B(List<SpiroData> expiration)
        {
            try
            {
                //convert
                var flowOverVolume = Math.GetFlowTime(expiration);

                //calculate all peaks in function
                var peaks = Math.FindPeaks(expiration);

                //if there is only one peak, criteria is irrelevant
                if (peaks.Count <= 1) return true;

                //determine maximum peak (PEF)
                var maxPeak = 0f;
                for (var j = 0; j < peaks.Count; j++)
                    if (peaks[j].y > maxPeak)
                        maxPeak = peaks[j].y;

                //if the maximum peak is the first, return correct
                if (maxPeak >= peaks[0].y) return true;
                
                //if the peak is not the first or second, return error
                if (maxPeak < peaks[0].y || maxPeak < peaks[1].y) return false;

                //a second peak is allowed, but only if it lies within a 10 sample (0.1 seconds) range of the PEF and within 95% of the PEF's value
                var diffFlow = Mathf.Abs(peaks[1].y - peaks[0].y);
                var diffTime = Mathf.Abs(peaks[1].x - peaks[0].x);
                var maxDiffFlow = 0.05f * maxPeak;
                const float maxDiffTime = 0.1f;
                return diffFlow < maxDiffFlow && diffTime < maxDiffTime;

            } catch (Exception e)
            {
                Debug.Log("Validation failed to calculate Criteria 1B, " + e.Message);
                return true;
            }
        }

        //if the volume difference for the last second of expiration is high enough, return error
        private static bool GetCriteria2A(List<SpiroData> expiration)
        {
            //actually should be 0.025 but expiration filtering does not allow for it
            const float maximumDiff = 0.025f;
            try
            {
                var endVolume = expiration[expiration.Count - 1].Volume;
                var endTime = expiration[expiration.Count - 1].Timestamp;
                var stamp = endTime - new TimeSpan(0, 0, 0, 1);
                float oneSecondBeforeEndVolume = 0;
                for (var i = 0; i < expiration.Count; i++)
                {
                    if (i + 1 >= expiration.Count) break;
                    if (expiration[i].Timestamp == stamp)
                        oneSecondBeforeEndVolume = expiration[i].Volume;
                    if (oneSecondBeforeEndVolume != 0) break;
                    if (expiration[i].Timestamp < stamp && expiration[i + 1].Timestamp > stamp)
                        oneSecondBeforeEndVolume = Math.Average(expiration[i].Volume, expiration[i + 1].Volume);
                }

                if (oneSecondBeforeEndVolume == 0)
                {
                    Debug.LogError("No timestamp was found");
                    return true;
                }

                if (Mathf.Abs(endVolume - oneSecondBeforeEndVolume) < maximumDiff)
                    return true;
                else
                    return false;
            } catch (Exception e)
            {
                Debug.LogError("Validation failed to calculate Criteria 2A, " + e.Message);
                return true;
            }
        }

        private static bool GetCriteria2B(float FET)
        {
            //threshold value, the minimum duration the expiration should last, according to Graham
            const float minFet = 15f;
            if (FET == -999)
                return true;
            
            //Criteria 3a: (END OF TEST) FET (forced expiratory time) is at least 3 seconds (should have been 3 according to Miller but is really hard!)
            bool output;
            output = FET > minFet;
            return output;
        }

        private static bool GetCriteria2C(List<SpiroData> expiration)
        {
            //criteria 3c: premature ending / steep fall off at the end of the flow volume curve detection
            try
            {
                //setup output
                bool output;

                var flowOverVolume = Math.GetFlowVolume(expiration);

                //derive expiration
                var Derivative = Math.Derive(flowOverVolume);

                //isolate fall off area
                var falloff = new List<float>();
                for (var j = Derivative.Count - FallOffLength; j < Derivative.Count - 1; j++)
                    falloff.Add(Derivative[j].y);

                //average difference
                var Steepness = Math.Average(falloff);

                //detection
                output = !(Steepness < -0.2f);

                //return output
                return output;
            } catch (Exception e)
            {
                Debug.LogError("Validation failed to calculate Criteria 2C, " + e.Message);
                return true;
            }
        }

        //check if there are more than 1 peak/valley in the first second of expiration, otherwise, check steepness of slopes
        private static bool GetCriteria3(List<SpiroData> expiration)
        {
            try
            {
                var startTime = expiration[0].Timestamp;
                var stamp = startTime + new TimeSpan(0, 0, 0, 1);
                var indexAt1Sec = -99;

                for (var i = 0; i < expiration.Count; i++)
                {
                    if (i + 1 >= expiration.Count) break;
                    if (expiration[i].Timestamp == stamp) indexAt1Sec = i;
                    if (indexAt1Sec != -99) break;
                    if (expiration[i].Timestamp < stamp && expiration[i + 1].Timestamp > stamp) indexAt1Sec = i;
                }

                //calculate all slopes
                var slopes = new List<float>();
                for (var i = 0; i < indexAt1Sec; i++)
                {
                    if (i >= indexAt1Sec) break;
                    var df = expiration[i].Flow - expiration[i + 1].Flow;
                    var dv = expiration[i].Volume - expiration[i + 1].Volume;
                    var slope = df / dv;
                    slopes.Add(slope);
                }

                //check if there are more than one switches in slopes
                var amountOfSwitches = 0;
                for (var index = 0; index < slopes.Count; index++)
                {
                    if (index + 1 >= slopes.Count) break;
                    if (slopes[index] < 0 && slopes[index + 1] > 0 || slopes[index] > 0 && slopes[index + 1] < 0)
                        amountOfSwitches++;
                }

                if (amountOfSwitches <= 1)
                    return true;
                //if there are more than 1 peaks found, calculate steepness of slopes as to determine significance
                
                //find index of PEF
                var highest = -999f;
                var highestIndex = 0;
                for (var i = 0; i < expiration.Count-1; i++)
                {
                    if (expiration[i].Flow > highest)
                    {
                        highest = expiration[i].Flow;
                        highestIndex = i;
                    }
                }

                //go through all slopes starting from PEF, if found one with high enough slopeValue value, return error
                var maxSlope = 2.5f;
                for (var index = highestIndex; index < slopes.Count; index++)
                {
                    var slope = slopes[index];
                    if (slope < 0) continue;
                    if (slope > maxSlope) return false;
                }
                return true;
            } catch (Exception e)
            {
                Debug.LogError("Validation failed to calculate Criteria 3A, " + e.Message);
                return true;
            }
        }

        private static bool GetCriteria4(List<SpiroData> rawFunction, List<SpiroData> expiration)
        {
            //the time between inspiration and expiration; end of inspiration at flow = -0.1; start of expiration at flow = 0.1
            //gap has to be smaller than 2sec = 200 samples

            //calculate values
            var cleanInspiration = Math.GetInspiration(rawFunction, 0.1f);
            var endOfInspir = cleanInspiration.Last().Timestamp;
            var startOfExpir = expiration[0].Timestamp;
            var diff = startOfExpir - endOfInspir;
            
            //determine if difference is good enough
            const int durationThreshold = 2;
            var threshold = new TimeSpan(0, 0, durationThreshold);
            return diff < threshold;
        }

        #endregion

        [System.Serializable]
        private class Slopestruct
        {
            public float slopeValue;
            public float startIndexTime;
            public float endIndexTime;
            public float startIndexVolume;
            public float endIndexVolume;

            public Slopestruct(float slopeValue, float startIndexTime, float endIndexTime, float startIndexVolume, float endIndexVolume)
            {
                this.slopeValue = slopeValue;
                this.startIndexTime = startIndexTime;
                this.endIndexTime = endIndexTime;
                this.startIndexVolume = startIndexVolume;
                this.endIndexVolume = endIndexVolume;
            }
        }
    }
}