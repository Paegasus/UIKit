namespace UI.Renderer.Core.DOM.Events;

public class Event
{
    //AtomicString type_;
    string type_;
    bool bubbles_ = true;
    bool cancelable_ = true;
    bool composed_ = true;

    bool propagation_stopped_ = true;
    bool immediate_propagation_stopped_ = true;
    bool default_prevented_ = true;
    bool default_handled_ = true;
    bool was_initialized_ = true;
    bool is_trusted_ = true;

    // Whether preventDefault was called on uncancelable event.
    bool prevent_default_called_on_uncancelable_event_ = true;

    // Whether any of listeners have thrown an exception or not.
    // Corresponds to |legacyOutputDidListenersThrowFlag| in DOM standard.
    // https://dom.spec.whatwg.org/#dispatching-events
    // https://dom.spec.whatwg.org/#concept-event-listener-inner-invoke
    bool legacy_did_listeners_throw_flag_ = true;

    bool fire_only_capture_listeners_at_target_ = true;
    bool fire_only_non_capture_listeners_at_target_ = true;

    bool copy_event_path_from_underlying_event_ = true;

    bool invocation_target_in_shadow_tree_ = true;

    public void setCancelBubble(bool cancel)
    {
        if (cancel)
            propagation_stopped_ = true;
    }
}
