namespace XRPerformanceLab.Core.Models
{
    [System.Serializable]
    public struct PerformanceMetrics
    {
        public float Fps;
        public float FrameTimeMs;
        public int DrawCalls;
        public int Batches;
        public int Triangles;
        public int Vertices;
    }
}