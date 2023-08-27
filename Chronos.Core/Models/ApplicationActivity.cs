namespace Chronos.Core.Models;

public class ApplicationActivity
{
    public ApplicationActivity(string processName, string applicationName, string title)
    {
        ProcessName = processName ?? throw new ArgumentNullException(nameof(processName));
        ApplicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    /// <summary>
    ///     Process name of the application.
    /// </summary>
    public string ProcessName { get; }

    /// <summary>
    ///     Name of the application based on the file description.
    /// </summary>
    public string ApplicationName { get; }

    /// <summary>
    ///     Application title taken from the window.
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     Time when application frame appeared in the foreground.
    /// </summary>
    public DateTime Start { get; } = DateTime.Now;

    /// <summary>
    ///     Time when application frame disappeared from the foreground.
    /// </summary>
    public DateTime? End { get; set; }

    public override int GetHashCode() => HashCode.Combine(ProcessName, ApplicationName, Start);

    public override bool Equals(object? obj)
        => obj is ApplicationActivity application &&
           application.ProcessName == ProcessName &&
           application.ApplicationName == ApplicationName &&
           application.Start == Start;
}
