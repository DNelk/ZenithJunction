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

   public bool TriggerAfterBattle = false;
   
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
         case 1:
            StartCoroutine(Step2());
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
      BattleManager.Instance.NumEngines = 1;

      _currentHead = Utils.GenerateTalkingHead();

      _currentHead.CharacterName = "Sir John";
  
      yield return new WaitUntil(() => _currentHead.IsIdle);

      _currentHead.Dialogue = "Ok kid.. now's your chance to prove yourself. Remember your training?";

      yield return new WaitUntil(AdvanceText);

      //Change Character head
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "...";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "...I see how it is. I swear, you squires get sloppier every year.";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "Well kid, if you want to be a full Knight of the Rails, the number one thing is using the rails to channel <color=red>Power</color>" + Utils.ReplaceWithSymbols("power") + " into your attacks.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Now focus on bashing this guy over the head. You should manifest some attacks in the form of cards to do some hurt.";
      
      yield return new WaitUntil(AdvanceText);
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);

      yield return fadeTween.WaitForCompletion();
      
      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);

      _currentHead.name = "Hugo";
      _currentHead.Dialogue = "Uuurrahh!";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.name = "Sir John";
      _currentHead.Dialogue = "There you go. Each of those strikes will give you one <color=red>Power</color>" + Utils.ReplaceWithSymbols("power") + ".";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "They always told us in the academy to put your mouse (whatever that is) over those cards to get a better picture.";

      yield return new WaitUntil(() => Utils.CurrentPreview != null);
      
      _currentHead.Dialogue = "Ok, now it's time to actually get this guy";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "As you were supposed to know, on the left side over there is the <color=orange>Engine</color>.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Our relics- our weapons, use cards to channel the energy of the rails into the <color=orange>Engine</color>."; 
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "The <color=orange>Engines</color> let us take our individual spells and attacks and combine them into a powerful force.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Just like our knightly order... stronger together!";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Exactly, kid.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue =  "Now drag those 3 Strikes into the <color=orange>Engine</color>. You can also right click each card to go to the first open <color=orange>Engine</color>.";
      
      yield return new WaitUntil(() => BattleManager.Instance.Engines[0].PendingCount == 3);

      _currentHead.Dialogue = "Since each of those strikes gives you 1  <color=red>Power</color>" +
                              Utils.ReplaceWithSymbols("power") + ", our whole <color=orange>Engine</color> has" +
                              " 3 <color=red>Power</color>" + Utils.ReplaceWithSymbols("power") + " total.";
     
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Ok, you're ready. Press confirm to lock in our engine so we can face off.";

      yield return new WaitUntil(() => BattleManager.Instance.BattleState == BattleStates.ChoosingAction);
      
      _currentHead.Dialogue = "Now select the engine you want to battle the enemy with and press confirm.";
      bool advance = false;
      BattleManager.Instance.ConfirmButton.onClick.AddListener(() => advance = true);
      
      yield return new WaitUntil(() => advance);
      
      _currentHead.RollOut();
      TutorialStep++;
      TriggerAfterBattle = true;
   }


   private IEnumerator Step2()
   {
      List<string> tutDeck = new List<string>();
      tutDeck.Add("manaboil");
      tutDeck.Add("manaboil");
      tutDeck.Add("manaboil");

      DeckManager.Instance.LoadDeck(tutDeck);

      DeckManager.Instance.DealAmt = 3;
      BattleManager.Instance.Engines[1].gameObject.SetActive(false);
      BattleManager.Instance.Engines[2].gameObject.SetActive(false);
      BattleManager.Instance.NumEngines = 1;

      _currentHead = Utils.GenerateTalkingHead();

      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Whoa! I hit him!";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Just barely. If you want to actually hurt your opponent, you'll have to have a higher  <color=red>Power</color>" + Utils.ReplaceWithSymbols("power") + "than them.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "As long as you keep training for the next few years, keep pushing your body to the limit, you'll win every battle- morally that is.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "...uh, Sir John, I don't think we're gonna live for a few more years if this guy has anything to say about it.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "Exactly my dear boy. Which is why all Knights worth their salt know true greatness comes from studying the truth of our world; the potential of Aetheric Science";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Sir Wolff! Are you talking about-";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "Yes, young Hugo... Magic!";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "All advanced tools of our world utilize the 'magic' of <color=blue>Aether</color>," +
                              " the element the great Mages of the past, the <color=purple>Locomancers</color>," +
                              " harnessed to create the rails and our very relics!";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "The key to understanding the mechanics of <color=blue>Aether</color> transmutation first requires an understanding of " +
                              "the molecular structure of <color=blue>Aether</color> crystals-";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Um, Sir Wolff, can we hurry the lecture a little... this guy looks mad.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "Hm. Given the circumstances, it does seem expedient to give you an abridged lesson.";
      
      yield return new WaitUntil(AdvanceText);
      _currentHead.Dialogue =
         "Briefly close your eyes and feel the mana coursing through the Leylines just underneath the surface of the ground." +
         " These lines are weaved into the rails, supplying all people with their power.";
      
      yield return new WaitUntil(AdvanceText);
      _currentHead.Dialogue = "Focus.";
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);

      yield return fadeTween.WaitForCompletion();
      
      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Like this?";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "Nicely done.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "As you can see, the most simple way to access the power of <color=blue>Aether</color>" + Utils.ReplaceWithSymbols("aether") +
                              " is through those 'mana boil' cards there.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Each one will give you precisely one unit of <color=blue>Aether</color>" +
                              Utils.ReplaceWithSymbols("aether.");
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "With the right equipment, color=blue>Aether</color>" +
                              Utils.ReplaceWithSymbols("aether ") + "can be cultivated to perform even more "+
                              "arcane and powerful spells, and amplify your potential skills to new levels.";
      
      yield return new WaitUntil(AdvanceText);
   }
   

   private bool AdvanceText()
   {
      return _currentHead.TextDone && Input.anyKey;
   }
}
