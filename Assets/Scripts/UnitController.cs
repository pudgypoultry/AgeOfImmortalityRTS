using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public float health;
    public float resource;
    public float movementSpeed;
    public float attackRange;
    public float attackDamage;
    public float attackSpeed;
    private float meleeRange = 1;
    private float attackRangeTolerance = 1;

    private NavMeshAgent navAgent;
    private Transform currentTarget;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = movementSpeed;
        navAgent.stoppingDistance = meleeRange;
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            navAgent.destination = currentTarget.position;
        }

        DeathFunction();
    }

    public void MoveUnit(Vector3 dest)
    {
        currentTarget = null;
        navAgent.destination = dest;
    }

    public void SetSelected(bool isSelected)
    {
        // transform.Find("Highlight").gameObject.SetActive(isSelected);
    }

    public void SetNewTarget(Transform enemy)
    {
        currentTarget = enemy;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void DeathFunction()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void AttackCommand(GameObject target)
    {
        //set navmesh agent stopping range to attack range
        navAgent.stoppingDistance = attackRange - attackRangeTolerance;
        //move navmesh agent toward target
        navAgent.destination = target.transform.position;
        //if unit is < attack range away from target
        //check difference between this object and target, take magnitude, check magnitude against attack range
        if (Vector3.Magnitude(gameObject.transform.position - target.transform.position) <= attackRange)
        {
            navAgent.destination = gameObject.transform.position;
            //look at target
            //call target's TakeDamage function with input attackDamage over interval attackSpeed


        }
        //else keep moving
        else
        {
            navAgent.destination = target.transform.position;
        }
    }
}
