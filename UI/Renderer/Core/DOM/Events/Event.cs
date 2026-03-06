namespace UI.Renderer.Core.DOM.Events;

public class Event
{
    public enum Bubbles
    {
        kYes,
        kNo,
    };

    public enum Cancelable
    {
        kYes,
        kNo,
    }

    public enum PhaseType
    {
        kNone = 0,
        kCapturingPhase = 1,
        kAtTarget = 2,
        kBubblingPhase = 3
    }

    public enum ComposedMode
    {
        kComposed,
        kScoped,
    }

    public enum PassiveMode
    {
        // Not passive, default initialized.
        kNotPassiveDefault,
        // Not passive, explicitly specified.
        kNotPassive,
        // Passive, explicitly specified.
        kPassive,
        // Passive, not explicitly specified and forced due to document level
        // listener.
        kPassiveForcedDocumentLevel,
        // Passive, default initialized.
        kPassiveDefault,
    }

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

    PassiveMode handling_passive_;
    PhaseType event_phase_;

    EventTarget current_target_;
    EventTarget target_;
    // Set eagerly in SetPseudoElementTarget() while the pseudo is connected.
    // Storing CSSPseudoElement directly avoids calling From() on a possibly
    // disconnected pseudo later during dispatch.
    //CSSPseudoElement pseudo_element_target_;
    Event underlying_event_; // const
    //EventPath event_path_;
    // The monotonic platform time in seconds, for input events it is the
    // event timestamp provided by the host OS and reported in the original
    // WebInputEvent instance.
    long platform_time_stamp_;

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
