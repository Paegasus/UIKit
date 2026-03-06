using UI.Renderer.Core.DOM;

namespace UI.Renderer.Core.CSS;

public interface IStyleSheet
{
    bool Disabled { get; set; }  // Property instead of separate get/set methods
    Node OwnerNode { get; }
    IStyleSheet ParentStyleSheet { get; }  // Note: returns interface, not concrete class
    //string Href { get; }
    string Title { get; }
    MediaList Media { get; }  // Consider if this should be nullable
    string Type { get; }
    CSSRule OwnerRule { get; }
    void ClearOwnerNode();
    //Uri BaseUrl { get; } 
    bool IsLoading { get; }
    bool IsCssStyleSheet { get; }
}