using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {

        const string saveFile = "save";
        SceneFader fader;

        void Start()
        {           
        }

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(LoadLastScene());
                
            }

            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(saveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(saveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(saveFile);
        }


        public IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(saveFile);
            fader = FindObjectOfType<SceneFader>();
            //GetComponent<SavingSystem>().Load(saveFile);

            fader.FadeOutImmediate();

            

            yield return fader.FadeIn(1f);

        }
    }
}
