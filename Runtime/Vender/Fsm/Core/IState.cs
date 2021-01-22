using System;
using System.Collections.Generic;

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

        void ChangeState(string stateName);

        void PushState(string stateName);

        void PopState();

        void TriggerEvent(string id, EventArgs eventArgs);

        #endregion
    }
}