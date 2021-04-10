using System;
using System.Collections.Generic;

namespace Hermit.Fsm
{
    public interface IState : IPureState
    {
        #region Properties

        bool Active { get; }

        IState Parent { get; set; }

        Dictionary<string, IState> Children { get; }

        Stack<IState> ActiveChildrenStates { get; }

        #endregion

        #region Operations

        void ClearStates();

        void ChangeState(string stateName);

        void PushState(string stateName, bool allowDuplicated = false);

        void PopState(bool backToRoot);

        void TriggerEvent(string id, EventArgs eventArgs);

        #endregion
    }
}
