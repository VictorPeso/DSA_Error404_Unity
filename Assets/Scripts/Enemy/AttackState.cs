using UnityEngine;

public class AttackState : BaseState
{
    // --- ESTAS SON LAS VARIABLES QUE FALTABAN ---
    private float losePlayerTimer;
    private float shootTimer; // <--- ¡Aquí está el cronómetro!
    // --------------------------------------------

    public override void Enter()
    {
        enemy.Agent.isStopped = true; // Se queda quieto para apuntar
        shootTimer = 0;
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer()) // MIENTRAS TE VEA
        {
            losePlayerTimer = 0;

            // 1. Girar hacia ti
            Vector3 lookPos = enemy.Player.transform.position - enemy.transform.position;
            lookPos.y = 0;
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);

            // 2. Disparar
            // Ahora sí funciona porque shootTimer y fireRate existen
            shootTimer += Time.deltaTime;
            if (shootTimer > enemy.fireRate)
            {
                Shoot();
                shootTimer = 0;
            }
        }
        else // SI TE PIERDE DE VISTA
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 2f)
            {
                stateMachine.ChangeState(new WanderState()); // Vuelve a patrullar
            }
        }
    }

    public override void Exit()
    {
        enemy.Agent.isStopped = false; // Vuelve a moverse
    }

    void Shoot()
    {
        Vector3 shootOrigin = enemy.transform.position;
        if (enemy.gunBarrel != null)
        {
            shootOrigin = enemy.gunBarrel.position;
        }
        else
        {
            shootOrigin += Vector3.up * 1.5f + enemy.transform.forward * 0.6f;
        }

        Vector3 targetPos = enemy.Player.transform.position + Vector3.up * 1.0f;
        Vector3 direction = (targetPos - shootOrigin).normalized;

        Debug.DrawRay(shootOrigin, direction * enemy.sightDistance, Color.blue, 1f);

        if (Physics.Raycast(shootOrigin, direction, out RaycastHit hit, enemy.sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerHealth>()?.TakeDamage(10);
            }
        }
    }

}