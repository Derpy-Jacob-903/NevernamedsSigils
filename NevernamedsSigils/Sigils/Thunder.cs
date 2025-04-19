using APIPlugin;
using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix;

namespace NevernamedsSigils.Bloons
{
    public class Thunder : AbilityBehaviour
    {
        public static void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Thunder Storm", "When [creature] is played, all opponent creatures are dealt 2 damage.",
                      typeof(Thunder),
                      categories: new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.GrimoraRulebook, AbilityMetaCategory.MagnificusRulebook },
                      powerLevel: 4,
                      stackable: false,
                      opponentUsable: true,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/wriggle.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/wriggle_pixel.png"));

            ability = newSigil.ability;
        }
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        public static Ability ability;

        public override bool RespondsToResolveOnBoard()
        {
            return Singleton<BoardManager>.Instance.GetSlots(base.Card.OpponentCard).Exists(x => x.Card != null);
        }

        public override IEnumerator OnResolveOnBoard()
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
            List<PlayableCard> cards = new List<PlayableCard>();
            cards.AddRange(Singleton<BoardManager>.Instance.CardsOnBoard.FindAll(x => x != base.Card && !x.Dead && x.OpponentCard));
            if (cards.Count > 0)
            {
                yield return base.PreSuccessfulTriggerSequence();
                base.Card.Anim.LightNegationEffect();
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    yield return cards[i].TakeDamage(2, base.Card);
                }
                yield return base.LearnAbility(0.5f);
            }
            Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
            yield break;
            /* List<CardSlot> slots = Singleton<BoardManager>.Instance.GetSlots(base.Card.OpponentCard).FindAll(x => x.Card != null);
             List<PlayableCard> targets = new List<PlayableCard>();
             foreach(CardSlot slot in slots) { targets.Add(slot.Card); }

                 Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                 for (int i = 0; i < targets.Count; i++)
                 {
                     if (base.Card != null && !base.Card.Dead && targets[i] != null && !targets[i].Dead)
                     {
                         //Debug.Log("Target " + i);
                         PlayableCard indivTarget = targets[i];
                         yield return new WaitForSeconds(0.25f);
                         if (indivTarget != null && !indivTarget.Dead)
                         {
                             //Debug.Log("Target not dead");
                             yield return base.PreSuccessfulTriggerSequence();
                             base.Card.Anim.LightNegationEffect();
                             if (base.Card.Anim is DiskCardAnimationController)
                             {
                                 (base.Card.Anim as DiskCardAnimationController).SetWeaponMesh(DiskCardWeapon.Fish);
                                 (base.Card.Anim as DiskCardAnimationController).AimWeaponAnim(indivTarget.Slot.transform.position);
                                 (base.Card.Anim as DiskCardAnimationController).ShowWeaponAnim();
                             }
                             yield return new WaitForSeconds(0.5f);
                             bool impactFrameReached = false;
                             base.Card.Anim.PlayAttackAnimation(base.Card.IsFlyingAttackingReach(), indivTarget.Slot, delegate ()
                             {
                                 impactFrameReached = true;
                             });
                             yield return new WaitUntil(() => impactFrameReached);
                             yield return indivTarget.TakeDamage(3, base.Card);
                         }
                     }
                 }
            */
        }       
    }
}
