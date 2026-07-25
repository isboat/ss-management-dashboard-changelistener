namespace Management.Dashboard.ChangeListener.Health;

public interface IHealthProbe
{
    string Name { get; }

    Task CheckAsync(CancellationToken cancellationToken);
}
