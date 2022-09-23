using HarmonyLib;
using ModLoader;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RocketTeleporter
{
    public class Main : Mod
    {
        public override string ModNameID => "RTP";
        public override string DisplayName => "Rocket Teleporter";
        public override string Author => "Osmo and N2O4";
        public override string MinimumGameVersionNecessary => "1.5.8";
        public override string ModVersion => "2.0";
        public override string Description => "Teleport your rockets anywhere you want! Ported to new ModLoader by N2O4.";

        public override void Early_Load()
        {
            Main.patcher = new Harmony("mods.Osmo.RTP");
            Main.patcher.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("Loaded");
            return;
        }

        public override void Load()
        {

        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            if (scene.name == "World_PC")
            {
                GameObject menu = new GameObject("Teleporter");
                menu.AddComponent<Teleporter>();
                UnityEngine.Object.DontDestroyOnLoad(menu);
                menu.SetActive(true);
            }
        }

        public static Harmony patcher;
    }

}
