using EmployeeManagerAPI.Infrastructure.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagerAPI.Helpers
{
    /// <summary>
    /// Provide the execution of queries in order to manage the data in a SQL Server.
    /// </summary>
    public class DataProvider : IDataProvider
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DataProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Execute async a stored procedure that returns a result.
        /// </summary>
        /// <param name="commandText">The name of the stored procedure.</param>
        /// <param name="parameters">Any parameters needed for the execution of the stored procedure.</param>
        /// <returns>Returns an IEnumerable of the given model, containg mapped data.</returns>
        public async Task<IEnumerable<T>> ExecuteReaderCommandAsync<T>(string commandText, object parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = commandText;
                    command.Parameters.AddRange(CreateParameters(parameters));

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var results = MapData<T>(reader);
                        return results.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Execute async a stored procedure that does not return any result
        /// </summary>
        /// <param name="commandText">The name of the stored procedure.</param>
        /// <param name="parameters">Any parameters needed for the execution of the stored procedure.</param>
        public async Task ExecuteNonQueryCommandAsync(string commandText, object parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = commandText;
                    command.Parameters.AddRange(CreateParameters(parameters));

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Create the parameters for the stored procedure.
        /// </summary>
        /// <param name="parameters">An object containg the procedure's parameters.</param>
        /// <returns>Returns an array of parameters.</returns>
        private static SqlParameter[] CreateParameters(object parameters)
        {
            var properties = parameters.GetType().GetProperties();
            var param = new List<SqlParameter>();

            foreach (var property in properties)
            {
                param.Add(new SqlParameter($"@{property.Name}", property.GetValue(parameters)));
            }

            return param.ToArray();
        }

        /// <summary>
        /// Map the data of the execution of a procedure to a model.
        /// Also maps nested models that represent relationships to the main model.
        /// </summary>
        /// <param name="reader">Reader used for the execution of the procedure.</param>
        /// <returns>Returns the given model with the mapped data.</returns>
        private static IEnumerable<T> MapData<T>(SqlDataReader reader)
        {
            var results = new List<T>();

            var columnNames = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToList();

            while (reader.Read())
            {
                var instance = Activator.CreateInstance<T>();
                var properties = instance.GetType().GetProperties();

                foreach (var property in properties)
                {
                    if (columnNames.Contains(property.Name) && reader[property.Name] != DBNull.Value)
                    {
                        property.SetValue(instance, reader[property.Name]);
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        var joinedInstance = Activator.CreateInstance(property.PropertyType);
                        var joinedProperties = property.PropertyType.GetProperties();

                        foreach (var joinedProperty in joinedProperties)
                        {
                            if (columnNames.Contains(joinedProperty.Name) && reader[joinedProperty.Name] != DBNull.Value)
                            {
                                joinedProperty.SetValue(joinedInstance, reader[joinedProperty.Name]);
                            }
                        }

                        property.SetValue(instance, joinedInstance);
                    }
                }

                results.Add(instance);
            }

            return results;
        }
    }
}
