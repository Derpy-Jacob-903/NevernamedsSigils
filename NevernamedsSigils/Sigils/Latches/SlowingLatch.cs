﻿using APIPlugin;
using DiskCardGame;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    public class SlowLatch : AltLatch
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Slowing Latch", "When [creature] perishes, its owner chooses a creature to gain the Delayed Attack sigil. If the target creature has the Delayed Attack or Docile sigil, that sigil's counter is increased by 1 instead.",
                      typeof(SlowLatch),
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
            //if (selectedSlot.Card.has)

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