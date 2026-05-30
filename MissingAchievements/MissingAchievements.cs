using Harmony;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace MissingAchievements
{
    [ModLoader.Mod]
    public class MissingAchievements : MonoBehaviour
    {
        static readonly string MOD_ID = "Missing Achievements";
        static Dictionary<çëèèñêëðïìó, string> achievementIds = new Dictionary<çëèèñêëðïìó, string>
        {
            {  çëèèñêëðïìó.ëææäíòçîîðå, "WPN_MATCHLOCK" },
            {  çëèèñêëðïìó.éåïñìäïîèçæ, "WPN_HANDMORTAR" },
            {  çëèèñêëðïìó.ïèîìçïêîéïì, "WPN_AXE" },
            {  çëèèñêëðïìó.íñäðîîåæéïó, "WPN_PIKE" },
            {  çëèèñêëðïìó.éîìîíóðéåòð, "WPN_SPYGLASS" },
            {  çëèèñêëðïìó.èåèéîíçåìîí, "WPN_CRATE" },
        };

        static Dictionary<string, string> gameModeAchievementIds = new Dictionary<string, string>
        {
            {  "tickets", "MODE_TDM" },
            {  "booty", "MODE_CTB" },
            {  "capture", "MODE_SIEGE" }
        };

        [HarmonyPatch(typeof(KillogFeed), "íïòéóéêêìèè")]
        class KillogFeedPatch
        {
            static void Postfix(PlayerInfo èíëçòåëçäìð, PlayerInfo îíïòçñéëíîé, int ïïîíäêèééíñ)
            {
                if (èíëçòåëçäìð != null && îíïòçñéëíîé != null && ïïîíäêèééíñ != 0)
                {
                    if (èíëçòåëçäìð.steamID == LocalPlayer.îêêæëçäëèñî.äíìíëðñïñéè.steamID)
                    {
                        çëèèñêëðïìó currentWpn = èíëçòåëçäìð.WeaponHandler.éïðæêåñåèñç;
                        if (achievementIds.TryGetValue(currentWpn, out string achievementId))
                        {
                            Tools.CommunicationPipes.incrementAchievement(MOD_ID, achievementId, 1);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerInfo), "updateScore")]
        class PlayerInfoUpdateScorePatch
        {
            static void Postfix(int amount, string note, bool tickSound)
            {
                if (note == "Enemy Ship Spot") // Spyglass
                {
                    Tools.CommunicationPipes.incrementAchievement(MOD_ID, achievementIds[çëèèñêëðïìó.éîìîíóðéåòð], 1);
                }
                else if (note == "Resupply") // Crate Resupply
                {
                    Tools.CommunicationPipes.incrementAchievement(MOD_ID, achievementIds[çëèèñêëðïìó.èåèéîíçåìîí], 1);
                }
            }
        }

        [HarmonyPatch(typeof(GameModeHandler), "win")]
        class GameModeHandlerPatch
        {
            static void Postfix(string ëëäíêðäóæîó, int íïïìîóðíçëæ, ïçîìäîóäìïæ.åéðñðçîîïêç äíìíëðñïñéè)
            {
                if (ëëäíêðäóæîó != LocalPlayer.îêêæëçäëèñî.ëëäíêðäóæîó)
                {
                    return; // Only care about local player wins
                }

                if (íïïìîóðíçëæ == 0) // TDM Win
                {
                    Tools.CommunicationPipes.incrementAchievement(MOD_ID, gameModeAchievementIds["tickets"], 1);
                }
                else if (íïïìîóðíçëæ == 5 || íïïìîóðíçëæ == 6) // CTB
                {
                    Tools.CommunicationPipes.incrementAchievement(MOD_ID, gameModeAchievementIds["booty"], 1);
                }
                else if (íïïìîóðíçëæ == 7) // Siege
                {
                    Tools.CommunicationPipes.incrementAchievement(MOD_ID, gameModeAchievementIds["capture"], 1);
                }
            }
        }

        void Start()
        {
            try
            {
                HarmonyInstance harmony = HarmonyInstance.Create("com.github.archie.MissingAchievements");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }
    }
}
