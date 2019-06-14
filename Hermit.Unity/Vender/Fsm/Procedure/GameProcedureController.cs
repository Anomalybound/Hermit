using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Hermit.Fsm;
using UnityEngine;
using Hermit.Injection;

namespace Hermit.Procedure
{
    public abstract class GameProcedureController<TProcedureController, TProcedureIndex> :
        FsmContainer, IProcedureController
        where TProcedureController : GameProcedureController<TProcedureController, TProcedureIndex>
        where TProcedureIndex : struct, IConvertible
    {
        private readonly Dictionary<TProcedureIndex, GameProcedure<TProcedureController, TProcedureIndex>> Indices =
            new Dictionary<TProcedureIndex, GameProcedure<TProcedureController, TProcedureIndex>>();

        private readonly Dictionary<IState, TProcedureIndex> IndexLookup = new Dictionary<IState, TProcedureIndex>();

        [SerializeField]
        private TProcedureIndex _initState = default(TProcedureIndex);

        public TProcedureIndex InitState => _initState;

        public TProcedureIndex Current => IndexLookup[Root.ActiveStates.Peek()];

        public event Action<IState, IState> OnStateChanged;

        protected override async Task<IState> BuildState()
        {
            var root = new State();

            var context = Context.GlobalContext;

            var types = GetType().Assembly.GetTypes()
                .Where(x => typeof(GameProcedure<TProcedureController, TProcedureIndex>).IsAssignableFrom(x));

            var procedures = new List<GameProcedure<TProcedureController, TProcedureIndex>>();

            foreach (var type in types)
            {
                if (!(context.Create(type) is GameProcedure<TProcedureController, TProcedureIndex> instance))
                {
                    continue;
                }

                await instance.SetContext((TProcedureController) this);
                procedures.Add(instance);
            }

            procedures = procedures.OrderBy(x => x.Index).ToList();

            foreach (var procedure in procedures)
            {
                var id = procedure.Index;

                if (Indices.ContainsKey(id))
                {
                    Debug.LogErrorFormat("{0}[{1}] already added.", id, procedure.GetType().Name);
                    continue;
                }

                Indices.Add(id, procedure);
                IndexLookup.Add(procedure, id);
                root.AddChild(id.ToString(CultureInfo.InvariantCulture), procedure);
            }

            Root = root;
            if (procedures.Count <= 0) { return Root; }

            if (procedures.Any(p => p.Index.Equals(InitState))) { await ChangeStateAsync(InitState); }
            else
            {
                var first = procedures[0].Index;
                Her.Warn($"Procedure of [{InitState}] is no available, change to {first} instead.");
                await ChangeStateAsync(first);
            }

            return Root;
        }

        #region Facade

        public async Task ChangeStateAsync(TProcedureIndex index)
        {
            await TrackStateEventAsync(Root.ChangeStateAsync(index.ToString(CultureInfo.InvariantCulture)));
        }

        public async Task PushStateAsync(TProcedureIndex index)
        {
            await TrackStateEventAsync(Root.PushStateAsync(index.ToString(CultureInfo.InvariantCulture)));
        }

        public async Task ChangeStateAsync(string stateName)
        {
            await TrackStateEventAsync(Root.ChangeStateAsync(stateName));
        }

        public async Task PushStateAsync(string stateName)
        {
            await TrackStateEventAsync(Root.PushStateAsync(stateName));
        }

        public async Task PopStateAsync()
        {
            await TrackStateEventAsync(Root.PopStateAsync());
        }

        public async Task TriggerEventAsync(string eventId, EventArgs args)
        {
            await TrackStateEventAsync(Root.TriggerEventAsync(eventId, args));
        }

        private async Task TrackStateEventAsync(Task asyncAction)
        {
            var previousActiveState = Root.ActiveStates.Count > 0 ? Root.ActiveStates.Pop() : null;
            await asyncAction;
            var currentActiveStates = Root.ActiveStates.Count > 0 ? Root.ActiveStates.Pop() : null;

            if (previousActiveState != currentActiveStates)
            {
                OnStateChanged?.Invoke(previousActiveState, currentActiveStates);
            }
        }

        #endregion
    }
}