using APIPlugin;
using BepInEx;
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
    public class Freeze : AbilityBehaviour
    {
        public static void Init()
        {

            baseIcon = Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_1.png");
            basePixelIcon = Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile_pixel.png");
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Freezw", "[creature] will wait an extra turn before attacking.",
                      typeof(Delayed),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: -3,
                      stackable: false,
                      opponentUsable: false,
                      tex: baseIcon,
                      pixelTex: basePixelIcon);

            ability = newSigil.ability;
        }
        public static Texture baseIcon;
        public static Texture2D basePixelIcon;
        public static Ability ability;
        private bool initialised;

        public int turnsUntilNextAttack;
        public int cooldownTotal;
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        private IEnumerator Initialise()
        {
            yield break;
        }
        public override bool RespondsToDrawn() { return true; }
        public override IEnumerator OnDrawn()
        {
            if (!initialised) { yield return Initialise(); }
            yield break;
        }
        public override bool RespondsToResolveOnBoard() { return true; }
        public override IEnumerator OnResolveOnBoard()
        {
            if (!initialised) { yield return Initialise(); }
            yield break;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep;
        }
        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if (this.Card.HasAbility(Delayed.ability))
            {
                Delayed d = this.Card.GetComponent<Delayed>();
                d.livedTurns--; d.livedTurns--;
                d.OnUpkeep(playerUpkeep);
                this.Card.Info.ModAbilities.Remove(Ability);
            }
            else
            {
                if (this.Card.HasAbility(NevernamedsSigils.Docile.ability))
                {
                    this.Card.GetComponent<Docile>().turnsUntilNextAttack++;
                    Delayed d = new Delayed();
                    this.Card.Info.ModAbilities.Remove(Ability);
                }
            }
            yield break;
        }
    }
}