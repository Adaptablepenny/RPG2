﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.SceneManagement
{

    public class SceneFader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        Coroutine currentActiveFade = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha,target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }


        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);

        }
        

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

    }

}

