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

        Task ChangeState(string stateName);

        Task PushState(string stateName);

        Task PopState();

        Task TriggerEvent(string id, EventArgs eventArgs);

        #endregion
    }
}