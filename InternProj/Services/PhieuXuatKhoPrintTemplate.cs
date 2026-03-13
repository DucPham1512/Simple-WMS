using InternProj.Models;
using InternProj.Services;
using System.Globalization;
using System.Text;

public static class PhieuXuatKhoPrintTemplate
{
    public static string Build(PhieuXuatKhoHeader header, IEnumerable<PhieuXuatKhoData> lines)
    {
        var rows = new StringBuilder();
        int stt = 1;
        decimal tongTien = 0;

        foreach (var line in lines)
        {
            var thanhTien = (decimal)line.SoLuong * (decimal)line.DonGia;
            tongTien += thanhTien;

            rows.Append($$"""
                <tr>
                    <td class="center">{{stt++}}</td>
                    <td>{{line.MaSP}}</td>
                    <td>{{line.TenSP}}</td>
                    <td></td>
                    <td></td>
                    <td class="center">{{line.SoLuong}}</td>
                    <td class="center">{{line.TenDonViTinh}}</td>
                    <td></td>
                    <td></td>
                </tr>
                """);
        }

        string tongTienChu = NumberToViet.Convert(tongTien);
        return $$"""
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset="utf-8"/>
            <style>
            @page { size: A4 portrait; margin: 15mm; }

            html, body {
                margin: 20;
                padding: 20;
                background-color: #ffffff !important;
                color: #000000 !important;
            }
            

            .title {
                text-align: center;
                font-weight: bold;
                font-size: 22px;
            }

            .subtitle {
                text-align: center;
                margin-top: 4px;
            }

            .section-title {
                font-weight: bold;
                margin-top: 16px;
            }

                        .info-row {
                display: flex;
                margin-bottom: 4px;
            }

            .label {
                width: 180px;   /* control label width here */
            }

            .value {
                flex: 1;
            }

            table {
                width: 100%;
                border-collapse: collapse;
                margin-top: 8px;
            }

            th, td {
                border: 1px solid #000;
                padding: 6px;
            }

            th {
                background: #ffff00;
                text-align: center;
            }

            .center { text-align: center; }
            .right { text-align: right; }

            .no-border td {
                border: none;
                padding: 2px 0;
            }

            .signature {
                margin-top: 40px;
                display: flex;
                justify-content: space-between;
                text-align: center;
            }

            .signature div {
                width: 22%;
            }
            </style>
            </head>

            <body>

            <div class="title">PHIẾU XUẤT KHO</div>

            <div class="subtitle">
                Ngày {{header.Ngay_Xuat_Kho.Day}} tháng {{header.Ngay_Xuat_Kho.Month}} năm {{header.Ngay_Xuat_Kho.Year}}
            </div>

            <div class="section-title">I. Thông tin chung</div>

            <div class="info-row">
                <div class="label">1. Số phiếu xuất kho:</div>
                <div class="value"><b>{{header.So_Phieu_Xuat_Kho}}</b></div>
            </div>

            <div class="info-row">
                <div class="label">2. Kho:</div>
                <div class="value"><b>{{header.Ten_Kho}}</b></div>
            </div>

            <div class="info-row">
                <div class="label">3. Ghi chú:</div>
                <div class="value"><b>{{header.Ghi_Chu}}</b></div>
            </div>

            <div class="section-title">II. Thông tin hàng</div>

            <table>
            <thead>
            <tr>
                <th>STT</th>
                <th>Mã Hàng</th>
                <th>Tên Hàng</th>
                <th>PO</th>
                <th>Số Chỉ Lệnh</th>
                <th>SL</th>
                <th>DVT</th>
                <th>Kiểm Kê</th>
                <th>Ghi Chú</th>
            </tr>
            </thead>
            <tbody>
            {{rows}}
            </tbody>
            </table>

            <div class="section-title">III. Kết luận</div>

            <div>1. Tổng số lượng thực xuất (viết bằng số): <b>{{lines.Sum(x => x.SoLuong)}}</b></div>
            <div>2. Tổng số lượng thực xuất (viết bằng chữ): {{NumberToViet.Convert(lines.Sum(x=>x.SoLuong))}}</div>
            <div>3. Mô tả khác biệt:</div>

            <div style="border:1px solid #000; height:80px; margin-top:5px;"></div>

            <div class="signature">
                <div><b>Người lập phiếu</b></div>
                <div><b>Người nhận hàng</b></div>
                <div><b>Điều phối</b></div>
                <div><b>Giám đốc</b></div>
            </div>

            </body>
            </html>
            """;
    }
}