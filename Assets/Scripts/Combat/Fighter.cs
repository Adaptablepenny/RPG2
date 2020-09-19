using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] WeaponConfig defaultWeapon = null;
        
        
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        

        private void Start()
        {
            currentWeapon.ForceInit();

            
        }
        
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
           
        }
        private void Update()
        {

            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;//if no target dont do anything below
            if (target.IsDead()) return;

            if (!GetInRange())// if there is a target and you are not in range, move into range
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }

            else// once in range stop
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)//Cooldown timer between attacks
            {
                //This will trigger Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;

            }
        }

        private void TriggerAttack()//triggers animations by reseting the cancel attack trigger, and setting the attack trigger
        {
            GetComponent<Animator>().ResetTrigger("CancelAttack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        public bool CanAttack(GameObject combatTarget)//Checks to see if the target is attackable
        {
            if(combatTarget == null ) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        void Hit()//Animation Event
        {

            if (target == null) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                
                target.TakeDamage(gameObject, damage);
            }
            
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetInRange()//Gets the distance between the player and the target and checks to see if it is within the range
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetRange();
        }

        public void Attack(GameObject combatTarget)//Declares the target as the selected combat target
        {
            GetComponent<ActionScheduler>().StartAction(this);//Although states start it actually cancels the previous action to move on to the next
            target = combatTarget.GetComponent<Health>();
            
        }

        public void Cancel()//clears the target and stops attacking
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()//triggers animations by reseting the attack trigger, and setting the cancel attack trigger
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("CancelAttack");
        }


        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            currentWeaponConfig = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(currentWeaponConfig);
        }

        public Health GetTarget()
        {
            return target;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}

