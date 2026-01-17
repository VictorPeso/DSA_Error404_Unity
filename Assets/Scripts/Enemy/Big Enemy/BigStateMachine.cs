using UnityEngine;

public class BigStateMachine : MonoBehaviour
{
    public BigBaseState BigActiveState;

    public void Initialize()//BaseState startingState)
    {
        ChangeState(new BigPatrolState());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (BigActiveState != null)
        {
            BigActiveState.Perform();
        }
    }
    public void ChangeState(BigBaseState newState)
    {
        if (BigActiveState != null)
        {
            BigActiveState.Exit();
        }
        BigActiveState = newState;
        if (BigActiveState != null)
        {
            BigActiveState.stateMachine = this;

            BigActiveState.enemy = GetComponent<BigEnemy>();
         
            BigActiveState.Enter();
        }
    }
}
