using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shootTimer;

    public override void Enter()
    {
        // Debug.Log("Entering Attack State");
    }
    public override void Perform()
    {
        if(enemy.CanSeePlayer())
        {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            shootTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);
            if (shootTimer > enemy.fireRate)
            {
                Shoot();
            }
            if (moveTimer > Random.Range(3,8))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            if(losePlayerTimer > 8)
            {
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public override void Exit()
    {
        // Debug.Log("Exiting Attack State");
    }

    public void Shoot()
    {
        Transform gunbarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, 
            gunbarrel.position, enemy.transform.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Bullet prefab is missing a Rigidbody!");
            return;
        }

        Vector3 shootDirection = (enemy.Player.transform.position  - gunbarrel.transform.position).normalized;
        //bullet.GetComponent<Rigidbody>().linearVelocity = shootDirection * 40f;
        Debug.Log("Pew Pew");
        rb.linearVelocity = shootDirection * 40f;
        shootTimer = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
