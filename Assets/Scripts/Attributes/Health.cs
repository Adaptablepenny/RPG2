using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;
using UnityEngine;
using RPG.Core;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70f;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;

        LazyValue<float> healthPoints;//imported from lesson
        bool isDead = false;


        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);//imported from lesson
        }
        private void Start()
        {

            healthPoints.ForceInit();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += HealOnLevelUp;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= HealOnLevelUp;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void HealOnLevelUp()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
            
        }

        public void Heal(float amount)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + amount, GetMaxHealthPoints());
            
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);//keeps the health points from dropping below zero

            if (healthPoints.value <= 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();

            if(experience == null) { return; }

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void Die()
        {
            if (isDead) { return; }//checks the target to see if its dead

            isDead = true;
            GetComponent<Animator>().SetTrigger("Death");//plays death animation
            GetComponent<CapsuleCollider>().enabled = false;//Disables collider on the object
            GetComponent<ActionScheduler>().CancelCurrentAction();//Cancels actions of the script that was running
        }

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}

