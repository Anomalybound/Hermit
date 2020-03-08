using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hermit.Fsm;
using UnityEngine;
using Hermit.Injection;

namespace Hermit.Procedure
{
    public abstract class GameProcedureController<TProcedureController, TProcedureIndex> : FsmContainer
        where TProcedureController : GameProcedureController<TProcedureController, TProcedureIndex>
        where TProcedureIndex : struct, IConvertible
    {
        private readonly Dictionary<TProcedureIndex, GameProcedure<TProcedureController, TProcedureIndex>> _indices =
            new Dictionary<TProcedureIndex, GameProcedure<TProcedureController, TProcedureIndex>>();

        private readonly Dictionary<IState, TProcedureIndex> _indexLookup = new Dictionary<IState, TProcedureIndex>();

        [SerializeField, HideInInspector]
        private TProcedureIndex initState = default(TProcedureIndex);

        public TProcedureIndex InitState => initState;

        public TProcedureIndex Current => _indexLookup[Root.ActiveStates.Peek()];

        public event Action<IState, IState> OnStateChanged;

        protected override IState BuildState()
        {
            HermitEvent.Send(HermitEvent.ProcedureBuildStateStarted);

            var root = new State();

            var types = GetType().Assembly.GetTypes()
                .Where(x => !x.IsAbstract &&
                            typeof(GameProcedure<TProcedureController, TProcedureIndex>).IsAssignableFrom(x));

            var procedures = new List<GameProcedure<TProcedureController, TProcedureIndex>>();

            foreach (var type in types)
            {
                if (!(Her.Create(type) is GameProcedure<TProcedureController, TProcedureIndex> instance))
                {
                    continue;
                }

                instance.SetContext((TProcedureController) this);
                procedures.Add(instance);
            }

            procedures = procedures.OrderBy(x => x.Index).ToList();

            foreach (var procedure in procedures)
            {
                var id = procedure.Index;

                if (_indices.ContainsKey(id))
                {
                    Debug.LogErrorFormat("{0}[{1}] already added.", id, procedure.GetType().Name);
                    continue;
                }

                _indices.Add(id, procedure);
                _indexLookup.Add(procedure, id);
                root.AddChild(id.ToString(CultureInfo.InvariantCulture), procedure);
            }

            Root = root;
            if (procedures.Count <= 0) { return Root; }

            if (procedures.Any(p => p.Index.Equals(InitState))) { ChangeState(InitState); }
            else
            {
                var first = procedures[0].Index;
                Her.Warn($"Procedure of [{InitState}] is no available, change to {first} instead.");
                ChangeState(first);
            }

            HermitEvent.Send(HermitEvent.ProcedureBuildStateFinished);
            return Root;
        }

        #region Facade

        public void ChangeState(TProcedureIndex index)
        {
            TrackStateEvent(() => Root.ChangeState(index.ToString(CultureInfo.InvariantCulture)));
        }

        public void PushState(TProcedureIndex index)
        {
            TrackStateEvent(() => Root.PushState(index.ToString(CultureInfo.InvariantCulture)));
        }

        public void ChangeState(string stateName)
        {
            TrackStateEvent(() => Root.ChangeState(stateName));
        }

        public void PushState(string stateName)
        {
            TrackStateEvent(() => Root.PushState(stateName));
        }

        public void PopState()
        {
            TrackStateEvent(() => Root.PopState());
        }

        public void TriggerEvent(string eventId, EventArgs args)
        {
            TrackStateEvent(() => Root.TriggerEvent(eventId, args));
        }

        private void TrackStateEvent(Action asyncAction)
        {
            var previousActiveState = Root.ActiveStates.Count > 0 ? Root.ActiveStates.Peek() : null;

            asyncAction?.Invoke();

            var currentActiveStates = Root.ActiveStates.Count > 0 ? Root.ActiveStates.Peek() : null;

            if (previousActiveState != currentActiveStates)
            {
                OnStateChanged?.Invoke(previousActiveState, currentActiveStates);
            }
        }

        #endregion
    }
}