using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using KustoFramework.Attributes;

namespace KustoFramework.Query;

public class KqlExpressionVisitor
{
    public string Translate(LambdaExpression expression) =>
        Visit(expression.Body);

    public string TranslateProjection(LambdaExpression expression)
    {
        var body = expression.Body;

        if (body is NewExpression newExpr)
            return TranslateNewExpression(newExpr);

        if (body is MemberInitExpression memberInit)
            return TranslateMemberInitExpression(memberInit);

        return Visit(body);
    }

    public string TranslateGroupBy(LambdaExpression expression)
    {
        var body = expression.Body;

        if (body is NewExpression newExpr)
            return TranslateGroupByNewExpression(newExpr);

        return GetColumnName(body);
    }

    public string TranslateMemberAccess(LambdaExpression expression) =>
        GetColumnName(expression.Body);

    private string TranslateNewExpression(NewExpression newExpr)
    {
        var parts = new List<string>();
        for (int i = 0; i < newExpr.Arguments.Count; i++)
        {
            var memberName = newExpr.Members![i].Name;
            var arg = newExpr.Arguments[i];

            var argStr = Visit(arg);
            var sourceName = GetSourceColumnName(arg);

            if (sourceName != null && sourceName == memberName)
                parts.Add(memberName);
            else
                parts.Add($"{memberName} = {argStr}");
        }
        return string.Join(", ", parts);
    }

    private string TranslateMemberInitExpression(MemberInitExpression memberInit)
    {
        var parts = new List<string>();
        foreach (var binding in memberInit.Bindings)
        {
            if (binding is MemberAssignment assignment)
            {
                var memberName = binding.Member.Name;
                var argStr = Visit(assignment.Expression);
                var sourceName = GetSourceColumnName(assignment.Expression);

                if (sourceName != null && sourceName == memberName)
                    parts.Add(memberName);
                else
                    parts.Add($"{memberName} = {argStr}");
            }
        }
        return string.Join(", ", parts);
    }

    private string TranslateGroupByNewExpression(NewExpression newExpr)
    {
        var parts = new List<string>();
        for (int i = 0; i < newExpr.Arguments.Count; i++)
        {
            var memberName = newExpr.Members![i].Name;
            var arg = newExpr.Arguments[i];
            var col = GetColumnName(arg);

            if (col == memberName)
                parts.Add(col);
            else
                parts.Add($"{memberName} = {col}");
        }
        return string.Join(", ", parts);
    }

    private string? GetSourceColumnName(Expression expr)
    {
        if (expr is MemberExpression member)
            return ResolveColumnName(member);

        return null;
    }

    private string Visit(Expression expression)
    {
        return expression switch
        {
            BinaryExpression binary => VisitBinary(binary),
            UnaryExpression unary => VisitUnary(unary),
            MemberExpression member => VisitMember(member),
            ConstantExpression constant => VisitConstant(constant),
            MethodCallExpression methodCall => VisitMethodCall(methodCall),
            ConditionalExpression conditional => VisitConditional(conditional),
            NewExpression newExpr => VisitNew(newExpr),
            NewArrayExpression newArray => VisitNewArray(newArray),
            _ => throw new NotSupportedException($"Expression type '{expression.NodeType}' is not supported.")
        };
    }

    private string VisitBinary(BinaryExpression binary)
    {
        // Handle string comparison to null
        if (IsNullComparison(binary, out var nullMember, out var isEquals))
        {
            var col = Visit(nullMember);
            return isEquals ? $"isnull({col})" : $"isnotnull({col})";
        }

        var left = Visit(binary.Left);
        var right = Visit(binary.Right);

        var op = binary.NodeType switch
        {
            ExpressionType.Equal => "==",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "and",
            ExpressionType.OrElse => "or",
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Modulo => "%",
            _ => throw new NotSupportedException($"Binary operator '{binary.NodeType}' is not supported.")
        };

        // Parenthesize logical sub-expressions
        if (binary.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse)
        {
            if (NeedsParentheses(binary.Left, binary.NodeType))
                left = $"({left})";
            if (NeedsParentheses(binary.Right, binary.NodeType))
                right = $"({right})";
        }

        return $"{left} {op} {right}";
    }

    private static bool NeedsParentheses(Expression inner, ExpressionType outerNodeType)
    {
        if (inner is BinaryExpression innerBinary)
        {
            // 'and' has higher precedence than 'or' in KQL, same as C#
            if (outerNodeType == ExpressionType.AndAlso && innerBinary.NodeType == ExpressionType.OrElse)
                return true;
        }
        return false;
    }

    private static bool IsNullComparison(BinaryExpression binary, out Expression member, out bool isEquals)
    {
        member = null!;
        isEquals = false;

        if (binary.NodeType is not (ExpressionType.Equal or ExpressionType.NotEqual))
            return false;

        if (IsNull(binary.Right))
        {
            member = binary.Left;
            isEquals = binary.NodeType == ExpressionType.Equal;
            return true;
        }
        if (IsNull(binary.Left))
        {
            member = binary.Right;
            isEquals = binary.NodeType == ExpressionType.Equal;
            return true;
        }

        return false;
    }

    private static bool IsNull(Expression expr) =>
        expr is ConstantExpression { Value: null };

    private string VisitUnary(UnaryExpression unary)
    {
        if (unary.NodeType == ExpressionType.Not)
        {
            var operand = Visit(unary.Operand);
            return $"not({operand})";
        }

        if (unary.NodeType == ExpressionType.Convert)
            return Visit(unary.Operand);

        throw new NotSupportedException($"Unary operator '{unary.NodeType}' is not supported.");
    }

    private string VisitMember(MemberExpression member)
    {
        // Handle closured variables (captured locals)
        if (IsClosureAccess(member))
        {
            var value = EvaluateExpression(member);
            return FormatValue(value);
        }

        return ResolveColumnName(member);
    }

    private string ResolveColumnName(MemberExpression member)
    {
        var prop = member.Member as PropertyInfo;
        if (prop != null)
        {
            var colAttr = prop.GetCustomAttribute<KqlColumnAttribute>();
            if (colAttr != null)
                return colAttr.Name;
        }
        return member.Member.Name;
    }

    private static bool IsClosureAccess(MemberExpression member)
    {
        var current = member.Expression;
        while (current is MemberExpression inner)
            current = inner.Expression;

        return current is ConstantExpression;
    }

    private string VisitConstant(ConstantExpression constant) =>
        FormatValue(constant.Value);

    private string VisitMethodCall(MethodCallExpression methodCall)
    {
        var method = methodCall.Method;
        var declaringType = method.DeclaringType;

        // Kql static methods (aggregations + time)
        if (declaringType == typeof(Functions.Kql))
            return TranslateKqlFunction(methodCall);

        // String extension methods (KqlHas etc.)
        if (declaringType == typeof(Functions.KqlStringExtensions))
            return TranslateKqlStringExtension(methodCall);

        // Standard string methods
        if (declaringType == typeof(string))
            return TranslateStringMethod(methodCall);

        // Enumerable.Contains for 'in' operator
        if (method.Name == "Contains" && declaringType != null &&
            (declaringType == typeof(Enumerable) ||
             declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(List<>)))
            return TranslateContains(methodCall);

        throw new NotSupportedException($"Method '{method.DeclaringType?.Name}.{method.Name}' is not supported.");
    }

    private string TranslateKqlFunction(MethodCallExpression methodCall)
    {
        var name = methodCall.Method.Name;

        return name switch
        {
            "Count" => "count()",
            "CountIf" => $"countif({Visit(methodCall.Arguments[0])})",
            "Sum" => $"sum({Visit(methodCall.Arguments[0])})",
            "SumIf" => $"sumif({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "Avg" => $"avg({Visit(methodCall.Arguments[0])})",
            "Min" => $"min({Visit(methodCall.Arguments[0])})",
            "Max" => $"max({Visit(methodCall.Arguments[0])})",
            "DCount" => $"dcount({Visit(methodCall.Arguments[0])})",
            "Percentile" => $"percentile({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "MakeList" => TranslateMakeListOrSet("make_list", methodCall),
            "MakeSet" => TranslateMakeListOrSet("make_set", methodCall),
            "MakeBag" => $"make_bag({Visit(methodCall.Arguments[0])})",
            "ArgMax" => $"arg_max({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "ArgMin" => $"arg_min({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "TakeAny" => $"take_any({Visit(methodCall.Arguments[0])})",
            "Stdev" => $"stdev({Visit(methodCall.Arguments[0])})",
            "Variance" => $"variance({Visit(methodCall.Arguments[0])})",
            "Percentiles" => TranslatePercentiles(methodCall),
            "Ago" => TranslateAgo(methodCall),
            "Now" => "now()",
            "Bin" => $"bin({Visit(methodCall.Arguments[0])}, {TranslateTimeSpan(EvaluateExpression(methodCall.Arguments[1]))})",
            "StartOfDay" => $"startofday({Visit(methodCall.Arguments[0])})",
            "StartOfMonth" => $"startofmonth({Visit(methodCall.Arguments[0])})",
            "StartOfWeek" => $"startofweek({Visit(methodCall.Arguments[0])})",
            "StartOfYear" => $"startofyear({Visit(methodCall.Arguments[0])})",
            "EndOfDay" => $"endofday({Visit(methodCall.Arguments[0])})",
            "EndOfMonth" => $"endofmonth({Visit(methodCall.Arguments[0])})",
            "EndOfWeek" => $"endofweek({Visit(methodCall.Arguments[0])})",
            "EndOfYear" => $"endofyear({Visit(methodCall.Arguments[0])})",
            "DatetimeDiff" => $"datetime_diff({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})",
            "DatetimeAdd" => $"datetime_add({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})",
            "DayOfWeek" => $"dayofweek({Visit(methodCall.Arguments[0])})",
            "FormatDatetime" => $"format_datetime({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "Between" => TranslateBetween(methodCall),
            "IsEmpty" => $"isempty({Visit(methodCall.Arguments[0])})",
            "IsNotEmpty" => $"isnotempty({Visit(methodCall.Arguments[0])})",
            "ToLong" => $"tolong({Visit(methodCall.Arguments[0])})",
            "ToInt" => $"toint({Visit(methodCall.Arguments[0])})",
            "ToDouble" => $"todouble({Visit(methodCall.Arguments[0])})",
            "ToReal" => $"toreal({Visit(methodCall.Arguments[0])})",
            "ToString" => $"tostring({Visit(methodCall.Arguments[0])})",
            "ToDateTime" => $"todatetime({Visit(methodCall.Arguments[0])})",
            "ToTimeSpan" => $"totimespan({Visit(methodCall.Arguments[0])})",
            "Strlen" => $"strlen({Visit(methodCall.Arguments[0])})",
            "Substring" => TranslateSubstring(methodCall),
            "Trim" => $"trim({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "ToUpper" => $"toupper({Visit(methodCall.Arguments[0])})",
            "ToLower" => $"tolower({Visit(methodCall.Arguments[0])})",
            "Strcat" => $"strcat({string.Join(", ", methodCall.Arguments.Select(a => Visit(a)))})",
            "Extract" => $"extract({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})",
            "Split" => $"split({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "ReplaceString" => $"replace_string({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})",
            "ReplaceRegex" => $"replace_regex({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})",
            "IndexOf" => $"indexof({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})",
            "ParseJson" => $"parse_json({Visit(methodCall.Arguments[0])})",
            "ArrayLength" => $"array_length({Visit(methodCall.Arguments[0])})",
            "Pack" => $"pack({string.Join(", ", methodCall.Arguments.Select(a => Visit(a)))})",
            "BagKeys" => $"bag_keys({Visit(methodCall.Arguments[0])})",
            "Iff" => $"iff({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})",
            "Coalesce" => $"coalesce({string.Join(", ", methodCall.Arguments.Select(a => Visit(a)))})",
            _ => throw new NotSupportedException($"Kql function '{name}' is not supported.")
        };
    }

    private string TranslateAgo(MethodCallExpression methodCall)
    {
        var value = EvaluateExpression(methodCall.Arguments[0]);
        if (value is TimeSpan ts)
            return $"ago({TranslateTimeSpan(ts)})";

        return $"ago({Visit(methodCall.Arguments[0])})";
    }

    private string TranslateBetween(MethodCallExpression methodCall)
    {
        var col = Visit(methodCall.Arguments[0]);
        var from = Visit(methodCall.Arguments[1]);
        var to = Visit(methodCall.Arguments[2]);
        return $"{col} between ({from} .. {to})";
    }

    private string TranslateSubstring(MethodCallExpression methodCall)
    {
        if (methodCall.Arguments.Count == 3)
            return $"substring({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])}, {Visit(methodCall.Arguments[2])})";
        return $"substring({Visit(methodCall.Arguments[0])}, {Visit(methodCall.Arguments[1])})";
    }

    private string TranslatePercentiles(MethodCallExpression methodCall)
    {
        var col = Visit(methodCall.Arguments[0]);
        var percentilesValue = EvaluateExpression(methodCall.Arguments[1]);
        if (percentilesValue is double[] pcts)
            return $"percentiles({col}, {string.Join(", ", pcts.Select(p => p.ToString(System.Globalization.CultureInfo.InvariantCulture)))})";

        return $"percentiles({col}, {Visit(methodCall.Arguments[1])})";
    }

    private string TranslateMakeListOrSet(string funcName, MethodCallExpression methodCall)
    {
        var col = Visit(methodCall.Arguments[0]);
        if (methodCall.Arguments.Count == 2)
            return $"{funcName}({col}, {Visit(methodCall.Arguments[1])})";
        return $"{funcName}({col})";
    }

    private string TranslateKqlStringExtension(MethodCallExpression methodCall)
    {
        var name = methodCall.Method.Name;
        var source = Visit(methodCall.Arguments[0]); // extension method, first arg is 'this'

        return name switch
        {
            "KqlHas" => $"{source} has {Visit(methodCall.Arguments[1])}",
            "KqlHasCs" => $"{source} has_cs {Visit(methodCall.Arguments[1])}",
            "KqlHasPrefix" => $"{source} hasprefix {Visit(methodCall.Arguments[1])}",
            "KqlHasPrefixCs" => $"{source} hasprefix_cs {Visit(methodCall.Arguments[1])}",
            "KqlHasSuffix" => $"{source} hassuffix {Visit(methodCall.Arguments[1])}",
            "KqlHasSuffixCs" => $"{source} hassuffix_cs {Visit(methodCall.Arguments[1])}",
            "KqlContains" => $"{source} contains {Visit(methodCall.Arguments[1])}",
            "KqlContainsCs" => $"{source} contains_cs {Visit(methodCall.Arguments[1])}",
            "KqlStartsWith" => $"{source} startswith {Visit(methodCall.Arguments[1])}",
            "KqlStartsWithCs" => $"{source} startswith_cs {Visit(methodCall.Arguments[1])}",
            "KqlEndsWith" => $"{source} endswith {Visit(methodCall.Arguments[1])}",
            "KqlEndsWithCs" => $"{source} endswith_cs {Visit(methodCall.Arguments[1])}",
            "KqlMatchesRegex" => $"{source} matches regex {Visit(methodCall.Arguments[1])}",
            "KqlIn" => TranslateIn(source, methodCall.Arguments[1]),
            "KqlNotIn" => TranslateNotIn(source, methodCall.Arguments[1]),
            "KqlNotHas" => $"{source} !has {Visit(methodCall.Arguments[1])}",
            "KqlNotContains" => $"{source} !contains {Visit(methodCall.Arguments[1])}",
            "KqlNotStartsWith" => $"{source} !startswith {Visit(methodCall.Arguments[1])}",
            "KqlNotEndsWith" => $"{source} !endswith {Visit(methodCall.Arguments[1])}",
            "KqlHasAny" => TranslateHasAnyAll("has_any", source, methodCall.Arguments[1]),
            "KqlHasAll" => TranslateHasAnyAll("has_all", source, methodCall.Arguments[1]),
            _ => throw new NotSupportedException($"KQL string extension '{name}' is not supported.")
        };
    }

    private string TranslateIn(string source, Expression valuesExpression)
    {
        var values = EvaluateExpression(valuesExpression);
        if (values is string[] arr)
            return $"{source} in ({string.Join(", ", arr.Select(v => $"\"{v}\""))})";

        return $"{source} in ({Visit(valuesExpression)})";
    }

    private string TranslateNotIn(string source, Expression valuesExpression)
    {
        var values = EvaluateExpression(valuesExpression);
        if (values is string[] arr)
            return $"{source} !in ({string.Join(", ", arr.Select(v => $"\"{v}\""))})";

        return $"{source} !in ({Visit(valuesExpression)})";
    }

    private string TranslateHasAnyAll(string funcName, string source, Expression valuesExpression)
    {
        var values = EvaluateExpression(valuesExpression);
        if (values is string[] arr)
            return $"{source} {funcName} ({string.Join(", ", arr.Select(v => $"\"{v}\""))})";

        return $"{source} {funcName} ({Visit(valuesExpression)})";
    }

    private string TranslateStringMethod(MethodCallExpression methodCall)
    {
        var name = methodCall.Method.Name;
        var obj = Visit(methodCall.Object!);
        var arg = methodCall.Arguments.Count > 0 ? Visit(methodCall.Arguments[0]) : null;

        return name switch
        {
            "Contains" => $"{obj} contains {arg}",
            "StartsWith" => $"{obj} startswith {arg}",
            "EndsWith" => $"{obj} endswith {arg}",
            "ToLower" or "ToLowerInvariant" => $"tolower({obj})",
            "ToUpper" or "ToUpperInvariant" => $"toupper({obj})",
            "Trim" => $"trim(\" \", {obj})",
            "IndexOf" => $"indexof({obj}, {arg})",
            _ => throw new NotSupportedException($"String method '{name}' is not supported.")
        };
    }

    private string TranslateContains(MethodCallExpression methodCall)
    {
        // Enumerable.Contains(source, value)  or  list.Contains(value)
        if (methodCall.Method.DeclaringType == typeof(Enumerable))
        {
            var source = methodCall.Arguments[0];
            var value = Visit(methodCall.Arguments[1]);
            var items = EvaluateExpression(source);
            return $"{value} in ({FormatCollection(items)})";
        }
        else
        {
            var source = methodCall.Object!;
            var value = Visit(methodCall.Arguments[0]);
            var items = EvaluateExpression(source);
            return $"{value} in ({FormatCollection(items)})";
        }
    }

    private string VisitConditional(ConditionalExpression conditional) =>
        $"iff({Visit(conditional.Test)}, {Visit(conditional.IfTrue)}, {Visit(conditional.IfFalse)})";

    private string VisitNew(NewExpression newExpr) =>
        TranslateNewExpression(newExpr);

    private string VisitNewArray(NewArrayExpression newArray) =>
        string.Join(", ", newArray.Expressions.Select(e => Visit(e)));

    private string GetColumnName(Expression expression)
    {
        if (expression is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            return GetColumnName(unary.Operand);

        if (expression is MemberExpression member)
            return ResolveColumnName(member);

        return Visit(expression);
    }

    private string FormatValue(object? value)
    {
        return value switch
        {
            null => "\"\"",
            string s => $"\"{EscapeString(s)}\"",
            bool b => b ? "true" : "false",
            DateTime dt => $"datetime({dt:yyyy-MM-dd HH:mm:ss})",
            DateTimeOffset dto => $"datetime({dto:yyyy-MM-dd HH:mm:ss})",
            TimeSpan ts => TranslateTimeSpan(ts),
            int or long or short or byte => value.ToString()!,
            double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
            decimal m => m.ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString()!
        };
    }

    private static string TranslateTimeSpan(object? value)
    {
        if (value is not TimeSpan ts)
            return value?.ToString() ?? "";

        if (ts.TotalDays >= 1 && ts.TotalDays == Math.Floor(ts.TotalDays))
            return $"{(int)ts.TotalDays}d";
        if (ts.TotalHours >= 1 && ts.TotalHours == Math.Floor(ts.TotalHours))
            return $"{(int)ts.TotalHours}h";
        if (ts.TotalMinutes >= 1 && ts.TotalMinutes == Math.Floor(ts.TotalMinutes))
            return $"{(int)ts.TotalMinutes}m";
        if (ts.TotalSeconds >= 1 && ts.TotalSeconds == Math.Floor(ts.TotalSeconds))
            return $"{(int)ts.TotalSeconds}s";

        return ts.ToString();
    }

    private static string EscapeString(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    private static object? EvaluateExpression(Expression expression)
    {
        try
        {
            var lambda = Expression.Lambda(expression);
            var compiled = lambda.Compile();
            return compiled.DynamicInvoke();
        }
        catch
        {
            return null;
        }
    }

    private string FormatCollection(object? collection)
    {
        if (collection is System.Collections.IEnumerable enumerable)
        {
            var items = new List<string>();
            foreach (var item in enumerable)
                items.Add(FormatValue(item));
            return string.Join(", ", items);
        }
        return "";
    }
}
