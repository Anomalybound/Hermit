using System;
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

        public override async Task<IState> BuildState()
        {
            var root = new State();

            var context = Context.GlobalContext;

            var types = GetType().Assembly.GetTypes()
                .Where(x => typeof(GameProcedure<TProcedureController, TProcedureIndex>).IsAssignableFrom(x));

            var procedures = new List<GameProcedure<TProcedureController, TProcedureIndex>>();

            foreach (var type in types)
            {
                if (context.Create(type) is GameProcedure<TProcedureController, TProcedureIndex> instance)
                {
                    await instance.SetContext((TProcedureController) this);
                    procedures.Add(instance);
                }
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

            if (procedures.Any(p => p.Index.Equals(InitState))) { await ChangeState(InitState); }
            else
            {
                var first = procedures[0].Index;
                Her.Warn($"Procedure of [{InitState}] is no available, change to {first} instead.");
                await ChangeState(first);
            }

            return Root;
        }

        #region Facade

        public async Task ChangeState(TProcedureIndex index)
        {
            await Root.ChangeState(index.ToString(CultureInfo.InvariantCulture));
        }

        public async Task PushState(TProcedureIndex index)
        {
            await Root.PushState(index.ToString(CultureInfo.InvariantCulture));
        }

        public async Task ChangeState(string stateName)
        {
            await Root.ChangeState(stateName);
        }

        public async Task PushState(string stateName)
        {
            await Root.PushState(stateName);
        }

        public async Task PopState()
        {
            await Root.PopState();
        }

        public async Task TriggerEvent(string eventId, EventArgs args)
        {
            await Root.TriggerEvent(eventId, args);
        }

        #endregion
    }
}