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
    public class HasteLatch : AltLatch
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Hastening Latch", "When [creature] perishes, its owner chooses a creature to attack once. If the target creature has the Delayed Attack or Docile sigil, that sigil's counter is decreased by 1 instead.",
                      typeof(HasteLatch),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 0,
                      stackable: false,
                      opponentUsable: true,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/Latches/ability_SlowLatch.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/Latches/weirdlatch_pixel.png"));

            ability = newSigil.ability;
        }
        public static new Ability ability;

        public override IEnumerator ForLatched(CardSlot selectedSlot)
        {
            if (selectedSlot.Card.HasAbility(Delayed.ability))
            {
                selectedSlot.Card.GetComponent<Delayed>().livedTurns++;
            }
            else
            {
                if (selectedSlot.Card.HasAbility(NevernamedsSigils.Docile.ability) && selectedSlot.Card.GetComponent<Docile>().turnsUntilNextAttack > 0)
                {
                    selectedSlot.Card.GetComponent<Docile>().turnsUntilNextAttack--;
                }
                else
                {
                    if (selectedSlot.Card.HasAbility(Charged.ability))
                    {
                        selectedSlot.Card.GetComponent<Charged>().livedTurns--;
                    }
                    else
                    {
                        selectedSlot.Card.ForceCardAttackOutsideOfCombat();
                    }
                    //CardModificationInfo cardModificationInfo = new CardModificationInfo(Delayed.ability);
                    //Card.AddTemporaryMod(cardModificationInfo);
                }
            }
            yield break;
        }

        public override Ability LatchAbility
        {
            get
            {
                return Delayed.ability;
            }
        }

    }
}