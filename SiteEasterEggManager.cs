using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WorkingComputers
{
    public class SiteEasterEggManager : Singleton<SiteEasterEggManager>
    {
        string[] visitedSites = new string[] { };
        public void TryForEasterEgg(string Site, PlayerManager player)
        {
            bool flag = false;
            //check full url
            switch (Site)
            {
             //Baldi's Basics Classic
                case "https://basically-games.itch.io/baldis-basics":
                case "https://gamejolt.com/games/baldis-basics/342754":
                    if (visitedSites.Contains("BBC"))
                    {
                        CoreGameManager.Instance.AddPoints(50, player.playerNumber, true);
                        visitedSites.AddItem("BBC");
                    }
                    flag = true;
                    break;
                    //Baldi's Basics Birthday Bash
                case "https://basically-games.itch.io/baldis-basics-birthday-bash":
                    if (visitedSites.Contains("Party"))
                    {
                        CoreGameManager.Instance.AddPoints(50, player.playerNumber, true);
                        visitedSites.AddItem("Party");
                    }
                    flag = true;
                    break;
                //Baldi's Basics Classic Remastered
                case "https://store.steampowered.com/app/1712830/Baldis_Basics_Classic_Remastered/":
                case "https://basically-games.itch.io/baldis-basics-classic-remastered":
                case "https://gamejolt.com/games/baldis-basics-classic-remastered/602328":
                    if (visitedSites.Contains("BBCR"))
                    {
                        CoreGameManager.Instance.AddPoints(100, player.playerNumber, true);
                        visitedSites.AddItem("BBCR");
                    }
                    flag = true;
                    break;
                //Baldi's Basics Plus
                case "https://store.steampowered.com/app/1275890/Baldis_Basics_Plus/":
                case "https://basically-games.itch.io/baldis-basics-plus":
                case "https://gamejolt.com/games/baldis-basics-plus/481026":
                    if (visitedSites.Contains("BBP"))
                    {
                        CoreGameManager.Instance.AddPoints(100, player.playerNumber, true);
                        visitedSites.AddItem("BBP");
                    }
                    flag = true;
                    break;
                //Clicky
                case "https://store.steampowered.com/app/2582130/Clicky/":
                    if (visitedSites.Contains("Clicky"))
                    {
                        CoreGameManager.Instance.AddPoints(150, player.playerNumber, true);
                        visitedSites.AddItem("Clicky");
                    }
                    flag = true;
                    break;
                default:
                    break;
            }
            if (flag)
            {
                string split = Site.Split2("/").ToArray()[2];
                Debug.LogWarning("Visited Site: " + split);
                switch (split)
                {
                    default:
                        break;
                }
            }
        }
    }
}
