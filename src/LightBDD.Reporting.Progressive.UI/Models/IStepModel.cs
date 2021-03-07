using System;
using System.Collections.Generic;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    public interface IStepModel
    {
        Guid Id { get; }
        Guid ParentId { get; }
        string GroupPrefix { get; }
        int Number { get; }
        IStepNameInfo Name { get; }
        IReadOnlyList<IStepModel> SubSteps { get; }
    }
}