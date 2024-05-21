using System.Reflection;
using System.Text;

namespace SuccessRateReport.Extensions;

public static class CsvExtensions
{
    public static string ToCsv<T>(this IEnumerable<T> objectlist, string separator)
    {
        var t = typeof(T);
        var properties = t.GetProperties();

        var header = string.Join(separator, properties.Select(f => f.Name).ToArray());

        var csvData = new StringBuilder();
        csvData.AppendLine(header);

        foreach (var o in objectlist)
            csvData.AppendLine(ToCsvFields(separator, properties, o));

        return csvData.ToString();
    }

    private static string ToCsvFields(string separator, PropertyInfo[] fields, object o)
    {
        var lines = new StringBuilder();

        foreach (var f in fields)
        {
            if (lines.Length > 0)
                lines.Append(separator);

            var x = f.GetValue(o);

            if (x != null)
                lines.Append(x);
        }

        return lines.ToString();
    }
}