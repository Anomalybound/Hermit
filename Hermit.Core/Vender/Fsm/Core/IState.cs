using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hermit.Fsm
{
    public interface IState : IPureState
    {
        #region Properties

        IState Parent { get; set; }

        Dictionary<string, IState> Children { get; }

        Stack<IState> ActiveStates { get; }

        #endregion

        #region Operations

        Task ChangeStateAsync(string stateName);

        Task PushStateAsync(string stateName);

        Task PopStateAsync();

        Task TriggerEventAsync(string id, EventArgs eventArgs);

        #endregion
    }
}