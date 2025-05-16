using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Triggers;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.XR;
using static EasyFeedback.Core.Constants.Web;
using static Rewired.Controller;
using static UnityEngine.ParticleSystem;

namespace NevernamedsSigils.Bloons
{
    public class BloonSwarm : AbilityBehaviour
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Swarm", "When [creature] is played, a copy of [creature] is created to it's left and right. These copies will not have sigils that create copies of itself.",
                      typeof(BloonSwarm),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 4,
                      stackable: false,
                      opponentUsable: false,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/boneduke.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/boneduke_pixel.png"));

            BloonSwarm.ability = newSigil.ability;
        }
        public static Ability ability;
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        private IEnumerator SpawnCardOnSlot(CardSlot slot)
        {

            CardInfo inf = CardLoader.GetCardByName(base.Card.Info.name);
            /*foreach (CardModificationInfo cardModificationInfo in base.Card.Info.Mods.FindAll((CardModificationInfo x) => !x.nonCopyable))
            {
                //So many sigils!!!
                CardModificationInfo item = (CardModificationInfo)cardModificationInfo.Clone();
                item.abilities.Remove(SwarmBloon.ability);
                item.abilities.Remove(Trio.ability);
                item.abilities.Remove(Copier.ability);
                item.abilities.Remove(Parthenogenesis.ability);
                item.abilities.Remove(TwinBond.ability);
                item.abilities.Remove(Dupeglitch.ability);
                item.abilities.Remove(Ability.DrawAnt);
                item.abilities.Remove(Ability.DrawCopy);
                item.abilities.Remove(Ability.DrawCopyOnDeath);
                item.abilities.Remove(Ability.DrawRabbits);
                item.abilities.Remove(Ability.DrawRandomCardOnDeath);
                //item.abilities.Remove(Ability.DrawVesselOnHit); // Draws from side deck
                item.abilities.Remove(Termatriarch.ability);
                item.abilities.Remove(GutSpewer.ability);
                item.abilities.Remove(Mason.ability);
                item.abilities.Remove(DeckedOut.ability);
                item.abilities.Remove(Moxcraft.ability);
                item.abilities.Remove(GiftBearerCustom.ability);
                item.abilities.Remove(KinBearer.ability);
                item.abilities.Remove(HartsWithin.ability);
                item.abilities.Remove(Summoner.ability);
                item.abilities.Remove(RemoteControlled.ability);
                item.abilities.Remove(Emancipation.ability);
                item.abilities.Remove(Disembowel.ability);
                item.abilities.Remove(NatureOfTheBeast.ability);
                item.abilities.Remove(Legion.ability);
                item.abilities.Remove(Goated.ability);
                item.abilities.Remove(GiftWhenPoweredCustom.ability);
                item.abilities.Remove(PrintWhenPowered.ability);
                item.abilities.Remove(Broodfeast.ability);
                inf.Mods.Add(item);
            }*/
            foreach (CardModificationInfo cardModificationInfo in base.Card.Info.Mods.FindAll((CardModificationInfo x) => !x.nonCopyable))
            {
                //So many sigils!!!
                CardModificationInfo item = (CardModificationInfo)cardModificationInfo.Clone();
                item.abilities.Remove(BloonSwarm.ability);
                item.abilities.Remove(Trio.ability);
                item.abilities.Remove(Ability.DrawCopy);
                item.abilities.Remove(Ability.DrawCopyOnDeath);
                item.abilities.Remove(TwinBond.ability);
                item.abilities.Remove(Broodfeast.ability);
                inf.Mods.Add(item);
            }
            foreach (CardModificationInfo cardModificationInfo in base.Card.temporaryMods.FindAll((CardModificationInfo x) => !x.nonCopyable))
            {
                CardModificationInfo item = (CardModificationInfo)cardModificationInfo.Clone();
                item.abilities.Remove(BloonSwarm.ability);
                item.abilities.Remove(Trio.ability);
                item.abilities.Remove(Ability.DrawCopy);
                item.abilities.Remove(Ability.DrawCopyOnDeath);
                item.abilities.Remove(TwinBond.ability);
                item.abilities.Remove(Broodfeast.ability);
                inf.Mods.Add(item);
            }
            // Swarm: When this card is played, a COPY of it is created to it's left and right. These copies will not have sigils that create copies of itself.
            // Trio: When this card is played, a COPY of it is created to it's left and right.
            // Fecundity: When this card is played, a COPY of it enters your hand.
            // Unkillable: When this card perishes, a COPY of it enters your hand.
            // Twin Bond: When the sigil bearer is played, an identical COPY is created to the left or right, even killing adjacent cards to make space if necessary. The original and it's copy share the same Health, and if one dies, the other will as well.
            // Broodfest: When activated, a COPY of this card is created in the owner's hand.
            inf.mods.Add(new CardModificationInfo() { negateAbilities = new List<Ability>() { BloonSwarm.ability, Trio.ability, Ability.DrawCopy, Ability.DrawCopyOnDeath, TwinBond.ability, Broodfeast.ability} });
            yield return Singleton<BoardManager>.Instance.CreateCardInSlot(inf, slot, 0.15f, true);
            yield break;
        }
        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }
        public override IEnumerator OnResolveOnBoard()
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
            CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
            CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
            yield return new WaitForSeconds(0.1f);
            bool toLeftValid = toLeft != null && toLeft.Card == null;
            bool toRightValid = toRight != null && toRight.Card == null;

            if (toLeftValid || toRightValid)
            {
                yield return base.PreSuccessfulTriggerSequence();
            }
            if (toLeftValid)
            {
                yield return new WaitForSeconds(0.1f);
                yield return this.SpawnCardOnSlot(toLeft);
            }

            if (toRightValid)
            {
                yield return new WaitForSeconds(0.1f);
                yield return this.SpawnCardOnSlot(toRight);
            }

            yield break;
        }
    }
}