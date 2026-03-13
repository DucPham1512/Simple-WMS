using InternProj.Models;
using InternProj.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;

namespace InternProj.Data
{
    /// <summary>
    /// Repository class for managing measure units in the database.
    /// </summary>
    public class XuatNhapTonDataRepository
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhieuXuatKhoRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public XuatNhapTonDataRepository(ILogger<XuatNhapTonDataRepository> logger)
        {
            _logger = logger;
        }
        public async Task<List<XuatNhapTonData>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {

            string fromDateString = DateStringConverter.toString(fromDate);
            string toDateString = DateStringConverter.toString(toDate);

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                WITH XuatNhap AS (
                    SELECT 
                        nkd.San_Pham_ID,
                        nk.Ngay_Nhap_Kho AS NgayGiaoDich,
                        nkd.SL_Nhap AS SoLuongNhap,
                        0 AS SoLuongXuat
                    FROM tbl_DM_Nhap_Kho_Raw_Data nkd
                    JOIN tbl_DM_Nhap_Kho nk ON nkd.Nhap_Kho_ID = nk.ID

                    UNION ALL

                    SELECT 
                        xkd.San_Pham_ID,
                        xk.Ngay_Xuat_Kho AS NgayGiaoDich,
                        0 AS SoLuongNhap,
                        xkd.SL_Xuat AS SoLuongXuat
                    FROM tbl_DM_Xuat_Kho_Raw_Data xkd
                    JOIN tbl_DM_Xuat_Kho xk ON xkd.Xuat_Kho_ID = xk.ID
                ),
                TongXuatNhap AS (
                    SELECT 
                        San_Pham_ID,
                        SUM(CASE WHEN NgayGiaoDich < @from THEN SoLuongNhap - SoLuongXuat ELSE 0 END) AS SL_Dau_Ky,
                        SUM(CASE WHEN NgayGiaoDich >= @from AND NgayGiaoDich <= @to THEN SoLuongNhap ELSE 0 END) AS SL_Nhap,
                        SUM(CASE WHEN NgayGiaoDich >= @from AND NgayGiaoDich <= @to THEN SoLuongXuat ELSE 0 END) AS SL_Xuat
                    FROM XuatNhap
                    WHERE NgayGiaoDich <= @to
                    GROUP BY San_Pham_ID
                )

                SELECT 
                    sp.Ma_SP AS MaSP,
                    sp.Ten_SP AS TenSP,
                    IFNULL(a.SL_Dau_Ky, 0) AS SL_DauKy,
                    IFNULL(a.SL_Nhap, 0) AS SL_Nhap,
                    IFNULL(a.SL_Xuat, 0) AS SL_Xuat,
        
                    -- Cuối kỳ = Đầu kỳ + Nhập - Xuất
                    (IFNULL(a.SL_Dau_Ky, 0) + IFNULL(a.SL_Nhap, 0) - IFNULL(a.SL_Xuat, 0)) AS SL_CuoiKy
        
                FROM tbl_DM_San_Pham sp
                LEFT JOIN TongXuatNhap a ON sp.ID = a.San_Pham_ID
       
                ORDER BY sp.Ma_SP;
            ";

            cmd.Parameters.AddWithValue("@from", fromDateString);
            cmd.Parameters.AddWithValue("@to", toDateString);

            var result = new List<XuatNhapTonData>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new XuatNhapTonData
                {
                    MaSP = reader.GetString(0),
                    TenSP = reader.GetString(1),
                    SoLuongDauKy = reader.GetDecimal(2),
                    SoLuongNhap = reader.GetDecimal(3),
                    SoLuongXuat = reader.GetDecimal(4),
                    SoLuongCuoiKy = reader.GetDecimal(5)
                });
            }

            return result;
        }
    }
}