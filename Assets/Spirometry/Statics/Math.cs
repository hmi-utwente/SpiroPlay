using System.Collections.Generic;
using UnityEngine;

namespace Spirometry.Statics
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// Math class:
    /// Basic and intermediate math functions c# is lacking by default, for use by other classes
    /// </summary>

    public static class Math
    {

        public static List<Vector2> Derive(List<Vector2> values)
        {
            List<Vector2> Diff = new List<Vector2>();
            for (int i = 0; i < values.Count - 1; i++)
                Diff.Add(new Vector2(values[i].x, values[i + 1].y - values[i].y));
            return Diff;
        }

        public static List<Vector2> FindPeaks(List<Vector2> values)
        {
            //returns all peaks in a function, with their index number
            List<Vector2> output = new List<Vector2>();

            List<Vector2> Derivative = Derive(values);
            for (int i = 1; i < Derivative.Count; i++)
            {
                if (Derivative[i].y < 0 && Derivative[i - 1].y > 0)
                    output.Add(new Vector2(values[i].x, values[i].y));
                if (Derivative[i].y == 0)
                {
                    //if derivative is 0, only check for peaks not valleys, and check for peaks that are longer than 1 flat data point
                    if (i < Derivative.Count-1)
                    {
                        if (i >= 3)
                            if (Derivative[i - 3].y > 0 && Derivative[i - 2].y == 0 && Derivative[i - 1].y == 0 && Derivative[i].y == 0 && Derivative[i + 1].y < 0)
                                output.Add(new Vector2(values[i].x, values[i].y));
                        if (i >= 2)
                            if (Derivative[i - 2].y > 0 && Derivative[i - 1].y == 0 && Derivative[i].y == 0 && Derivative[i + 1].y < 0)
                                output.Add(new Vector2(values[i].x, values[i].y));
                        if (i >= 1)
                            if (Derivative[i - 1].y > 0 && Derivative[i].y == 0 && Derivative[i + 1].y < 0)
                                output.Add(new Vector2(values[i].x, values[i].y));
                    }
                }
            }
            return output;
        }

        public static List<Vector2> FindPeaks(List<SpiroData> Inputvalues)
        {
            //returns all peaks in a function, with their index number
            List<Vector2> output = new List<Vector2>();

            //convert spirodata to vector2
            List<Vector2> values = new List<Vector2>();
            for (int i = 0; i < Inputvalues.Count; i++)
                values.Add(new Vector2(Inputvalues[i].Volume, Inputvalues[i].Flow));

            List<Vector2> Derivative = Derive(values);
            for (int i = 1; i < Derivative.Count; i++)
            {
                if (Derivative[i].y < 0 && Derivative[i - 1].y > 0)
                    output.Add(new Vector2(values[i].x, values[i].y));
                if (Derivative[i].y == 0)
                {
                    //if derivative is 0, only check for peaks not valleys, and check for peaks that are longer than 1 flat data point
                    if (i < Derivative.Count - 1)
                    {
                        if (i >= 3)
                            if (Derivative[i - 3].y > 0 && Derivative[i - 2].y == 0 && Derivative[i - 1].y == 0 && Derivative[i].y == 0 && Derivative[i + 1].y < 0)
                                output.Add(new Vector2(values[i].x, values[i].y));
                        if (i >= 2)
                            if (Derivative[i - 2].y > 0 && Derivative[i - 1].y == 0 && Derivative[i].y == 0 && Derivative[i + 1].y < 0)
                                output.Add(new Vector2(values[i].x, values[i].y));
                        if (i >= 1)
                            if (Derivative[i - 1].y > 0 && Derivative[i].y == 0 && Derivative[i + 1].y < 0)
                                output.Add(new Vector2(values[i].x, values[i].y));
                    }
                }
            }
            return output;
        }

        #region averaging functions
        public static float Average(float f1, float f2)
        {
            float output = f1 + f2;
            output = output / 2;
            return output;
        }
        public static float Average(float[] values)
        {
            float output = 0;
            for (int i = 0; i < values.Length; i++)
                output += values[i];
            output = output / values.Length;
            return output;
        }

        public static float Average(int[] values)
        {
            float output = 0;
            for (int i = 0; i < values.Length; i++)
                output += values[i];
            output = output / values.Length;
            return output;
        }

        public static float Average(List<float> values)
        {
            float output = 0;
            for (int i = 0; i < values.Count; i++)
                output += values[i];
            output = output / values.Count;
            return output;
        }
        #endregion

        #region Inhalation & Expiration filtering

        //filter out expiration from a raw function - SPIRODATA instead of Vector2
        public static List<SpiroData> GetInspiration(List<SpiroData> rawFunction, float flowThreshold)
        {
            //setup
            var output = new List<SpiroData>();
            if (rawFunction.Count == 0)
            {
                Debug.LogWarning("Function does not contain data");
                return output;
            }

            //find lowest flow value with its index in the function
            float lowest = 999;
            int lowestIndex = 0;
            for (var index = 0; index < rawFunction.Count-1; index++)
            {
                var point = rawFunction[index].Flow;
                if (!(point < lowest)) continue;
                lowest = point;
                lowestIndex = index;
            }

            //every point from start/positives till the index of the lowest flow value is inspiration
            for (var index = 0; index < lowestIndex; index++)
            {
                if (rawFunction[index].Flow > 0)
                    continue;
                output.Add(rawFunction[index]);
            }

            //every points from lowest point until positive flow value
            for (var index = lowestIndex; index < rawFunction.Count-1; index++)
            {
                if (rawFunction[index].Flow > flowThreshold)
                    break;
                output.Add(rawFunction[index]);
            }
            return output;
        }

        //filter out inhalation from a complete raw function - SPIRODATA instead of vector2
        public static List<SpiroData> GetExpiration(List<SpiroData> rawFunction)
        {
            //for second peak filtering
            const int minimumTimeDiffBetweenPeaks = 3;
            const float minimumFlowDiffBetweenPeaks = 0.5f;
            
            //setup
            var output = new List<SpiroData>();
            if (rawFunction.Count == 0)
            {
                Debug.LogWarning("Function does not contain data");
                return output;
            }

            //convert to vector2
            var data = GetFlowIndex(rawFunction);

            //find highest flow value in curve
            float highestFlow = -999;
            int highestIndex = 0;
            for (int index = 0; index < rawFunction.Count-1; index++)
            {
                if (rawFunction[index].Flow > highestFlow)
                {
                    highestFlow = rawFunction[index].Flow;
                    highestIndex = index;
                }
            }

            //find second highest flow value in curve
            float secondHighestFlow = -999f;
            int secondHighestIndex = 0;
            for (int index = 0; index < rawFunction.Count-1; index++)
            {
                //peaks have to be at least a certain time apart
                if (Mathf.Abs(highestIndex - index) < minimumTimeDiffBetweenPeaks * 100) continue;
                if (rawFunction[index].Flow > secondHighestFlow)
                {
                    secondHighestFlow = rawFunction[index].Flow;
                    secondHighestIndex = index;
                }
            }
            
            //if identical peaks are detected, choose first one
            var actualPeakIndex = 0;
            Debug.Log("Difference in index between two highest peaks is " + Mathf.Abs(highestIndex - secondHighestIndex));
            Debug.Log("Difference in flow between two highest peaks is " + Mathf.Abs(highestFlow - secondHighestFlow));
            if (Mathf.Abs(highestFlow - secondHighestFlow) > minimumFlowDiffBetweenPeaks)
                actualPeakIndex = highestIndex;
            else
            {
                actualPeakIndex = highestIndex < secondHighestIndex ? highestIndex : secondHighestIndex;
            }
            Debug.Log("Actual peak has a flow of " + rawFunction[actualPeakIndex].Flow + " and index of " + actualPeakIndex);
            
            //find index of the point intersecting with the x axis
            var intersectIndex = 0;
            for (int i = actualPeakIndex - 1; i >= 0; i--)
            {
                if (rawFunction[i].Flow >= 0.1) continue;
                intersectIndex = i;
                break;
            }

            //every value from intersection to peak
            for (int index = intersectIndex; index < actualPeakIndex; index++)
            {
                output.Add(rawFunction[index]);
            }
            
            //every value from peak to next intersection
            for (var index = actualPeakIndex; index < rawFunction.Count-1; index++)
            {
                output.Add(rawFunction[index]);
                if (rawFunction[index].Flow < 0.1)
                    break;
            }
            return output;
        }
        #endregion

        public static List<Vector2> GetVolumeTime(List<SpiroData> function)
        {
            List<Vector2> output = new List<Vector2>();
            for (var index = 0; index < function.Count; index++)
            {
                var point = function[index];
                var time = (float) (point.Timestamp - function[0].Timestamp).TotalMilliseconds / 1000;
                time = (float)index / 100;
                output.Add(new Vector2(time, point.Volume));
            }
            return output;
        }

        public static List<Vector2> GetFlowIndex(List<SpiroData> function)
        {
            List<Vector2> output = new List<Vector2>();
            output.Clear();
            for (int i = 0; i < function.Count-1; i++)
            {
                output.Add(new Vector2(i, function[i].Flow));
            }
            return output;
        }
        
        public static List<Vector2> GetFlowTime(List<SpiroData> function)
        {
            List<Vector2> output = new List<Vector2>();
            output.Clear();
            foreach (var point in function)
            {
                float time = (float)(point.Timestamp - function[0].Timestamp).TotalMilliseconds / 1000;
                output.Add(new Vector2(time, point.Flow));
            }
            return output;
        }
        
        public static List<Vector2> GetFlowVolume(List<SpiroData> function)
        {
            List<Vector2> output = new List<Vector2>();
            output.Clear();
            foreach (var point in function)
            {
                output.Add(new Vector2(point.Volume, point.Flow));
            }
            return output;
        }
        
        public static List<Vector2> Interpolate(List<Vector2> function, int resolution)
        {
            List<Vector2> output = new List<Vector2>();
            
            for (int i = 0; i < function.Count-1; i++)
            {
                if (i + 1 > function.Count) break;
                float time = function[i].x;
                float volume = function[i].y;
                float timeNext = function[i + 1].x;
                float volumeNext = function[i + 1].y;
                float timeDiff = timeNext - time;
                float volumeDiff = volumeNext - volume;
                float newTimeDiff = timeDiff / resolution;
                float newVolumeDiff = volumeDiff / resolution;

                float newTime = time, newVolume = volume;
                for (int j = 0; j < resolution; j++)
                {
                    output.Add(new Vector2(newTime, newVolume));
                    newTime += newTimeDiff;
                    newVolume += newVolumeDiff;
                }
            }

            return output;
        }

        public static int FindHighestYindex(List<Vector2> values)
        {
            var highest = -999;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].y > highest)
                    highest = i;
            }
            return highest;
        }
        
        public static int FindHighestXindex(List<Vector2> values)
        {
            var highest = -999;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].x > highest)
                    highest = i;
            }
            return highest;
        }
        
        public static float FindHighestYvalue(List<Vector2> values)
        {
            var highest = -999f;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].y > highest)
                    highest = values[i].y;
            }
            return highest;
        }
        
        public static float FindHighestXvalue(List<Vector2> values)
        {
            var highest = -999f;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].x > highest)
                    highest = values[i].x;
            }
            return highest;
        }
    }
}
