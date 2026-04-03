using KustoFramework.Extensions;

namespace KustoFramework.Tests;

public class ExtendTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Extend_SingleColumn()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { DoubleDamage = e.DamageProperty * 2 })
            .ToKql();

        Assert.Equal("StormEvents\n| extend DoubleDamage = DamageProperty * 2", kql);
    }

    [Fact]
    public void Extend_MultipleColumns()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { TotalCasualties = e.DeathsDirect + e.InjuriesDirect, HasDamage = e.DamageProperty > 0 })
            .ToKql();

        Assert.Equal("StormEvents\n| extend TotalCasualties = DeathsDirect + InjuriesDirect, HasDamage = DamageProperty > 0", kql);
    }
}
