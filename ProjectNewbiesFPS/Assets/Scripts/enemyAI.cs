using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : Spawnable, IDamage
{
    [SerializeField] private Renderer model;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform headPos;
    
    [SerializeField] private int HP;
    [SerializeField] private int HPMax;
    [SerializeField] private int faceTargetSpeed;
    
    [SerializeField] private float searchLength = 5;
    [SerializeField] private float timeToWait = 1;
    float searchRadius;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootRate;
    [SerializeField] float shootRange;
    [SerializeField] LayerMask playerLayerMask;
    private Color colorOriginal;

    private bool playerInRange;
    private bool playerDetected;
    private bool isShooting;

    private Vector3 playerDir;
    private Vector3 playerPos;
    private Vector3 lastSeenPlayerPos;
    
    public Image healthBar;
    public GameObject healthBarVisibility;

    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject debugPlayerPos;

    private float initialAgentStoppingDistance;
    [SerializeField] LayerMask ground;
    enum EnemyState {idle, chasing, searching, wander}
    [SerializeField] private EnemyState _defaultState = EnemyState.idle;
    [SerializeField] private EnemyState _currentState;
    EnemyState _previousState;
    bool randomPositionFound = false;
    Coroutine searchRoutine = null;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.enemyAI.Add(gameObject);
        GameManager.instance.enemyAIScript.Add(this);
        GameManager.instance.enemyHealthBar.Add(healthBar);
        GameManager.instance.enemyHealthBarVisibility.Add(healthBarVisibility);
        _currentState = _defaultState;
        _previousState = _defaultState;
        model = GetComponentInChildren<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        colorOriginal = model.material.color;
        initialAgentStoppingDistance = agent.stoppingDistance;
        healthBar.fillAmount = (float)HP;
        HPMax = HP;
        GameManager.instance.EnemyCount++;
        searchRadius = shootRange;
    }

    // Update is called once per frame
    void Update()
    {
        enemyAIStates();
        _animator.SetFloat("AgentSpeed", agent.velocity.magnitude);
    }

    void enemyAIStates()
    {
        debugPlayerPos.transform.position = new Vector3(lastSeenPlayerPos.x, lastSeenPlayerPos.y - 1, lastSeenPlayerPos.z);
        playerPos = GameManager.instance.player.transform.position;
        playerDir = (playerPos - headPos.position).normalized;
        if (isPlayerVisible() && CheckForPlayerInRange() && !isShooting)
        {
            StartCoroutine(shoot());
        }
        switch (_currentState)
        {
            case EnemyState.idle:
                idleState();
                break;
            case EnemyState.chasing:
                chasingState();
                break;
            case EnemyState.searching:
               searchingState(); 
                break;
            case EnemyState.wander:
             //   WanderState();
                break;
            default:
                break;
        }
      
    }

  

    bool isPlayerVisible()
    {
        
            Vector3 enemyForward = headPos.transform.forward;
            float angle = Vector3.Angle(enemyForward, playerDir);
            float fov = 180f / 2;


        // Check if the player is visible via raycast
        if (angle < fov)
        {


            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
         return false;
    }
 
    void idleState()
    {
        if (CheckForPlayerInRange())
        {
            Vector3 enemyForward = headPos.transform.forward;
            float angle = Vector3.Angle(enemyForward, playerDir)/2;
        

            if (isPlayerVisible())
            {
                _currentState = EnemyState.chasing;
                return;
            }
        }
    }

    void chasingState()
    {
        if (!isPlayerVisible() && !CheckForPlayerInRange())
        {
            agent.SetDestination(lastSeenPlayerPos);
            faceTarget(lastSeenPlayerPos);
            _currentState = EnemyState.searching;
            return;
        }

            lastSeenPlayerPos = playerPos;
            agent.SetDestination(playerPos);
            faceTarget(playerDir);

    }

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    void searchingState()
    {
        if (isPlayerVisible() && CheckForPlayerInRange())
        {
            _currentState = EnemyState.chasing;
            return;
        }

        Vector3 Dir = UnityEngine.Random.insideUnitSphere * shootRange;
        Dir += lastSeenPlayerPos;
        NavMeshHit hit;
        Vector3 final = Vector3.zero;
        if(NavMesh.SamplePosition(Dir,out hit, shootRange, ground))
        {
            final = hit.position;
        }
        faceTarget(final);
        agent.SetDestination(final);
        /* if (!walkPointSet) SearchWalkPoint();
         if (walkPointSet)
         {
             agent.SetDestination(walkPoint);
         }
         Vector3 distanceToWalkPoint = transform.position - walkPoint;
         if (distanceToWalkPoint.magnitude < 1f)
             walkPointSet = false;*/
        //   _currentState = _defaultState; 


    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
            walkPointSet = true;
    }

    /*    void WanderState()
   {
       if (isPlayerVisible() && CheckForPlayerInRange())
       {
           StopCoroutine(searchRoutine);
           agent.stoppingDistance = initialAgentStoppingDistance;
           searchRoutine = null;
           _currentState = EnemyState.chasing;

       }
       else
       {
           if (!isPositionFound)
           {
             //searchRoutine = StartCoroutine(waitToFindNextPos(timeToWait));
           }
       }
   }
   bool isPositionFound = false;
  */
    /*   IEnumerator waitToFindNextPos(float timeToWait)
       {      
           isPositionFound = true;
           NavMeshHit hit;

           float counter = searchLength;

           while (counter > 0)
           {

               counter--;
           }
           isPositionFound = false;
           _currentState = _defaultState;
          // searchRoutine = null;
       }
   */


    void faceTarget(Vector3 lookTarget)
    {    
            Quaternion rot = Quaternion.LookRotation(lookTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        _animator.SetBool("isFiring",true);
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        _animator.SetBool("isFiring", false);
        isShooting = false;
    }
    
    public void takeDamage(int amount, Vector3 shooterPos)
    {
        StartCoroutine(flashColor());
        playerDetected = true;
        
        HP -= amount;
        healthBar.fillAmount = (float)HP / HPMax;

        _currentState = EnemyState.chasing;
        if (HP <= 0)
        {
            healthBar.fillAmount = (float)HP / HPMax;
            GameManager.instance.playerScript.money += 5;
            GameManager.instance.enemyAIScript.Remove(this);
            GameManager.instance.enemyAI.Remove(this.gameObject);
           
            GameManager.instance.EnemyCount--;
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        
        yield return new WaitForSeconds(0.1f);
        
        model.material.color = colorOriginal;
    }
    bool CheckForPlayerInRange()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, searchRadius, playerLayerMask, QueryTriggerInteraction.Collide);
        if (cols.Count() > 0)
        {
            return true;
        }
        else
            return false;
    }
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerStay(Collider other)
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
        
    }*/
}
