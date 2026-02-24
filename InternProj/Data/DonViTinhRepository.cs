using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using InternProj.Models;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class DonViTinhRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonViTinhRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public DonViTinhRepository(ILogger<DonViTinhRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the tbl_DonViTinh table if it does not exist.
        /// </summary>
        private async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS tbl_DM_DonViTinh  (
                Ten_Don_Vi_Tinh TEXT PRIMARY KEY,
                Ghi_Chu TEXT
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tbl_DM_DonViTinh table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="DonViTinh"/> objects.</returns>
        public async Task<List<DonViTinh>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_Don_Vi_Tinh";
            var DonViTinh = new List<DonViTinh>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                DonViTinh.Add(new DonViTinh
                {
                    Ten_Don_Vi_Tinh = reader.GetString(255),
                    Ghi_Chu = reader.GetString(255)
                });
            }

            return DonViTinh;
        }

        /// <summary>
        /// Retrieves a specific DonViTinh by its name.
        /// </summary>
        /// <param name="Ten_Don_Vi_Tinh">The ID of the DonViTinh.</param>
        /// <returns>A <see cref="DonViTinh"/> object if found; otherwise, null.</returns>
        public async Task<DonViTinh?> GetAsync(int Ten_Don_Vi_Tinh)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_Don_Vi_Tinh WHERE Ten_Don_Vi_Tinh = @ten_don_vi";
            selectCmd.Parameters.AddWithValue("@ten_don_vi", Ten_Don_Vi_Tinh);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DonViTinh
                {
                    Ten_Don_Vi_Tinh = reader.GetString(255),
                    Ghi_Chu = reader.GetString(255)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a DonViTinh to the database. If the DonViTinh hasn't existed in the database, a new DonViTinh is created; otherwise, the existing DonViTinh is updated.
        /// </summary>
        /// <param name="item">The DonViTinh to save.</param>
        /// <returns>The ID of the saved DonViTinh.</returns>
        public async Task SaveItemAsync(DonViTinh item, bool isEdit, string oldKey = "")
        {
            await Init();

            if (string.IsNullOrWhiteSpace(item.Ten_Don_Vi_Tinh))
                throw new Exception("Measurement name cannot be left blank!");

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (!isEdit)
            {
                saveCmd.CommandText = @"
                    INSERT INTO tbl_DM_DonViTinh (Ten_Don_Vi_Tinh, Ghi_Chu)
                    VALUES (@Ten, @GhiChu)";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE tbl_DM_DonViTinh 
                    SET Ten_Don_Vi_Tinh = @Ten, Ghi_Chu = @GhiChu
                    WHERE Ten_Don_Vi_Tinh = @OldKey";
                saveCmd.Parameters.AddWithValue("@OldKey", oldKey);
            }

            saveCmd.Parameters.AddWithValue("@Ten", item.Ten_Don_Vi_Tinh);
            saveCmd.Parameters.AddWithValue("@GhiChu", item.Ghi_Chu ?? string.Empty);

            try
            {
                await saveCmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Constraint Violation
            {
                throw new Exception($"Measurement unit '{item.Ten_Don_Vi_Tinh}' already exist.");
            }
        }

        /// <summary>
        /// Deletes a DonViTinh from the database.
        /// </summary>
        /// <param name="item">The DonViTinh to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(DonViTinh item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM DonViTinh WHERE Ten_Don_Vi_Tinh = @Ten_Don_Vi_Tinh";
            deleteCmd.Parameters.AddWithValue("@Ten_Don_Vi_Tinh", item.Ten_Don_Vi_Tinh);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Drops the DonViTinh table from the database.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropTableCmd = connection.CreateCommand();
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS DonViTinh";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}