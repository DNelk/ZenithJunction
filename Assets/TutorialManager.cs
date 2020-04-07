using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
   public static TutorialManager Instance = null;

   public int TutorialStep;

   private TalkingHead _currentHead;
   
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
            StartCoroutine(Step1());
            break;
      } 
   }

   private IEnumerator Step1()
   {
      List<string> tutDeck = new List<string>();
      tutDeck.Add("strike");
      tutDeck.Add("strike");
      tutDeck.Add("strike");

      DeckManager.Instance.LoadDeck(tutDeck);

      DeckManager.Instance.DealAmt = 3;
      BattleManager.Instance.Engines[1].gameObject.SetActive(false);
      BattleManager.Instance.Engines[2].gameObject.SetActive(false);

      _currentHead = Utils.GenerateTalkingHead();

      _currentHead.Name = "Test Name!";
  
      yield return new WaitUntil(() => _currentHead.IsIdle);

      _currentHead.Dialogue = "Hello, welcome to Zenith Junction. A very good card game.";

      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "Yes, this game is so good. Now lets deal cards";
      
      yield return new WaitUntil(AdvanceText);
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);

      yield return fadeTween.WaitForCompletion();
      
      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);

      _currentHead.Dialogue = "There are the cards!";

   }

   private bool AdvanceText()
   {
      return _currentHead.TextDone && Input.anyKey;
   }
}
