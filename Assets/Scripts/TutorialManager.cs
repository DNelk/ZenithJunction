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

      _currentHead.CharacterName = "";
  
      yield return new WaitUntil(() => _currentHead.IsIdle);

      _currentHead.Dialogue = "Ok kid.. now's your chance to prove yourself. Remember your training?";

      yield return new WaitUntil(AdvanceText);

      //Change Character head
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "...";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.CharacterName = "";
      _currentHead.Dialogue = "...I see how it is. I swear, you squires get sloppier every year.";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "Well kid, if you want to be a full Knight of the Rails, the number one thing is using the rails to channel <color=red>Power</color>" + Utils.ReplaceWithSymbols("power") + " into your attacks.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Now focus on bashing this guy over the head. You should manifest some spells to do some hurt.";
      
      yield return new WaitUntil(AdvanceText);
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);

      yield return fadeTween.WaitForCompletion();
      
      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);

      _currentHead.Dialogue = "There you go. Each of those strikes will give you one <color=red>Power</color>" + Utils.ReplaceWithSymbols("power") + ".";

   }

   private bool AdvanceText()
   {
      return _currentHead.TextDone && Input.anyKey;
   }
}
