using UnityEngine;
using UnityEngine.AI;

public class AI_Controller : MonoBehaviour
{
    public NavMeshAgent agent;

    public float acceleration;
    public float speed;
    public float walkTurnSpeed; 
    public float lookAroundSpeed; 

    public float patrolTime;
    public float newPatrolTime;

    private Vector3 destination;
    private bool destinationFlag;

    public Vector2[] areaBorders;

    private Quaternion rightLook;
    private Quaternion leftLook;
    public float quaternionLookAngle;
    
    private bool isWaiting;

    public float stoppingDistance;


    private Animator soldierAnimation;

    void Start()
    {
        destinationFlag = false;
        isWaiting = false;
        
        soldierAnimation = GetComponent<Animator>();

        agent.stoppingDistance = stoppingDistance;
        newPatrolTime = patrolTime;
        
        agent.acceleration = acceleration;         

        agent.speed = speed;
        agent.angularSpeed = walkTurnSpeed; 

        destination = agent.transform.position;
        
        if(patrolTime < 0)
            Debug.Log("Patrol Time must be greater than or equal to zero.\n");
    }

    void Update()
    {
        if(destinationFlag == false)
        {
            destination.x = Random.Range(areaBorders[0].x, areaBorders[1].x);
            destination.z = Random.Range(areaBorders[0].y, areaBorders[1].y);
            agent.SetDestination(destination);
            
            soldierAnimation.SetBool("isWalking", true);
            destinationFlag = true;
            isWaiting = false; 
        }
        
        
        if(destinationFlag == true && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            soldierAnimation.SetBool("isWalking", false);
            if(isWaiting == false)
            {
                isWaiting = true;
            }
            
            patrolTime -= Time.deltaTime;
            if(patrolTime >= 3 * newPatrolTime / 4)
                agent.transform.Rotate(0.0f, quaternionLookAngle, 0.0f); 
            else if(patrolTime >= newPatrolTime / 4)
                agent.transform.Rotate(0.0f, (-1) * quaternionLookAngle, 0.0f); 
            else if(patrolTime > 0)
                agent.transform.Rotate(0.0f, quaternionLookAngle, 0.0f);
        }

        if(patrolTime <= 0)
        {
            patrolTime = newPatrolTime;
            destinationFlag = false; 
        }
    }
}
