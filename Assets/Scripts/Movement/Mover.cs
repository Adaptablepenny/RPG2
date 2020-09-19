using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {

        NavMeshAgent agent;
        Health health;
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        // Start is called before the first frame update

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            agent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction (Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);//Although states start it actually cancels the previous action to move on to the next
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction) //Moves player to where they clicked
        {
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
            
        }

        public void Cancel()//stops the character from moving
        {
            agent.isStopped = true;
        }

        
        private void UpdateAnimator()
        {
            Vector3 vel = GetComponent<NavMeshAgent>().velocity; //gets the velocity from the nav mesh agent
            Vector3 localvel = transform.InverseTransformDirection(vel);//converts the velocity from world space to local space
            float speed = localvel.z;//gets the z velocity and stores it in the speed variable
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed); //Applies the speed to the ForwardSpeed parameter in the blend tree
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
