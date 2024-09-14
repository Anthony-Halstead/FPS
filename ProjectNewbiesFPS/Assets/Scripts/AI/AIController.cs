using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIController : Spawnable, IDamage
{
    [SerializeField] private Renderer model;
    private Color colorOriginal;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform headPos;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] private int HP;
    [SerializeField] private int HPMax;
    public Image healthBar;
    public GameObject healthBarVisibility;
    [SerializeField] private int faceTargetSpeed;
    [SerializeField] float sightRange;
    [SerializeField] private float shootRate;
    [SerializeField] float shootRange;
    [SerializeField] private GameObject bullet;

    [SerializeField, Tooltip("How long the AI should wait at each point when searching?")] private float timeToWait = 3;
    public Coroutine searchRoutine = null;
    [SerializeField, Tooltip("How long should the AI stay in the search state?")] private float searchLength = 20;
    public float SearchLength => searchLength;
    private float currentTime;
    public float CurrentTime {  get { return currentTime; } set { currentTime = value; } }
    public bool IsShooting { get; set; }

    private Vector3 playerDir;
    public Vector3 playerPos { get; set; }
    public Vector3 lastSeenPlayerPos { get; set; }
    private bool canMove = true;
    public bool CanMove
    {
        get { return canMove; }
        set
        {

            canMove = value;
           
        }
    }

 
    [SerializeField] private State _defaultState;
    public State DefaultState => _defaultState;
    [SerializeField] private State _chaseState;
    [SerializeField,Tooltip("Do not add a state here from the inspector!")] private State _currentState;
    public State PreviousState { get;  set; }

    public Vector3 target = Vector3.zero;
    public Vector3 lookTarget = Vector3.zero;
    private float originalStopDistance;

    void Start()
    {
        GameManager.instance.enemyAI.Add(gameObject);
        GameManager.instance.enemyAIScript.Add(this);
        GameManager.instance.enemyHealthBar.Add(healthBar);
        GameManager.instance.enemyHealthBarVisibility.Add(healthBarVisibility);
        originalStopDistance = agent.stoppingDistance;
        model = GetComponentInChildren<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        colorOriginal = model.material.color;
        healthBar.fillAmount = (float)HP;
        HPMax = HP;
        GameManager.instance.EnemyCount++;
        CurrentTime = 0;
        TransitionToState(_defaultState);
    }


    void Update()
    {       
        _currentState.UpdateState(this);
        TrackedStats();
    }

    void TrackedStats()
    {

        float clampedVelocity = Mathf.Clamp01(agent.velocity.magnitude);
        _animator.SetFloat("AgentSpeed", clampedVelocity);
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        agent.SetDestination(target);        
        faceTarget(lookTarget - headPos.position);  
    }
   public void TransitionToState(State state)
    {
        
        _currentState?.ExitState(this);
        _currentState = state;
        _currentState?.EnterState(this);
    }


    public bool TargetIsVisible()
    {
        
            Vector3 enemyForward = headPos.transform.forward;
            float angle = Vector3.Angle(enemyForward, playerDir);
            float fov = 180f / 2;



        if (angle < fov)
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir, out hit,sightRange,playerLayerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
         return false;
    }
   public bool TargetInShootRange()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, sightRange, playerLayerMask, QueryTriggerInteraction.Collide);
        if (cols.Count() > 0)
        {
            return true;
        }
        else
            return false;
    }

    public IEnumerator SearchSpot()
    {
        Debug.Log("In coroutine");
        Vector3 final = Vector3.zero;

    
        int walkableMask = NavMesh.GetAreaFromName("Walkable");
        float searchRadius = UnityEngine.Random.Range(3f, shootRange);  

        while (final == Vector3.zero)
        {

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * searchRadius;
            randomDirection += transform.position;  
           
            NavMeshHit hit;

      
            if (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, 1 << walkableMask))
            {
                final = hit.position;
                agent.stoppingDistance = 2f;
                target = hit.position;
                lookTarget = hit.position;
            }
            else
            {
                Debug.LogWarning("No valid NavMesh position found.");
            }

            yield return null;  
        }

        yield return new WaitForSeconds(timeToWait);  
        agent.stoppingDistance = originalStopDistance;
        searchRoutine = null;
    }


    void faceTarget(Vector3 lookTarget)
    {    
            Quaternion rot = Quaternion.LookRotation(lookTarget);
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(currentRotation, targetRotation,Time.deltaTime * faceTargetSpeed);
    }

    public IEnumerator shoot()
    {
        IsShooting = true;   
        _animator.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        IsShooting = false;
    }
    
    public void TakeDamage(int amount, Vector3 shooterPos)
    {
        StartCoroutine(flashColor());
        
        
        HP -= amount;
        healthBar.fillAmount = (float)HP / HPMax;
        if(!IsShooting)
        TransitionToState(_chaseState);
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
   

}
