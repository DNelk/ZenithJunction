using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
   public static TutorialManager Instance = null;

   public int TutorialStep;
   
   private void Awake()
   {
      if (Instance == null)
         Instance = this;
      else if(Instance != this)
         Destroy(gameObject);
   }

   private void Start()
   {
      TutorialStep = 0;
   }

   public void Step()
   {
      switch (TutorialStep)
      {
         case 0:
            Step1();
            break;
      } 
   }

   private void Step1()
   {
      List<string> tutDeck = new List<string>();
      tutDeck.Add("strike");
      tutDeck.Add("strike");
      tutDeck.Add("strike");

      DeckManager.Instance.LoadDeck(tutDeck);

      DeckManager.Instance.DealAmt = 3;
   }
}
