using System.Threading.Tasks;

namespace Hermit.Fsm
{
    public interface IPureState
    {
        #region Lifetime

        Task EnterAsync();

        void Update(float deltaTime);

        Task ExitAsync();

        float ElapsedTime { get; }

        #endregion
    }
}