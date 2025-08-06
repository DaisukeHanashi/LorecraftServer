using Dapper;
using Npgsql;
using Lorecraft_API.StaticFactory;
using Lorecraft_API.Resources;
using Lorecraft_API.Manager;

namespace Lorecraft_API.Data.Repository
{
    public abstract class BaseRepository(SqlConnectionFactory factory, ILoggerFactory loggerFactory)
    {
        private readonly SqlConnectionFactory _factory = factory;
        protected readonly ILogger _logger = loggerFactory.CreateLogger<BaseRepository>();
        protected const bool Plural = true; 
        protected static bool Single => !Plural; 

        protected async Task<IEnumerable<T>> GetAllAsync<T>(string sql) where T : class
        {
            using var connection = _factory.Create();

            return await connection.QueryAsync<T>(sql);
        }

        protected async Task<IEnumerable<T>> GetSomeAsync<T>(string sql, object request) where T : class
        {
            using var connection = _factory.Create();
            
            Console.WriteLine($"Using SQL Script: {sql}");


            return await connection.QueryAsync<T>(sql, request);
        }

        protected async Task<IEnumerable<TResult1>> GetJointListAsync<TResult1, TSub1> (string sql, object request, string splitParameters
        , Func<TResult1, TSub1, TResult1> map) 
        where TResult1 : class
        where TSub1 : class
        {
            using var connection = _factory.Create();

            Console.WriteLine($"Using SQL Script: {sql}");

            return await connection.QueryAsync(sql, map, request, splitOn : splitParameters);
        }
        protected async Task<IEnumerable<TResult1>> GetJointListAsync<TResult1, TSub1, TSub2> (string sql, object request, string splitParameters
        , Func<TResult1, TSub1, TSub2, TResult1> map) 
        where TResult1 : class
        where TSub1 : class
        where TSub2 : class
        {
            using var connection = _factory.Create(); 

            return await connection.QueryAsync(sql, map, request, splitOn : splitParameters);
        }

        protected async Task<T?> GetAsync<T>(string sql, object request) where T : class
        {
            using var connection = _factory.Create();

            Console.WriteLine($"Using SQL Script: {sql}");

            return await connection.QuerySingleOrDefaultAsync<T>(sql, request);
        }

        protected async Task<string?> GetStringAsync(string sql, object val)
        {
            using var connection = _factory.Create();

            Console.WriteLine($"Using SQL Script : {sql}");

            return await connection.QueryFirstOrDefaultAsync<string>(sql, val);
        }

        protected async Task ExecuteAsync<T>(string sql, T request) where T : class
        {
            int retries = 0;

            using var connection = _factory.Create();
            connection.Open();

            Console.WriteLine($"Using SQL Script : {sql}");

            using var transaction = connection.BeginTransaction();
            while (true)
            {
                if (retries >= Constants.MaxRetries)
                    throw new SystemException(Constants.TooManyRetries);
                try
                {
                    await connection.ExecuteAsync(sql, request);
                    await transaction.CommitAsync();
                    break;
                }
                catch (NpgsqlException ex) when (ex.InnerException is PostgresException { ErrorCode: Constants.DuplicateKey })
                {
                    await transaction.RollbackAsync();
                    request.ResetID();
                    retries++;
                    continue;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Failed to insert a data in the table!");
                }
                finally
                {
                    connection.Close();
                }
            }

        }

        protected string CreateMessageFromExecuteResult(Exception ex, string normalMessage,
        string genericOne = "Too many retries! ID Generation issue")
        => ex.InnerException is SystemException && ex.Message.Equals(Constants.TooManyRetries)
        ? genericOne : normalMessage;

    }
}