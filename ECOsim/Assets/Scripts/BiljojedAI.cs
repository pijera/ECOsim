using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BiljojedAI : MonoBehaviour
{
    public float moveSpeed;
    public float sightRange;

    public float hunger;
    public float hungerRate;
    public float thirst;
    public float thirstRate;
    
    private Vector2 targetPosition;
    private bool hasTarget = false;    
    
    private bool isWaiting = false;
    private bool isInteracting = false;

    public Vector2 maxBounds;
    public Vector2 minBounds;
    void Update()
    {
        if (isWaiting)return;
        if (isInteracting)return;
        
        hunger += Time.deltaTime * hungerRate;
        thirst += Time.deltaTime * thirstRate;

        GameObject target = FindTarget();
        
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);

            if (dist < 1.5f)
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
        yield return new WaitForSecondsRealtime(0.5f);
        targetPosition = GetRandomTargetInSightRange();
        hasTarget = true;
        isWaiting = false;
    }

    private void MoveTowardsTarget()
    {
        Vector2 pos = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);
        
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        
        transform.position = pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
