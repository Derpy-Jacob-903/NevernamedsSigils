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
    public class JV : AbilityBehaviour
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Jungle's Bounty", "At the end of the owner's turn, [creature] will add one damage to the scales in the owner's favour, regardless of it's attack power or obstruction, unless the scales are tipped in favor its owner.",
                      typeof(JV),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 3,
                      stackable: true,
                      opponentUsable: false,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/toothpuller.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/toothpuller_pixel.png"));

            JV.ability = newSigil.ability;
        }
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return base.Card.slot.IsPlayerSlot == playerTurnEnd;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            yield return new WaitForSeconds(0.15f);
            int damageUntilPlayerLoss = (LifeManager.GOAL_BALANCE * 2) - Singleton<LifeManager>.Instance.DamageUntilPlayerWin;
            if (base.Card.slot.IsPlayerSlot)
            {
                if (damageUntilPlayerLoss < LifeManager.GOAL_BALANCE)
                {
                    for (int i = 0; i < Math.Min(LifeManager.GOAL_BALANCE - damageUntilPlayerLoss, 1); i++)
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
                    for (int i = 0; i < Math.Min(LifeManager.GOAL_BALANCE - Singleton<LifeManager>.Instance.DamageUntilPlayerWin, 1); i++)
                    {
                        yield return Singleton<LifeManager>.Instance.ShowDamageSequence(1, 1, false, 0.125f, null, 0f, true);
                    }
                    yield return new WaitForSeconds(0.125f);
                }
            }
            yield return new WaitForSeconds(0.1f);
            yield return base.LearnAbility(0.1f);
            yield break;
        }
    }
}
