namespace UI.Renderer.Framework.Text;

public static class TextPositionUtil
{
    // Returns a list of offsets of all '\n' characters, plus the string length
    // as the final entry.
    public static List<uint> GetLineEndings(string text)
    {
        var result = new List<uint>();
        int start = 0;
        while (start < text.Length)
        {
            int lineEnd = text.IndexOf('\n', start);
            if (lineEnd == -1)
                break;
            result.Add((uint)lineEnd);
            start = lineEnd + 1;
        }
        result.Add((uint)text.Length);
        return result;
    }
}
