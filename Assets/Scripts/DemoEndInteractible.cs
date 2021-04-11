using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoEndInteractible : MapObstacle
{
   public GameObject EndMessage;
   private bool _triggered = false;
   public MapNode startingNode;
   public Image fadeInImage;
   public float fadeTime;

   private void Awake()
   {
      StartCoroutine(FadeOutScreen(0));
   }

   private void Update()
   {
      if (_triggered)
      {
         if (Input.GetKeyDown(KeyCode.Return))
         {
            StartCoroutine(FadeInScreen(1));
         }
      }
   }

   private IEnumerator FadeInScreen(float targetValue)
   {
      fadeInImage.gameObject.SetActive(true);
      fadeInImage.DOFade(targetValue, fadeTime);
      //Fade out audio
      BGMManager.Instance._audio.DOFade(0, fadeTime);
      yield return new WaitForSeconds(fadeTime + 0.5f);
      OverworldTrain.Instance.CurrentNode = startingNode;
      DemoManager.Instance.LoopDemo();
   }
   
   private IEnumerator FadeOutScreen(float targetValue)
   {
      fadeInImage.DOFade(targetValue, fadeTime);
      yield return new WaitForSeconds(fadeTime + 0.5f);
      fadeInImage.gameObject.SetActive(false);
   }
   
   private void OnTriggerStay2D(Collider2D other)
   {
      OnTriggerEnter2D(other);
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.CompareTag("OverworldTrain"))
         return;
      if(OverworldTrain.Instance.NextNode != Node)
         return;
      if(_triggered)
         return;
      _triggered = true;
      EndMessage.GetComponent<CanvasGroup>().DOFade(1, 2f);
      EndMessage.GetComponent<Image>().raycastTarget = true;
   }
}
