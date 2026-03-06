namespace UI.Renderer.Framework.Text;

public readonly struct AtomicString // : IEquatable<AtomicString>
{
    private readonly string? _value;

    public AtomicString(string? value)
    {
        _value = value != null ? string.Intern(value) : null;
    }

    public bool IsNull  => _value == null;
    public bool IsEmpty => _value == null || _value.Length == 0;

    public override string? ToString() => _value;
    
    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    // Fast pointer comparison since both sides are interned
    public bool Equals(in AtomicString other) => ReferenceEquals(_value, other._value);
    public override bool Equals(object? obj) => obj is AtomicString other && Equals(other);

    public static bool operator ==(in AtomicString a, in AtomicString b) => a.Equals(b);
    public static bool operator !=(in AtomicString a, in AtomicString b) => !a.Equals(b);

    public static implicit operator string?(AtomicString s) => s._value;
    public static explicit operator AtomicString(string? s) => new(s);
}
