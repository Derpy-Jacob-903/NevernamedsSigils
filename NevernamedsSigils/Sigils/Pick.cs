using APIPlugin;
using BepInEx;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Triggers;
using Pixelplacement;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using static Rewired.Controller;

namespace NevernamedsSigils.Bloons
{
    public class Pick : AbilityBehaviour
    {
            public static void Init()
            {
                baseIcon = Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_1.png");
                basePixelIcon = Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile_pixel.png");
                AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Pick", "Look at 3 random cards in your deck. Choose one and add it to your hand.",
                          typeof(Pick),
                          categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                          powerLevel: 3,
                          stackable: false,
                          opponentUsable: false,
                          tex: baseIcon,
                          pixelTex: basePixelIcon);

                ability = newSigil.ability;
            }
            public static Texture baseIcon;
            public static Texture2D basePixelIcon;
            public static Ability ability;
            List<CardInfo> deck = new List<CardInfo>();
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        public override bool RespondsToResolveOnBoard()
            {
                return Singleton<CardDrawPiles>.Instance.Deck.CardsInDeck > 0;
            }

        public override IEnumerator OnResolveOnBoard()
            {
                yield return PreSuccessfulTriggerSequence();
                //yield return SafeTutor();
                yield return GetCards();
                yield return LearnAbility(0.3f);
                Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
        }
        public IEnumerator GetCards()
        {
            CardInfo selectedCard = null;

            for (int i = 0; i < 3; i++)
            {
                if (Singleton<CardDrawPiles>.Instance.Deck.cards.Count == 0)
                {
                    switch (Tools.GetActAsInt())
                    {
                        case 3:
                            switch (i)
                            {
                                case 1:
                                    CardLoader.GetCardByName("LatcherBomb");
                                    break;
                                case 2:
                                    CardLoader.GetCardByName("SentryBot");
                                    break;
                                default:
                                    CardLoader.GetCardByName("LeapBot");
                                    break;
                            }
                            break;
                        case 2:
                            switch (i)
                            {
                                case 1:
                                    CardLoader.GetCardByName("Skeleton");
                                    break;
                                case 2:
                                    CardLoader.GetCardByName("LeapBot");
                                    break;
                                default:
                                    CardLoader.GetCardByName("Rabbit");
                                    break;
                            }
                            break;
                        default:
                            switch (i)
                            {
                                case 1:
                                    CardLoader.GetCardByName("Skeleton");
                                    break;
                                case 2:
                                    CardLoader.GetCardByName("Dam");
                                    break;
                                default:
                                    CardLoader.GetCardByName("Rabbit");
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    deck.Add(Singleton<CardDrawPiles>.Instance.Deck.cards[SeededRandom.Range(0, Singleton<CardDrawPiles>.Instance.Deck.cards.Count, Singleton<CardDrawPiles>.Instance.Deck.randomSeed++)]); //uses main deck's RNG
                }
            }

            yield return ChooseCard(deck, delegate (CardInfo c)
            {
                selectedCard = c;
            });
            Singleton<ViewManager>.Instance.SwitchToView(View.Default);
            yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(selectedCard); //
            //StartCoroutine((Singleton<CardDrawPiles>.Instance as CardDrawPiles3D).Pile.SpawnCards(Singleton<CardDrawPiles>.Instance.Deck.cards.Count)); //Uncommeting this br8ks the next Tutor or Pick
        }
        public IEnumerator ChooseCard(List<CardInfo> cards, Action<CardInfo> cardSelectedCallback)
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.DeckSelection, immediate: false, lockAfter: true);
            SelectableCard selectedCard = null;
            yield return Singleton<BoardManager>.Instance.CardSelector.SelectCardFrom(cards, null, delegate (SelectableCard x)
            {
                selectedCard = x;
            });
            Tween.Position(selectedCard.transform, selectedCard.transform.position + Vector3.back * 4f, 0.1f, 0f, Tween.EaseIn);
            UnityEngine.Object.Destroy(selectedCard.gameObject, 0.1f);
            cardSelectedCallback(selectedCard.Info);
        }
        //deck is a DiskCardGame.Deck, not
        public IEnumerator ChooseCard(Deck deck, Action<CardInfo> cardSelectedCallback)
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.DeckSelection, immediate: false, lockAfter: true);
            SelectableCard selectedCard = null;
            yield return Singleton<BoardManager>.Instance.CardSelector.SelectCardFrom(deck.cards, (Singleton<CardDrawPiles>.Instance as CardDrawPiles3D).Pile, delegate (SelectableCard x)// -->
            {
                selectedCard = x;
            });
            Tween.Position(selectedCard.transform, selectedCard.transform.position + Vector3.back * 4f, 0.1f, 0f, Tween.EaseIn);
            UnityEngine.Object.Destroy(selectedCard.gameObject, 0.1f);
            cardSelectedCallback(selectedCard.Info);
        }
        // need to get DiskCardGame.SelectableCardArray
        /*public IEnumerator SelectCardFrom(List<CardInfo> cards, CardPile pile, Action<SelectableCard> cardSelectedCallback, Func<bool> cancelCondition = null, bool forPositiveEffect = true)
        {
            InitializeGamepadGrid();
            yield return SpawnAndPlaceCards(cards, pile, GetNumRows(cards.Count), isDeckReview: false, forPositiveEffect);
            yield return new WaitForSeconds(0.15f);
            SetCardsEnabled(enabled: true);
            selectedCard = null;
            yield return new WaitUntil(() => selectedCard != null || (cancelCondition != null && cancelCondition()));
            SetCardsEnabled(enabled: false);
            if (selectedCard != null)
            {
                displayedCards.Remove(selectedCard);
                selectedCard.SetLocalPosition(Vector3.zero, 30f);
            }
            yield return CleanUpCards();
            if (selectedCard != null)
            {
                cards.Remove(selectedCard.Info);
                cardSelectedCallback?.Invoke(selectedCard);
            }
            if (pile != null)
            {
                StartCoroutine(pile.SpawnCards(cards.Count)); //Spawns the deck we chose from? 
                                                              //Also playing a Magpie (a card with the Hoarder / Tutor sigil) fixes this issue
            }
        }
        

        public IEnumerator SafeTutor()
        {
            List<CardInfo> deck = Singleton<CardDrawPiles>.Instance.Deck.cards;

            // Pick up to 3 random unique cards
            List<CardInfo> selection = new List<CardInfo>();
            int seed = Singleton<CardDrawPiles>.Instance.Deck.randomSeed;

            while (selection.Count < 3 && selection.Count < deck.Count)
            {
                CardInfo randomCard = deck[SeededRandom.Range(0, deck.Count, seed++)];
                if (!selection.Contains(randomCard))
                {
                    selection.Add((CardInfo)randomCard.Clone()); // cast to CardInfo
                }
            }

            CardInfo selectedCard = null;

            // Use the DECK'S built-in chooser, not on the list
            yield return ChooseCard(Singleton<CardDrawPiles>.Instance.Deck, c =>
            {
                selectedCard = c;
            }, selection); // <-- This is the overload you want: pick from specific list

            // Remove matching card from deck
            CardInfo toRemove = deck.FirstOrDefault(c => c.name == selectedCard.name);
            if (toRemove != null)
            {
                deck.Remove(toRemove);
            }

            Singleton<ViewManager>.Instance.SwitchToView(View.Default);
            yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(selectedCard);
        }
         */
        protected List<CardInfo> BackupCards => new List<CardInfo>
            {
                CardLoader.GetCardByName("Rabbit"),
                CardLoader.GetCardByName("Rabbit"),
                CardLoader.GetCardByName("Rabbit"),
                CardLoader.GetCardByName("Skeleton"),
                CardLoader.GetCardByName("Skeleton"),
                CardLoader.GetCardByName("Skeleton"),
                CardLoader.GetCardByName("MoxEmerald"),
                CardLoader.GetCardByName("MoxRuby"),
                CardLoader.GetCardByName("MoxSapphire"),
                CardLoader.GetCardByName("Dam"),
                CardLoader.GetCardByName("Dam"),
                CardLoader.GetCardByName("Dam")
            };
        protected List<CardInfo> BackupCards2 => new List<CardInfo>
            {
                CardLoader.GetCardByName("Ant"),        //Ant Spawner
                CardLoader.GetCardByName("Bee"),        //Bees Within
                CardLoader.GetCardByName("Rabbit"),     //Rabbit Hole
                CardLoader.GetCardByName("Rabbit"),     //Rabbit Hole
                CardLoader.GetCardByName("Cockroach"),  //Unkillable
                CardLoader.GetCardByName("FieldMouse"), //Fecundity
                CardLoader.GetCardByName("Skeleton"), //Fecundity
                CardLoader.GetCardByName("Skeleton"), //Fecundity
                CardLoader.GetCardByName("Squirrel"), //Fecundity
                CardLoader.GetCardByName("Squirrel"), //Fecundity
                CardLoader.GetCardByName("Opossum"), //Fecundity
                CardLoader.GetCardByName("Dam")         //Vessel Printer (0,2 card that can't NORMALLY be sacrificed)
            };
        //List<CardInfo> customCards = CardLoader.AllData.FindAll(card => card.metaCategories.Contains(CardMetaCategory.Rare));
    }
}