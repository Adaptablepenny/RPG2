using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Controller;

namespace RPG.SceneManagement

{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D
        }

        [SerializeField] int sceneIndex;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        private void OnTriggerEnter(Collider col)
        {
            var currScene = SceneManager.GetActiveScene().buildIndex;
            if (col.gameObject.tag == "Player")
            {
                //print("Load Scene");
                //SceneManager.LoadScene(sceneIndex);
                StartCoroutine(Transition());
                

                //load scene
            }
        }


        private IEnumerator Transition()
        {
            SceneFader fader = FindObjectOfType<SceneFader>();

            DontDestroyOnLoad(this.gameObject);

            //remove control
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeOut(0.5f);

            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneIndex);
            //remove control
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(0.5f);
            fader.FadeIn(1f);

            //restore control
            newPlayerController.enabled = true;

            Destroy(gameObject);
            
            
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.transform.position);
            player.transform.rotation = otherPortal.spawnPoint.transform.rotation;
            
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if(portal == this)
                {
                    continue;
                }
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}

