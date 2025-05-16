using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Triggers;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    public class Bionic : AbilityBehaviour
    {
        private bool startedAttack;
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Bionic Strike", "At the end of the an opponent's turn, a [creature] will generate bones equal to its Power for its owner.",
                      typeof(Bionic),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 4,
                      stackable: false,
                      opponentUsable: false,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/boneduke.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/boneduke_pixel.png"));

            FarmBone.ability = newSigil.ability;
        }
        public static Ability ability;
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        public override bool RespondsToAttackEnded()
        {
            return this;
        }

        public override bool RespondsToDrawn()
        {
            return true;
        }

        public override IEnumerator OnAttackEnded()
        {
            this.Card.Info.SetExtendedProperty("DoubleAllStrikes", null);
            yield break;
        }
        public override IEnumerator OnDrawn()
        {
            this.Card.Info.SetExtendedProperty("DoubleAllStrikes", "yes");
            yield break;
        }
    }
}