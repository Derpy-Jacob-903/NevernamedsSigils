using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GraveyardHandler;

namespace NevernamedsSigils.Bloons
{
    public class Reinflate : AbilityBehaviour
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Reinflate", "When [creature] is played, it's owner adds a random card from their graveyard to their hand.",
                      typeof(Reinflate),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 5,
                      stackable: false,
                      opponentUsable: false,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/exhume.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/exhume_pixel.png"));

            ability = newSigil.ability;
        }
        public static Ability ability;
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        public override bool RespondsToResolveOnBoard()
        {
            return true && GraveyardManager.instance != null;
        }
        public override IEnumerator OnResolveOnBoard()
        {
            if (base.Card.OpponentCard)
            {
                if (Singleton<BoardManager>.Instance.OpponentSlotsCopy.Exists(x => Singleton<BoardManager>.Instance.GetCardQueuedForSlot(x) == null) && GraveyardManager.instance.opponentGraveyard.Count > 0)
                {
                    CardInfo toSpawn = Tools.SeededRandomElement<CardInfo>(GraveyardManager.instance.opponentGraveyard, GetRandomSeed());
                    GraveyardManager.instance.opponentGraveyard.Remove(toSpawn);
                    PlayableCard playableCard = CardSpawner.SpawnPlayableCard(toSpawn);
                    playableCard.SetIsOpponentCard(true);
                    Singleton<TurnManager>.Instance.Opponent.ModifyQueuedCard(playableCard);

                    Singleton<BoardManager>.Instance.QueueCardForSlot(playableCard,
                        Tools.SeededRandomElement(Singleton<BoardManager>.Instance.OpponentSlotsCopy.FindAll(x => Singleton<BoardManager>.Instance.GetCardQueuedForSlot(x) == null)));
                    Singleton<TurnManager>.Instance.Opponent.Queue.Add(playableCard);
                }
            }
            else
            {
                if (GraveyardManager.instance.playerGraveyard.Count > 0)
                {
                    CardInfo selectedCard = Tools.SeededRandomElement<CardInfo>(GraveyardManager.instance.playerGraveyard, GetRandomSeed());
                    GraveyardManager.instance.playerGraveyard.Remove(selectedCard);
                    PlayableCard playableCard = CardSpawner.SpawnPlayableCard(selectedCard);
                    yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(selectedCard, 0.25f);
                }
            }
        }
    }
}