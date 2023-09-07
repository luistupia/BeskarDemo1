namespace Common.Extensions;

public static class StringExtension
{
   public static string EmptyNull(this string? str)
    {
        return str ?? "";
    }

    public static string? ToUpperIgnoreNull(this string? value)
    {
        return value?.Trim().ToUpper();
    }
}