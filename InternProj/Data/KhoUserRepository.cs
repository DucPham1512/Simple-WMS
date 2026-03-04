using InternProj.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class KhoUserRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KhoUserRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public KhoUserRepository(ILogger<KhoUserRepository> logger)
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
                    CREATE TABLE IF NOT EXISTS tbl_DM_Kho_User (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Ma_Dang_Nhap TEXT NOT NULL,
                        Kho_ID INTEGER NOT NULL,
                        UNIQUE (Ma_Dang_Nhap, Kho_ID),
                        FOREIGN KEY (Kho_ID) REFERENCES tbl_DM_Kho(ID) ON DELETE CASCADE
                    );";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating tbl_DM_Kho_User  table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="KhoUser"/> objects.</returns>
        public async Task<List<KhoUser>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"SELECT ku.ID, ku.Ma_Dang_Nhap, ku.Kho_ID, k.Ten_Kho 
                                      FROM tbl_DM_Kho_User ku
                                      JOIN tbl_DM_Kho k ON ku.Kho_ID = k.ID";
            var KhoUser = new List<KhoUser>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                KhoUser.Add(new KhoUser
                {
                    Id = reader.GetInt32(0),
                    MaDangNhap = reader.GetString(1),
                    KhoId = reader.GetInt32(2),
                    Ten_Kho = reader.GetString(3)
                }
                    );
            }

            return KhoUser;
        }

        /// <summary>
        /// Retrieves a specific DonViTinh by its name.
        /// </summary>
        /// <param name="Id">The ID of the DonViTinh.</param>
        /// <returns>A <see cref="KhoUser"/> object if found; otherwise, null.</returns>
        public async Task<KhoUser?> GetAsync(int Id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM tbl_DM_Kho_User  WHERE ID = @Id";
            selectCmd.Parameters.AddWithValue("@Id", Id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new KhoUser
                {
                    Id = reader.GetInt32(0),
                    MaDangNhap = reader.GetString(1),
                    KhoId = reader.GetInt32(2)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a DonViTinh to the database. If the DonViTinh hasn't existed in the database, a new DonViTinh is created; otherwise, the existing DonViTinh is updated.
        /// </summary>
        /// <param name="item">The DonViTinh to save.</param>
        /// <returns>The ID of the saved DonViTinh.</returns>
        public async Task SaveItemAsync(KhoUser item, bool isEdit)
        {
            await Init();


            if (string.IsNullOrWhiteSpace(item.MaDangNhap))
                throw new Exception("Mã đăng nhập và kho không được để trống!");

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();

            if (!isEdit)
            {
                saveCmd.CommandText = @"
                    INSERT INTO tbl_DM_Kho_User (Ma_Dang_Nhap, Kho_ID)
                    VALUES (@Pass, @Kho)";
            }
            else
            {
                saveCmd.CommandText = @"
                    UPDATE tbl_DM_Kho_User  
                    SET Ma_Dang_Nhap = @Pass, Kho_ID = @Kho
                    WHERE @Id = ID";
                saveCmd.Parameters.AddWithValue("@Id", item.Id);
            }
            saveCmd.Parameters.AddWithValue("@Pass", item.MaDangNhap);
            saveCmd.Parameters.AddWithValue("@Kho", item.KhoId);

            try
            {
                await saveCmd.ExecuteNonQueryAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Constraint Violation
            {
                throw new Exception($"Đã tồn tại mã đăng nhập này đã tồn tại cho kho này.");
            }
        }

        /// <summary>
        /// Deletes a DonViTinh from the database.
        /// </summary>
        /// <param name="item">The KhoUser to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(KhoUser item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_Kho_User  WHERE ID = @Id";
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
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS tbl_DM_Kho_User ";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}