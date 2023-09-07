namespace Application.Common.Interfaces;

public interface IExcelFileBuilder
{
    byte[] GetStreamFileExcel<T>(IEnumerable<T>? records, string sheetName = "");
}