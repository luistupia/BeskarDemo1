using System.Globalization;
using Application.Common.Interfaces;
using CsvHelper;

namespace Infraestructure.Files;

internal class CsvFileBuilder : ICsvFileBuilder
{
    public byte[] GetStreamFileCsv<T>(IEnumerable<T> records)
    {
        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream))
        {
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(records);
        }
        return memoryStream.ToArray();
    }
}