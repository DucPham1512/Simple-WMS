using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using InternProj.Models;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class NhaCungCapRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NhaCungCapRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public NhaCungCapRepository(ILogger<NhaCungCapRepository> logger)
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
            CREATE TABLE IF NOT EXISTS tbl_DM_NCC  (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Ma_NCC STRING NOT NULL UNIQUE,
                Ten_NCC TEXT NOT NULL UNIQUE,
                Ghi_Chu TEXT
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tbl_DM_NCC");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="NhaCungCap"/> objects.</returns>
        public async Task<List<NhaCungCap>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_NCC";
            var NhaCungCap = new List<NhaCungCap>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                NhaCungCap.Add(new NhaCungCap
                {
                    Id = reader.GetInt32(0),
                    Ma_Ncc = reader.GetString(1),
                    Ten_Ncc = reader.GetString(2),
                    Ghi_Chu = reader.GetString(3)
                }
                    );
            }

            return NhaCungCap;
        }

        /// <summary>
        /// Retrieves a specific DonViTinh by its name.
        /// </summary>
        /// <param name="Ma_Ncc">The ID of the DonViTinh.</param>
        /// <returns>A <see cref="NhaCungCap"/> object if found; otherwise, null.</returns>
        public async Task<NhaCungCap?> GetAsync(int Id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_NCC WHERE ID = @Id";
            selectCmd.Parameters.AddWithValue("@Id", Id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new NhaCungCap
                {
                    Id = reader.GetInt32(0),
                    Ma_Ncc = reader.GetString(1),
                    Ten_Ncc = reader.GetString(2),
                    Ghi_Chu = reader.GetString(3)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a DonViTinh to the database. If the DonViTinh hasn't existed in the database, a new DonViTinh is created; otherwise, the existing DonViTinh is updated.
        /// </summary>
        /// <param name="item">The DonViTinh to save.</param>
        /// <returns>The ID of the saved DonViTinh.</returns>
        public async Task SaveItemAsync(NhaCungCap item, bool isEdit)
        {
            await Init();


            string? error = item switch
            {
                null => "Dữ liệu nhà cung cấp không hợp lệ.",

                { Ma_Ncc: var s } when string.IsNullOrWhiteSpace(s)
                    => "Mã nhà cung cấp không được để trống.",

                { Ten_Ncc: var s } when string.IsNullOrWhiteSpace(s)
                    => "Tên nhà cung cấp không được để trống.",

                _ => null
            };

            if(error is not null)
                throw new Exception(error);

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (!isEdit)
            {
                saveCmd.CommandText = @"
                    INSERT INTO tbl_DM_NCC (Ma_NCC, Ten_NCC, Ghi_Chu)
                    VALUES (@Ma, @Ten, @GhiChu)";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE tbl_DM_NCC 
                    SET Ma_NCC = @Ma, Ten_NCC = @Ten, Ghi_Chu = @GhiChu
                    WHERE @Id = ID";
                saveCmd.Parameters.AddWithValue("@Id", item.Id);
            }
            saveCmd.Parameters.AddWithValue("@Ma", item.Ma_Ncc);
            saveCmd.Parameters.AddWithValue("@Ten", item.Ten_Ncc);
            saveCmd.Parameters.AddWithValue("@GhiChu", item.Ghi_Chu ?? string.Empty);

            try
            {
                await saveCmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Constraint Violation
            {
                // Log the full unexpected error
                _logger.LogError(ex, "Unexpected error saving NhaCungCap");

                // Pass the exact message up to the UI so you can read it immediately
                throw new Exception($"Lỗi hệ thống: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a DonViTinh from the database.
        /// </summary>
        /// <param name="item">The DonViTinh to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(NhaCungCap item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_NCC WHERE ID = @Id";
            deleteCmd.Parameters.AddWithValue("@Id", item.Id);

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
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS tbl_DM_NCC";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}