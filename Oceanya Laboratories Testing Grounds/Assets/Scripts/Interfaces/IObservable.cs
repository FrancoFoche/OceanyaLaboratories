public enum ObservableActionTypes
{
    ItemContextActivated,
    SkillContextActivated
}

/// <summary>
/// Interface that notifies an IObservable about a change.
/// </summary>
public interface IObservable
{
    void AddToObserver(IObserver obs);
    void RemoveFromObserver(IObserver obs);
    void NotifyObserver(ObservableActionTypes action);
}
