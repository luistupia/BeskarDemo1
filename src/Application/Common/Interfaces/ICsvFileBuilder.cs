namespace Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] GetStreamFileCsv<T>(IEnumerable<T> records);
}