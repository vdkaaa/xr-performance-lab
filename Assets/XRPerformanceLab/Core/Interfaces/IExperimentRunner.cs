namespace XRPerformanceLab.Core.Interfaces
{
    public interface IExperimentRunner
    {
        void Activate(string experimentId);
        void Deactivate(string experimentId);
        void DeactivateAll();
    }
}