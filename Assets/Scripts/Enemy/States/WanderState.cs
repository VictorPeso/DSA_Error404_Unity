using UnityEngine;
using UnityEngine.AI;

public class WanderState : BaseState
{
    private float waitTimer;

    public override void Enter()
    {
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
            return;
        }

        if (!enemy.Agent.isOnNavMesh || !enemy.Agent.isActiveAndEnabled)
        {
            return;
        }

        if (!enemy.Agent.hasPath || enemy.Agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 1f)
            {
                FindRandomDestination();
                waitTimer = 0;
            }
        }
    }

    public override void Exit()
    {
    }

    void FindRandomDestination()
    {
        if (!enemy.Agent.isOnNavMesh) return;

        Vector3 randomDir = Random.insideUnitSphere * 10f;
        randomDir += enemy.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, 10f, NavMesh.AllAreas))
        {
            enemy.Agent.SetDestination(hit.position);
        }
    }
}