using System.Text;

namespace KustoFramework.Query;

/// <summary>
/// Fluent builder for constructing a KQL <c>scan</c> operator body.
/// Use with <see cref="Extensions.KqlQueryExtensions.Scan{T}"/> to build complex scan operations.
/// </summary>
/// <example>
/// <code>
/// query.Scan(b => b
///     .WithMatchId("sessionId")
///     .Declare("InSession:bool = false, SessionStart:datetime")
///     .WithStep("start", "EventType == 'Login'", "InSession = true, SessionStart = Timestamp")
///     .WithStep("end", "EventType == 'Logout' and InSession", "InSession = false"));
/// </code>
/// </example>
public class KqlScanBuilder
{
    private string? _matchId;
    private string? _declaration;
    private readonly List<(string Name, string Predicate, string? Output)> _steps = [];

    /// <summary>Adds a <c>with_match_id</c> option to the scan operator.</summary>
    /// <param name="columnName">The column name used to track match identifiers.</param>
    /// <returns>This builder instance for chaining.</returns>
    public KqlScanBuilder WithMatchId(string columnName)
    {
        _matchId = columnName;
        return this;
    }

    /// <summary>Adds a <c>declare</c> block defining state columns and optional default values.</summary>
    /// <param name="declaration">The declaration string (e.g. <c>"InSession:bool = false, SessionStart:datetime"</c>).</param>
    /// <returns>This builder instance for chaining.</returns>
    public KqlScanBuilder Declare(string declaration)
    {
        _declaration = declaration;
        return this;
    }

    /// <summary>Adds a step with a predicate and optional output assignments.</summary>
    /// <param name="name">The step name.</param>
    /// <param name="predicate">The KQL predicate expression for this step.</param>
    /// <param name="output">Optional output assignments (e.g. <c>"InSession = true, SessionStart = Timestamp"</c>).</param>
    /// <returns>This builder instance for chaining.</returns>
    public KqlScanBuilder WithStep(string name, string predicate, string? output = null)
    {
        _steps.Add((name, predicate, output));
        return this;
    }

    internal string Build()
    {
        var sb = new StringBuilder();

        if (_matchId is not null)
            sb.Append($"with_match_id={_matchId} ");

        if (_declaration is not null)
            sb.Append($"declare ({_declaration}) ");

        sb.Append("with (");

        for (int i = 0; i < _steps.Count; i++)
        {
            var (name, predicate, output) = _steps[i];
            if (i > 0)
                sb.Append(", ");

            sb.Append($"step {name}: {predicate}");
            if (output is not null)
                sb.Append($" => {output};");
        }

        sb.Append(')');

        return sb.ToString();
    }
}
