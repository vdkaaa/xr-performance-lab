namespace XRPerformanceLab.Core.Interfaces
{
    public interface IExperiment
    {
        string Id { get; }
        string DisplayName { get; }

        void Setup();
        void Run();
        void Teardown();
    }
}