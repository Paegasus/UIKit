namespace UI.Renderer.Framework.Text;

public readonly struct AtomicString : IEquatable<AtomicString>
{
    private readonly string? _value;

    public AtomicString(string? value)
    {
        _value = value != null ? string.Intern(value) : null;
    }

    public bool IsNull  => _value == null;
    public bool IsEmpty => _value == null || _value.Length == 0;

    // Fast pointer comparison since both sides are interned
    public bool Equals(AtomicString other) => ReferenceEquals(_value, other._value);

    public static implicit operator string?(AtomicString s) => s._value;
    public static explicit operator AtomicString(string? s) => new(s);

    public override bool Equals(object? obj) => obj is AtomicString other && Equals(other);
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    public static bool operator ==(AtomicString a, AtomicString b) => a.Equals(b);
    public static bool operator !=(AtomicString a, AtomicString b) => !a.Equals(b);
    public override string? ToString() => _value;
}
