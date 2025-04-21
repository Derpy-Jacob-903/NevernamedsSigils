using BepInEx;
using BepInEx.Configuration;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Guid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static InscryptionAPI.Card.AbilityManager;

namespace NevernamedsSigils.Bloons
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("nevernamed.inscryption.opponentbones", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("nevernamed.inscryption.graveyardhandler", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("nevernamed.inscryption.sigils", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin2 : BaseUnityPlugin
    {
        public const string PluginGuid = "druidSurv";
        private const string PluginName = "NevernamedsSigils-Bloon";
        private const string PluginVersion = "1.10.0.0";

        public static AssetBundle bundle;
        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");

            //bundle = LoadBundle("NevernamedsSigils/Resources/sigilassetbundle");

            Harmony harmony = new Harmony("NevernamedsSigils.Bloons.harmonypatcher");
            harmony.PatchAll();

            NoBonesDecal = Tools.LoadTex("NevernamedsSigils/Resources/Other/preventbonesdecal.png");

            //Abstain2.Init();
            ArmorPiercing.Init(); Logger.LogInfo("ArmorPiercing INIT WAS FUCKING LOADED"); //logs using Plugin2.Logger
            Thunder.Init(); Logger.LogInfo("Thunder INIT WAS FUCKING LOADED");
            FarmBone.Init(); Logger.LogInfo("FarmBone INIT WAS FUCKING LOADED");
            Marketplace.Init(); Logger.LogInfo("Marketplace INIT WAS FUCKING LOADED");
            Delayed.Init(); Logger.LogInfo("Delayed INIT WAS FUCKING LOADED");
            Volatile.Init(); Logger.LogInfo("Volatile INIT WAS FUCKING LOADED");

            SlowLatch.Init(); Logger.LogInfo("SlowLatch WAS FUCKING LOADED");

            EternalBloonTrait.Init(); Logger.LogInfo("EternalBloonTrait INIT WAS FUCKING LOADED");

            CardManager.ModifyCardList += delegate (List<CardInfo> cards)
            {
                return cards;
            };
            AbilityManager.ModifyAbilityList += delegate (List<FullAbility> abilities)
            {
                return abilities;
            };
        }
        public static List<string> toBeMadeRodent = new List<string>()
        {
            "Amalgam",
            "Hydra",
            "PackRat",
            "Porcupine",
            "Beaver",
            "FieldMouse",
            "RatKing"
        };
        public static List<string> toBeMadeArachnid = new List<string>()
        {
            "Amalgam",
            "Hydra"
        };
        public static List<string> toBeMadeCrustacean = new List<string>()
        {
            "Amalgam",
            "Hydra"
        };

        public void log(string message)
        {
            this.Logger.LogInfo(message);
        }
        public static AssetBundle LoadBundle(string path)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(path.Replace("\\", ".").Replace("/", ".")))
            {
                return AssetBundle.LoadFromStream(s);
            }
        }

        public static List<CardInfo> mycoRequiredCardAdded = new List<CardInfo>();
        public static List<CardInfo> mycoFusedCardAdded = new List<CardInfo>();

        internal static ConfigEntry<bool> arachnophobiaMode;

        public static Texture2D NoBonesDecal;
        public static readonly AbilityMetaCategory GrimoraModChair1 = (AbilityMetaCategory)GuidManager.GetEnumValue<AbilityMetaCategory>("arackulele.inscryption.grimoramod", "ElectricChairLevel1");
        public static readonly AbilityMetaCategory GrimoraModChair2 = (AbilityMetaCategory)GuidManager.GetEnumValue<AbilityMetaCategory>("arackulele.inscryption.grimoramod", "ElectricChairLevel2");
        public static readonly AbilityMetaCategory GrimoraModChair3 = (AbilityMetaCategory)GuidManager.GetEnumValue<AbilityMetaCategory>("arackulele.inscryption.grimoramod", "ElectricChairLevel3");
        public static readonly AbilityMetaCategory Part2Modular = (AbilityMetaCategory)GuidManager.GetEnumValue<AbilityMetaCategory>("cyantist.inscryption.api", "Part2Modular");

        public static readonly CardMetaCategory GrimoraChoiceNode = GuidManager.GetEnumValue<CardMetaCategory>("arackulele.inscryption.grimoramod", "GrimoraModChoiceNode");

        public static readonly CardMetaCategory P03KayceesWizardRegion = (CardMetaCategory)GuidManager.GetEnumValue<CardMetaCategory>("zorro.inscryption.infiniscryption.p03kayceerun", "WizardRegionCards");
        public static readonly CardMetaCategory P03KayceesBastionRegion = (CardMetaCategory)GuidManager.GetEnumValue<CardMetaCategory>("zorro.inscryption.infiniscryption.p03kayceerun", "TechRegionCards");
        public static readonly CardMetaCategory P03KayceesNatureRegion = (CardMetaCategory)GuidManager.GetEnumValue<CardMetaCategory>("zorro.inscryption.infiniscryption.p03kayceerun", "NatureRegionCards");
        public static readonly CardMetaCategory P03KayceesUndeadRegion = (CardMetaCategory)GuidManager.GetEnumValue<CardMetaCategory>("zorro.inscryption.infiniscryption.p03kayceerun", "UndeadRegionCards");
        public static readonly CardMetaCategory P03KayceesNeutralRegion = GuidManager.GetEnumValue<CardMetaCategory>("zorro.inscryption.infiniscryption.p03kayceerun", "NeutralRegionCards");

        public static readonly Ability GrimoraBurning = (Ability)GuidManager.GetEnumValue<Ability>("arackulele.inscryption.grimoramod", "Burning");
    }
}
