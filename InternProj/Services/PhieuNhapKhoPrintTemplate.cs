using InternProj.Models;
using InternProj.Services;
using System.Globalization;
using System.Text;
using VietnamNumber;
public static class PhieuNhapKhoPrintTemplate
{
    public static string Build(PhieuNhapKhoHeader header, IEnumerable<PhieuNhapKhoData> lines)
    {
        var rows = new StringBuilder();
        int stt = 1;
        decimal tongTien = 0;
        string tongTienChu = string.Empty;

        foreach (var line in lines)
        {
            var thanhTien = (decimal)line.SoLuong * (decimal)line.DonGia;
            tongTien += thanhTien;
            rows.Append($$"""
                <tr>
                    <td class="center">{{stt++}}</td>
                    <td>{{line.TenSP}}</td>
                    <td>{{line.MaSP}}</td>
                    <td class="center">{{line.TenDonViTinh}}</td>
                    <td class="right">{{line.SoLuong}}</td>
                    <td class="right">{{line.DonGia.ToString("N4", CultureInfo.InvariantCulture)}}</td>
                    <td class="right">{{thanhTien.ToString("N4", CultureInfo.InvariantCulture)}}</td>
                </tr>
            """);
        }
        tongTienChu = NumberToViet.Convert(tongTien);

        return $$"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8" />
            <style>
            @page { size: A4 portrait; margin: 12mm; }

            html, body {
                margin: 0;
                padding: 0;
                background: #ffffff !important;
                color: #000000 !important;
            }

            :root {
                color-scheme: light;
            }

            body {
                font-family: Arial, sans-serif;
                font-size: 13px;
            }

            .sheet {
                background: #ffffff;
                min-height: 100vh;
                box-sizing: border-box;
                padding: 12mm;
            }

            .title { text-align:center; font-size: 20px; font-weight: bold; margin-bottom: 12px; }
            .top { display:flex; justify-content:space-between; margin-bottom: 10px; }
            .block { width: 48%; line-height: 1.5; }
            table { width:100%; border-collapse:collapse; margin-top: 10px; background:#fff; }
            th, td { border:1px solid #666; padding:6px; }
            th { background:#e9edf5; }
            .center { text-align:center; }
            .right { text-align:right; }
            .footer { margin-top: 16px; }
            .sign { display:flex; justify-content:space-between; margin-top:40px; }
            .sign div { width:22%; text-align:center; }
        </style>
        </head>
        <body>
        <div class="sheet">
            <div class="title">PHIẾU NHẬP KHO</div>

            <div class="top">
                <div class="block">
                    <div><b>Số phiếu:</b> {{header.So_Phieu_Nhap_Kho}}</div>
                    <div><b>Ngày nhập:</b> {{header.Ngay_Nhap_Kho:dd/MM/yyyy}}</div>
                    <div><b>Kho:</b> {{header.Ten_Kho}}</div>
                    <div><b>NCC:</b> {{header.Ten_NCC}}</div>
                    <div><b>Ghi chú:</b> {{header.Ghi_Chu}}</div>
                </div>
                <div class="block" style="text-align:right;">
                    <div><b>Mẫu số:</b> 01 - VT</div>
                </div>
            </div>

            <table>
                <thead>
                    <tr>
                        <th>STT</th>
                        <th>Tên hàng</th>
                        <th>Mã hàng</th>
                        <th>ĐVT</th>
                        <th>Số lượng</th>
                        <th>Đơn giá</th>
                        <th>Thành tiền</th>
                    </tr>
                </thead>
                <tbody>
                    {{rows}}
                    <tr>
                        <td colspan="6" class="right"><b>Tổng</b></td>
                        <td class="right"><b>{{tongTien.ToString("N4", CultureInfo.InvariantCulture)}}</b></td>
                    </tr>
                </tbody>
            </table>

            <div class="footer">
                <div><b>Tổng số tiền:</b> {{tongTien.ToString("N4", CultureInfo.InvariantCulture)}}</div>
                <div><b>Tổng số tiền (bằng chữ):</b> {{tongTienChu}}</div>
            </div>

            <div class="sign">
                <div><b>Người lập phiếu</b><br/><br/><br/>(Ký, họ tên)</div>
                <div><b>Người giao hàng</b><br/><br/><br/>(Ký, họ tên)</div>
                <div><b>Thủ kho</b><br/><br/><br/>(Ký, họ tên)</div>
                <div><b>Kế toán trưởng</b><br/><br/><br/>(Ký, họ tên)</div>
            </div>
        </body>
        </div>
        </html>
        """;
    }
}