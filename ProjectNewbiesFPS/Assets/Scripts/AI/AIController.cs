using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;


public class AIController : Spawnable, IDamage
{
    private enum EnemyType
    {
        Boss,
        Minion,
    }

    [Header("AnimationRig")]
    [SerializeField] Rig rig;
    [SerializeField] Transform defaultRigTarget;
    public Transform DefaultRigTarget => defaultRigTarget;
    [SerializeField] MultiAimConstraint head;
    [SerializeField] MultiAimConstraint body;
    [SerializeField] MultiAimConstraint rArm;
    [Tooltip("For dual wield use only"), SerializeField] MultiAimConstraint lArm;


    [Header("AI")]
    [SerializeField] private EnemyType enemyType = EnemyType.Minion;
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
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
    [SerializeField] private Weapon weapon;
 
    [Header("Cooldown Timers")]
    [SerializeField] private float dodgeCooldownTimer = 0f;
    [SerializeField] float notificationLength = .5f;
    public float DodgeCooldownTimer { get { return dodgeCooldownTimer; } set { if (value < 0) dodgeCooldownTimer = 0; else dodgeCooldownTimer = value; } }

    [SerializeField] private float sweepCooldownTimer = 0f;
    public float SweepCooldownTimer { get { return sweepCooldownTimer; } set { if (value < 0) sweepCooldownTimer = 0; else sweepCooldownTimer = value; } }
    [SerializeField] private float holdCoverCooldownTimer = 0f;
    public float HoldCoverCooldownTimer { get { return holdCoverCooldownTimer; } set { if (value < 0) holdCoverCooldownTimer = 0; else holdCoverCooldownTimer = value; } }


    [SerializeField] private float spawnReinforcementCooldownTimer = 0f;
    public float SpawnReinforcementCooldownTimer { get { return spawnReinforcementCooldownTimer; } set { if (value < 0) spawnReinforcementCooldownTimer = 0; else spawnReinforcementCooldownTimer = value; } }

    public bool IsDodging { get; set; }
    public bool IsShooting { get; set; }
    public bool IsSearching { get; set; }
    public bool HasDodged { get; set; } = false;

    private Vector3 playerDir;
    public Vector3 PlayerDirection => playerDir;
    public Vector3 playerPos { get; private set; }
    public Vector3 lastSeenPlayerPos { get; set; }
   
    private Vector3 startPosition;
    private Collider currentCover;
    public Vector3 StartPosition => startPosition;
    public bool PathFound = false;
    public NavMeshAgent Agent => agent;
    public Animator Anim => _animator;

    [SerializeField] private State _defaultState;
    public State DefaultState => _defaultState;
    [SerializeField, Tooltip("Do not add a state here from the inspector!")] private State _currentState;
    public State PreviousState { get; set; }
    public Coroutine PositionRoutine { get; set; }
    public Vector3 target;
    public Vector3 lookTarget = Vector3.zero;
    private float originalStopDistance;
    private float duration;
    [Range(1,10),SerializeField] int maxCoverHistory = 5;
    private Queue<Collider> coverColliders = new Queue<Collider>();
    Vector3 areaPosition;
    void Awake()
    {
        rig ??= GetComponentInChildren<Rig>();
        model ??= GetComponentInChildren<Renderer>();
        agent ??= GetComponent<NavMeshAgent>();


        StopRig();
        originalStopDistance = agent.stoppingDistance;
        HP = HPMax;
    }
    void Start()
    {

        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameManager.instance.enemyAI.Add(gameObject);
        GameManager.instance.enemyAIScript.Add(this);
        GameManager.instance.enemyHealthBar.Add(healthBar);
        GameManager.instance.enemyHealthBarVisibility.Add(healthBarVisibility);
        head.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        body.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        rArm.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        if (lArm != null)
        {
            lArm.data.sourceObjects.Add(new WeightedTransform(GameManager.instance.player.transform, 0));
        }

        weapon ??= GetComponentInChildren<Weapon>();
      
        if (weapon != null)
        {
            bullet.TryGetComponent(out damage component);
            if (component != null)
            {
                component.damageAmount = weapon.GetGunDamage();
            }
            shootPos = weapon.GetFirePoint();
            shootRate = weapon.GetShootRate();
            shootRange = weapon.GetShootDist();
        }

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
        if (DodgeCooldownTimer > 0)
        {
            DodgeCooldownTimer -= Time.deltaTime;
        }
        if (SweepCooldownTimer > 0)
        {
            SweepCooldownTimer -= Time.deltaTime;
        }
        if (SpawnReinforcementCooldownTimer > 0)
        {
            SpawnReinforcementCooldownTimer -= Time.deltaTime;
        }
        if (HoldCoverCooldownTimer > 0)
        {
            HoldCoverCooldownTimer -= Time.deltaTime;
        }    
        playerPos = GameManager.instance.player.transform.position;
        playerDir = playerPos - headPos.position;
        agent.SetDestination(target);
        /*if(lookTarget != Vector3.zero)*/faceTarget(lookTarget);
      /*  else
        faceMovementDirection();*/
        float agentSpeed = Mathf.Clamp(agent.velocity.magnitude, 0f, 1); 
        float currentAnimSpeed = _animator.GetFloat("AgentSpeed");
        float smoothSpeed = Mathf.Lerp(currentAnimSpeed, agentSpeed, Time.deltaTime * 5f);  
        _animator.SetFloat("AgentSpeed", smoothSpeed);
 
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
            if (Physics.Raycast(headPos.position, playerDir, out hit, sightRange, playerLayerMask))
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
    void faceTarget(Vector3 lookTarget)
    {
        
        Vector3 directionToLookTarget = (lookTarget - transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(directionToLookTarget);
        Quaternion currentRotation = transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);  

        transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, Time.deltaTime * faceTargetSpeed);
    }
   /* void faceMovementDirection()
    {
        // Only update rotation if the agent has some velocity
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * faceTargetSpeed);
        }
    }*/

    public void SetDodgeFalse()
    {
        if(IsDodging)IsDodging = false;
    }
    public IEnumerator SearchSpot(float timeToWait, float searchTime)
    {
        do { yield return null; } while (GameManager.instance.isPaused);
        Vector3 final = Vector3.zero;
        IsSearching = true;
        float currentTime = 0;
        int walkableMask = NavMesh.GetAreaFromName("Walkable");
        float searchRadius = UnityEngine.Random.Range(3f, shootRange);
      
        while (final == Vector3.zero && currentTime < searchTime)
        {
            currentTime += Time.deltaTime;
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * searchRadius;
            randomDirection += transform.position;  
           
            NavMeshHit hit;

      
            if (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, 1 << walkableMask))
            {
                final = hit.position;
                agent.stoppingDistance = 2f;
                target = hit.position;
                lookTarget = new Vector3(agent.steeringTarget.x, agent.steeringTarget.y, agent.steeringTarget.z + .9f);
                while (agent.remainingDistance > agent.stoppingDistance)
                {
                    lookTarget = new Vector3(agent.steeringTarget.x, agent.steeringTarget.y, agent.steeringTarget.z + .9f);
                    yield return null;
                }
                yield return new WaitForSeconds(timeToWait);
            }
            else
            {
                Debug.LogWarning("No valid NavMesh position found.");
            }

            yield return null;  
        }      
        agent.stoppingDistance = originalStopDistance;
        IsSearching = false;      
        PositionRoutine = null;
    }
    public IEnumerator GetDodgePositionInRange(float range)
    {
        do { yield return null; } while (GameManager.instance.isPaused);
        Vector3 final = Vector3.zero;
        int walkableMask = NavMesh.GetAreaFromName("Walkable");


        while (final == Vector3.zero)
        {

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range;
            randomDirection += transform.position;
            NavMeshPath path = new NavMeshPath();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, range, 1 << walkableMask))
            {
                agent.CalculatePath(hit.position, path);
                float distance = Vector3.Distance(transform.position, hit.position);
                if (path.status == NavMeshPathStatus.PathComplete && distance <= range && distance >= 4 && path.corners.Length <= 3)
                {
                    final = hit.position;
                    target = hit.position;
                    lookTarget = hit.position;
                    break;
                }
            }
            else
            {
                Debug.LogWarning("No valid NavMesh position found.");
            }
            yield return null;
        }
        PathFound = true;
        agent.stoppingDistance = originalStopDistance;
       
    }
    public IEnumerator GetRandomClearPositionInRange(float range)
    {
        do { yield return null; } while (GameManager.instance.isPaused);
        Debug.Log(PathFound);
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
                    target = hit.position;
                    lookTarget = new Vector3(agent.steeringTarget.x, agent.steeringTarget.y, agent.steeringTarget.z * 1f); 
                    break;
                }
            }
            else
            {
             Debug.LogWarning("No valid NavMesh position found.");
            }
            yield return null;  
        }
        while (Vector3.Distance(transform.position, target) > agent.stoppingDistance)
        {
            lookTarget = new Vector3(agent.steeringTarget.x, agent.steeringTarget.y, agent.steeringTarget.z * 1f);
            yield return null;
        }
  
        PathFound = true;
        Debug.Log(PathFound);
      
        agent.stoppingDistance = originalStopDistance;
        PositionRoutine = null;
    }
    public IEnumerator GetOpenCoverPosition(float range, LayerMask mask, int attempts, AreaType type)
    {
        do { yield return null; } while (GameManager.instance.isPaused);
        lookTarget = Vector3.zero;
        PathFound = false;
        if (type is AreaType.Player)
        {
            areaPosition = playerPos;
        }else if (type is AreaType.Defensive)
        {
            areaPosition = startPosition;
        }
        else
        {
            areaPosition = transform.position;
        }
    
        Vector3 final = Vector3.zero;
        int walkableMask = NavMesh.GetAreaFromName("Walkable");
        Collider[] covers = Physics.OverlapSphere(areaPosition, range, mask);
        if (covers.Count() <= 0)
        {
            yield return StartCoroutine(GetRandomClearPositionInRange(range));
            yield break;
        }
        int currentAttempt = attempts;
       
        while (final==Vector3.zero && currentAttempt > 0)
        {

            currentAttempt--;

            foreach (Collider col in covers)
            {
            
                Vector3 coverPosition = col.bounds.center;
                Vector3 coverToPlayerDir = (playerPos - coverPosition).normalized;

                float coverX = col.bounds.extents.x;
                float coverZ = col.bounds.extents.z;

                Vector3 oppositeCoverPosition = Vector3.zero;
                if (Mathf.Abs(coverToPlayerDir.x) > Mathf.Abs(coverToPlayerDir.z))
                {
                    oppositeCoverPosition = coverPosition - Mathf.Sign(coverToPlayerDir.x) * col.transform.right;
                }
                else
                {
                    oppositeCoverPosition = coverPosition - Mathf.Sign(coverToPlayerDir.z) * col.transform.forward;
                }
              
                NavMeshHit hit;
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.SamplePosition(oppositeCoverPosition, out hit, 1f, 1 << walkableMask))
                {
                    agent.CalculatePath(hit.position, path);
                    if (!coverColliders.Contains(col)&&path.status == NavMeshPathStatus.PathComplete)
                    {
                        coverColliders.Enqueue(col);
                        if (coverColliders.Count > maxCoverHistory)
                        {
                            coverColliders.Dequeue();
                        }

                        Debug.DrawLine(coverPosition, oppositeCoverPosition, Color.red, 2f);
                        Debug.Log("valid NavMesh position found.");
                        final = hit.position;
                        target = hit.position;
                        lookTarget = new Vector3(agent.steeringTarget.x, agent.steeringTarget.y, agent.steeringTarget.z *1f);
                        currentCover = col;
                        break;
                    }

                }
                else
                {
                    Debug.Log("No valid NavMesh position found.");
                }
                yield return null;
            }
        }
    

        if(final == Vector3.zero)
        {
            yield return StartCoroutine(GetRandomClearPositionInRange(range));
            yield break;
        }
        while ( Vector3.Distance(transform.position, target) > agent.stoppingDistance)
        {
            lookTarget = new Vector3(agent.steeringTarget.x, agent.steeringTarget.y, agent.steeringTarget.z * 1f);
            yield return null;
        }       
        PathFound = true;   
        lookTarget = playerPos;
        agent.stoppingDistance = originalStopDistance;
        PositionRoutine = null;
    }
    public IEnumerator Patrol()
    {
        int currentPointIndex = 0;
      

        //int walkableMask = NavMesh.GetAreaFromName("Walkable");
        while (true) {
            target = patrolPoints[currentPointIndex].position;
            lookTarget = patrolPoints[currentPointIndex].position;
            // NavMeshHit hit;
            /*     if(NavMesh.SamplePosition(patrolPoints[currentPointIndex].position, out hit, 10f, walkableMask))
                 {
                     target = hit.position;
                     lookTarget = hit.position;*/
            while (Vector3.Distance(transform.position, patrolPoints[currentPointIndex].position) > agent.stoppingDistance)
                {
                    yield return null;
                }
          //  }
           

            currentPointIndex++;
            if (currentPointIndex >= patrolPoints.Count)
            {
                currentPointIndex = 0;
            }
            yield return null;

        }      
    }
    public IEnumerator Shoot()
    {
     
        IsShooting = true;
        lookTarget = playerPos;
        _animator.SetTrigger("Shoot");
       // AudioManager.instance.playSFX(AudioManager.instance.shootPistol);
        yield return new WaitForSeconds(shootRate);
      //  _animator.SetBool("IsAttacking", false);
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
      

        Destroy(followObj);
        IsShooting = false;
    }
    public IEnumerator ShootFromCover(float shootTime, float rangeFromCoverOffset)
    {
        if (currentCover == null) yield break;
        do { yield return null; } while (GameManager.instance.isPaused);
        IsShooting = true;
        Vector3 originalPosition = transform.position;
        RaycastHit hit;
        if(Physics.Raycast(new Vector3(originalPosition.x, originalPosition.y + agent.height, originalPosition.z), playerDir,out hit, float.MaxValue, playerLayerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                _animator.SetBool("IsCrouching", false);
                do
                {
                    _animator.SetTrigger("Shoot");
                    AudioManager.instance.playSFX(AudioManager.instance.shootPistol);
                    yield return new WaitForSeconds(shootRate);
                    shootTime -= Time.deltaTime;
                } while (shootTime > 0);
                _animator.SetBool("IsCrouching", true);
                IsShooting = false;
            }
        }
        else
        {
            Vector3 final = Vector3.zero;
            int walkableMask = NavMesh.GetAreaFromName("Walkable");

            Bounds coverBounds = currentCover.bounds;
            Vector3[] potentialPosition = new Vector3[] { 
                coverBounds.center +( coverBounds.extents+Vector3.right*rangeFromCoverOffset),
                coverBounds.center +( coverBounds.extents+Vector3.left*rangeFromCoverOffset),
                coverBounds.center +( coverBounds.extents+Vector3.forward*rangeFromCoverOffset),
                coverBounds.center +( coverBounds.extents+Vector3.back*rangeFromCoverOffset),
            };
            NavMeshHit navHit;
            foreach(var pos in potentialPosition)
            {
                if (NavMesh.SamplePosition(pos, out navHit, rangeFromCoverOffset, 1 << walkableMask))
                {
                    if(Physics.Raycast(new Vector3(pos.x, pos.y + agent.height/2, pos.z), playerDir, out hit, float.MaxValue, playerLayerMask))
                    {
                        final = navHit.position;
                        target = navHit.position;
                        agent.stoppingDistance = .1f;
                        break;
                    }
                }
                else
                {
                    Debug.LogWarning("No valid NavMesh position found.");
                }
                yield return null;
            }
            if(final == Vector3.zero)
            {
                IsShooting = false;
                yield break;
            }
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
            do
            {
                _animator.SetTrigger("Shoot");
                AudioManager.instance.playSFX(AudioManager.instance.shootPistol);
                yield return new WaitForSeconds(shootRate);
                shootTime -= Time.deltaTime;
            } while (shootTime > 0);
            agent.stoppingDistance = originalStopDistance;
            IsShooting = false;
        }
    }
    public IEnumerator TookDamageTimer()
    {
        isTakingDamage = true;
        yield return new WaitForSeconds(notificationLength);
        isTakingDamage = false;
    }
    public void CallReinforcements(float distanceCheck)
    {
        SpawnManager.instance.TriggerEnemySpawn(transform.position,distanceCheck);
    }
    public void CreateBullet()
    {
        Instantiate(bullet, shootPos.position, shootPos.rotation);
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
        if (!IsShooting)
        {
            lookTarget = playerPos;
        }

        HP -= amount;
        healthBar.fillAmount = (float)HP / HPMax;
        
        if (HP <= 0)
        {
            if(enemyType is EnemyType.Boss)
            {
                GameManager.instance.WinGame();
            }
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
    public void SetDualWieldRig()
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
   
}
