namespace Application.Common.Wrappers;

public class ExportQuery
{
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
    public byte[]? Content { get; set; }
}