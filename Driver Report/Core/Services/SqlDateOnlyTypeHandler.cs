using Dapper;
using System.Data;

namespace Driver_Report.Core.Services
{
    public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly date)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = date.ToDateTime(TimeOnly.MinValue);
        }

        public override DateOnly Parse(object value)
        {
            return DateOnly.FromDateTime((DateTime)value);
        }
    }
}
