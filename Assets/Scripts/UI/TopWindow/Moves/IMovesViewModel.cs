using ReactiveProperty;

namespace UI.TopWindow.Moves
{
    public interface IMovesViewModel
    {
        IReactiveProperty<string> TotalMove { get; }
    }
}
