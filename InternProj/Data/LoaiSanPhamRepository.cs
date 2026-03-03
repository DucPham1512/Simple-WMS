using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using InternProj.Models;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class LoaiSanPhamRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonViTinhRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LoaiSanPhamRepository(ILogger<LoaiSanPhamRepository> logger)
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
            CREATE TABLE IF NOT EXISTS tbl_DM_Loai_San_Pham  (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Ma_LSP STRING NOT NULL UNIQUE,
                Ten_LSP TEXT NOT NULL UNIQUE,
                Ghi_Chu TEXT
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tbl_DM_Loai_San_Pham table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="LoaiSanPham"/> objects.</returns>
        public async Task<List<LoaiSanPham>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_Loai_San_Pham";
            var loaiSanPham = new List<LoaiSanPham>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                loaiSanPham.Add(new LoaiSanPham
                {
                    Id = reader.GetInt32(0),
                    Ma_LSP = reader.GetString(1),
                    Ten_LSP = reader.GetString(2),
                    Ghi_Chu = reader.GetString(3)
                }
                    );
            }

            return loaiSanPham;
        }

        /// <summary>
        /// Retrieves a specific DonViTinh by its name.
        /// </summary>
        /// <param name="Ma_LSP">The ID of the DonViTinh.</param>
        /// <returns>A <see cref="LoaiSanPham"/> object if found; otherwise, null.</returns>
        public async Task<LoaiSanPham?> GetAsync(string tenSp)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_Loai_San_Pham WHERE ID = @Id";
            selectCmd.Parameters.AddWithValue("@Ten_SP", tenSp);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LoaiSanPham
                {
                    Id = reader.GetInt32(0),
                    Ma_LSP = reader.GetString(1),
                    Ten_LSP = reader.GetString(2),
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
        public async Task SaveItemAsync(LoaiSanPham item, bool isEdit)
        {
            await Init();


            if (string.IsNullOrWhiteSpace(item.Ten_LSP) || string.IsNullOrWhiteSpace(item.Ma_LSP))
                throw new Exception("Tên và mã loại sản phầm không được để trống!");

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (!isEdit)
            {
                saveCmd.CommandText = @"
                    INSERT INTO tbl_DM_Loai_San_Pham (Ma_LSP, Ten_LSP, Ghi_Chu)
                    VALUES (@Ma, @Ten, @GhiChu)";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE tbl_DM_Loai_San_Pham 
                    SET Ma_LSP = @Ma, Ten_LSP = @Ten, Ghi_Chu = @GhiChu
                    WHERE @Id = ID";
                saveCmd.Parameters.AddWithValue("@Id", item.Id);
            }
            saveCmd.Parameters.AddWithValue("@Ma", item.Ma_LSP);
            saveCmd.Parameters.AddWithValue("@Ten", item.Ten_LSP);
            saveCmd.Parameters.AddWithValue("@GhiChu", item.Ghi_Chu ?? string.Empty);

            try
            {
                await saveCmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Constraint Violation
            {
                throw new Exception($"Loại sản phẩm '{item.Ten_LSP}' đã tồn tại.");
            }
        }

        /// <summary>
        /// Deletes a DonViTinh from the database.
        /// </summary>
        /// <param name="item">The DonViTinh to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(LoaiSanPham item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_Loai_San_Pham WHERE ID = @Id";
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
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS tbl_DM_Loai_San_Pham";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}