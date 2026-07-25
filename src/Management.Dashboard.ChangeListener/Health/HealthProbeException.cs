namespace Management.Dashboard.ChangeListener.Health;

/// <summary>
/// Represents a dependency failure whose message is safe to include in the public health response.
/// </summary>
public sealed class HealthProbeException : Exception
{
    public HealthProbeException(string message)
        : base(message)
    {
    }
}
