using UnityEngine;

public class BigAttackState : BigBaseState
{
    private float attackTimer;
    private Axe axe;

    private float moveTimer;
    private float losePlayerTimer;

    public override void Enter()
    {
        axe = enemy.GetComponentInChildren<Axe>();
    }
    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            attackTimer += Time.deltaTime;

            float distance = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);

            float range = 3.5f;

            enemy.transform.LookAt(enemy.Player.transform);

            if (distance > range)
            {
                enemy.Agent.isStopped = false;
                enemy.Agent.SetDestination(enemy.Player.transform.position);
            }
            else
            {
                enemy.Agent.isStopped = true;
                enemy.Agent.velocity = Vector3.zero;

                if (attackTimer > enemy.fireRate)
                {
                    ExecuteAxeAttack();
                    attackTimer = 0;
                }
            }
        }
        else
        {
            enemy.Agent.isStopped = false;
            {
                losePlayerTimer += Time.deltaTime;
                if (losePlayerTimer > 8)
                {
                    enemy.Agent.isStopped = false;
                    stateMachine.ChangeState(new BigPatrolState());
                }
            }
        }
    }
    

    void ExecuteAxeAttack()
    {
        enemy.GetComponent<Animator>().SetTrigger("AxeSwing");

        if (axe != null) axe.isAttacking = true;
    }

    public override void Exit() { }
}
