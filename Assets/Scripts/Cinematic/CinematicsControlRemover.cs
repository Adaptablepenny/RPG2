using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Controller;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        GameObject player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
        }
        private void Start()
        {
            
           
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;//event methods add functions to their list and execute them as soon as the event occurs
            GetComponent<PlayableDirector>().stopped += EnableControl;//event methods add functions to their list and execute them as soon as the event occurs
            //In the scenario above, the DisableControl() is added to the list of functions that are executed when the method .played is called
            //Same thing goes for the .stopped
        }
        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;//event methods add functions to their list and execute them as soon as the event occurs
            GetComponent<PlayableDirector>().stopped -= EnableControl;//event methods add functions to their list and execute them as soon as the event occurs
            //In the scenario above, the DisableControl() is added to the list of functions that are executed when the method .played is called
            //Same thing goes for the .stopped
        }

        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }


        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}