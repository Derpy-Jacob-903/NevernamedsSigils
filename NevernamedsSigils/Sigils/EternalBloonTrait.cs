using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    public class EternalBloonTrait : SpecialCardBehaviour
    {
        public static SpecialTriggeredAbility ability;
        public static void Init()
        {
            ability = SpecialTriggeredAbilityManager.Add("druidSurv.sigils", "EternalBloonTrait", typeof(EternalBloonTrait)).Id;
        }
    }

[HarmonyPatch(typeof(DrawCopyOnDeath), nameof(DrawCopyOnDeath.OnDie))]
    public class EternalBloonTraitPatch
    {
        private static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("NevernamedsSigils-Bloon.EternalBloonTraitPatch");

        [HarmonyPrefix]
        public static bool Prefix(DrawCopyOnDeath __instance, bool wasSacrifice, PlayableCard killer)
        {
            logger.LogInfo("Prefix called for DrawCopyOnDeath.OnDie.");
            var cardToDraw = __instance.CardToDraw;

            if (cardToDraw != null && cardToDraw.HasSpecialAbility(EternalBloonTrait.ability))
            {
                logger.LogInfo($"Card {cardToDraw.name} has EternalBloonTrait.");
                SetKillCount(__instance.Card.Info);
                SetKillCount(cardToDraw);
                IEnumerator(__instance, wasSacrifice, killer);
                return true;
            }
            logger.LogInfo("Card does not have EternalBloonTrait.");
            return false;
        }

        public static IEnumerator IEnumerator(DrawCopyOnDeath __instance, bool wasSacrifice, PlayableCard killer)
        {
            var cardToDraw = __instance.CardToDraw;
            if (cardToDraw != null && cardToDraw.HasSpecialAbility(EternalBloonTrait.ability))
            {
                logger.LogInfo($"Starting IEnumerator for Card {cardToDraw.name}.");
                CardModificationInfo m = new CardModificationInfo(0, 2);

                __instance.Card.temporaryMods.Add(m);
                __instance.CardToDrawTempMods.Add(m);

                yield return __instance.PreSuccessfulTriggerSequence();
                yield return __instance.CreateDrawnCard();
                yield return __instance.LearnAbility(0.5f);
                yield break;
            }
            logger.LogInfo("Card to draw is null or missing required ability.");
        }

        public static IEnumerator CreateDrawnCard(CardInfo cardToDraw, List<CardModificationInfo> CardToDrawTempMods)
        {
            logger.LogInfo($"Creating card: {cardToDraw.name}.");
            if (Singleton<ViewManager>.Instance.CurrentView != View.Default)
            {
                logger.LogDebug("Switching to Default view.");
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                yield return new WaitForSeconds(0.2f);
            }
            yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(cardToDraw, CardToDrawTempMods, 0.25f, null);
            yield return new WaitForSeconds(0.45f);
            yield break;
        }

        public static void SetKillCount(CardInfo cardToDraw)
        {
            int? count = cardToDraw.GetExtendedPropertyAsInt("deathsWithEternalBloonTraitAndUnkillableSigil");
            count = count ?? 0;
            logger.LogInfo($"Updating kill count for {cardToDraw.name}. New count: {count} => {count + 1}");
            cardToDraw.SetExtendedProperty("deathsWithEternalBloonTraitAndUnkillableSigil", count + 1);
        }
    }
}
