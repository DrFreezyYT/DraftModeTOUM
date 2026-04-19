using System;
using System.Collections;
using HarmonyLib;

namespace DraftModeTOUM.Managers
{
    internal static class TouMRoleListCaps
    {
        private static readonly System.Reflection.MethodInfo BuildRoleListBucketsMethod =
            AccessTools.Method("TownOfUs.Patches.TouRoleManagerPatches:BuildRoleListBuckets");

        public static bool TryResolve(int playerCount, out int impostors, out int neutralKillings, out int neutralOthers)
        {
            impostors = 0;
            neutralKillings = 0;
            neutralOthers = 0;

            if (BuildRoleListBucketsMethod == null)
            {
                DraftModePlugin.Logger.LogWarning("[TouMRoleListCaps] TOU:M role list API not found.");
                return false;
            }

            try
            {
                if (BuildRoleListBucketsMethod.Invoke(null, new object[] { playerCount }) is not IEnumerable buckets)
                {
                    DraftModePlugin.Logger.LogWarning("[TouMRoleListCaps] BuildRoleListBuckets returned no buckets.");
                    return false;
                }

                foreach (var bucket in buckets)
                {
                    switch (bucket?.ToString())
                    {
                        case "ImpConceal":
                        case "ImpKilling":
                        case "ImpPower":
                        case "ImpSupport":
                        case "ImpCommon":
                        case "ImpSpecial":
                        case "ImpRandom":
                            impostors++;
                            break;

                        case "NeutKilling":
                            neutralKillings++;
                            break;

                        case "NeutBenign":
                        case "NeutEvil":
                        case "NeutOutlier":
                        case "NeutCommon":
                        case "NeutSpecial":
                        case "NeutWildcard":
                        case "NeutRandom":
                            neutralOthers++;
                            break;
                    }
                }

                DraftModePlugin.Logger.LogInfo(
                    $"[TouMRoleListCaps] Resolved role-list caps for {playerCount} players: " +
                    $"Imp={impostors}, NK={neutralKillings}, NP={neutralOthers}");

                return true;
            }
            catch (Exception ex)
            {
                DraftModePlugin.Logger.LogWarning(
                    $"[TouMRoleListCaps] Failed to resolve TOU:M role-list caps: {ex.Message}");
                return false;
            }
        }
    }
}
