using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Ascension;
using InscryptionAPI.Card;
using System;
using System.Collections.Generic;
using System.Text;

namespace NevernamedsSigils.Bloons
{
    public partial class Challenges
    {
        private static AscensionChallengeInfo waterborneStarterChallenge;
        private static AscensionChallengeInfo shockedStarterChallenge;
        private static AscensionChallengeInfo travelingOuroChallenge;
        private void AddWaterborneStarterChallenge()
        {
            waterborneStarterChallenge = ChallengeManager.AddSpecific(
                    Plugin2.PluginGuid,
                    "Energy To Bone",
                    "After drawing and ending your turn, lose all unspent Energy and gain that many Bones.",
                    -10,
                    Tools.LoadTexture("ascensionicon_EnergyToBone"),
                    Tools.LoadTexture("ascensionicon_EnergyToBone"),
                    2
                    );
            shockedStarterChallenge = ChallengeManager.AddSpecific(
                    Plugin2.PluginGuid,
                    "Bone Limit",
                    "You can have a maximum of 20 Bones.",
                    10,
                    Tools.LoadTexture("ascensionicon_boneLimit"),
                    Tools.LoadTexture("ascensionicon_boneLimit"),
                    2
                    );
        }


    }
}
