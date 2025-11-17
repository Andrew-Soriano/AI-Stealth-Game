using UnityEngine;

//State for chasing the player
public class PursuitState : BaseState
{
    public PursuitState(EnemyStateMachineController controller, EnemyStateFactory factory) : base(controller, factory)
    {
    }

    public override void CheckSwitchState()
    {
        //If we are close enough, go on the offensive
        if(_controller.playerVision() &&
           Vector3.Distance(_controller.Trans.position, _controller.Player.position) < _controller.AttackRange)
        {
            this.SwitchState(_factory.AttackState());
        }
        //If we reach the destination without switching to an attack state, the player is not there anymore
        else if (_controller.HasReachedDestination() && !_controller.playerVision())
        {
            this.SwitchState(_controller.DefaultState); //Switch back to the default state, end pursuit
        }
    }

    public override void EnterState()
    {
        //We saw the player, and are moving to his last known location. Update accordingly
        _controller.Goal = _controller.Player.position;
        _controller.Agent.destination = _controller.Goal;

        //For debug, go to new material
        _controller.Renderer.material = _controller.SkinMaterial[3];
    }

    public override void ExitState()
    {
        //Player lost, return to default goal on exit
        _controller.Goal = _controller.Trans.position;
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //Check if we should switch (such as if we are close enough to attack)
        CheckSwitchState();

        //If we can still see the player, update the goal to their new position
        if(_controller.playerVision())
        {
            _controller.Goal = _controller.Player.position;
        }
    }

}
