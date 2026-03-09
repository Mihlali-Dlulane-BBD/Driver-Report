using System.Data;

namespace Driver_Report.Core.Interface
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
