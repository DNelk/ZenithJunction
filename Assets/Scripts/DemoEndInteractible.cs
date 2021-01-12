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

   private void Update()
   {
      if (_triggered)
      {
         if (Input.GetKeyDown(KeyCode.Return))
         {
            //PlayerPrefs.SetInt("restart",1);
            OverworldTrain.Instance.CurrentNode = startingNode;
            DemoManager.Instance.LoopDemo();
         }
      }
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
