using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craby : MonoBehaviour
{
    public enum State{
        Moving,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField]
    private float groundCheckDistance, wallCheckDistance , movementSpeed, maxHealth , knockbackDuration;


    [SerializeField]
    
    private Transform groundCheck,wallCheck;
        
    [SerializeField]

    private LayerMask whatIsGround;

    private float currentHealth , knockbackStartTime;

    private int Direction , damageDirection;


    [SerializeField]
    private Vector2 knockbackSpeed;


    private bool groundDetected, wallDetected;

    public GameObject alive;
    private Rigidbody2D aliveRb;
    
    private Animator aliveAnim;
    
    private Vector2 movement;

    [SerializeField] private ParticleSystem blueBlood;


    private void Start()
    {
        currentHealth = maxHealth;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();
        Direction = 1;
    }
    private void Update()
    {
        switch(currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    void CreateBlueBlood(){
        blueBlood.Play();
    }

    //Walking

    private void EnterMovingState()
    {
        
    }

    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        if(!groundDetected || wallDetected){
            Flip();
        }
        else if(groundDetected){
            movement.Set(movementSpeed * Direction, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    private void ExitMovingState()
    {

    }

    //Knockback
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("Knockback", true);
    }
    private void UpdateKnockbackState()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }


    private void ExitKnockbackState()
    {

        aliveAnim.SetBool("Knockback", false);
    }

    //Dead

    private void EnterDeadState()
    {
        //spawn particle
        Destroy(gameObject);
    }
    private void UpdateDeadState()
    {

    }


    private void ExitDeadState()
    {
        
    }

    //Other Fonctions

    public void Damage(float damage,float position)
    {
        currentHealth -= damage;
        CreateBlueBlood();
        if(position < alive.transform.position.x){
            damageDirection = 1;
        }
        else{
            damageDirection = -1;
        }

        //Hit particle

        if(currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        }
        else if(currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }
    private void Flip()
    {
        Direction *= -1;
        alive.transform.Rotate(0.0f,180.0f,0.0f);
    }
    private void SwitchState(State state)
    {
        switch(currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch(state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

    }

}