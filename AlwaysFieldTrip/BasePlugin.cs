using BepInEx;
using HarmonyLib;

[BepInPlugin("basicallyukrainian.denyscrasav4ik.alwaysfieldtrip", "Always Field Trip", "1.0.0")]
public class BasePlugin : BaseUnityPlugin
{
    void Awake()
    {
        var harmony = new Harmony("basicallyukrainian.denyscrasav4ik.alwaysfieldtrip");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(CoreGameManager))]
public class CoreGameManagerPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    static void AlwaysTripAvailable(CoreGameManager __instance)
    {
        __instance.tripAvailable = true;
    }
}

[HarmonyPatch(typeof(PitstopGameManager), "PrepareLevelData")]
public class PitstopGeneratorPatch
{
    [HarmonyPostfix]
    static void ForceTripGeneration(PitstopGameManager __instance, LevelData data)
    {
        var traverse = Traverse.Create(__instance);
        var currentFieldTripField = traverse.Field("currentFieldTrip");

        if (currentFieldTripField.GetValue() == null)
        {
            WeightedSelection<FieldTripObject>[] tierOneTrips = traverse.Field("tierOneTrips").GetValue<WeightedSelection<FieldTripObject>[]>();

            if (tierOneTrips != null && tierOneTrips.Length > 0)
            {
                FieldTripObject forcedTrip = WeightedSelection<FieldTripObject>.RandomSelection(tierOneTrips);

                currentFieldTripField.SetValue(forcedTrip);
                data.roomAssetsPlacements.Add(forcedTrip.tripHub);

                Singleton<CoreGameManager>.Instance.currentFieldTrip = forcedTrip;
                Singleton<CoreGameManager>.Instance.tripAvailable = true;
            }
        }
    }
}
