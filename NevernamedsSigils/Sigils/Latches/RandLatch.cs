using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionCommunityPatch;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    public abstract class RandLatch : Latch
    {
        public virtual void Init()
        {
            AbilityInfo newSigil = SigilSetupUtility.MakeNewSigil("Rand Latch", "An abstract AltLatch that targets randomly.",
                      typeof(SlowLatch),
                      categories: new List<AbilityMetaCategory> { },
                      powerLevel: 0,
                      stackable: false,
                      opponentUsable: true,
                      tex: Tools.LoadTex("NevernamedsSigils/Resources/Sigils/Latches/ability_SlowLatch.png"),
                      pixelTex: Tools.LoadTex("NevernamedsSigils/Resources/PixelSigils/Latches/weirdlatch_pixel.png"));

            ability = newSigil.ability;
        }
        public static Ability ability;
        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
        public override Ability LatchAbility
        {
            get
            {
                return Ability.DeleteFile;
            }
        }
        public static bool targetsPlayersSide = true;
        public static bool targetsOppsSide = false;
        public class LambdaBalls
        {
            public DiskCardAnimationController cardAnim = null;
            public GameObject claw = null;
            public CardSlot selectedSlot = null;
        }
        private new IEnumerator AISelectTarget(List<CardSlot> validTargets, Action<CardSlot> chosenCallback, bool targetsFrendlySide, bool targetsOppsSide, bool amPlayer)
        {
            if (validTargets.Count > 0)
            {

            }
            else
            {
                base.Card.Anim.LightNegationEffect();
                yield return new WaitForSeconds(0.2f);
            }
            chosenCallback(validTargets[0]);
            yield return new WaitForSeconds(0.1f);
            else
            {
                base.Card.Anim.LightNegationEffect();
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }
        public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
        {
            List<CardSlot> validTargets = Singleton<BoardManager>.Instance.AllSlotsCopy;
            validTargets.RemoveAll((CardSlot x) => x.Card == null || x.Card.Dead || this.CardHasLatchMod(x.Card) || x.Card == base.Card);
            if (validTargets.Count > 0)
            {
                AltLatch.LambdaBalls loc1 = new AltLatch.LambdaBalls();
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                base.Card.Anim.PlayHitAnimation();
                yield return new WaitForSeconds(0.1f);
                loc1.cardAnim = (base.Card.Anim as DiskCardAnimationController);
                loc1.claw = UnityEngine.Object.Instantiate<GameObject>(this.clawPrefab, loc1.cardAnim.WeaponParent.transform);
                loc1.selectedSlot = null;
                if (base.Card.OpponentCard)
                {
                    yield return new WaitForSeconds(0.3f);
                    yield return this.AISelectTarget(validTargets, delegate (CardSlot s)
                    {
                        loc1.selectedSlot = s;
                    }, targetsOppsSide, targetsOppsSide);
                    if (loc1.selectedSlot != null && loc1.selectedSlot.Card != null)
                    {
                        loc1.cardAnim.AimWeaponAnim(loc1.selectedSlot.transform.position);
                        yield return new WaitForSeconds(0.3f);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.3f);
                    yield return this.AISelectTarget(validTargets, delegate (CardSlot s)
                    {
                        loc1.selectedSlot = s;
                    }, targetsMySide, targetsPlayersSide);
                    if (loc1.selectedSlot != null && loc1.selectedSlot.Card != null)
                    {
                        loc1.cardAnim.AimWeaponAnim(loc1.selectedSlot.transform.position);
                        yield return new WaitForSeconds(0.3f);
                    }
                }
                CustomCoroutine.FlickerSequence(delegate
                {
                    loc1.claw.SetActive(true);
                }, delegate
                {
                    loc1.claw.SetActive(false);
                }, true, false, 0.05f, 2, null);
                if (loc1.selectedSlot != null && loc1.selectedSlot.Card != null)
                {
                    this.ForLatched(loc1.selectedSlot);
                    this.OnSuccessfullyLatched(loc1.selectedSlot.Card);
                    yield return new WaitForSeconds(0.75f);
                    yield return base.LearnAbility(0f);
                }
                yield return new WaitForSeconds(0.75f);
                yield return base.LearnAbility(0f);
                yield break;
            }
        }
        public virtual IEnumerator ForLatched(CardSlot selectedSlot)
        {
            CardModificationInfo cardModificationInfo = new CardModificationInfo(this.LatchAbility);
            cardModificationInfo.fromLatch = true;
            selectedSlot.Card.Anim.ShowLatchAbility();
            selectedSlot.Card.AddTemporaryMod(cardModificationInfo);
            yield break;
        }
        public virtual IEnumerator LearnAbility(Latch ability)
        {
            yield return ability.LearnAbility();
            yield break;
        }
    }

    [HarmonyPatch]
    public class Act1RandLatchAbilityFix
    {
        public static GameObject _clawPrefab;
        private static GameObject ClawPrefab
        {
            get
            {
                if (_clawPrefab == null)
                    _clawPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/LatchClaw");

                return _clawPrefab;
            }
        }

        private class SelectTargetHolder : MonoBehaviour
        {
            public bool isSelectingTarget;
        }

        private static void AimWeaponAnim(GameObject tweenObj, Vector3 target) => Tween.LookAt(tweenObj.transform, target, Vector3.up, 0.075f, 0.0f, Tween.EaseInOut);

        [HarmonyPrefix, HarmonyPatch(typeof(AltLatch), nameof(AltLatch.OnPreDeathAnimation))]
        private static void PrefixPassStateOnPreDeath(out AltLatch __state, ref AltLatch __instance) => __state = __instance;

        [HarmonyPostfix, HarmonyPatch(typeof(AltLatch), nameof(AltLatch.OnPreDeathAnimation))]
        private static IEnumerator Postfix(IEnumerator enumerator, AltLatch __state, bool wasSacrifice)
        {
            // return default logic for Part 3
            if (SaveManager.SaveFile.IsPart3)
            {
                yield return enumerator;
                yield break;
            }

            List<CardSlot> validTargets = BoardManager.Instance.AllSlotsCopy;
            validTargets.RemoveAll(slot => slot.Card == null || slot.Card.Dead || __state.CardHasLatchMod(slot.Card) || slot.Card == __state.Card);

            /*if (PatchPlugin.configFullDebug.Value)
            {
                PatchPlugin.Logger.LogDebug($"[LatchFix] Started death, latch name: [{__state.name}]");
                PatchPlugin.Logger.LogDebug("[LatchFix] Count of Valid Targets : " + validTargets.Count);
            }*/

            // break if no valid targets
            if (validTargets.Count == 0)
                yield break;

            ViewManager.Instance.SwitchToView(View.Board);
            __state.Card.Anim.PlayHitAnimation();

            yield return new WaitForSeconds(0.1f);

            CardAnimationController anim = __state.Card.Anim;

            GameObject latchParentGameObject = new GameObject
            {
                name = "LatchParent",
                transform =
                {
                    position = anim.transform.position
                }
            };
            latchParentGameObject.transform.SetParent(anim.transform);

            Transform latchParent = latchParentGameObject.transform;
            GameObject claw = GameObject.Instantiate(ClawPrefab, latchParent);
            Material cannonMat = null;
            try
            {
                cannonMat = new Material(ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/CannonTargetIcon").GetComponentInChildren<Renderer>().material);
            }
            catch { }
            if (cannonMat != null)
            {
                Renderer[] renderers = claw.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers.Where(rend => rend))
                {
                    rend.material = cannonMat;
                }
            }

            CardSlot selectedSlot = null;

            if (__state.Card.OpponentCard)
            {
                yield return new WaitForSeconds(0.3f);
                yield return __state.AISelectTarget(validTargets, s => selectedSlot = s);

                if (selectedSlot != null && selectedSlot.Card != null)
                {
                    AimWeaponAnim(latchParent.gameObject, selectedSlot.transform.position);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else
            {
                List<CardSlot> allSlotsCopy = BoardManager.Instance.AllSlotsCopy;
                allSlotsCopy.Remove(__state.Card.Slot);

                yield return BoardManager.Instance.ChooseTarget(allSlotsCopy, validTargets,
                    s => selectedSlot = s, // target selected callback
                    __state.OnInvalidTarget, // invalid target callback
                    s => // slot cursor enter callback
                    {
                        if (s.Card == null)
                            return;

                        AimWeaponAnim(latchParent.gameObject, s.transform.position);
                    },
                    null, // cancel condition
                    CursorType.Target);
            }

            claw.SetActive(true);

            CustomCoroutine.FlickerSequence(
                () => claw.SetActive(true),
                () => claw.SetActive(false),
                true,
                false,
                0.05f,
                2
            );

            if (selectedSlot != null && selectedSlot.Card != null)
            {
                __state.ForLatched(selectedSlot);
                //CardModificationInfo mod = new CardModificationInfo(__state.LatchAbility)
                //{
                    // these control rendering, so only set to true if said rendering won't butt everything
                    //fromCardMerge = SaveManager.SaveFile.IsPart1
                //};
                //mod.SetExtendedProperty("LatchMod", true);

                //if (PatchPlugin.configFullDebug.Value)
                    //PatchPlugin.Logger.LogDebug($"[LatchFix] Selected card name [{selectedSlot.Card.name}]");

                //selectedSlot.Card.AddTemporaryMod(mod);
                selectedSlot.Card.Anim.LightNegationEffect();
                __state.OnSuccessfullyLatched(selectedSlot.Card);

                yield return new WaitForSeconds(0.75f);
                yield return __state.LearnAbility(__state);
            }
        }
    }
}