using APIPlugin;
using DiskCardGame;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    public class DruidHeal : AbilityBehaviour
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Druidic Healing", "When [creature] is played, will add up to two damage to the scales in the owner's favour, until the scales are not tipped against its owner.",
                      typeof(DruidHeal),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 1,
                      stackable: false,
                      opponentUsable: false,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/desperate.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/desperate_pixel.png"),
                      triggerText: "[creature] heals you!"
                      );

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
        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return true;
        }
        public override IEnumerator OnPlayFromHand()
        {
            yield return base.PreSuccessfulTriggerSequence();
            int damageUntilPlayerLoss = (LifeManager.GOAL_BALANCE * 2) - Singleton<LifeManager>.Instance.DamageUntilPlayerWin;
            //Debug.Log($"until {damageUntilPlayerLoss}");
            if (base.Card.slot.IsPlayerSlot)
            {
                if (damageUntilPlayerLoss < LifeManager.GOAL_BALANCE)
                {
                    for (int i = 0; i < Math.Min(LifeManager.GOAL_BALANCE - damageUntilPlayerLoss, 2); i++)
                    {
                        yield return Singleton<LifeManager>.Instance.ShowDamageSequence(1, 1, true, 0.125f, null, 0f, true);
                    }
                    yield return new WaitForSeconds(0.125f);
                }
            }
            else
            {
                if (damageUntilPlayerLoss > LifeManager.GOAL_BALANCE)
                {
                    for (int i = 0; i < Math.Min(LifeManager.GOAL_BALANCE - Singleton<LifeManager>.Instance.DamageUntilPlayerWin, 2); i++)
                    {
                        yield return Singleton<LifeManager>.Instance.ShowDamageSequence(1, 1, false, 0.125f, null, 0f, true);
                    }
                    yield return new WaitForSeconds(0.125f);
                }
            }
            yield return base.LearnAbility(0.25f);
            yield break;
        }
    }
}