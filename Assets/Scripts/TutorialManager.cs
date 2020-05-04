using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
   public static TutorialManager Instance = null;

   public int TutorialStep = 0;

   private TalkingHead _currentHead;

   public bool TriggerAfterBattle = true;

   private void Awake()
   {
      if (Instance == null)
         Instance = this;
      else if(Instance != this)
         Destroy(gameObject);
   }

   private void Start()
   {
      
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
         case 2:
            StartCoroutine(Step3());
            break;
         case 3:
            StartCoroutine(Step4());
            break;
         case 4:
            StartCoroutine(Step5());
            break;
         case 5:
            StartCoroutine(Step6());
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

      _currentHead = Utils.GenerateTalkingHead(GameObject.Find("MainCanvas").transform);

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

      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Uuurrahh!";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.CharacterName = "Sir John";
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

      _currentHead = Utils.GenerateTalkingHead(GameObject.Find("MainCanvas").transform);

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
      
      _currentHead.Dialogue = "With the right equipment, <color=blue>Aether</color>" +
                              Utils.ReplaceWithSymbols("aether ") + "can be cultivated to perform even more "+
                              "arcane and powerful spells, and amplify your potential skills to new levels.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "But if I'm focusing on making aether, isn't that thing gonna hurt me?";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "True. But that is the price of progress.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Now let's load the engine with these aether cards.";
      
      yield return new WaitUntil(AdvanceText);

      
      _currentHead.RollOut();
      yield return null;
      TutorialStep++;
   }

   private IEnumerator Step3()
   {
      List<string> tutDeck = new List<string>();
      tutDeck.Add("railcharge");

      BuyManager.Instance.LoadNewCatalog(tutDeck);
      BuyManager.Instance.DealAmt = 1;

      _currentHead = Utils.GenerateTalkingHead(GameObject.Find("BuyScreen").transform);

      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Yeowch! That stings!";

      yield return new WaitUntil(AdvanceText);

      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Pain is in the mind!";

      yield return new WaitUntil(AdvanceText);

      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);

      yield return fadeTween.WaitForCompletion();

      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "...yes. Either way, you can use the <color=blue>Aether</color>" +
                              Utils.ReplaceWithSymbols("aether ") + "you channeled to transmute a new card.";
                              
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Please note that this is not a permanent transmutation. After this encounter you'll forget about it like you never learned it!";


      _currentHead.Dialogue = "You have 3 <color=blue>Aether</color>" +
                              Utils.ReplaceWithSymbols("aether, ") + "so go ahead and buy that Rail Charge.";
      yield return new WaitUntil(() => DeckManager.Instance.InDiscardCount() == 4);

      _currentHead.Dialogue = "Transmutation Complete! Now let's see that new skill in action!";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.RollOut();
      TutorialStep++;
   }

   private IEnumerator Step4()
   {
      List<string> tutDeck = new List<string>();
      tutDeck.Add("strike");
      tutDeck.Add("strike");
      tutDeck.Add("railcharge");

      DeckManager.Instance.LoadDeck(tutDeck);

      DeckManager.Instance.DealAmt = 3;
      BattleManager.Instance.Engines[1].gameObject.SetActive(false);
      BattleManager.Instance.Engines[2].gameObject.SetActive(false);
      BattleManager.Instance.NumEngines = 1;

      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);
      
      _currentHead = Utils.GenerateTalkingHead(GameObject.Find("MainCanvas").transform);

      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Oh yeah! I like what's gonna happen here! Give him hell, kid!";

      yield return new WaitUntil(AdvanceText); 
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);
      yield return fadeTween.WaitForCompletion();

      _currentHead.RollOut();
      TutorialStep++;
   }
   
   private IEnumerator Step5()
   {
      List<string> tutDeck = new List<string>();
      tutDeck.Add("allaboard");
      tutDeck.Add("strike");
      tutDeck.Add("tinylittlelaserbeam");

      DeckManager.Instance.LoadDeck(tutDeck);

      DeckManager.Instance.DealAmt = 3;
      BattleManager.Instance.Engines[1].gameObject.SetActive(false);
      BattleManager.Instance.Engines[2].gameObject.SetActive(false);
      BattleManager.Instance.NumEngines = 1;

    
      _currentHead = Utils.GenerateTalkingHead(GameObject.Find("MainCanvas").transform);

      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "Now do you see the value of Aether transmutation?";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Oh yeah! Did you see how powerful I was?";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "I feel that I have one more thing to tell you about before we let you loose: <color=yellow>Ranged</color> Tactics!";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "You see, you won't always be able to simply remain in the same spot and take a beating from some ne'er-do-well." +
                              " You'll often want to <color=yellow>Move</color>" + Utils.ReplaceWithSymbols("move ") + "around to dodge, plan, and retreat!";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Retreat?! Never! But you will need to get back up in the enemy's face if they get away from you, that's true.";

      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "And of course, there's the matter of lining up a nicely timed <color=yellow>Ranged</color> attack.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "A shockingly salient point coming from you, John.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Try this hand of cards:";

      yield return new WaitUntil(AdvanceText);
      
      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);
      yield return fadeTween.WaitForCompletion();


      _currentHead.Dialogue = "Two cards are of note in this hand. Your \"All Aboard\" and your \"Tiny Little Laser Beam.\"";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "The \"Tiny Little Laser Beam.\" has two range markers in the corner as you can see.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "That means it's a <color=yellow>long range</color> attack.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Let's go down the list!";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "No Range Markers: That's a <color=yellow>melee</color> attack. It only gives you power if you're right next to the enemy.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "One Range Marker: That's a <color=yellow>short range</color> attack. It gives you power if you're next to the enemy or one car away. Any further, not gonna work.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Two Range Markers: That's <color=yellow>long range</color>. It's only working if you're not next to the enemy.";

      yield return new WaitUntil(AdvanceText);

      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "So as you can see, <color=yellow>Moving</color>" + Utils.ReplaceWithSymbols("move ") + "will open up new possibilities of attack.";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "The \"All Aboard\" card you see there can either be used at melee range, or can <color=yellow>Move</color>" + Utils.ReplaceWithSymbols("move ") +"you further from our foe.";

      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "<color=yellow>Moving</color>" + Utils.ReplaceWithSymbols("move ") +
                              "one space away will let you use that laser beam.";

      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Since the enemy is only attacking us at melee range, you'll not only dodge it's attack, but you'll have more power and win the exchange!";
      
      yield return new WaitUntil(AdvanceText);

      _currentHead.Dialogue = "Give it a try!";
      
      yield return new WaitUntil(AdvanceText);
         
      _currentHead.RollOut();
      TutorialStep++;
   }

   private IEnumerator Step6()
   {
      PlayerCollection pc = Utils.Load<PlayerCollection>("playercollection");
      BuyManager.Instance.GenerateCatalog();
      BuyManager.Instance.DealAmt = 5;
      DeckManager.Instance.LoadDeck(pc.Equipped);

      DeckManager.Instance.DealAmt = 9;
      
      _currentHead = Utils.GenerateTalkingHead(GameObject.Find("MainCanvas").transform);

      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "And with that, I believe you will be quite able to handle this foe on your own.";
      
      yield return new WaitUntil(AdvanceText); 
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "Don't hold back. Show him what you're made of.";
      
      yield return new WaitUntil(AdvanceText); 

      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Alright. Here I go-";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "...";
      _currentHead.Dialogue = "BRRRR";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Hugo";
      _currentHead.Dialogue = "Whoa... wait a second! what's happening to my arm!";
      
      yield return new WaitUntil(AdvanceText);
      
      Tween fadeTween = _currentHead.BG.DOFade(0f, 0.1f);
      yield return fadeTween.WaitForCompletion();
      
      BattleManager.Instance.Engines[1].gameObject.SetActive(true);
      BattleManager.Instance.Engines[2].gameObject.SetActive(true);
      BattleManager.Instance.Engines[1].GetComponent<CanvasGroup>().alpha = 0;
      BattleManager.Instance.Engines[2].GetComponent<CanvasGroup>().alpha = 0;

      Sequence engineLoadTween = DOTween.Sequence();
      engineLoadTween.Append(BattleManager.Instance.Engines[1].GetComponent<CanvasGroup>().DOFade(1f, 1f));
      engineLoadTween.Join(BattleManager.Instance.Engines[2].GetComponent<CanvasGroup>().DOFade(1f, 1f));

      BattleManager.Instance.NumEngines = 3;

      yield return engineLoadTween.WaitForCompletion();
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "I-impossible.. it's...";
      
      yield return new WaitUntil(AdvanceText); 
      
      _currentHead.CharacterName = "Sir Wolff";
      _currentHead.Dialogue = "...The tri-engine Gauntlet!";
      
      yield return new WaitUntil(AdvanceText); 
      
      _currentHead.Dialogue = "It's been thought to be lost for years... that gauntlet is the most powerful relic of the <color=purple>Locomancers</color>.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "Any Knight who wields it is said to have three times the potential of an average knight.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.CharacterName = "Sir John";
      _currentHead.Dialogue = "What he's trying to say is instead of one <color=orange>Engine</color>, you can use 3 <color=orange>Engines</color>!";
      
      yield return new WaitUntil(AdvanceText); 
      
      _currentHead.Dialogue = "Kid.. if anyone can send these <color=#730901>Followers of Dran</color> back to wherever they came... it's you.";
      
      yield return new WaitUntil(AdvanceText);
      
      _currentHead.Dialogue = "You've got this!";

      _currentHead.RollOut();
      
      DeckManager.Instance.DealHand();

      yield return new WaitUntil(() => DeckManager.Instance.CardsToBeSorted.Count == 3);

      
      TutorialStep++;
      TriggerAfterBattle = false;
   }
   private bool AdvanceText()
   {
      return _currentHead.TextDone && Input.anyKey;
   }
}
