﻿using BepInEx.Logging;
using HarmonyLib;
using RG.Scene;
using RG.Scene.Action.UI;
using RG.Scene.Action.Core;
using Il2CppSystem.Collections.Generic;


namespace RGActionPatches
{
    internal static class Hooks
    {
        private static ManualLogSource Log = RGActionPatchesPlugin.Log;
        // Capture CommandList instance at scene load and initialize our other patches
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CommandList), nameof(CommandList.Awake))]
        private static void AwakePost(CommandList __instance)
        {
            StateManager.Instance.currentCommandList = __instance;
            Harmony.CreateAndPatchAll(typeof(AddCommands.Hooks), AddCommands.Hooks.GUID);
            Harmony.CreateAndPatchAll(typeof(DateSpotMovement.Hooks), DateSpotMovement.Hooks.GUID);
            Harmony.CreateAndPatchAll(typeof(TalkTarget.Hooks), TalkTarget.Hooks.GUID);
            Harmony.CreateAndPatchAll(typeof(Guests.Hooks), Guests.Hooks.GUID);
            Harmony.CreateAndPatchAll(typeof(ADV.Hooks), ADV.Hooks.GUID);
            Harmony.CreateAndPatchAll(typeof(Threesome.Hooks), Threesome.Hooks.GUID);

            Guests.Patches.ChangeCommandStates(ActionScene.Instance.Actors);
        }

        // Release CommandList instance on scene destroy and unpatch
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActionScene), nameof(ActionScene.OnDestroy))]
        private static void DestroyPre()
        {
            StateManager.Instance.currentCommandList = null;
            Harmony.UnpatchID(AddCommands.Hooks.GUID);
            Harmony.UnpatchID(DateSpotMovement.Hooks.GUID);
            Harmony.UnpatchID(TalkTarget.Hooks.GUID);
            Harmony.UnpatchID(Guests.Hooks.GUID);
            Harmony.UnpatchID(ADV.Hooks.GUID);
            Harmony.UnpatchID(Threesome.Hooks.GUID);
        }
                
        //Check the MapID and SubMapID(PrivateKeyID) and remove guest actors that do not belong to the private map
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActionScene), nameof(ActionScene.InitializePartnerState))]
        private static void InitializePartnerStatePre(ActionScene __instance , List<Actor> actors)
        {
            Guests.Patches.RemoveGuestsDoNotBelongToScene(__instance, actors);
        }
    }
}
