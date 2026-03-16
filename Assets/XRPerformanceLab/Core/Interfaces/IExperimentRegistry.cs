using System.Collections.Generic;

namespace XRPerformanceLab.Core.Interfaces
{
    public interface IExperimentRegistry
    {
        IReadOnlyList<IExperiment> GetAll();
        IExperiment GetById(string id);
        void Register(IExperiment experiment);
    }
}