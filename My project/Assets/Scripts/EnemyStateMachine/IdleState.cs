using UnityEngine;
using static NoiseHandler;

//Controls how an enemy behaves when they have nothing to do (such as guarding a fixed position)
public class IdleState : BaseState, ICanBeDamaged, ICanHear
{
    public IdleState(EnemyStateMachineController controller, EnemyStateFactory factory) : base(controller, factory)
    {
    }

    public override void CheckSwitchState()
    {

    }

    //On entering state, update the agent destination to current position so the agent no longer moves
    public override void EnterState()
    {
        _controller.MyState = EnemyStateType.Idle;
        _controller.Agent.destination = _controller.Trans.position;
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
    }

    //React to noises. Note: Idle state does not subscribe to noises, this is invoked by a the alert manager for better full scene control
    public void HearNoise(NoiseID id, Transform origin, double range)
    {
        Debug.Log("I heard a noise!");

        if (Vector3.Distance(origin.position, _controller.Trans.position) <= range)
        {
            Debug.Log("It was in Range");
            _controller.Goal = origin.position;
            this.SwitchState(_factory.MoveToNoise());
        }
        else
        {
            Debug.Log("It was out of range!");
            Debug.Log(Vector3.Distance(origin.position, _controller.Trans.position));
        }
    }

    //React to being stabbed in the back.
    public void getBackStabbed()
    {
        this.SwitchState(_factory.KnifedState());
    }
}
