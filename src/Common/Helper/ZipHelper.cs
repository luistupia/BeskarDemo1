using System.IO.Compression;

namespace Common.Helper;

public class ZipHelper
{
    public static async Task<MemoryStream?> CreatedZipOfMemory2(Dictionary<string, byte[]> entries)
    {
        using var zipFileMemoryStream = new MemoryStream();
        using (var archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
        {
            foreach (var (key,value) in entries)
            {
                var botFileName = Path.GetFileName(key);
                var entry = archive.CreateEntry(botFileName);
                await using var entryStream = entry.Open();
                var memoryStream = value != null ? new MemoryStream(value) : throw new ArgumentException("bad argument", nameof(value));
                await memoryStream.CopyToAsync(entryStream);
            }
        }
        zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
        var result = zipFileMemoryStream;

        return result;
    }
}