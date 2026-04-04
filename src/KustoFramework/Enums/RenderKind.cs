namespace KustoFramework.Enums;

/// <summary>Specifies the visualization type for the KQL <c>render</c> operator.</summary>
public enum RenderKind
{
    /// <summary>Renders results as a table.</summary>
    Table,
    /// <summary>Renders results as a bar chart.</summary>
    BarChart,
    /// <summary>Renders results as a column chart.</summary>
    ColumnChart,
    /// <summary>Renders results as a pie chart.</summary>
    PieChart,
    /// <summary>Renders results as a time chart (time series).</summary>
    TimeChart,
    /// <summary>Renders results as a line chart.</summary>
    LineChart,
    /// <summary>Renders results as an area chart.</summary>
    AreaChart,
    /// <summary>Renders results as a stacked area chart.</summary>
    StackedAreaChart,
    /// <summary>Renders results as a ladder diagram.</summary>
    Ladder,
    /// <summary>Renders results as a scatter chart.</summary>
    ScatterChart,
    /// <summary>Renders results as a tree map.</summary>
    TreeMap,
    /// <summary>Renders results as a card.</summary>
    Card
}
