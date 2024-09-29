using System.Resources;
using Stateless;
using Stateless.Graph;

namespace Processor;

class RobotStateMachine
{
    private const string _explore = "state.explore";
    private const string _greet   = "state.greet";
    private const string _direct  = "state.direct";

    enum State
    {
        Explore,
        DirectControl,
        Greeting
    }

    private WorldModel _worldModel;

    private StateMachine<State, string> _stateMachine;

    public RobotStateMachine()
    {
        _stateMachine = new StateMachine<State, string>(State.DirectControl);
        _worldModel = new WorldModel();

        _stateMachine.Configure(State.DirectControl)
            .OnEntry(StartDirectControl)
            .OnExit(StopDirectControl)
            .Permit(_explore, State.Explore)
            .Permit(_greet, State.Greeting);

        _stateMachine.Configure(State.Explore)
            .OnEntry(StartExplore)
            .OnExit(StopExplore)
            .Permit(_direct, State.DirectControl)
            .Permit(_greet, State.Greeting);

        _stateMachine.Configure(State.Greeting)
            .OnEntry(StartGreeting)
            .OnExit(StopGreeting)
            .Permit(_explore, State.Explore)
            .Permit(_direct, State.DirectControl);
    }

    private void ClearAll()
    {
        _worldModel.Reset();
    }

    public void SetState(string state)
    {
        _stateMachine.Fire(state);
    }

    public void StartDirectControl()
    {
        ClearAll();
    }

    public void StopDirectControl()
    {
    }

    public void StartGreeting()
    {
        ClearAll();
    }

    public void StopGreeting()
    {
    }

    public void StartExplore()
    {
        ClearAll();
    }

    public void StopExplore()
    {

    }
}