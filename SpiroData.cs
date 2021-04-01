namespace Spirometry.Statics
{
    /// <summary>
    /// Spiro-Play project by University of Twente and MST
    /// Made by Koen Vogel - k.a.vogel@student.utwente.nl
    /// 
    /// SpiroData class:
    /// Just a data container to be used in stead of vector2 or vector3, contains basic spirometry data
    /// </summary>

    [System.Serializable]
    public class SpiroData
    {
        public System.DateTime Timestamp { get; }
        //volume of air, in Liters
        public float Volume { get; }
        //airflow in Liters per second
        public float Flow { get; }

        public SpiroData(System.DateTime timestamp, float volume, float flow)
        {
            if (timestamp != null)
                Timestamp = timestamp;

            if (volume != 999)
                Volume = volume;

            if (flow != 999)
                Flow = flow;
        }
    }
}
