using DiskCardGame;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BepInEx;
using System.Collections;
using InscryptionAPI.Card;
using GBC;
using InscryptionAPI.Helpers.Extensions;

namespace NevernamedsSigils.Bloons
{
    [HarmonyPatch(typeof(PlayableCard), "CanAttackDirectly", MethodType.Normal)]
    public class CanAttackDirectlyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref CardSlot opposingSlot, ref bool __result, PlayableCard __instance)
        {
            /*if (opposingSlot.Card)
            {
                if (opposingSlot.Card.HasAbility(Immaterial.ability)) { __result = true; }
            }*/
        }
    }
    [HarmonyPatch(typeof(PlayableCard), "AttackIsBlocked", MethodType.Normal)]
    public class AttackIsBlockedPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref CardSlot opposingSlot, ref bool __result, PlayableCard __instance)
        {
            //if (opposingSlot.Card && opposingSlot.Card.Info.GetExtendedProperty("InherentRepulsive") != null) __result = true;
        }
    }
    [HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", 0)]
    public class SlotAttackSlotPrefix
    {
        [HarmonyPrefix]
        public static bool SlotAttackSlot(ref CardSlot attackingSlot, ref CardSlot opposingSlot, float waitAfter = 0f)
        {
            if (attackingSlot.Card != null)
            {

                //Monkeykind //Delayed Attack
                if ((attackingSlot.Card.HasAbility(Abstain2.ability) && opposingSlot.Card != null) || attackingSlot.Card.HasAbility(Delayed.ability) && (attackingSlot.Card.GetComponent<Delayed>().livedTurns < attackingSlot.Card.GetComponent<Delayed>().LifeSpan) || attackingSlot.Card.HasAbility(Freeze.ability))
                {
                    attackingSlot.Card.Anim.StrongNegationEffect();
                    if (attackingSlot.Card.HasAbility(Freeze.ability))
                    {
                        Delayed d = new Delayed();
                        d.RemoveSigil(attackingSlot.Card, Freeze.ability);
                    }
                    return false;
                }
                if ((attackingSlot.Card.HasAbility(ArmorPiercing.ability) && opposingSlot.Card != null))
                {
                    Delayed balls = new Delayed();
                    balls.RemoveSigil(opposingSlot.Card, Ability.DeathShield);
                    balls.RemoveSigil(opposingSlot.Card, Sturdy.ability);
                }


                PlayableCard card = attackingSlot.Card;

                List<CardSlot> opponentSlotsWithCards = (attackingSlot.Card.IsPlayerCard() ? Singleton<BoardManager>.Instance.opponentSlots : Singleton<BoardManager>.Instance.playerSlots).FindAll(x => x.Card != null);
                if (!attackingSlot.Card.HasAbility(Ability.AllStrike))
                {
                    if (attackingSlot.Card.HasAbility(TrophyHunter.ability) || Singleton<BoardManager>.Instance.AllSlots.Find(x => (x.IsPlayerSlot == card.IsPlayerCard()) && x.Card && x.Card.HasAbility(Telepathic.ability)))
                    {
                    }
                    else if (attackingSlot.Card.HasAbility(Bully.ability))
                    {
                    }
                    else if (attackingSlot.Card.HasAbility(HomeRun.ability) && attackingSlot.Card.GetComponent<HomeRun>())
                    {
                    }
                    else if (attackingSlot.Card.HasAbility(ArrowStrike.ability))
                    {
                        List<CardSlot> viableslots = new List<CardSlot>();
                        if (opponentSlotsWithCards.Count > 0)
                        {
                            viableslots = opponentSlotsWithCards;
                            CardSlot cardSlot = Tools.SeededRandomElement(viableslots);
                            opposingSlot = cardSlot;
                        }
                        else
                        {
                            attackingSlot.Card.Anim.StrongNegationEffect();
                            return false;
                        }
                    }
                }
            }
            return true;
        }


    }

    [HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
    public class SlotAttackSlotPostfix
    {
        [HarmonyPostfix]
        public static IEnumerator Postfix(IEnumerator enumerator, CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
        {
            yield return enumerator; 
            yield break;
        }
    }
    [HarmonyPatch(typeof(PlayableCard), "TakeDamage", MethodType.Normal)]
    public class TakeDamagePatch
    {
        [HarmonyPrefix]
        public static void Prefix(out int __state, PlayableCard __instance, ref int damage, ref PlayableCard attacker)
        {
            __state = damage;
            if (__instance)
            {
                {
                    float damToFloat = damage;
                    int final = Mathf.CeilToInt(damToFloat * 0.5f);
                    __state = final;
                    damage = final;
                }
            }
        }
        [HarmonyPostfix]
        public static IEnumerator Postfix(IEnumerator enumerator, int __state, PlayableCard __instance, int damage, PlayableCard attacker)
        {
            yield return enumerator;
            if (__state <= 0 && __instance.NotDead())
            {
                __instance.Anim.PlayHitAnimation();
                if (__instance.TriggerHandler.RespondsToTrigger(Trigger.TakeDamage, new object[] { attacker }))
                {
                    yield return __instance.TriggerHandler.OnTrigger(Trigger.TakeDamage, new object[] { attacker });
                }

                if (attacker != null)
                {
                    if (attacker.TriggerHandler.RespondsToTrigger(Trigger.DealDamage, new object[] { damage, __instance }))
                    {
                        yield return attacker.TriggerHandler.OnTrigger(Trigger.DealDamage, new object[] { damage, __instance });
                    }
                    yield return Singleton<GlobalTriggerHandler>.Instance.TriggerCardsOnBoard(Trigger.OtherCardDealtDamage, false, new object[] { attacker, attacker.Attack, __instance });
                }
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(CombatPhaseManager), "DealOverkillDamage", MethodType.Normal)]
    public class OverkillDamagePatch
    {
        [HarmonyPostfix]
        public static IEnumerator PostFix(IEnumerator enumerator, int damage, CardSlot attackingSlot, CardSlot opposingSlot)
        {
            bool skipOverkill = false;
            if (attackingSlot && attackingSlot.Card)
            {
            }
            if (!skipOverkill) { yield return enumerator; }
            yield break;
        }

        /*public static IEnumerator DealHorizontalOverkill(CardSlot damagedSlot, CardSlot attackingSlot, bool left, int amountOfOverkill)
        {
            CardSlot horizSlot = Singleton<BoardManager>.Instance.GetAdjacent(damagedSlot, left);
            if (horizSlot && horizSlot.Card && !horizSlot.Card.Dead && (!horizSlot.Card.FaceDown))
            {
                bool wasFaceDown = false;
                if (horizSlot.Card.FaceDown)
                {
                    wasFaceDown = true;
                    horizSlot.Card.SetFaceDown(false, false);
                    horizSlot.Card.UpdateFaceUpOnBoardEffects();
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.1f);
                int leftovers = amountOfOverkill - horizSlot.Card.Health;
                yield return horizSlot.Card.TakeDamage(amountOfOverkill, attackingSlot.Card);

                if (wasFaceDown && horizSlot.Card != null && !horizSlot.Card.Dead)
                {
                    yield return new WaitForSeconds(0.1f);
                    horizSlot.Card.SetFaceDown(true, false);
                    horizSlot.Card.UpdateFaceUpOnBoardEffects();
                }

                if (leftovers > 0)
                {
                    yield return DealHorizontalOverkill(horizSlot, attackingSlot, left, leftovers);
                }           
            }
            yield break;
        }*/
    }
}