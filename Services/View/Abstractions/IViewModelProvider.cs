namespace Hermit
{
    public interface IViewModelProvider
    {
        void SetViewModel(object context);

        ViewModel GetViewModel();

        string GetViewModelTypeName { get; }
        
        void ReBindAll();
    }
}