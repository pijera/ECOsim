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
    void Update()
    {
        if (isWaiting)return;
        hunger += Time.deltaTime * hungerRate;
        thirst += Time.deltaTime * thirstRate;

        GameObject target = FindTarget();
        if (target != null && Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
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
        return (Vector2)transform.position + offset;
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
        float dis = Vector2.Distance(transform.position, targetPosition);
        if (dis > 0.2f)
        {
            Vector2 pos = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed*Time.deltaTime);
            transform.position = pos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
