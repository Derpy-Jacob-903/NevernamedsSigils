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
    public class Marketplace : AbilityBehaviour
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Bone Market", "At the end of the an opponent's turn, a [creature] will generate 1 bone for each friendly card.",
                      typeof(FarmBone),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook },
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
        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return base.Card.OpponentCard == playerTurnEnd;
        }

        // Token: 0x06001465 RID: 5221 RVA: 0x00048486 File Offset: 0x00046686
        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
            yield return new WaitForSeconds(0.1f);
            base.Card.Anim.LightNegationEffect();
            yield return base.PreSuccessfulTriggerSequence();

            int balls = 0;
            List<CardSlot> slots = Singleton<BoardManager>.Instance.GetSlots(base.Card.IsPlayerCard()).FindAll(x => x.Card != null);
            List<PlayableCard> targets = new List<PlayableCard>();
            foreach (CardSlot slot in slots) { targets.Add(slot.Card); }

            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
            for (int i = 0; i < targets.Count; i++)
            {
                if (base.Card != null && !base.Card.Dead && targets[i] != null && !targets[i].Dead)
                {
                    PlayableCard indivTarget = targets[i];
                    if (indivTarget != null && !indivTarget.Dead)
                    {
                        balls++;
                    }
                }
            }
            yield return Singleton<ResourcesManager>.Instance.AddBones(balls, base.Card.Slot);
            yield return new WaitForSeconds(0.1f);
            yield return base.LearnAbility(0.1f);
            Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
            yield break;
        }
    }
}