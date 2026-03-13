using System;
using System.Collections.Generic;
using System.Text;
using VietnamNumber;

namespace InternProj.Services
{
    internal class NumberToViet
    {
        public static string Convert(decimal num)
        {
            int beforeDec = (int)num;
            int afterDec = (int)((num - beforeDec) * 10000);

            string beforeText = VietnamNumber.Number.ToVietnameseWords(beforeDec);

            // Pad to 4 digits to preserve leading zeros e.g. 0.05 -> "0500"
            string afterStr = afterDec.ToString("D4").TrimEnd('0');

            // Read digit by digit
            string afterText = VietnamNumber.Number.ToVietnameseSingleWords(afterStr);

            var result = afterDec == 0 ? beforeText : $"{beforeText} phẩy {afterText}";
            return char.ToUpper(result[0]) + result[1..];
        }
    }
}
