using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{

    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text damageText = null;

        // Start is called before the first frame update
        public void DestroyText()
        {
            Destroy(gameObject);
        }
        public void SetDamageValue(float amount)
        {
            damageText.text = String.Format("{0:0}", amount);
        }
    }

}