using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waitTime;
    public override void Enter()
    {
       
    }

    public override void Perform()
    {
        PatrolCycle();
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }

    public override void Exit()
    {
        //Debug.Log("Exiting Patrol State");
        //enemy.Agent.isStopped = true;
    }

    public void PatrolCycle()
    {
        if(enemy.Agent.remainingDistance < 0.2f)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= 3)
            {

                if (waypointIndex < enemy.path.waypoints.Count - 1)
                {
                    waypointIndex++;
                }
                else
                {
                    waypointIndex = 0;
                }
                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
                waitTime = 0;
            }
        }
    }
}
