using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool alreadyTriggered = false;
        private void OnTriggerEnter(Collider col)
        {
           if (!alreadyTriggered && col.gameObject.tag == "Player")
           {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
           }
        }
    }
}

