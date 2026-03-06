namespace UI.Renderer.Core.DOM.Events;

public abstract class EventTarget
{
    // Pure virtual — must be implemented by derived classes
    public abstract string InterfaceName { get; }
    public abstract ExecutionContext? GetExecutionContext();

    // Virtual with default null returns — override in specific subclasses
    public virtual Node? ToNode() => null;
    public virtual DOMWindow? ToDOMWindow() => null;
    public virtual LocalDOMWindow? ToLocalDOMWindow() => null;
    public virtual MessagePort? ToMessagePort() => null;
    public virtual ServiceWorker? ToServiceWorker() => null;
    public virtual void ResetEventQueueStatus(string eventType) { }

    // Non-virtual — sealed behavior, not overridable
    public bool AddEventListener(string eventType, EventListener listener)
    {
        // implementation
    }

    public bool RemoveEventListener(string eventType, EventListener listener)
    {
        // implementation
    }

    public Observable When(string eventType, ObservableEventListenerOptions? options)
    {
        // implementation
    }
}