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
    public class BoostLatch : AltLatch
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Booster Latch", "When [creature] perishes, its owner chooses a creature to gain 1 Power, 2 Health, and the Delayed Attack sigil. If the target creature has a Variable Power Stat, that creature gains +0/+3 instead of +1/+2. If the target creature has the Delayed Attack or Docile sigil, that sigil's counter is increased by 1 instead of gaining the Delayed Attack sigil.",
                      typeof(BoostLatch),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 0,
                      stackable: false,
                      opponentUsable: true,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/Latches/ability_SlowLatch.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/Latches/weirdlatch_pixel.png"));

            ability = newSigil.ability;
        }
        public static Ability ability;

        public override IEnumerator ForLatched(CardSlot selectedSlot)
        {
            if (selectedSlot.Card.HasAbility(Delayed.ability))
            {
                selectedSlot.Card.GetComponent<Delayed>().livedTurns--;
            }
            else
            {
                if (selectedSlot.Card.HasAbility(NevernamedsSigils.Docile.ability))
                {
                    selectedSlot.Card.GetComponent<Docile>().turnsUntilNextAttack++;
                }
                else
                {
                    if (selectedSlot.Card.HasAbility(Charged.ability))
                    {
                        selectedSlot.Card.GetComponent<Charged>().livedTurns--;
                    }
                    else
                    {
                        CardModificationInfo cardModificationInfo = new CardModificationInfo(Delayed.ability);
                        selectedSlot.Card.AddTemporaryMod(cardModificationInfo);
                    }
                }
            }
            foreach (CardModificationInfo cardModificationInfo in this.Card.TemporaryMods)
            {
                if (cardModificationInfo.singletonId == "VARIABLE_STAT")
                {
                    this.Card.AddTemporaryMod(new CardModificationInfo(0, 3));
                    yield break;
                }
            }
            this.Card.AddTemporaryMod(new CardModificationInfo(1, 2));
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