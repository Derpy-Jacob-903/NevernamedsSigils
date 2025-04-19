using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    public class Delayed : AbilityBehaviour
    {
        public static void Init()
        {
            baseIcon = Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_1.png");
            basePixelIcon = Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile_pixel.png");
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Delayed Attack", "[creature] will wait a set number of turns before being allowed to attack.",
                      typeof(Delayed),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: -3,
                      stackable: false,
                      opponentUsable: false,
                      tex: baseIcon,
                      pixelTex: basePixelIcon);

            ability = newSigil.ability;
            countDownIcons = new Dictionary<int, Texture>()
            {
                {0, Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack.png") },
                {1, Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_1.png") },
                {2, Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_2.png") },
                {3, Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_3.png") },
                {4, Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_4.png") },
                {5, Tools.LoadTex("NevernamedsSigils/Resources/Sigils/ability_delayattack_5.png") },
            };
            pixelCountDownIcons = new Dictionary<int, Texture>()
            {
                {0, Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile0_pixel.png") },
                {1, Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile1_pixel.png") },
                {2, Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile2_pixel.png") },
                {3, Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile3_pixel.png") },
                {4, Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile4_pixel.png") },
                {5, Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/docile5_pixel.png") },
            };
        }
        public static Ability ability;
        public static Dictionary<int, Texture> countDownIcons;
        public static Dictionary<int, Texture> pixelCountDownIcons;
        public static Texture baseIcon;
        public static Texture2D basePixelIcon;
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        private void ReRenderCard(int num)
        {
            if (Tools.GetActAsInt() == 2)
            {
                base.Card.RenderInfo.OverrideAbilityIcon(Doomed.ability, pixelCountDownIcons.ContainsKey(num) ? pixelCountDownIcons[num] : basePixelIcon);
            }
            else
            {
                base.Card.RenderInfo.OverrideAbilityIcon(Doomed.ability, countDownIcons.ContainsKey(num) ? countDownIcons[num] : baseIcon);
            }
            base.Card.RenderCard();
        }
        private int LifeSpan
        {
            get
            {
                int customLifespan = 1;
                if (base.Card.Info.GetExtendedProperty("CustomDocileCounter") != null)
                {
                    bool succeed = int.TryParse(base.Card.Info.GetExtendedProperty("CustomDocileCounter"), out customLifespan);
                    customLifespan = succeed ? customLifespan : 1;
                }
                return customLifespan;
            }
        }
        public override bool RespondsToDrawn()
        {
            return true;
        }
        public override IEnumerator OnDrawn()
        {
            ReRenderCard(LifeSpan);
            yield break;
        }
        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }
        public override IEnumerator OnResolveOnBoard()
        {
            ReRenderCard(LifeSpan);
            yield break;
        }
        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return base.Card.slot.IsPlayerSlot == playerTurnEnd;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            livedTurns++;
            int life = LifeSpan;
            int lifeRemaining = Mathf.Max(1, life - livedTurns);
            ReRenderCard(lifeRemaining);
            if (livedTurns >= life)
            {
                //yield return base.PreSuccessfulTriggerSequence();
                //Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                //yield return new WaitForSeconds(0.15f);

                RemoveSigil(this.Card, this.Ability);

                //yield return new WaitForSeconds(0.3f);
                yield return base.LearnAbility(0.1f);
            }
            yield break;
        }
        private int livedTurns;
        public IEnumerator RemoveSigil(PlayableCard targetCard, Ability ability)
        {
            if (targetCard != null && targetCard.HasAbility(ability))
            {
                yield return PreSuccessfulTriggerSequence();
                CardModificationInfo newMod = new CardModificationInfo();
                newMod.negateAbilities = new List<Ability>() { ability };
                targetCard.AddTemporaryMod(newMod);
                targetCard.Status.hiddenAbilities.Add(ability);
                targetCard.RenderCard();
                targetCard.Anim.StrongNegationEffect();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
