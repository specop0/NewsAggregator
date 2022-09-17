using System;

namespace NewsAggregator;

[AttributeUsage(AttributeTargets.Method)]
public class OperationIdAttribute : Attribute
{
    public OperationIdAttribute(string operationId)
    {
        this.Id = operationId;
    }

    public string Id { get; }
}