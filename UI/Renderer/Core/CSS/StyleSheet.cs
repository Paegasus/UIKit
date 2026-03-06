using UI.Renderer.Core.DOM;

namespace UI.Renderer.Core.CSS;

public abstract class StyleSheet
{
    public abstract bool Disabled { get; set; }
    public abstract Node? OwnerNode { get; }
    public virtual StyleSheet? ParentStyleSheet => null;
    public abstract string Href { get; }
    public abstract string Title { get; }
    public virtual MediaList? Media => null;
    public abstract string Type { get; }
    public virtual CSSRule? OwnerRule => null;
    public abstract void ClearOwnerNode();
    public abstract Uri BaseUrl { get; }
    public abstract bool IsLoading { get; }
    public virtual bool IsCSSStyleSheet => false;
}
