//Used to generate new instances of states when the enemy switches their state.
public class EnemyStateFactory
{
    private EnemyStateMachineController _controller;
    
    public EnemyStateFactory(EnemyStateMachineController controller)
    {
        _controller = controller;
    }

    public BaseState Idle()
    {
        return new IdleState(_controller, this);
    }

    public BaseState Patrol()
    {
        return new PatrolState(_controller, this);
    }

    public BaseState MoveToNoise()
    {
        return new MoveToNoiseState(_controller, this);
    }
    public BaseState KnifedState()
    {
        return new KnifedState(_controller, this);
    }
    public BaseState DeadState()
    {
        return new DeadState(_controller, this);
    }
}
