using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private Renderer model;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform headPos;
    
    [SerializeField] private int HP;
    [SerializeField] private int faceTargetSpeed;
    [SerializeField] private int searchRadius;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootRate;

    private Color colorOriginal;

    private bool playerInRange;
    private bool playerDetected;
    private bool isShooting;

    private Vector3 playerDir;
    private Vector3 playerPos;
    private Vector3 lastSeenPlayerPos;

    [SerializeField] private GameObject debugPlayerPos;

    private float initialAgentStoppingDistance;
    
    enum EnemyState {idle, chasing, searching, shooting}
    [SerializeField] private EnemyState _enemyState = EnemyState.idle;
    
    bool randomPositionFoundAndReached = false;
    
    // Start is called before the first frame update
    void Start()
    {
        model = GetComponentInChildren<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        colorOriginal = model.material.color;
        initialAgentStoppingDistance = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        enemyAIStates();
    }

    void enemyAIStates()
    {
        debugPlayerPos.transform.position = new Vector3(lastSeenPlayerPos.x, lastSeenPlayerPos.y - 1, lastSeenPlayerPos.z);
        playerPos = GameManager.instance.player.transform.position;
        playerDir = playerPos - headPos.position;
        
        switch (_enemyState)
        {
            case EnemyState.idle:
                idleState();
                break;
            case EnemyState.chasing:
                chasingState();
                break;
            //case EnemyState.shooting:
                //break;
            case EnemyState.searching:
                searchingState(); 
                break;
            default:
                break;
        }
    }

    bool isPlayerVisible()
    {
        if (playerInRange)
        {
            Vector3 enemyForward = headPos.transform.forward;
            float angle = Vector3.Angle(enemyForward, playerDir);
            float fov = 90f;
            
            if (angle < fov / 2f)
            {
                // Check if the player is visible via raycast
                RaycastHit hit;
                if (Physics.Raycast(headPos.position, playerDir, out hit))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    lastSeenPlayerPos = hit.transform.position;
                    agent.SetDestination(lastSeenPlayerPos);

                    return true;
                }
            }
        }

        return false;
    }
    
    void idleState()
    {
        if (playerInRange)
        {
            Vector3 enemyForward = headPos.transform.forward;
            float angle = Vector3.Angle(enemyForward, playerDir);
            float fov = 90f;

            if (isPlayerVisible())
            {
                lastSeenPlayerPos = playerPos;  // Update last seen position
                _enemyState = EnemyState.chasing;
            }
        }
        else
        {
            // enemy idle state stuff here
        }
        
    }

    void chasingState()
    {
        if (playerInRange && isPlayerVisible())
        {
            lastSeenPlayerPos = playerPos;
            agent.SetDestination(lastSeenPlayerPos);

            if (isPlayerVisible() && agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
        else if (!playerInRange && isPlayerVisible())
        {
            agent.SetDestination(lastSeenPlayerPos);

            // Only face the target if the player is visible
            if (isPlayerVisible() && agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
        }
        else
        {
            _enemyState = EnemyState.searching;
        }
    }

    void searchingState()
    {
        if (isPlayerVisible())
        {
            _enemyState = EnemyState.chasing;
            agent.stoppingDistance = initialAgentStoppingDistance;
        }
    }

    IEnumerator waitToFindNextPos(float timeToWait)
    {
        
        
        randomPositionFoundAndReached = true;
        yield return new WaitForSeconds(timeToWait);

        randomPositionFoundAndReached = false;
    }

    void attackState()
    {
        // bang bang shoot the player if some bools are active or whatever
        // TODO: later tonight, make this work
    }

    void faceTarget()
    {
        if (isPlayerVisible())
        {
            Quaternion rot = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        
        isShooting = false;
    }
    
    public void takeDamage(int amount, Vector3 shooterPos)
    {
        StartCoroutine(flashColor());
        playerDetected = true;
        
        HP -= amount;

        lastSeenPlayerPos = shooterPos;

        agent.SetDestination(lastSeenPlayerPos);
        if (HP <= 0)
        {
            GameManager.instance.playerScript.money += 5;
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        
        yield return new WaitForSeconds(0.1f);
        
        model.material.color = colorOriginal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerPos = lastSeenPlayerPos;
        }
        
    }
}
