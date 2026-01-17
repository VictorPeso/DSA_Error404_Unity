public abstract class BigBaseState
{
    public BigEnemy enemy;
    public BigStateMachine stateMachine; 
    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}
