namespace XRPerformanceLab.Core.Interfaces
{
    public interface IExperiment
    {
        string Id { get; }
        string DisplayName { get; }
        bool IsActive { get; }

        void Activate();
        void Deactivate();
    }
}