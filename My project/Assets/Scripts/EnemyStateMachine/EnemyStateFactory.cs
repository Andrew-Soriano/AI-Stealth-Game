//Used to generate new instances of states when the enemy switches their state.
using UnityEngine;

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

    public BaseState SoloPursuitState()
    {
        return new SoloPursuitState(_controller, this);
    }

    public BaseState SoloAttackState()
    {
        return new SoloAttackState(_controller, this);
    }
    public BaseState FormationPursuitState(Transform leader, Vector3 formationOffset)
    {
        return new FormationPursuitState(_controller, this, leader, formationOffset);
    }

    public BaseState FormationAttackState(Transform leader, Vector3 formationOffset)
    {
        return new FormationAttackState(_controller, this, leader, formationOffset);
    }

    public BaseState SpawnPursuitState(Vector3 pos, EnemySpawnerController spawner)
    {
        return new SpawnPursuitState (_controller, this, pos, spawner);
    }

    public BaseState SpawnAttackState(Vector3 pos, EnemySpawnerController spawner)
    {
        return new SpawnAttackState(_controller, this, pos, spawner);
    }
    public BaseState SpawnReturnState(Vector3 pos, EnemySpawnerController spawner)
    {
        return new SpawnReturnState(_controller, this, pos, spawner);
    }
}
