using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Controller
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 10f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;

        [Range(0,1)]
        [SerializeField] float speedFraction = 0.2f;
        

        Fighter fight;
        GameObject player;
        Health health;

        float timeSinceLastWaypoint = Mathf.Infinity;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        //Vector3 guardPosition;
        int currentWaypointIndex = 0;

        LazyValue<Vector3> guardPosition;


        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fight = GetComponent<Fighter>();
            health = GetComponent<Health>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }
        private void Start()
        {
            guardPosition.ForceInit();
            
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }
        private void Update()
        {
            if (health.IsDead()) return;// checks to see if the AI is dead
            if (DistanceToPlayer() && fight.CanAttack(player))
            {
                
                AttackBehaviour();
            }
            else if (suspicionTime > timeSinceLastSawPlayer)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastWaypoint += Time.deltaTime;
            
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPos = guardPosition.value;
            //Decrease speed to patrol
           
            if (patrolPath != null)
            {
                
                if(AtWaypoint())
                {
                    timeSinceLastWaypoint = 0;
                    CycleWaypoint();

                }
                nextPos = GetCurrentWaypoint();
                
                
            }


            if(waypointDwellTime < timeSinceLastWaypoint)
            {
                GetComponent<Mover>().StartMoveAction(nextPos, speedFraction);
            }
            
            
           

        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }


        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            //Increase speed to chase
            fight.Attack(player);
            timeSinceLastSawPlayer = 0;
        }

        private bool DistanceToPlayer()//calculates distance between AI and target (player)
        {            
            float distanceToPlayer =  Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }


        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color =  Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

