using System.Collections.Generic;
using Spirometry.Statics;
using UnityEngine;

namespace Spirometry.Debugging
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// PlotBehaviour class:
    /// To be called in order for data to be plotted; should be attached to the plotter prefab
    /// </summary>

    public class PlotBehaviour : MonoBehaviour
    {

        #region variables

        [Header("References")]
        [Tooltip("This prefab will represent peaks in a plotted function")]
        public GameObject PointPrefab;
        private LineRenderer Line;
        private List<GameObject> Points = new List<GameObject>();
        private Vector3 origin;

        private const float windowSizeX = 10f;
        private const float windowSizeY = 10f;

        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            //setup line renderer
            if (gameObject.GetComponentInChildren<LineRenderer>() != null)
                Line = gameObject.GetComponentInChildren<LineRenderer>();
            else
                Debug.LogError("Line Renderer could not be found in plotter prefab");

            //setup variables
            //PlotElements = transform.GetComponentsInChildren<Transform>();
            origin = new Vector3(Line.transform.position.x, Line.transform.position.y, Line.transform.position.z);

            //reset line renderer
            ClearPlot();
        }

        public void ClearPlot()
        {
            //remove all points and lines
            for (int i = 0; i < Points.Count; i++)
                Destroy(Points[i]);
            Line.positionCount = 0;
        }

        public void NewPlot(List<Vector2> values, bool IncludePeaks)
        {
            //remove previous points
            for (int i = 0; i < Points.Count; i++)
                Destroy(Points[i]);

            //convert List<Vector2> to seperate x and y values
            List<float> Xvalues = new List<float>();
            List<float> Yvalues = new List<float>();
            for (int i = 0; i < values.Count; i++)
            {
                Xvalues.Add(values[i].x);
                Yvalues.Add(values[i].y);
            }

            DisplayPlot(Xvalues, Yvalues, IncludePeaks);
        }

        public void NewPlot(SpiroResult result, bool IncludePeaks)
        {
            List<SpiroData> values = result.Validation.FullFunction;

            //remove previous points
            for (int i = 0; i < Points.Count; i++)
                Destroy(Points[i]);

            //convert List<Vector2> to seperate x and y values
            List<float> Xvalues = new List<float>();
            List<float> Yvalues = new List<float>();
            for (int i = 0; i < values.Count; i++)
            {
                Xvalues.Add(values[i].Volume);
                Yvalues.Add(values[i].Flow);
            }

            DisplayPlot(Xvalues, Yvalues, IncludePeaks);
        }

        public void NewPlot(List<SpiroData> function, bool IncludePeaks)
        {

            //remove previous points
            for (int i = 0; i < Points.Count; i++)
                Destroy(Points[i]);

            //convert List<Vector2> to seperate x and y values
            List<float> Xvalues = new List<float>();
            List<float> Yvalues = new List<float>();
            for (int i = 0; i < function.Count; i++)
            {
                Xvalues.Add(function[i].Volume);
                Yvalues.Add(function[i].Flow);
            }

            DisplayPlot(Xvalues, Yvalues, IncludePeaks);
        }
        public void NewPoints(List<Vector2> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                Points.Add(GameObject.Instantiate(PointPrefab, new Vector3(origin.x + values[i].x * 37.5f, origin.y + values[i].y * 37.5f, origin.z), Quaternion.identity, Line.transform));
                Points[Points.Count - 1].transform.localPosition = new Vector3(values[i].x, values[i].y, 0);
                Points[Points.Count - 1].transform.localScale = Points[Points.Count - 1].transform.localScale / 40;
            }
        }

        public void NewPoints(Vector2[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Points.Add(GameObject.Instantiate(PointPrefab, new Vector3(origin.x + values[i].x * 37.5f, origin.y + values[i].y * 37.5f, origin.z), Quaternion.identity, Line.transform));
                Points[Points.Count - 1].transform.localPosition = new Vector3(values[i].x, values[i].y, 0);
                Points[Points.Count - 1].transform.localScale = Points[Points.Count - 1].transform.localScale / 40;
            }
        }

        private void DisplayPlot(List<float> Xvalues, List<float> Yvalues, bool IncludePeaks)
        {
            //make sure there the amount of x and y values is equal
            if (Xvalues.Count < Yvalues.Count)
            {
                Debug.LogError("Not enough X values to plot graph, cutting values off...");
                Yvalues.RemoveRange(Xvalues.Count-1, Yvalues.Count-1);
            } else if (Yvalues.Count < Xvalues.Count)
            {
                Debug.LogError("Not enough Y values to plot graph, cutting values off...");
                Yvalues.RemoveRange(Yvalues.Count-1, Xvalues.Count-1);
            }

            //calculate minima and maxima in data
            float lowestX = 999, highestX = -999, lowestY = 999, highestY = -999;
            foreach (var x in Xvalues)
            {
                if (x > highestX)
                    highestX = x;
                if (x < lowestX)
                    lowestX = x;
            }
            foreach (var y in Yvalues)
            {
                if (y > highestY)
                    highestY = y;
                if (y < lowestY)
                    lowestY = y;
            }
            
            //remap values to window size
            List<float> mappedXvalues = new List<float>();
            for (int i = 0; i < Xvalues.Count-1; i++)
            {
                mappedXvalues.Add(Xvalues[i].Remap(lowestX, highestX, 0, windowSizeX));
            } 
            List<float> mappedYvalues = new List<float>();
            for (int i = 0; i < Yvalues.Count-1; i++)
            {
                mappedYvalues.Add(Yvalues[i].Remap(lowestY, highestY, 0, windowSizeY));
            } 
            
            //set amount of vertices in the line
            Line.positionCount = Xvalues.Count;
            //actually modifying line
            for (int i = 0; i < mappedXvalues.Count; i++)
                    Line.SetPosition(i, new Vector3(mappedXvalues[i], mappedYvalues[i], 0));
            
            //visualize red points for every positive peak
            if (IncludePeaks)
            {
                //formate vector2
                List<Vector2> function = new List<Vector2>();
                for (int i = 0; i < mappedXvalues.Count-1; i++)
                {
                    function[i] = new Vector2(mappedXvalues[i], mappedYvalues[i]);
                }
                
                //find the peaks
                List<Vector2> peaks = Math.FindPeaks(function);
                List<Vector2> positivePeaks = new List<Vector2>();
                for (int i = 0; i < peaks.Count; i++)
                    if (peaks[i].y > 0)
                        positivePeaks.Add(peaks[i]);
                
                //instantiate points at peak positions
                NewPoints(positivePeaks);
            }
        }
    }
}
