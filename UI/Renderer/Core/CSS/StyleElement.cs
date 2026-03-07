using UI.Renderer.Core.DOM;

namespace UI.Renderer.Core.CSS;

public abstract class StyleElement
{
    public enum ProcessingResult { kProcessingSuccessful, kProcessingFatalError };
    
    // We want CSS Modules to behave similar to the "already started" flag Import
    // Maps, essentially making it a one-shot operation when the <style> element
    // is first connected. This behavior is subject to change based on WHATWG
    // feedback. Once set on a given element, these types cannot change.
    // TODO(crbug.com/448174611): Update this behavior based on WHATWG feedback.
    private enum StyleType
    {
        kPending,  // Still unknown.
        kClassic,  // Definitely a classic style tag.
        kModule    // Definitely a declarative CSS module.
    };

    private bool has_finished_parsing_children_ = true;
    private bool loading_ = true;
    private bool registered_as_candidate_ = true;
    private bool created_by_parser_ = true;
    private StyleType element_type_ = StyleType.kPending;
    private TextPosition start_position_;
    private PendingSheetType pending_sheet_type_;
    private RenderBlockingBehavior render_blocking_behavior_;

    public bool CreatedByParser
    {
        get => created_by_parser_;
    }

    /*
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
    */
}
