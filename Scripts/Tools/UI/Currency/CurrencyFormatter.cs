public static class CurrencyFormatter
{
    public static string Format(long value)
    {
        if (value >= 1_000_000_000L)
            return FormatTier(value, 1_000_000_000L, "B");
        if (value >= 1_000_000L)
            return FormatTier(value, 1_000_000L, "M");
        if (value >= 1_000L)
            return FormatTier(value, 1_000L, "K");
        return value.ToString();
    }

    private static string FormatTier(long value, long divisor, string suffix)
    {
        double scaled = (double)value / divisor;
        double rounded = System.Math.Round(scaled, 1);
        string number = rounded % 1 == 0 ? $"{(long)rounded}" : $"{rounded:F1}";
        return $"{number}{suffix}";
    }
}
