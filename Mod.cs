using System.Linq;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using MTM101BaldAPI;
using System.Collections;
using MTM101BaldAPI.Registers;
using ZenFulcrum.EmbeddedBrowser;
using MTM101BaldAPI.AssetTools;

namespace WorkingComputers
{
    public static class ProjectData
    {
        public const string projectGuid = "silverspringing.workingcomputers";
        public const string projectName = "Working Computers";
        public const string projectVersion = "0.0.1.0";
    }

    [BepInPlugin(ProjectData.projectGuid, ProjectData.projectName, ProjectData.projectVersion)]
    public class WorkingComputersPlugin : BaseUnityPlugin
    {
        public static WorkingComputersPlugin plugin;
        public ConfigEntry<bool> characterUse;
        public ConfigEntry<bool> timeFreeze;

        void Awake()
        {
            Harmony harmony = new Harmony(ProjectData.projectGuid);
            harmony.PatchAllConditionals();
            plugin = this;
            characterUse = plugin.Config.Bind(
                "Working Computers", "Characters Use Computers", false, "If enabled, certain NPCs will also be able to use computers."
            );
            timeFreeze = plugin.Config.Bind(
                "Working Computers", "Time Freeze", false, "If enabled, time will freeze while you're using a computer."
            );
            LoadingEvents.RegisterOnLoadingScreenStart(Info, Configure());
        }

        IEnumerator Configure()
        {
            yield return 1;
            yield return "Updating computers...";
            SetComputers();
            yield break;
        }

        void SetComputers()
        {
            //add component to all computers
            GameObject Computer = Resources.FindObjectsOfTypeAll<GameObject>().ToList().First(x => x.name == "MyComputer");
            Computer.AddComponent<RealComputerComponent>();
            Instantiate(Computer);
        }
    }
}
