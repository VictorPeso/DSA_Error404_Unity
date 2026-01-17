using UnityEngine;
using UnityEngine.AI;

public class WanderState : BaseState
{
    private float waitTimer;

    public override void Enter()
    {
        // YA NO buscamos destino al entrar.
        // Esperamos a estar seguros de que hay suelo en el Update (Perform).
    }

    public override void Perform()
    {
        // 1. Si veo al jugador -> ATACAR
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
            return;
        }

        // 2. SEGURIDAD TOTAL (Aquí estaba el fallo)
        // Si el agente no está activo o NO está sobre el NavMesh (el suelo azul)...
        // ...simplemente no hacemos nada y esperamos al siguiente frame.
        if (!enemy.Agent.isOnNavMesh || !enemy.Agent.isActiveAndEnabled)
        {
            return;
        }

        // 3. Patrullar
        // Si no tenemos camino o hemos llegado al destino...
        if (!enemy.Agent.hasPath || enemy.Agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 1f) // Esperamos 1 segundo antes de buscar otro punto
            {
                FindRandomDestination();
                waitTimer = 0;
            }
        }
    }

    public override void Exit() { }

    void FindRandomDestination()
    {
        // Doble seguridad antes de movernos
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