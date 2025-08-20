using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI; // required for NavMeshAgent
using Random = UnityEngine.Random;

public class BiljojedAI : MonoBehaviour
{
    // Brzina kretanja
    [Header("Brzina")]
    public float moveSpeed;
    public float maxMoveSpeed;
    public float minMoveSpeed;
    
    // Vid
    [Header("Vid")]
    [SerializeField]
    public float sightRange;
    public float maxSightRange;
    public float minSightRange;
    
    // Glad
    [Header("Potrebe")]
    [SerializeField]
    private float hunger;
    private float dyingOfHunger;
    public float hungerRate;
    
    // Zedj
    [SerializeField]
    private float thirst;
    private float dyingOfThirst;
    public float thirstRate;
    
    // Zivotni vek
    [Header("Zivotni vek")] 
    public float lifespan;
    public float lifespanValue;
    public float maxLifespan;
    public float minLifespan;
    
    // Reprodukcija
    [Header("Reprodukcija")]
    [SerializeField]
    private bool gender; // 0-zensko(false) 1-musko(true)
    public float isTimeToReproduce;
    public float readyToReproduceValue;
    public float maxReadyToReproduceValue;
    public float minReadyToReproduceValue;
    public float readyToReproduceRate;
    public float maxReadyToReproduceRate;
    public float minReadyToReproduceRate;
    public Boolean isBorn = false;
    public int maxChildren;
    public int numbOfChildren;
    
    // Boja
    [Header("Boja")]
    private Color color;
    private float rValue;
    private float gValue;
    private float bValue;

    [SerializeField]
    private bool albinoGene;
    [SerializeField]
    private bool isAlbino;
    
    [Header("Ostalo")]
    public float maxGeneticMutation;
    public float minGeneticMutation;
    
    private Vector2 targetPosition;
    private bool hasTarget = false;    
    
    private bool isWaiting = false;
    private bool isInteracting = false;

    public Vector2 maxBounds;
    public Vector2 minBounds;

    public GameObject agentPrefab;
    public Animator animator;
    private Vector2 previousPosition;

    // NavMeshAgent reference
    private NavMeshAgent agent;
    
    void Start()
    {
        previousPosition = transform.position;

        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false; // keep sprites upright
            agent.updateUpAxis = false;   // force XY plane
            agent.speed = moveSpeed;
        }
        
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.avoidancePriority = 99;

        if (!isBorn) // samo ako je na pocetku postavljen onda ce dobijati nasumicne osobine
        {
            hungerRate = (moveSpeed + sightRange);
            thirstRate = (moveSpeed + sightRange);
            
            lifespanValue = lifespan;
            color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            rValue = color.r;
            gValue = color.g;
            bValue = color.b;
            
            int malefemale = Random.Range(0, 2);
            gameObject.GetComponent<SpriteRenderer>().color = color;
            gender = malefemale != 0;

            int randAlbino = Random.Range(0, 5000);
            if (randAlbino == 2)
            {
                isAlbino = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                albinoGene = true;
            }
            else
            {
                int randomCarrier = Random.Range(0, 3000);
                if (randomCarrier == 2)
                {
                    albinoGene = true;
                }
            }
            
        }
    }
    
    void Update()
    {
        GameObject target = FindTarget();
        if ((hunger >= 200 || thirst >= 200) || lifespan <= 0)
        {
            Destroy(gameObject);
        }

        if (isWaiting) return;
        if (isInteracting) return;
        
        hunger += Time.deltaTime * hungerRate;
        thirst += Time.deltaTime * thirstRate;
        isTimeToReproduce += Time.deltaTime * readyToReproduceRate;
        lifespan -= Time.deltaTime;

        if (isReadyToReproduce())
        {
            var mate = FindMate();
            if (mate != null)
            {
                targetPosition = mate.transform.position;
                hasTarget = true;

                if (Vector2.Distance(transform.position, mate.transform.position) < 0.5f)
                {
                    StartCoroutine(ReproduceWith(mate.GetComponent<BiljojedAI>()));
                }
            }
        }
        
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);

            if (dist < 0.5f)
            {
                StartCoroutine(Interacting(target));
                return;
            }

            targetPosition = target.transform.position;
            hasTarget = true;
        }
        
        if (!hasTarget || Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            StartCoroutine(WaitBeforGoing());
        }

        MoveTowardsTarget();
        UpdateAnimation();
    }

    GameObject FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightRange);
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (var hit in hits)
        {
            if (hunger > 135f && hit.CompareTag("Food"))
            {
                var food = hit.GetComponent<FoodGrowth>();
                if (food == null || food.numbOfFruits <= 0) continue;
                
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closest = hit.gameObject;
                    closestDistance = distance;
                }
            }
            if (thirst > 135f && hit.CompareTag("Water"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closest = hit.gameObject;
                    closestDistance = distance;
                }
            }
        }
        return closest;
    }

    GameObject FindMate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightRange);
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Biljojed")) continue;

            var other = hit.GetComponent<BiljojedAI>();
            if (other == null || other == this) continue;
            if (isReadyToReproduce() && other.isReadyToReproduce() && gender != other.gender)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closest = hit.gameObject;
                    closestDistance = distance;
                }
            }
        }
        return closest;
    }

    Vector2 GetRandomTargetInSightRange()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(0.3f, sightRange);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        Vector2 point = (Vector2)transform.position + offset;

        // Clamp inside min/max bounds
        point.x = Mathf.Clamp(point.x, minBounds.x, maxBounds.x);
        point.y = Mathf.Clamp(point.y, minBounds.y, maxBounds.y);

        // Snap to NavMesh (so we don't pick unwalkable spots)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, 1f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // fallback: just return current position if no valid navmesh found
        return transform.position;
    }

    IEnumerator ReproduceWith(BiljojedAI mate)
    {
        isInteracting = true;
        hasTarget = false;
        yield return new WaitForSecondsRealtime(2f);

        if (gender == false) // ako je zensko
        {
            for (int i = 0; i < numbOfChildren; i++)
            {
                Vector2 spawnPosition = (Vector2)transform.position;
                GameObject child = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);
        
                var childAi = child.GetComponent<BiljojedAI>();
                childAi.isBorn = true;

                geneInheritance(mate, childAi);
                childAi.isTimeToReproduce = 0;
            }
        }
        
        isTimeToReproduce = 0;
        mate.isTimeToReproduce = 0;
        
        isInteracting = false;
    }

    public void geneInheritance(BiljojedAI mate, BiljojedAI childAi)
    {
        float geneticMutation = Random.Range(minGeneticMutation, maxGeneticMutation);
        
        childAi.moveSpeed = (moveSpeed + mate.moveSpeed + sightRange) / 2 + geneticMutation;
        childAi.sightRange = (sightRange + mate.sightRange) / 2 + geneticMutation;
        
        childAi.hungerRate = (moveSpeed + sightRange);
        childAi.thirstRate = (moveSpeed + sightRange);

        childAi.lifespan = (lifespanValue + mate.lifespanValue) / 2 + Random.Range(-50f, 50f);
        childAi.lifespanValue = (lifespanValue + mate.lifespanValue) / 2 + Random.Range(-50f, 50f);
        childAi.readyToReproduceValue = (readyToReproduceValue + mate.readyToReproduceValue) / 2 + Random.Range(-30f, 30f);
        childAi.readyToReproduceRate = (readyToReproduceRate + mate.readyToReproduceRate) / 2 + Random.Range(-3.5f, 3.5f);
        
        int rand = Random.Range(0, 15);
        if (rand == 1)
            childAi.numbOfChildren = (int)Math.Round(((float)numbOfChildren + (float)mate.numbOfChildren) / 2) + 1;
        else childAi.numbOfChildren = (int)Math.Round(((float)numbOfChildren + (float)mate.numbOfChildren) / 2);

        childAi.rValue = Random.Range(rValue, mate.rValue);
        childAi.gValue = Random.Range(gValue, mate.gValue);
        childAi.bValue = Random.Range(bValue, mate.bValue);
        
        childAi.color = new Color(childAi.rValue, childAi.gValue, childAi.bValue);
        isChildAlbino(mate, childAi);

        int malefemale = Random.Range(0, 2);
        childAi.gender = malefemale != 0;
    }

    public void isChildAlbino(BiljojedAI mate, BiljojedAI childAi)
    {
        if (!albinoGene && !mate.albinoGene)//oba nemaju gen(AA i AA) 0% albnio
        {
            childAi.albinoGene = false;
            childAi.isAlbino = false;
        }
        else if (albinoGene!=mate.albinoGene)//jedan od roditelja ima gen ali niko nije albino(AA i Aa ili Aa i AA) 50% nosi gen 50% ne
        {
            int rand = Random.Range(0, 2);
            if (rand==0)
            {
                childAi.albinoGene = true;
                childAi.isAlbino = false;
            }
            else
            {
                childAi.albinoGene = false;
                childAi.isAlbino = false;
            }
        }
        else if(albinoGene && mate.albinoGene)//oba roditelja imaju gen ali nisu albino(Aa i Aa) 25% albino 25% nema gen 50% ima gen
        {
            int rand = Random.Range(0, 4);
            if (rand == 0)
            {
                childAi.albinoGene = false;
                childAi.isAlbino = false;
            }
            else if (rand==1)
            {
                childAi.albinoGene = true;
                childAi.isAlbino = true;
                childAi.color = Color.white;
            }
            else
            {
                childAi.albinoGene = true;
                childAi.isAlbino = false;
            }
        }
        else if (isAlbino != mate.isAlbino && !mate.albinoGene)//jedan je albino drugi nije i ne nosi gen (aa i AA) 100% Aa
        {
            childAi.albinoGene = true;
            childAi.isAlbino = false;
        }
        else if (isAlbino!=mate.isAlbino && mate.albinoGene)//jedan je albino drugi nije ali ima gen (aa i Aa) 25% Aa 75% aa
        {
            int rand = Random.Range(0, 4);
            if (rand == 0)
            {
                childAi.albinoGene = true;
                childAi.isAlbino = false;
            }
            else
            {
                childAi.albinoGene = true;
                childAi.isAlbino = true;
                childAi.color = Color.white;
            }
        }
        else if (isAlbino==mate.isAlbino)//oba su albino (aa i aa) 100% albino
        {
            childAi.albinoGene = true;
            childAi.isAlbino = true;
            childAi.color = Color.white;
        }
    }

    IEnumerator Interacting(GameObject target)
    {
        isInteracting = true;
        hasTarget = false;
        yield return new WaitForSecondsRealtime(1f);
        if (target.CompareTag("Food"))
        {
            var food = target.GetComponent<FoodGrowth>();
            if (food != null && food.TryConsumeFruit())
            {
                hunger = 0;
            }
            else
            {
                isInteracting = false;
                yield break;
            }
        }
        if (target.CompareTag("Water")) thirst = 0;
        isInteracting = false;
    }
    
    IEnumerator WaitBeforGoing()
    {
        isWaiting = true;
        hasTarget = false;
        yield return new WaitForSecondsRealtime(0f);
        targetPosition = GetRandomTargetInSightRange();
        hasTarget = true;
        isWaiting = false;
    }

    private void MoveTowardsTarget()
    {
        if (hunger < 190)
        {
            if (agent == null) return;
            agent.speed = moveSpeed;
            agent.SetDestination(targetPosition);
        }
    }

    private bool isReadyToReproduce()
    {
        return hunger <= 60 && thirst <= 80 && isTimeToReproduce >= readyToReproduceValue;
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;

        float movementThreshold = 0.0001f;
        float distanceMoved = Vector2.Distance((Vector2)transform.position, previousPosition);

        bool isWalking = distanceMoved > movementThreshold;
        animator.SetBool("isWalking", isWalking);
        
        previousPosition = transform.position;
        if (!isWalking)
        {
            transform.rotation = Quaternion.identity;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
