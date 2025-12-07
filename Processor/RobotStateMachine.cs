using NLog;
using Stateless;

namespace Processor;

class RobotStateMachine
{
    private readonly Logger _logs;

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

    public RobotStateMachine(Logger log)
    {
        _logs = log;

        _stateMachine = new StateMachine<State, string>(State.DirectControl);
        _worldModel = new WorldModel();

        _stateMachine.Configure(State.DirectControl)
            .OnEntry(StartDirectControl)
            .Permit(_explore, State.Explore)
            .Permit(_greet, State.Greeting);

        _stateMachine.Configure(State.Explore)
            .OnEntry(StartExplore)
            .Permit(_direct, State.DirectControl)
            .Permit(_greet, State.Greeting);

        _stateMachine.Configure(State.Greeting)
            .OnEntry(StartGreeting)
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
        _logs.Info("State changed to DirectControl");

        ClearAll();
    }

    public void StartGreeting()
    {
        _logs.Info("State changed to Greeting");

        ClearAll();
    }

    public void StartExplore()
    {
        _logs.Info("State changed to Explore");

        ClearAll();
    }

    public WorldModel WorldModel => _worldModel;
}