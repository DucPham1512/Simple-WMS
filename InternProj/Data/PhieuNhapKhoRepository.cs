using InternProj.Models;
using InternProj.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using System.Globalization;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class PhieuNhapKhoRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhieuNhapKhoRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public PhieuNhapKhoRepository(ILogger<PhieuNhapKhoRepository> logger)
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
                    CREATE TABLE IF NOT EXISTS tbl_DM_Nhap_Kho (
                        ID                 INTEGER PRIMARY KEY AUTOINCREMENT,
                        So_Phieu_Nhap_Kho  TEXT    NOT NULL UNIQUE,
                        Kho_ID             INTEGER NOT NULL,
                        NCC_ID             INTEGER NOT NULL,
                        Ngay_Nhap_Kho      TEXT    NOT NULL,   -- store as ISO string: yyyy-MM-dd or yyyy-MM-dd HH:mm:ss
                        Ghi_Chu            TEXT,
                        FOREIGN KEY (Kho_ID) REFERENCES tbl_DM_Kho(ID) ON DELETE RESTRICT,
                        FOREIGN KEY (NCC_ID) REFERENCES tbl_DM_NCC(ID) ON DELETE RESTRICT
                    );

                    CREATE TABLE IF NOT EXISTS tbl_DM_Nhap_Kho_Raw_Data (
                        ID           INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nhap_Kho_ID  INTEGER NOT NULL,
                        San_Pham_ID  INTEGER NOT NULL,
                        SL_Nhap      REAL NOT NULL CHECK (SL_Nhap > 0),
                        Don_Gia_Nhap REAL    NOT NULL CHECK (Don_Gia_Nhap >= 0),
                        FOREIGN KEY (Nhap_Kho_ID) REFERENCES tbl_DM_Nhap_Kho(ID) ON DELETE CASCADE,
                        FOREIGN KEY (San_Pham_ID) REFERENCES tbl_DM_San_Pham(ID) ON DELETE RESTRICT
                    );
                    ";
                await createTableCmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating required table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        /// <returns>A list of <see cref="PhieuNhapKhoHeader"/> objects.</returns>
        public async Task<List<PhieuNhapKhoHeader>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"SELECT 
                                    nk.ID AS ID,
                                    So_Phieu_Nhap_Kho, 
                                    Kho_ID,
                                    Ten_Kho,
                                    NCC_ID,
                                    Ten_NCC,
                                    Ngay_Nhap_Kho, 
                                    nk.Ghi_Chu  
                                    FROM tbl_DM_Nhap_Kho nk
                                    JOIN tbl_DM_Kho k ON nk.Kho_ID = k.ID
                                    JOIN tbl_DM_NCC ncc on nk.NCC_ID = ncc.ID";
            var item = new List<PhieuNhapKhoHeader>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                item.Add(new PhieuNhapKhoHeader
                {
                    Id = reader.GetInt32(0),
                    So_Phieu_Nhap_Kho = reader.GetString(1),
                    Kho_ID = reader.GetInt32(2),
                    Ten_Kho = reader.GetString(3),
                    NCC_ID = reader.GetInt32(4),
                    Ten_NCC = reader.GetString(5),
                    Ngay_Nhap_Kho = DateStringConverter.toDateTime(reader.GetString(6)),
                    Ghi_Chu = reader.IsDBNull(7) ? null : reader.GetString(7)
                }
                    );
            }

            return item;
        }

        /// <summary>
        /// Retrieves a specific DonViTinh by its name.
        /// </summary>
        /// <param name="Ma_LSP">The ID of the DonViTinh.</param>
        /// <returns>A <see cref="LoaiSanPham"/> object if found; otherwise, null.</returns>
        public async Task<List<PhieuNhapKhoData>> GetAsync(int nhapKhoId)
        {
            await Init();

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    nk.So_Phieu_Nhap_Kho     AS SoPhieu,
                    nk.Ngay_Nhap_Kho         AS NgayNhap,
                    sp.Ten_SP                AS TenSP,
                    sp.Ma_SP                 AS MaSP,
                    dvt.Ten_Don_Vi_Tinh      AS TenDVT,
                    nkd.SL_Nhap              AS SLNhap,
                    nkd.Don_Gia_Nhap         AS DonGiaNhap,
                    (nkd.SL_Nhap * nkd.Don_Gia_Nhap) AS ThanhTien,
                    nkd.ID                    AS ID,
                    nkd.San_Pham_ID                     AS SanPhamId
                FROM tbl_DM_Nhap_Kho nk
                JOIN tbl_DM_Nhap_Kho_Raw_Data nkd ON nkd.Nhap_Kho_ID = nk.ID
                JOIN tbl_DM_San_Pham sp ON nkd.San_Pham_ID = sp.ID
                LEFT JOIN tbl_DM_Don_Vi_Tinh dvt ON dvt.ID = sp.Don_Vi_Tinh_ID
                WHERE nk.ID = @id
                ORDER BY nkd.ID;
            ";
            cmd.Parameters.AddWithValue("@id", nhapKhoId);

            var result = new List<PhieuNhapKhoData>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new PhieuNhapKhoData
                {
                    SoPhieu = reader.GetString(0),
                    NgayNhapKho = DateStringConverter.toDateTime(reader.GetString(1)),

                    TenSP = reader.GetString(2),
                    MaSP = reader.GetString(3),
                    TenDonViTinh = reader.GetString(4),

                    SoLuong = reader.GetDecimal(5),
                    DonGia = reader.GetDecimal(6),
                    ThanhTien = reader.GetDecimal(7),
                    Id = reader.GetInt32(8),
                    SanPhamId = reader.GetInt32(9),
                });
            }

            return result;
        }

        /// <summary>
        /// Saves a DonViTinh to the database. If the DonViTinh hasn't existed in the database, a new DonViTinh is created; otherwise, the existing DonViTinh is updated.
        /// </summary>
        /// <param name="item">The DonViTinh to save.</param>
        /// <returns>The ID of the saved DonViTinh.</returns>
        public async Task<int> SaveAsync(PhieuNhapKhoHeader header, IList<PhieuNhapKhoRawData> lines)
        {
            await Init();

            if (lines == null || lines.Count == 0)
                throw new InvalidOperationException("Phiếu nhập phải có ít nhất 1 dòng hàng.");

            await using var conn = new SqliteConnection(Constants.DatabasePath);
            await conn.OpenAsync();

            await using var tx = conn.BeginTransaction();

            try
            {
                // Decide date to save
                var ngay = header.Ngay_Nhap_Kho == default ? DateTime.Now : header.Ngay_Nhap_Kho;

                // 1) insert header
                await using var insertHeader = conn.CreateCommand();
                insertHeader.Transaction = tx;
                insertHeader.CommandText = @"
                    INSERT INTO tbl_DM_Nhap_Kho (So_Phieu_Nhap_Kho, Kho_ID, NCC_ID, Ngay_Nhap_Kho, Ghi_Chu)
                    VALUES (@so, @kho, @ncc, @ngay, @gc);
                    SELECT last_insert_rowid();
        ";

                insertHeader.Parameters.AddWithValue("@so", header.So_Phieu_Nhap_Kho);
                insertHeader.Parameters.AddWithValue("@kho", header.Kho_ID);
                insertHeader.Parameters.AddWithValue("@ncc", header.NCC_ID);
                insertHeader.Parameters.AddWithValue("@ngay", DateStringConverter.toString(ngay));
                insertHeader.Parameters.AddWithValue("@gc", (object?)header.Ghi_Chu ?? DBNull.Value);

                var nhapKhoId = Convert.ToInt32(await insertHeader.ExecuteScalarAsync());

                // 2) insert lines
                await using var insertLine = conn.CreateCommand();
                insertLine.Transaction = tx;
                insertLine.CommandText = @"
                    INSERT INTO tbl_DM_Nhap_Kho_Raw_Data (Nhap_Kho_ID, San_Pham_ID, SL_Nhap, Don_Gia_Nhap)
                    VALUES (@id, @sp, @sl, @dg);
        ";

                var pId = insertLine.CreateParameter(); pId.ParameterName = "@id"; insertLine.Parameters.Add(pId);
                var pSp = insertLine.CreateParameter(); pSp.ParameterName = "@sp"; insertLine.Parameters.Add(pSp);
                var pSl = insertLine.CreateParameter(); pSl.ParameterName = "@sl"; insertLine.Parameters.Add(pSl);
                var pDg = insertLine.CreateParameter(); pDg.ParameterName = "@dg"; insertLine.Parameters.Add(pDg);

                foreach (var l in lines)
                {
                    pId.Value = nhapKhoId;
                    pSp.Value = l.SanPhamId;
                    pSl.Value = l.SoLuong;
                    pDg.Value = l.DonGia;

                    await insertLine.ExecuteNonQueryAsync();
                }

                tx.Commit();
                return nhapKhoId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        public async Task<int> EditHeaderAsync(PhieuNhapKhoHeader item)
        {
            await Init();

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            if (item.Id > 0) { 
            cmd.CommandText = @"
                UPDATE tbl_DM_Nhap_Kho
                SET So_Phieu_Nhap_Kho = @spnk,
                    Kho_ID            = @kho,
                    NCC_ID            = @ncc,
                    Ngay_Nhap_Kho     = @ngay,
                    Ghi_Chu           = @ghichu
                WHERE ID = @id;
            ";
                cmd.Parameters.AddWithValue("@id", item.Id);
            } 
            else
            {
                cmd.CommandText = @"
                INSERT INTO tbl_DM_Nhap_Kho (So_Phieu_Nhap_Kho, Kho_ID, NCC_ID, Ngay_Nhap_Kho, Ghi_Chu)
                VALUES (@spnk, @kho, @ncc, @ngay, @ghichu);
                ";
            }

            cmd.Parameters.AddWithValue("@spnk", item.So_Phieu_Nhap_Kho);
            cmd.Parameters.AddWithValue("@kho", item.Kho_ID);
            cmd.Parameters.AddWithValue("@ncc", item.NCC_ID);
            cmd.Parameters.AddWithValue("@ngay", DateStringConverter.toString(item.Ngay_Nhap_Kho));
            cmd.Parameters.AddWithValue("@ghichu", (object?)item.Ghi_Chu ?? DBNull.Value);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> EditDataAsync(PhieuNhapKhoRawData item)
        {
            await Init();

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            if (item.Id > 0)
            {
                cmd.CommandText = @"
                UPDATE tbl_DM_Nhap_Kho_Raw_Data
                SET SL_Nhap      = @sl,
                    Don_Gia_Nhap = @dg
                WHERE ID = @id;
            ";
                cmd.Parameters.AddWithValue("@id", item.Id);
            }
            else
            {
                cmd.CommandText = @"
                INSERT INTO tbl_DM_Nhap_Kho_Raw_Data (Nhap_Kho_ID, San_Pham_ID, SL_Nhap, Don_Gia_Nhap)
                VALUES (@nkid, @sp, @sl, @dg);
            ";
                cmd.Parameters.AddWithValue("@nkid", item.NhapKhoId);
            }
            cmd.Parameters.AddWithValue("@sl", item.SoLuong);
            cmd.Parameters.AddWithValue("@dg", item.DonGia);
            cmd.Parameters.AddWithValue("@sp", item.SanPhamId);
            return await cmd.ExecuteNonQueryAsync();
        } 



        public async Task<List<NhapKhoReportData>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            await Init();

            string fromDateString = DateStringConverter.toString(fromDate);
            string toDateString = DateStringConverter.toString(toDate);

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT
                    nk.So_Phieu_Nhap_Kho     AS SoPhieu,
                    nk.Ngay_Nhap_Kho         AS NgayNhap,
                    sp.Ten_SP                AS TenSP,
                    sp.Ma_SP                 AS MaSP,
                    ncc.Ten_NCC              AS TenNCC,
                    nkd.SL_Nhap              AS SLNhap,
                    nkd.Don_Gia_Nhap         AS DonGiaNhap,
                    (nkd.SL_Nhap * nkd.Don_Gia_Nhap) AS ThanhTien,
                    nkd.ID                    AS ID,
                    nkd.San_Pham_ID                     AS SanPhamId
                FROM tbl_DM_Nhap_Kho nk
                JOIN tbl_DM_Nhap_Kho_Raw_Data nkd ON nkd.Nhap_Kho_ID = nk.ID
                JOIN tbl_DM_San_Pham sp ON nkd.San_Pham_ID = sp.ID
                JOIN tbl_DM_NCC ncc on nk.NCC_ID = ncc.ID
                WHERE nk.Ngay_Nhap_Kho BETWEEN @from AND @to
                ORDER BY nkd.ID;
            ";

            cmd.Parameters.AddWithValue("@from", fromDateString);
            cmd.Parameters.AddWithValue("@to", toDateString);

            var result = new List<NhapKhoReportData>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new NhapKhoReportData
                {
                    SoPhieu = reader.GetString(0),
                    NgayNhap = DateStringConverter.toDateTime(reader.GetString(1)),

                    TenSP = reader.GetString(2),
                    MaSP = reader.GetString(3),

                    TenNCC = reader.GetString(4),


                    SoLuong = reader.GetDecimal(5),
                    DonGia = reader.GetDecimal(6),
                    ThanhTien = reader.GetDecimal(7),
                });
            }

            return result;

        }

        /// <summary>
        /// Deletes a DonViTinh from the database.
        /// </summary>
        /// <param name="item">The DonViTinh to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(PhieuNhapKhoHeader item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_Nhap_Kho WHERE ID = @Id";
            deleteCmd.Parameters.AddWithValue("@Id", item.Id);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteDataAsync(PhieuNhapKhoRawData item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM tbl_DM_Nhap_Kho_Raw_Data WHERE ID = @Id";
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
            dropTableCmd.CommandText = @"
                DROP TABLE IF EXISTS tbl_DM_Nhap_Kho_Raw_Data;
                DROP TABLE IF EXISTS tbl_DM_Nhap_Kho;
            ";

            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}