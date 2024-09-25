using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AIController : Spawnable, IDamage
{
    [Header("AnimationRig")]
    [SerializeField] Rig rig;
    [SerializeField] Transform defaultRigTarget;
    public Transform DefaultRigTarget => defaultRigTarget;  
    [SerializeField] MultiAimConstraint head;
    [SerializeField] MultiAimConstraint body;
    [SerializeField] MultiAimConstraint rArm;
    [Tooltip("For dual weild use only"),SerializeField] MultiAimConstraint lArm;


    [Header("AI")]
    [SerializeField] private Renderer model;
    private Color colorOriginal;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform shootPos;
    [Tooltip("For dual wield use only"), SerializeField] private Transform shootPosTwo;
    [SerializeField] private Transform headPos;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] private int HP;
    [SerializeField] private int HPMax;
    public UnityEngine.UI.Image healthBar;
    public GameObject healthBarVisibility;
    [SerializeField] private int faceTargetSpeed;
    [SerializeField] float sightRange;
    [SerializeField] private float shootRate;
    [SerializeField] float shootRange;
    [SerializeField] private GameObject bullet;
    private bool isTakingDamage = false;
    public bool IsTakingDamage { get => isTakingDamage; set => isTakingDamage = value; }
    private bool canTransition = false;
    public bool CanTransition { get => canTransition; set => canTransition = value;}
    [SerializeField] private Weapon weapon;

    [SerializeField, Tooltip("How long the AI should wait at each point when searching?")] private float timeToWait = 3;
    public Coroutine searchRoutine = null;
    public Coroutine positionRoutine = null;
    [SerializeField, Tooltip("How long should the AI stay in the search state?")] private float searchLength = 20;
    [Header("Cooldown Timers")]
    [SerializeField] private float dodgeCooldownTimer = 0f;

    public float DodgeCooldownTimer { get { return dodgeCooldownTimer; } set { if (value < 0) dodgeCooldownTimer = 0; else dodgeCooldownTimer = value; } }
    [SerializeField] private float sweepCooldownTimer = 0f;
    [SerializeField] float notificationLength = .5f;
    public float SweepCooldownTimer { get { return sweepCooldownTimer; } set { if (value < 0) sweepCooldownTimer = 0; else sweepCooldownTimer = value; } }

    public float SearchLength => searchLength;
    private float currentTime;
    public float CurrentTime {  get { return currentTime; } set { currentTime = value; } }
    public bool IsShooting { get; set; }

    private Vector3 playerDir;
    public Vector3 PlayerDirection => playerDir;
    public Vector3 playerPos { get; set; }
    public Vector3 lastSeenPlayerPos { get; set; }
   
    public bool PathFound { get; set; } = false;
    public NavMeshAgent Agent => agent;
    public Animator Anim => _animator;
    [SerializeField] float animSpeedTransition;
    [SerializeField] private State _defaultState;
    public State DefaultState => _defaultState;
    //[SerializeField] private State _chaseState;
    [SerializeField,Tooltip("Do not add a state here from the inspector!")] private State _currentState;
    public State PreviousState { get;  set; }

    public Vector3 target = Vector3.zero;
    public Vector3 lookTarget = Vector3.zero;
    private float originalStopDistance;
    public float duration;
    void Awake()
    {
        rig ??= GetComponentInChildren<Rig>();
        model ??= GetComponentInChildren<Renderer>();
        agent ??= GetComponent<NavMeshAgent>();
        

        StopRig();
        originalStopDistance = agent.stoppingDistance;
        HP = HPMax;
        CurrentTime = 0;
    }
    void Start()
    {
        GameManager.instance.enemyAI.Add(gameObject);
        GameManager.instance.enemyAIScript.Add(this);
        GameManager.instance.enemyHealthBar.Add(healthBar);
        GameManager.instance.enemyHealthBarVisibility.Add(healthBarVisibility);
        head.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        body.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        rArm.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        if(lArm != null)
        {
            lArm.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        }
        
        weapon = GetComponentInChildren<Weapon>();
        shootPos = weapon.GetFirePoint();
        shootRate = weapon.GetShootRate();
        shootRange = weapon.GetShootDist(); 

              
        colorOriginal = model.material.color;
        healthBar.fillAmount = (float)HP;
        StopRig();
        TransitionToState(_defaultState);
    }


    void Update()
    {       
        _currentState.UpdateState(this);
        TrackedStats();
    }

    void TrackedStats()
    {
        if(DodgeCooldownTimer > 0)
        {
            DodgeCooldownTimer -= Time.deltaTime;
        }
        if (SweepCooldownTimer > 0)
        {
            SweepCooldownTimer -= Time.deltaTime;
        }
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = _animator.GetFloat("AgentSpeed");

        _animator.SetFloat("AgentSpeed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animSpeedTransition));
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
    public IEnumerator GetRandomClearPositionInRange(float range)
    {
      
        Vector3 final = Vector3.zero;
        int walkableMask = NavMesh.GetAreaFromName("Walkable");
      

        while (final == Vector3.zero)
        {

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range;
            randomDirection += transform.position;  
            NavMeshPath path = new NavMeshPath();
            NavMeshHit hit;      
            if (NavMesh.SamplePosition(randomDirection, out hit, range,1 << walkableMask))
            {
                agent.CalculatePath(hit.position, path);
                float distance = Vector3.Distance(transform.position, hit.position);
                if (path.status == NavMeshPathStatus.PathComplete && distance <= range && distance >= 4 && path.corners.Length <= 3)
                {
                    final = hit.position;
                    agent.stoppingDistance = 0f;
                    target = hit.position;
                    lookTarget = hit.position;
                    PathFound = true;
                }
            }
            else
            {
             Debug.LogWarning("No valid NavMesh position found.");
            }
            yield return null;  
        }

        yield return new WaitForSeconds(.5f);
        agent.stoppingDistance = originalStopDistance;
        positionRoutine = null;
    }
    public IEnumerator GetOpenCoverPosition()
    {
        yield return new WaitForSeconds(1f);
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
        GameObject clone = Instantiate(bullet, shootPos.position, transform.rotation);
        clone.GetComponent<damage>().damageAmount = weapon.GetGunDamage();
        AudioManager.instance.playEnemy(AudioManager.instance.enemyShoot, AudioManager.instance.enemyShootVol);
        yield return new WaitForSeconds(shootRate);
        IsShooting = false;
    }
    public IEnumerator SweepAttack(Vector3[] sweepPoints, float sweepSpeed)
    {
        IsShooting = true;
        if(sweepPoints.Length < 2)
        {
            IsShooting = false;
            yield break;
        }
       
        
        int randomStart = UnityEngine.Random.Range(0, sweepPoints.Length);
        int randomFinish = (randomStart == 0) ? 1 : 0;

        Vector3 startPoint  = sweepPoints[randomStart];
        Vector3 finishPoint = sweepPoints[randomFinish];
       
        GameObject followObj = new GameObject("FollowTarget");
        followObj.transform.position = startPoint;
       
      
        float elapsedTime = 0f;
        float totalDistance = Vector3.Distance(startPoint,finishPoint);
        duration = totalDistance / sweepSpeed;
        if (!_animator.GetBool("IsAttacking"))
            _animator.SetBool("IsAttacking", true);
        while (elapsedTime < duration)
        {        
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            followObj.transform.position = Vector3.Lerp(startPoint, finishPoint, t);
            
            lookTarget = followObj.transform.position;
            
            AudioManager.instance.playEnemy(AudioManager.instance.enemyShoot, AudioManager.instance.enemyShootVol);
            yield return new WaitForSeconds(shootRate);
        }
        _animator.SetBool("IsAttacking", false);
      
        lookTarget = Vector3.zero;
        Destroy(followObj);
        IsShooting = false;
    }
    public IEnumerator TookDamageTimer()
    {
        isTakingDamage = true;
        yield return new WaitForSeconds(notificationLength);
        isTakingDamage = false;
    }
    public void CreateBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    public void CreateBullets()
    {
        Instantiate(bullet, shootPos.position, shootPos.rotation);
        Instantiate(bullet, shootPosTwo.position, shootPos.rotation);
    }
    public void TakeDamage(int amount, Vector3 shooterPos)
    {
        StartCoroutine(flashColor());
        StartCoroutine(TookDamageTimer());
        
        HP -= amount;
        healthBar.fillAmount = (float)HP / HPMax;
        
        if (HP <= 0)
        {
            healthBar.fillAmount = (float)HP / HPMax;
            GameManager.instance.playerScript.money += 5;
            GameManager.instance.enemyAIScript.Remove(this);
            GameManager.instance.enemyAI.Remove(this.gameObject);
           
    
            Destroy(gameObject);
        }
    }

    void enemyFootSteps()
    {
        AudioManager.instance.playEnemy(AudioManager.instance.footStepsForest[UnityEngine.Random.Range(0, AudioManager.instance.footStepsForest.Length)], AudioManager.instance.footStepsVol);
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        AudioManager.instance.playEnemy(AudioManager.instance.hurt[UnityEngine.Random.Range(0, AudioManager.instance.footStepsForest.Length)], AudioManager.instance.hurtVol);

        yield return new WaitForSeconds(0.1f);
        
        model.material.color = colorOriginal;
    }

    public void StartRig()
    {
        head.data.sourceObjects.SetWeight(0, 0);
        head.data.sourceObjects.SetWeight(1, 1);
        body.data.sourceObjects.SetWeight(0, 0);
        body.data.sourceObjects.SetWeight(1, 1);
        rArm.data.sourceObjects.SetWeight(0, 0);
        rArm.data.sourceObjects.SetWeight(1, 1);
        if(lArm != null)
        {
            lArm.data.sourceObjects.SetWeight(0, 0);
            lArm.data.sourceObjects.SetWeight(1, 1);
        }
        rig.weight = 1.0f;
    }
    public void StopRig()
    {
        head.data.sourceObjects.SetWeight(0, 1);
        head.data.sourceObjects.SetWeight(1, 0);
        body.data.sourceObjects.SetWeight(0, 1);
        body.data.sourceObjects.SetWeight(1, 0);
        rArm.data.sourceObjects.SetWeight(0, 1);
        rArm.data.sourceObjects.SetWeight(1, 0);
        if (lArm != null)
        {
            lArm.data.sourceObjects.SetWeight(0, 1);
            lArm.data.sourceObjects.SetWeight(1, 0);
        }
        rig.weight = 0f;
    }
    public void SetDualWeildRig()
    {
        head.data.sourceObjects.SetWeight(0, 1);
        head.data.sourceObjects.SetWeight(1, 0);
        body.data.sourceObjects.SetWeight(0, 1);
        body.data.sourceObjects.SetWeight(1, 0);
        rArm.data.sourceObjects.SetWeight(0, 1);
        rArm.data.sourceObjects.SetWeight(1, 0);
        lArm.data.sourceObjects.SetWeight(0, 1);
        lArm.data.sourceObjects.SetWeight(1, 0);
        rig.weight = 1.0f;
    }
    public void AwaitAnimation()
    {
        if(canTransition == false)
            canTransition = true;
    }
   
}
