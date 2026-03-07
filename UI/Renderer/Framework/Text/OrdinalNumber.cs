namespace UI.Renderer.Framework.Text;

// An abstract number of element in a sequence. The sequence has a first
// element. This type should be used instead of integer because 2
// contradicting traditions can call a first element '0' or '1' which makes
// integer type ambiguous.
public readonly struct OrdinalNumber : IEquatable<OrdinalNumber>
{
    private readonly int zero_based_value_;

    private OrdinalNumber(int zeroBased) => zero_based_value_ = zeroBased;

    public static OrdinalNumber FromZeroBasedInt(int zeroBased) => new(zeroBased);
    public static OrdinalNumber FromOneBasedInt(int oneBased) => new(oneBased - 1);

    public static OrdinalNumber First()       => new(0);
    public static OrdinalNumber BeforeFirst() => new(-1);

    public int ZeroBasedInt => zero_based_value_;
    public int OneBasedInt  => zero_based_value_ + 1;

    public bool Equals(OrdinalNumber other) => zero_based_value_ == other.zero_based_value_;
    public override bool Equals(object? obj) => obj is OrdinalNumber other && Equals(other);
    public override int GetHashCode() => zero_based_value_.GetHashCode();
    public static bool operator ==(OrdinalNumber a, OrdinalNumber b) => a.Equals(b);
    public static bool operator !=(OrdinalNumber a, OrdinalNumber b) => !a.Equals(b);
}
