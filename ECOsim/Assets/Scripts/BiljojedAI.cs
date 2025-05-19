using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BiljojedAI : MonoBehaviour
{
    //Brzina kretanja
    [Header("Brzina")]
    [SerializeField]
    private float moveSpeed;
    public float maxMoveSpeed;
    public float minMoveSpeed;
    
    //Vid
    [Header("Vid")]
    [SerializeField]
    private float sightRange;
    public float maxSightRange;
    public float minSightRange;
    
    //Glad
    [Header("Potrebe")]
    [SerializeField]
    private float hunger;
    private float dyingOfHunger;
    private float hungerRate;
    
    //Zedj
    [SerializeField]
    private float thirst;
    private float dyingOfThirst;
    private float thirstRate;
    
    //Zivotni vek
    [Header("Zivotni vek")] 
    public float lifespan;
    private float lifespanValue;
    public float maxLifespan;
    public float minLifespan;
    
    //Reprodukcija
    [Header("Reprodukcija")]
    [SerializeField]
    private bool gender;//0-zensko(false) 1-musko(true)
    public float isTimeToReproduce;
    private float readyToReproduceValue;
    public float maxReadyToReproduceValue;
    public float minReadyToReproduceValue;
    private float readyToReproduceRate;
    public float maxReadyToReproduceRate;
    public float minReadyToReproduceRate;
    public Boolean isBorn = false;

    public float maxGeneticMutation;
    public float minGeneticMutation;
    
    private Vector2 targetPosition;
    private bool hasTarget = false;    
    
    private bool isWaiting = false;
    private bool isInteracting = false;

    public Vector2 maxBounds;
    public Vector2 minBounds;

    public GameObject agentPrefab;
    
    
    
    void Start()
    {
        if (!isBorn)
        {
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
            sightRange = Random.Range(minSightRange, maxSightRange);
            hungerRate = (moveSpeed + sightRange) * 1.5f;
            thirstRate = (moveSpeed + sightRange) * 2f;
            readyToReproduceRate = Random.Range(minReadyToReproduceRate, maxReadyToReproduceRate);
            readyToReproduceValue = Random.Range(minReadyToReproduceValue, maxReadyToReproduceValue);
            lifespan = Random.Range(minLifespan, maxLifespan);
            lifespanValue = lifespan;
            
            int malefemale = Random.Range(0, 2);
            if (malefemale == 0)
            {
                gender = false;
            }
            else
            {
                gender = true;
            }
        }

    }
    
    void Update()
    {
        if (lifespan<=0)
        {
            Destroy(gameObject);
        }
        
        if (isWaiting)return;
        if (isInteracting)return;
        
        hunger += Time.deltaTime * hungerRate;
        thirst += Time.deltaTime * thirstRate;
        isTimeToReproduce += Time.deltaTime * readyToReproduceRate;
        lifespan -= Time.deltaTime;
        
        GameObject target = FindTarget();
        
        if ((hunger >= 120 || thirst >= 120)&&target!=null)
        {
            Destroy(gameObject);
        }

        if (isReadyToReproduce())
        {
            var mate = FindMate();
            if (mate!=null)
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
    }

    GameObject FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightRange);
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (var hit in hits)
        {
            if (hunger > 50f && hit.CompareTag("Food"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closest = hit.gameObject;
                    closestDistance = distance;
                }
            }
            if (thirst > 50f && hit.CompareTag("Water"))
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
                Debug.Log("Valid mate found: " + hit.name);
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
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))*radius;
        Vector2 point = (Vector2)transform.position + offset;
        return new Vector2(
            Math.Clamp(point.x,minBounds.x,maxBounds.x),
            Math.Clamp(point.y,minBounds.y,maxBounds.y)
            );
    }

    IEnumerator ReproduceWith(BiljojedAI mate)
    {
        isInteracting = true;
        hasTarget = false;
        yield return new WaitForSecondsRealtime(2f);
        
        Vector2 spawnPosition = (Vector2)transform.position;
        GameObject child = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);
        
        var childAi = child.GetComponent<BiljojedAI>();
        childAi.isBorn = true;

        geneInheritance(mate, childAi);
        
        isTimeToReproduce = 0;
        mate.isTimeToReproduce = 0;
        childAi.isTimeToReproduce = 0;
        
        isInteracting = false;
    }

    public void geneInheritance(BiljojedAI mate, BiljojedAI childAi)
    {
        float geneticMutation = Random.Range(minGeneticMutation, maxGeneticMutation);
        
        childAi.moveSpeed = (moveSpeed+mate.moveSpeed+sightRange)/2+geneticMutation;
        childAi.sightRange = (sightRange+mate.sightRange)/2+geneticMutation;
        
        childAi.hungerRate = (moveSpeed + sightRange) * 1.5f;
        childAi.thirstRate = (moveSpeed + sightRange) * 2f;

        childAi.lifespan = (lifespanValue + mate.lifespanValue)/2 + Random.Range(-3f, 3f);
        childAi.lifespanValue = (lifespanValue + mate.lifespanValue)/2 + Random.Range(-3f, 3f);
        childAi.readyToReproduceValue = (readyToReproduceValue + mate.readyToReproduceValue) / 2 + Random.Range(-10f, 10f);
        childAi.readyToReproduceRate = (readyToReproduceRate + mate.readyToReproduceRate) / 2 + Random.Range(-2f, 2f);
        
        int malefemale = Random.Range(0, 2);
        if (malefemale == 0) childAi.gender = false;
        else childAi.gender = true;
    }

    IEnumerator Interacting(GameObject target)
    {
        isInteracting = true;
        hasTarget = false;
        yield return new WaitForSecondsRealtime(1f);
        if (target.CompareTag("Food")) hunger = 0;
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
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition-currentPosition).normalized;
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        
        float stepSize = Mathf.Max(1.5f,moveSpeed*Time.deltaTime);
        float actualStep = Math.Min(stepSize, distanceToTarget);
        
        Vector2 pos = Vector2.MoveTowards(currentPosition, targetPosition, actualStep*Time.deltaTime*moveSpeed);
        
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        
        transform.position = pos;
    }

    private bool isReadyToReproduce()
    {
        return hunger<=40 && thirst<=40 && isTimeToReproduce>=readyToReproduceValue;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
