namespace KustoFramework.Enums;

/// <summary>Specifies the join flavor used in a KQL <c>join</c> operator.</summary>
public enum JoinKind
{
    /// <summary>KQL: <c>kind=innerunique</c> — deduplicates the left side on the join key, then performs an inner join.</summary>
    InnerUnique,
    /// <summary>KQL: <c>kind=inner</c> — standard inner join.</summary>
    Inner,
    /// <summary>KQL: <c>kind=leftouter</c> — all rows from the left, matched rows from the right (null if no match).</summary>
    LeftOuter,
    /// <summary>KQL: <c>kind=rightouter</c> — all rows from the right, matched rows from the left (null if no match).</summary>
    RightOuter,
    /// <summary>KQL: <c>kind=fullouter</c> — all rows from both sides.</summary>
    FullOuter,
    /// <summary>KQL: <c>kind=leftsemi</c> — returns rows from the left that have a match on the right.</summary>
    LeftSemi,
    /// <summary>KQL: <c>kind=leftanti</c> — returns rows from the left that have no match on the right.</summary>
    LeftAnti,
    /// <summary>KQL: <c>kind=rightsemi</c> — returns rows from the right that have a match on the left.</summary>
    RightSemi,
    /// <summary>KQL: <c>kind=rightanti</c> — returns rows from the right that have no match on the left.</summary>
    RightAnti
}
