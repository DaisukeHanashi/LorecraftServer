using Npgsql;

namespace Lorecraft_API.StaticFactory
{
    public class SqlConnectionFactory(string connectionString)
    {
        private readonly string _connectionString = connectionString; 
        
        public NpgsqlConnection Create() => new(_connectionString); 
    }
}