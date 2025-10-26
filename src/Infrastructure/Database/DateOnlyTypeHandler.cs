using System.Data;
using Dapper;

namespace Infrastructure.Data;

/// <summary>
/// DateOnlyTypeHandler is a Dapper extension that handles reading and writing DateOnly values.
/// Ensures smooth mapping between .NET DateOnly and SQL DATE.
/// Needed because DateOnly is not natively supported by Dapper.
/// </summary>
internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value;
    }
}
