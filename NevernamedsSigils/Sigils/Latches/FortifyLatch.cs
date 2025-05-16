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
    public class FortLatch : AltLatch
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Fortify Latch", "When [creature] perishes, its owner chooses a creature to gain 1 Power, 1 Health. If the target creature has a Variable Power Stat, that creature gains +0/+2 instead of +1/+1.",
                      typeof(FortLatch),
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