namespace UI.Renderer.Framework.Text;

// TextPosition structure specifies coordinates within a text resource.
// Used mostly for saving script source position.
public readonly struct TextPosition : IEquatable<TextPosition>
{
    public readonly OrdinalNumber Line;
    public readonly OrdinalNumber Column;

    public TextPosition(OrdinalNumber line, OrdinalNumber column)
    {
        Line   = line;
        Column = column;
    }

    // A 'minimum' value of position, used as a default value.
    public static TextPosition MinimumPosition() =>
        new(OrdinalNumber.First(), OrdinalNumber.First());

    // A value with line value less than minimum; used as an impossible position.
    public static TextPosition BelowRangePosition() =>
        new(OrdinalNumber.BeforeFirst(), OrdinalNumber.BeforeFirst());

    public OrdinalNumber ToOffset(ReadOnlySpan<uint> lineEndings)
    {
        uint lineStartOffset = Line != OrdinalNumber.First()
            ? lineEndings[Line.ZeroBasedInt - 1] + 1
            : 0;
        return OrdinalNumber.FromZeroBasedInt(
            (int)(lineStartOffset + Column.ZeroBasedInt));
    }

    // A value corresponding to a position with given offset within text
    // having the specified line ending offsets.
    public static TextPosition FromOffsetAndLineEndings(
        uint offset, ReadOnlySpan<uint> lineEndings)
    {
        // std::lower_bound — find first element >= offset
        int lineIndex = lineEndings.BinarySearch(offset);
        if (lineIndex < 0)
            lineIndex = ~lineIndex; // BinarySearch returns bitwise complement if not found

        uint lineStartOffset = lineIndex > 0
            ? lineEndings[lineIndex - 1] + 1
            : 0;
        int column = (int)(offset - lineStartOffset);
        return new TextPosition(
            OrdinalNumber.FromZeroBasedInt(lineIndex),
            OrdinalNumber.FromZeroBasedInt(column));
    }

    public bool Equals(TextPosition other) =>
        Line == other.Line && Column == other.Column;
    public override bool Equals(object? obj) =>
        obj is TextPosition other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Line, Column);
    public static bool operator ==(TextPosition a, TextPosition b) => a.Equals(b);
    public static bool operator !=(TextPosition a, TextPosition b) => !a.Equals(b);
}
