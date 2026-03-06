 namespace UI.Renderer.Core.DOM;

public class DOMTokenList // ElementRareDataField
{
    //private

    //SpaceSplitString token_set_;

    // Normal DOMTokenList instances is associated to an attribute name.
    // So |attribute_name_| is typically an html_names::kFooAttr.
    // CustomStateTokenList is associated to no attribute name.
    // |attribute_name_| is |g_null_name| in that case.
    //const QualifiedName attribute_name_;
    Element element_;
    bool is_in_update_step_ = false;

    //private void AddTokens(const Vector<String>&);
    //private void RemoveTokens(const Vector<String>&);
    //private void UpdateWithTokenSet(const SpaceSplitString&);

    /*
    public DOMTokenList(Element& element, const QualifiedName& attr)
    {
        attribute_name_(attr), element_(element)
    }
    */

    //public DOMTokenList(const DOMTokenList&) = delete;

    //public DOMTokenList& operator=(const DOMTokenList&) = delete;

    /*
    unsigned length() const { return token_set_.size(); }
    const AtomicString item(unsigned index) const;
    bool contains(const AtomicString&) const;
    void add(const Vector<String>&, ExceptionState&);
    void remove(const Vector<String>&, ExceptionState&);
    bool toggle(const AtomicString&, ExceptionState&);
    bool toggle(const AtomicString&, bool force, ExceptionState&);
    bool replace(const AtomicString& token,
               const AtomicString& new_token,
               ExceptionState&);
    bool supports(const AtomicString&, ExceptionState&);
    AtomicString value() const;
    void setValue(const AtomicString&);

    // This function should be called when the associated attribute value was
    // updated.
    void DidUpdateAttributeValue(const AtomicString& old_value,
                               const AtomicString& new_value);

    const SpaceSplitString& TokenSet() const { return token_set_; }
    // Add() and Remove() have DCHECK for syntax of the specified token.
    void Add(const AtomicString&);
    void Remove(const AtomicString&);
    */
    
    public Element GetElement()
    {
        return element_;
    }

    //virtual bool ValidateTokenValue(const AtomicString&, ExceptionState&) const;
}
