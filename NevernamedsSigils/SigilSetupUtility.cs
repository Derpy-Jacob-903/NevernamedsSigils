﻿using DiskCardGame;
using InscryptionAPI.Card;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NevernamedsSigils.Bloons
{
    class SigilSetupUtility
    {
        public static AbilityInfo MakeNewSigil(string name, string description, Type behaviour, List<AbilityMetaCategory> categories = null, int powerLevel = 0, bool stackable = false, bool opponentUsable = false, Texture tex = null, Texture2D pixelTex = null,
            bool isConduit = false, bool isActivated = false, string triggerText = null, bool isConduitCell = false)
        {
            AbilityInfo info = AbilityManager.New("druidSurv.sigils",
                name, description,
               behaviour, tex != null ? tex : Tools.LoadTex("NevernamedsSigils/Resources/Sigils/placeholdersigil.png"));
            info.powerLevel = powerLevel;
            info.canStack = stackable;
            info.opponentUsable = opponentUsable;
            if (pixelTex != null) info.pixelIcon = Tools.ConvertTexToSprite(pixelTex);
            info.metaCategories = categories != null ? categories : new List<AbilityMetaCategory>() { };
            info.conduit = isConduit;
            info.activated = isActivated;
            info.triggerText = triggerText;
            info.conduitCell = isConduitCell;
            return info;
        }

        public static StatIconInfo MakeNewStatIcon(string name, string description, Type behaviour, Texture2D tex = null, Texture2D pixelTex = null, bool isForHealth = false, List<AbilityMetaCategory> categories = null, string gbcDescription = null)
        {
            StatIconInfo info = StatIconManager.New("druidSurv.sigils", name, description, behaviour);

            info.iconGraphic = tex != null ? tex : Tools.LoadTex("NevernamedsSigils/Resources/Sigils/placeholdersigil.png");
            info.pixelIconGraphic = pixelTex != null ? Tools.GenerateAct2Portrait(pixelTex) : null;
            info.gbcDescription = gbcDescription;
            info.appliesToAttack = !isForHealth;
            info.appliesToHealth = isForHealth;

            if (categories != null) info.metaCategories.AddRange(categories);

            return info;
        }
        public static Dictionary<Tribe, string> TribalTailDefault = new Dictionary<Tribe, string>()
        {
            {Tribe.Canine, "Tail_Furry" },
            {Tribe.Bird, "Tail_Bird" },
            {Tribe.Insect, "Tail_Insect" },
            {Tribe.Hooved, "Tail_Furry" },
            {Tribe.Squirrel, "Tail_Furry" }
        };
        public static CardInfo NewCard(string internalName, string displayname, int power, int health, List<CardMetaCategory> categories, CardTemple temple, string description,
            int bloodCost = 0, int bonesCost = 0, int energyCost = 0, List<GemType> gemsCost = null, List<Ability> abilities = null, Texture2D defaultTex = null, Texture2D emissionTex = null, Texture2D pixelTex = null, Texture2D altTexture = null, Texture2D altTextureEmission = null,
           Texture2D titleGraphic = null, List<Texture> decals = null, List<CardAppearanceBehaviour.Appearance> appearanceBehaviour = null, List<Tribe> tribes = null, bool preventSignature = false, List<Trait> traits = null,
            string defaultTailCard = null, string defaultEvolutionCard = null, string defaultEvolutionName = null, int defaultEvolutionTurns = 1, string defaultFrozenCard = null, Texture2D tailLostTexture = null,
            List<SpecialTriggeredAbility> specialAbilities = null, SpecialStatIcon variableStat = SpecialStatIcon.None, bool hideStats = false, bool onePerDeck = false, GameObject animatedPortrait = null, bool flipPortraitWhenStrafing = false, string overrideDamID = null, string overrideDamBlockedDialogue = null, string overrideFactoryConduitSpawn = null,
            string overrideRabbitHoleSpawn = null, string overrideAntSpawnerSpawn = null, string overrideBellistID = null, string overrideBellistBlockedDialogue = null, string overrideSquirrelShedderId = null, string overrideSkeletonCrewID = null, string overrideBeesWithinID = null,
            string twisterForm = null, string transformerForm = null, string overrideLeftClaw = null, string overrideRightClaw = null, string customDoomedLifespan = null, string customGutSpewerGuts = null, string customExplodingCorpseGuts = null, string customOrganThiefGuts = null,
            string harbingerLeaveBehind = null, string tramplerLeaveBehind = null, string flightyLeaveBehind = null, string parthenogenesisOverride = null, string sigilShedderDef = null, bool preventBones = false, bool preventCampfire = false, bool preventSigilVictim = false, bool preventSigilHost = false, bool customGiftBearerSpawnsRares = false,
           string erraticLeaveBehind = null, bool preventPlay = false, string fatalFlankDef = null, bool allStrikesDoubled = false, string phantasmicLeaveBehind = null, bool preventCampfireHealth = false, bool preventCampfireDamage = false)
        {
            CardInfo newInfo = CardManager.New("SigilNevernamed", internalName, displayname, power, health, description);
            newInfo.metaCategories = categories;
            newInfo.temple = temple;
            newInfo.cost = bloodCost;
            newInfo.cardComplexity = CardComplexity.Simple;
            newInfo.bonesCost = bonesCost;
            newInfo.gemsCost = gemsCost != null ? gemsCost : new List<GemType>();
            newInfo.energyCost = energyCost;
            newInfo.abilities = abilities != null ? abilities : new List<Ability>();
            newInfo.tribes = tribes != null ? tribes : new List<Tribe>();
            newInfo.traits = traits != null ? traits : new List<Trait>();

            if (specialAbilities != null && specialAbilities.Count > 0) newInfo.AddSpecialAbilities(specialAbilities.ToArray());

            if (variableStat != SpecialStatIcon.None) newInfo.specialStatIcon = variableStat;


            //Setup Tails, Frozens, and Evolutions         
            if (!string.IsNullOrEmpty(defaultEvolutionName)) newInfo.defaultEvolutionName = defaultEvolutionName;
            if (!string.IsNullOrEmpty(defaultEvolutionCard)) newInfo.SetEvolve(defaultEvolutionCard, defaultEvolutionTurns);
            if (!string.IsNullOrEmpty(defaultFrozenCard)) newInfo.SetIceCube(defaultFrozenCard);
            if (!string.IsNullOrEmpty(defaultTailCard)) newInfo.tailParams = new TailParams() { tail = CardLoader.GetCardByName(defaultTailCard) };
            else if (newInfo.tribes.Count > 0 && TribalTailDefault.ContainsKey(newInfo.tribes[0]))
            {
                newInfo.tailParams = new TailParams() { tail = CardLoader.GetCardByName(TribalTailDefault[newInfo.tribes[0]]) };
            }

            //Special override properties
            if (!string.IsNullOrEmpty(overrideDamID)) newInfo.SetExtendedProperty("CustomDamDefinition", overrideDamID);
            if (!string.IsNullOrEmpty(overrideDamBlockedDialogue)) newInfo.SetExtendedProperty("CustomDamDefinitionDialogue", overrideDamID);
            if (!string.IsNullOrEmpty(overrideRabbitHoleSpawn)) newInfo.SetExtendedProperty("CustomRabbitHoleDefinition", overrideRabbitHoleSpawn);
            if (!string.IsNullOrEmpty(overrideAntSpawnerSpawn)) newInfo.SetExtendedProperty("CustomAntSpawnerDefinition", overrideAntSpawnerSpawn);
            if (!string.IsNullOrEmpty(overrideFactoryConduitSpawn)) newInfo.SetExtendedProperty("CustomFactoryConduitDefinition", overrideFactoryConduitSpawn);
            if (!string.IsNullOrEmpty(overrideBellistID)) newInfo.SetExtendedProperty("CustomBellDefinition", overrideBellistID);
            if (!string.IsNullOrEmpty(overrideBellistBlockedDialogue)) newInfo.SetExtendedProperty("CustomBellDefinitionDialogue", overrideBellistBlockedDialogue);
            if (!string.IsNullOrEmpty(overrideSquirrelShedderId)) newInfo.SetExtendedProperty("CustomSquirrelShedderDefinition", overrideSquirrelShedderId);
            if (!string.IsNullOrEmpty(overrideSkeletonCrewID)) newInfo.SetExtendedProperty("CustomSkeletonCrewDefinition", overrideSkeletonCrewID);
            if (!string.IsNullOrEmpty(overrideBeesWithinID)) newInfo.SetExtendedProperty("CustomBeesWithinDefinition", overrideBeesWithinID);
            if (!string.IsNullOrEmpty(twisterForm)) newInfo.SetExtendedProperty("TwisterTransformation", twisterForm);
            if (!string.IsNullOrEmpty(transformerForm)) newInfo.SetExtendedProperty("CustomTransformerTransformation", transformerForm);
            if (!string.IsNullOrEmpty(overrideLeftClaw)) newInfo.SetExtendedProperty("ClawedLeftClawOverride", overrideLeftClaw);
            if (!string.IsNullOrEmpty(overrideRightClaw)) newInfo.SetExtendedProperty("ClawedRightClawOverride", overrideRightClaw);
            if (!string.IsNullOrEmpty(customGutSpewerGuts)) newInfo.SetExtendedProperty("GutSpewerGutOverride", customGutSpewerGuts);
            if (!string.IsNullOrEmpty(customExplodingCorpseGuts)) newInfo.SetExtendedProperty("ExplodingCorpseGutOverride", customExplodingCorpseGuts);
            if (!string.IsNullOrEmpty(customOrganThiefGuts)) newInfo.SetExtendedProperty("OrganThiefGutOverride", customOrganThiefGuts);
            if (!string.IsNullOrEmpty(parthenogenesisOverride)) newInfo.SetExtendedProperty("ParthenogenesisOverride", parthenogenesisOverride);
            if (!string.IsNullOrEmpty(sigilShedderDef)) newInfo.SetExtendedProperty("OverrideSigilShedderDefinition", sigilShedderDef);
            if (!string.IsNullOrEmpty(fatalFlankDef)) newInfo.SetExtendedProperty("FatalFlankOverride", fatalFlankDef);

            if (!string.IsNullOrEmpty(harbingerLeaveBehind)) newInfo.SetExtendedProperty("HarbingerLeaveBehind", harbingerLeaveBehind);
            if (!string.IsNullOrEmpty(tramplerLeaveBehind)) newInfo.SetExtendedProperty("TramplerLeaveBehind", tramplerLeaveBehind);
            if (!string.IsNullOrEmpty(flightyLeaveBehind)) newInfo.SetExtendedProperty("FlightyLeaveBehind", flightyLeaveBehind);
            if (!string.IsNullOrEmpty(erraticLeaveBehind)) newInfo.SetExtendedProperty("ErraticLeaveBehind", erraticLeaveBehind);
            if (!string.IsNullOrEmpty(phantasmicLeaveBehind)) newInfo.SetExtendedProperty("PhantasmicLeaveBehind", phantasmicLeaveBehind);

            if (!string.IsNullOrEmpty(customDoomedLifespan)) newInfo.SetExtendedProperty("CustomDoomedDuration", customDoomedLifespan);

            if (preventBones == true) { newInfo.SetExtendedProperty("PreventBones", "Yes"); }
            if (preventCampfire == true) { newInfo.SetExtendedProperty("BannedFromCampfire", "Yes"); }
            if (preventCampfireDamage == true) { newInfo.SetExtendedProperty("BannedFromCampfireDamage", "Yes"); }
            if (preventCampfireHealth == true) { newInfo.SetExtendedProperty("BannedFromCampfireHealth", "Yes"); }
            if (preventSigilVictim == true) { newInfo.SetExtendedProperty("BannedSigilTransferVictim", "Yes"); }
            if (preventSigilHost == true) { newInfo.SetExtendedProperty("BannedSigilTransferTarget", "Yes"); }
            if (customGiftBearerSpawnsRares == true) { newInfo.SetExtendedProperty("CustomGiftBearerSpawnsRare", "Yes"); }
            if (preventPlay == true) { newInfo.SetExtendedProperty("PreventPlay", "Yes"); }
            if (allStrikesDoubled == true) { newInfo.SetExtendedProperty("AllStrikesDoubled", "Yes"); }

            //Setup Textures
            if (defaultTex != null) newInfo.SetPortrait(defaultTex);
            if (emissionTex != null) newInfo.SetEmissivePortrait(emissionTex);
            if (pixelTex != null) newInfo.SetPixelPortrait(pixelTex);
            if (altTexture != null) newInfo.SetAltPortrait(altTexture);
            if (altTextureEmission != null) newInfo.SetEmissiveAltPortrait(altTextureEmission);
            if (titleGraphic != null) newInfo.titleGraphic = titleGraphic;

            if (tailLostTexture != null) newInfo.SetLostTailPortrait(tailLostTexture, FilterMode.Point);
            //else if (defaultTex != null && !string.IsNullOrEmpty(defaultTailCard)) newInfo.SetLostTailPortrait(defaultTex, FilterMode.Point);

            newInfo.hideAttackAndHealth = hideStats;
            newInfo.onePerDeck = onePerDeck;
            newInfo.animatedPortrait = animatedPortrait;
            newInfo.flipPortraitForStrafe = flipPortraitWhenStrafing;

            //Handle Decals
            newInfo.decals = decals != null ? decals : new List<Texture>();

            if (appearanceBehaviour != null) newInfo.AddAppearances(appearanceBehaviour.ToArray());

            CardManager.Add("SigilNevernamed", newInfo);

            return newInfo;
        }
    }
}
