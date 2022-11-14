using Dapper;
using KA.Entities.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KA.Repositories
{
    public interface IBaseRepository
    {
        /// <summary>
        /// Get ConnectionString From Config File
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConnectionString(IConfiguration configuration, string key);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string spName, DynamicParameters parameters, string connectionString = "");
        
        /// <summary>
        /// Execute a query, returning the data typed as T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T GetSingleResult<T>(string spName, DynamicParameters parameters, string connectionString = "");

        /// <summary>
        /// Execute a query, returning the data typed as T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> GetResults<T>(string spName, DynamicParameters parameters, string connectionString = "");

        /// <summary>
        /// 프로시저 이름, 파라미터 정보를 받아 처리하여 DataSet으로 결과 리턴
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string spName, SqlParameter[] parameters = null, string connectionString = "");
    }

    public class BaseRepository : IBaseRepository
    {
        protected string _kauctionConnectionString;
        protected string _logConnectionString;

        public string GetConnectionString(IConfiguration configuration, string key)
        {
            return DESCryptoHelper.DESDecrypt(configuration.GetSection("ConnectionStrings").GetSection(key).Value);
        }

        public object ExecuteScalar(string spName, DynamicParameters parameters, string connectionString = "")
        {
            connectionString = string.IsNullOrWhiteSpace(connectionString) ? _kauctionConnectionString : connectionString;

            using IDbConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed) connection.Open();

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            return connection.ExecuteScalar<object>(spName, parameters, commandType: CommandType.StoredProcedure);
        }

        public T GetSingleResult<T>(string spName, DynamicParameters parameters, string connectionString = "")
        {
            connectionString = string.IsNullOrWhiteSpace(connectionString) ? _kauctionConnectionString : connectionString;

            using IDbConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed) connection.Open();

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            return connection.Query<T>(spName, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public IEnumerable<T> GetResults<T>(string spName, DynamicParameters parameters, string connectionString = "")
        {
            connectionString = string.IsNullOrWhiteSpace(connectionString) ? _kauctionConnectionString : connectionString;

            using IDbConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Closed) connection.Open();

            DefaultTypeMap.MatchNamesWithUnderscores = true;

            return connection.Query<T>(spName, parameters, commandType: CommandType.StoredProcedure);
        }

        public DataSet GetDataSet(string spName, SqlParameter[] parameters = null, string connectionString = "")
        {
            using SqlConnection connection = new SqlConnection(string.IsNullOrWhiteSpace(connectionString) ? _kauctionConnectionString : connectionString);
            using SqlCommand command = new SqlCommand(spName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 120;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();

            using DataSet dataSet = new DataSet();
            using SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataSet);

            return dataSet.Tables.Count > 0 ? dataSet : null;
        }

        public int ExecuteQuery(string query)
        {
            using IDbConnection connection = new SqlConnection(_kauctionConnectionString);
            return connection.Execute(query);
        }
    }
}
