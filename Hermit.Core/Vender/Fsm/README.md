# wFsm
This is a tool for creating hierarchical finite state machines. You can handmade every single state or use the fluent state machine builder provided.

It can be used in c# based software development. Extra support for the Unity3D game engine.

## Structure
`/Core` Codebase

`/Builder` StateBuilder & StateMachineBuilder 

`/Unity` Unity supports

## Intall
You can either

- add this repo as a submodule to your project
- clone/download and unzip WaEvent folder to your project


## Tutorial

### State

You can use `State` class provided, or make a new one by your own.

```csharp

public class SampleState : StateBase
{
    public string SampleText = "Hello";
    
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter State.");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit State.");
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        Debug.Log("Update State");
    }
}
```

Config it by hand.

```csharp

    public IState AssemblyState()
    {
        var state = new State();
        state.SetEnterAction(OnStateEntered);
        state.SetExitAction(() => { Debug.Log("Exit State."); });
        state.AddCondition(ShouldChange, () => state.Parent.ChangeState("ChangeToState"));
        state.SetEvent("PopEvent", args => state.Parent.PopState());
        return state;
    }

    public bool ShouldChange()
    {
        return false;
    }

    public void OnStateEntered()
    {
        Debug.Log("Enter State.");
    }
```

### State Machine Builder

Use fluent API to build state machine:

```csharp
public IState AssemblyState()
    {
        // Create a new state machine builder
        // will also create a root state 
        return new StateMachineBuilder<State>()
        
            // Add new StateBase(SampleState here)
            .State<SampleState>("State1")
            
                // Set Enter Action
                .OnEnter(OnStateEntered)
                
                // Add conditions (condition will check every frame)
                .Condition(ShouldChange, state => Debug.Log("Condition1 met."))
                .Condition(ShouldChange2, state => Debug.Log("Condition2 met."))
                
                // Add events (event action only get fired when event is triggered)
                .Event("Push", (state, args) => state.Parent.PushState("State2"))
                
            // End State construction, return to state machine builder
            .End()
            
            // Return Root state
            // Add new StateBase(State here)
            .State("State2")
                .OnEnter(state => {Debug.Log("Enter State2");})
                .Condition(ShouldChange, state => Debug.Log("Condition1 met."))
                .Condition(ShouldChange2, state => Debug.Log("Condition2 met."))
                .Event("PopEvent", (state, args) => state.Parent.PopState())
            .End()
            .Build();
    }

    public void OnStateEntered(SampleState state)
    {
        Debug.Log("Enter State. " + state.SampleText);
    }

    public bool ShouldChange()
    {
        return false;
    }

    public bool ShouldChange2()
    {
        return true;
    }
```

### Fsm Container

You can inherit `FsmContainer` and implement `BuildState` method to build a FSM in Unity quickly.

```csharp
public class SampleFsm : FsmContainer
{
    public override IState BuildState()
    {
        var builder = new StateMachineBuilder<State>();
        var root = builder
            .State("State1")
                .OnEnter(state => { Debug.Log("Enter State1."); })
                .OnExit(state => { Debug.Log("Leave State1."); })
                .Condition(() => Input.GetKeyDown(KeyCode.A), state => state.Parent.ChangeState("State2"))
                .Condition(() => Input.GetKeyDown(KeyCode.O), state => state.Parent.PushState("State2"))
                .Condition(() => Input.GetKeyDown(KeyCode.R), state => state.Parent.PopState())
            .End()
            .State("State2")
                .OnEnter(state => { Debug.Log("Enter State2."); })
                .OnExit(state => { Debug.Log("Leave State2."); })
                .Condition(() => Input.GetKeyDown(KeyCode.D), state => state.Parent.ChangeState("State1"))
                .Condition(() => Input.GetKeyDown(KeyCode.P), state => state.Parent.PushState("State1"))
                .Condition(() => Input.GetKeyDown(KeyCode.R), state => state.Parent.PopState())
            .End().Build();

        root.ChangeState("State1");
        return root;
    }
}
```

Also, `FsmContainer` has its editor inspector to help user debugging the FSM.

![inspector](https://github.com/cushmily/wFsm/blob/master/Docs/inspector.png?raw=true)



### Game Procedure Controller

Game procedure controller is a Unity specified component, based on `FsmContainer`.

First, you can create a GameController that inherits from the GameProcedureController and create an enumeration for it to indicate the game states.

```csharp
using wLib.Procedure;

public enum KarmaGameState
{
    Initialization,
    MainMenu,
	NewGame
}

public class KarmaGameController : GameProcedureController<KarmaGameController, KarmaGameState>
{
	public float SomeFloat;
}
```

Then, you can create a GameProcedure for each state and assign them the corresponding state. If available, GameProcedureController will automatically enter first game procedure.

```csharp
using wLib.Procedure;

public class InitializationProcedure : GameProcedure<KarmaGameController, KarmaGameState>
    {
    	// Setting represented state.
        public override KarmaGameState Index => KarmaGameState.Initialization;

        /// <summary>
        /// Enter will be called whenever controller change to this state.
        /// </summary>
        /// <param name="controller"></param>
        public override void Enter(KarmaGameController controller) { }

        /// <summary>
        /// Exit will be called whenever controller left this state.
        /// </summary>
        /// <param name="controller"></param>
        public override void Exit(KarmaGameController controller) { }

        /// <summary>
        /// Init will be called after setting a context.
        /// </summary>
        /// <param name="controller"></param>
        public override void Init(KarmaGameController controller) { }

        /// <summary>
        /// Update will be called in every controller update step. 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="deltaTime"></param>
        public override void Update(KarmaGameController controller, float deltaTime) { }

        /// <summary>
        /// The controller instance.
        /// </summary>
        public override KarmaGameController Context { get; protected set; }
    }
```

Game Procedure is inherited from StateBase so that you can still do Add Event/Condition actions.

```csharp
// Add events
procedure.AddEvent("eventId", args => {});

// Add conditions
procedure.AddCondition(() => false, () => { });
```

Game Procedure Controller is inherited from FsmCotnaier so that you can still do push/pop actions.

```csharp
// Directly change state, ignore current stack.
controller.ChangeState(KarmaGameState.NewGame);

// Push a new state into stack.
controller.PushState(KarmaGameState.NewGame);

// Pop the toppeest state of the stack.
controller.PopState();

// Trigger a event with some args.
controller.TriggerEvent("eventId", EventArgs.Empty);
```

