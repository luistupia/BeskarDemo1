using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Common.Helper;

public static class Utils
{
    public static string? GetDescriptionFromAttribute(MemberInfo? member)
    {
        if (member is null) return null;

        var attrib = (DescriptionAttribute)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false)!;
        return (attrib?.Description);
    }

    public static decimal MonthDifference(DateTime start, DateTime end) => Math.Abs(start.Month - end.Month + 12 * (start.Year - end.Year));

    public static string MonthName(int month)
    {
        var dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
        return dtinfo.GetMonthName(month);
    }
    
    public static async Task<byte[]?> DownloadFileToArrayAsync(string uri, string payload)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.PostAsync(uri, new StringContent(payload, Encoding.UTF8,
                "application/json"));
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsByteArrayAsync();
            return result;
        }
        catch
        {
            return null;
        }
    }
}