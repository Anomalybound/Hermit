namespace Hermit
{
    public interface IViewModelProvider
    {
        ViewModel GetViewModel();

        string GetViewModelTypeName { get; }
    }
}