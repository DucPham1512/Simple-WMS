using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using InternProj.Models;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class SanPhamRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonViTinhRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public SanPhamRepository(ILogger<SanPhamRepository> logger)
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
            CREATE TABLE IF NOT EXISTS tbl_DM_San_Pham  (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Ma_SP STRING NOT NULL UNIQUE,
                Ten_SP TEXT NOT NULL,
                Loai_San_Pham_ID INTEGER NOT NULL,
                Don_Vi_Tinh_ID INTEGER NOT NULL,
                Ghi_Chu TEXT,
                FOREIGN KEY (Loai_San_Pham_ID) REFERENCES tbl_DM_Loai_San_Pham(ID) ON DELETE CASCADE,
                FOREIGN KEY (Don_Vi_Tinh_ID) REFERENCES tbl_DM_Don_Vi_Tinh(ID) ON DELETE RESTRICT
            );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tbl_DM_San_Pham table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="SanPham"/> objects.</returns>
        public async Task<List<SanPham>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"SELECT
                                    sp.ID, sp.Ma_SP, sp.Ten_SP, sp.Loai_San_Pham_ID, sp.Don_Vi_Tinh_ID, sp.Ghi_Chu, lsp.Ten_LSP, dvt.Ten_Don_Vi_Tinh
                                    FROM tbl_DM_San_Pham sp
                                    JOIN tbl_DM_Loai_San_Pham lsp ON sp.Loai_San_Pham_ID = lsp.ID
                                    JOIN tbl_DM_Don_Vi_Tinh dvt ON sp.Don_Vi_Tinh_ID = dvt.ID";
            var sanPham = new List<SanPham>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sanPham.Add(new SanPham
                    {
                        Id = reader.GetInt32(0),
                        Ma_SP = reader.GetString(1),
                        Ten_SP = reader.GetString(2),
                        Id_LSP = reader.GetInt32(3),
                        Id_DVT = reader.GetInt32(4),
                        Ghi_Chu = reader.GetString(5),
                        Ten_LSP = reader.GetString(6),
                        Ten_DVT = reader.GetString(7)
                }
                );
            }

            return sanPham;
        }

        /// <summary>
        /// Retrieves a specific DonViTinh by its name.
        /// </summary>
        /// <param name="Id">The ID of the DonViTinh.</param>
        /// <returns>A <see cref="SanPham"/> object if found; otherwise, null.</returns>
        public async Task<SanPham?> GetAsync(int Id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_San_Pham WHERE ID = @Id";
            selectCmd.Parameters.AddWithValue("@Id", Id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SanPham
                {
                    Id = reader.GetInt32(0),
                    Ma_SP = reader.GetString(1),
                    Ten_SP = reader.GetString(2),
                    Id_LSP = reader.GetInt32(3),
                    Id_DVT = reader.GetInt32(4),
                    Ghi_Chu = reader.GetString(5)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a DonViTinh to the database. If the DonViTinh hasn't existed in the database, a new DonViTinh is created; otherwise, the existing DonViTinh is updated.
        /// </summary>
        /// <param name="item">The DonViTinh to save.</param>
        /// <returns>The ID of the saved DonViTinh.</returns>
        public async Task SaveItemAsync(SanPham item, bool isEdit)
        {
            await Init();

            string? error = item switch
            {
                null => "Dữ liệu sản phẩm không hợp lệ.",

                _ when string.IsNullOrWhiteSpace(item.Ten_SP)
                    => "Tên sản phẩm không được để trống.",

                _ when string.IsNullOrWhiteSpace(item.Ma_SP)
                    => "Mã sản phẩm không được để trống.",

                _ when item.Id_LSP <= 0
                    => "Vui lòng chọn loại sản phẩm.",

                _ when item.Id_DVT <= 0
                    => "Vui lòng chọn đơn vị tính.",

                _ => null
            };

            if (error is not null)
                throw new Exception(error);

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (!isEdit)
            {
                saveCmd.CommandText = @"
                    INSERT INTO tbl_DM_San_Pham (Ma_SP, Ten_SP, Loai_San_Pham_Id, Don_Vi_Tinh_ID, Ghi_Chu)
                    VALUES (@Ma, @Ten, @Lsp, @Dvt, @GhiChu)";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE tbl_DM_San_Pham 
                    SET Ma_SP = @Ma, Ten_SP = @Ten, Loai_San_Pham_Id = @Lsp, Don_Vi_Tinh_ID = @Dvt, Ghi_Chu = @GhiChu
                    WHERE @Id = ID";
                saveCmd.Parameters.AddWithValue("@Id", item.Id);
            }
            saveCmd.Parameters.AddWithValue("@Ma", item.Ma_SP);
            saveCmd.Parameters.AddWithValue("@Ten", item.Ten_SP);
            saveCmd.Parameters.AddWithValue("@Lsp", item.Id_LSP);
            saveCmd.Parameters.AddWithValue("@Dvt", item.Id_DVT);
            saveCmd.Parameters.AddWithValue("@GhiChu", item.Ghi_Chu ?? string.Empty);

            try
            {
                await saveCmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 787) // SQLITE_CONSTRAINT_FOREIGNKEY
            {
                throw new Exception("Không tồn tại Loại sản phẩm hoặc Đơn vị tính không hợp lệ.");
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067) // SQLITE_CONSTRAINT_UNIQUE
            {
                throw new Exception($"Sản phẩm '{item.Ma_SP}' đã tồn tại.");
            }
        }

        /// <summary>
        /// Deletes a DonViTinh from the database.
        /// </summary>
        /// <param name="item">The DonViTinh to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(SanPham item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_San_Pham WHERE ID = @Id";
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
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS tbl_DM_San_Pham";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}