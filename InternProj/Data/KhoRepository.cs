using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using InternProj.Models;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class KhoRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KhoRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public KhoRepository(ILogger<KhoRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the tbl_Kho table if it does not exist.
        /// </summary>
        private async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            System.Diagnostics.Debug.WriteLine(Constants.DatabasePath);
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS tbl_DM_Kho  (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Ten_Kho TEXT UNIQUE NOT NULL,
                Ghi_Chu TEXT
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tbl_DM_Kho table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="Kho"/> objects.</returns>
        public async Task<List<Kho>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_Kho";
            var Kho = new List<Kho>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Kho.Add(new Kho
                {
                    Id = reader.GetInt32(0),
                    Ten_Kho = reader.GetString(1),
                    Ghi_Chu = reader.GetString(2)
                });
            }

            return Kho;
        }

        /// <summary>
        /// Retrieves a specific Kho by its name.
        /// </summary>
        /// <param name="Ten_Kho">The ID of the Kho.</param>
        /// <returns>A <see cref="Kho"/> object if found; otherwise, null.</returns>
        public async Task<Kho?> GetAsync(int ID)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_Kho WHERE ID = @Id";
            selectCmd.Parameters.AddWithValue("@Id", ID);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Kho
                {
                    Id = reader.GetInt32(0),
                    Ten_Kho = reader.GetString(1),
                    Ghi_Chu = reader.GetString(2)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a Kho to the database. If the Kho hasn't existed in the database, a new Kho is created; otherwise, the existing Kho is updated.
        /// </summary>
        /// <param name="item">The Kho to save.</param>
        /// <returns>The ID of the saved Kho.</returns>
        public async Task SaveItemAsync(Kho item, bool isEdit)
        {
            await Init();

            if (string.IsNullOrWhiteSpace(item.Ten_Kho))
                throw new Exception("Measurement name cannot be left blank!");

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (!isEdit)
            {
                saveCmd.CommandText = @"
                    INSERT INTO tbl_DM_Kho (Ten_Kho, Ghi_Chu)
                    VALUES (@Ten, @GhiChu)";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE tbl_DM_Kho 
                    SET Ten_Kho = @Ten, Ghi_Chu = @GhiChu
                    WHERE ID = @Id";
                saveCmd.Parameters.AddWithValue("@Id", item.Id);
                System.Diagnostics.Debug.WriteLine($"Updating Kho with ID: {item.Id}");
            }
            saveCmd.Parameters.AddWithValue("@Ten", item.Ten_Kho);
            saveCmd.Parameters.AddWithValue("@GhiChu", item.Ghi_Chu ?? string.Empty);

            try
            {
                await saveCmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Constraint Violation
            {
                throw new Exception($"Measurement unit '{item.Ten_Kho}' already exist.");
            }
        }

        /// <summary>
        /// Deletes a Kho from the database.
        /// </summary>
        /// <param name="item">The Kho to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(Kho item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_Kho WHERE ID = @Id";
            deleteCmd.Parameters.AddWithValue("@Id", item.Id);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Drops the Kho table from the database.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropTableCmd = connection.CreateCommand();
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS tbl_DM_Kho";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}