using UnityEngine;
using UnityEngine.AI;

public class Detection : MonoBehaviour
{
    public GameObject player;
    private ControlPlayer playerScript;
    private NavMeshAgent agent;

    public float normalDetectionRange;
    public float hiddenDetectionRange;
    private float currentDetectionRange;

    public float fieldOfViewAngle;
    

    private Animator soldierAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soldierAnimation = GetComponent<Animator>();
        playerScript = player.GetComponent<ControlPlayer>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
       if (playerScript.is_hiding) currentDetectionRange = hiddenDetectionRange;
       else currentDetectionRange = normalDetectionRange;

        if (currentDetectionRange > Vector3.Distance(transform.position, player.transform.position))
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < fieldOfViewAngle / 2f)
            {
                Vector3 agentEye = transform.position + Vector3.up * 1.5f; // Askerin göz hizasının 1.5f olduğu varsayılarak
                Vector3 playerEye = player.transform.position + Vector3.up * 1.5f; // Oyuncunun göz hizasının 1.5f olduğu varsayılarak
                float distanceToPlayer = Vector3.Distance(agentEye, playerEye);
                RaycastHit hit;

                if (Physics.Raycast(agentEye, directionToPlayer, out hit, distanceToPlayer))
                {
                    if (hit.collider.gameObject == player)
                    {
                        Debug.Log("Asker seni gördü!");
                        agent.SetDestination(player.transform.position);
                        soldierAnimation.SetBool("isWalking", true);
                        if(distanceToPlayer  < currentDetectionRange / 2)
                        {
                            soldierAnimation.SetBool("isWalking", false);
                            agent.SetDestination(agent.transform.position);
                            soldierAnimation.SetBool("player_in_range", true);
                            player.SetActive(false);
                            soldierAnimation.SetBool("player_in_range", false);
                        }
                    }

                    else
                    {
                        Debug.Log("Asker seni göremiyor!");
                        return;
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireSphere(transform.position, normalDetectionRange);

        Vector3 rightBoundary = Quaternion.Euler(0f, fieldOfViewAngle / 2f, 0f) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0f, -fieldOfViewAngle / 2f, 0f) * transform.forward;

        
        Gizmos.color = Color.yellow; 
        Gizmos.DrawRay(transform.position, rightBoundary * normalDetectionRange);
        Gizmos.DrawRay(transform.position, leftBoundary * normalDetectionRange);
    }
}
