using System.Globalization;

namespace csvUploadDomain.Extensions;

public static class StringExtensions
{
    public static DateTime? ParseDateTime(this string inputDate, string dateFormat = "dd/MM/yyyy HH:mm:ss")
    {
        if (string.IsNullOrWhiteSpace(inputDate))
            return null;

        if (dateFormat.Contains(' ')) //formatting contains time
        {
            if (!inputDate.Contains(' '))
                return DateTime.ParseExact(inputDate, dateFormat.Split(' ')[0], CultureInfo.InvariantCulture);

            //when no seconds
            if (dateFormat.Contains(":ss") && inputDate.Split(':').Length < 2)
                return DateTime.ParseExact(inputDate, dateFormat.Replace(":ss", ""), CultureInfo.InvariantCulture);
        }

        return DateTime.ParseExact(inputDate, dateFormat, CultureInfo.InvariantCulture);
    }
}