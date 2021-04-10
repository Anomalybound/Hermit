using System;
using System.Collections.Generic;

namespace Hermit.Fsm
{
    public interface IState : IPureState
    {
        #region Properties

        IState Parent { get; set; }

        Dictionary<string, IState> Children { get; }

        Stack<IState> ActiveChildrenStates { get; }

        #endregion

        #region Operations

        void ChangeState(string stateName);

        void PushState(string stateName, bool allowDuplicated = false);

        void PopState();

        void TriggerEvent(string id, EventArgs eventArgs);

        #endregion
    }
}