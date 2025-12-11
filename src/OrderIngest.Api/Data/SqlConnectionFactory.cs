using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OrderIngest.Api.Data
{
    public interface IDbConnectionFactory 
    { 
        SqlConnection CreateConnection(); 
    }

    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;
        public SqlConnectionFactory(IConfiguration config) 
        { 
            _config = config; 
        }

        public SqlConnection CreateConnection()
        {
            var cs = _config.GetConnectionString("OrdersDb");
            return new SqlConnection(cs);
        }
    }
}
