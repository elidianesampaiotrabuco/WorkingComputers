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
        public const string projectVersion = "0.0.3.0";
    }

    [BepInPlugin(ProjectData.projectGuid, ProjectData.projectName, ProjectData.projectVersion)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class WorkingComputersPlugin : BaseUnityPlugin
    {
        public static WorkingComputersPlugin plugin;
        //features
        public ConfigEntry<bool> characterUse;
        public ConfigEntry<bool> timeFreeze;
        public ConfigEntry<bool> easterEggs;
        //navigation keys (temporary workaround until i add the ui)
        public ConfigEntry<KeyCode> previousKey;
        public ConfigEntry<KeyCode> resetKey;
        public ConfigEntry<KeyCode> leaveKey;
        //presets
        public ConfigEntry<string> homePage;

        void Awake()
        {
            Harmony harmony = new Harmony(ProjectData.projectGuid);
            harmony.PatchAllConditionals();
            plugin = this;

            //Features
            characterUse = plugin.Config.Bind(
                "Features", "Characters Use Computers (NOT CURRENTLY IMPLEMENTED!)", false, "If enabled, certain NPCs will also be able to use computers."
            );
            timeFreeze = plugin.Config.Bind(
                "Features", "Time Freeze", false, "If enabled, time will freeze while you're using a computer."
            );
            easterEggs = plugin.Config.Bind(
                "Features", "Easter Eggs", true, "If enabled, visiting certain sites will trigger unique events."
            );
            //Keybinds
            previousKey = plugin.Config.Bind(
                "Keybinds", "Previous Key", KeyCode.Insert, "Key to go back to the previous page. This may be removed in the future."
            );
            resetKey = plugin.Config.Bind(
                "Keybinds", "Reset Key", KeyCode.Home, "Key to reset the computer to the home page. This may be removed in the future."
            );
            leaveKey = plugin.Config.Bind(
                "Keybinds", "Leave Key", KeyCode.Escape, "Key to leave the computer. This may be removed in the future."
            );
            //Presets
            homePage = plugin.Config.Bind(
                "Presets", "Home Page", "https://google.com", "The computer's home page. I wouldn't advise changing this!"
            );

            LoadingEvents.RegisterOnLoadingScreenStart(Info, Configure());
        }

        IEnumerator Configure()
        {
            yield return 1;
            yield return "Setting up...";
            Setup();
            yield break;
        }

        void Setup()
        {
            //add component to all computers
            GameObject Computer = Resources.FindObjectsOfTypeAll<GameObject>().ToList().First(x => x.name == "MyComputer");
            Computer.AddComponent<RealComputerComponent>();
            //create site easter egg manager
            GameObject newManager = new GameObject("SiteEasterEggManager");
            newManager.AddComponent<SiteEasterEggManager>();
        }
    }
}
