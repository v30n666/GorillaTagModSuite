using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace MultiModSuite
{
    // ==========================================
    // MOD 1: VERITY GHOST / HORROR MOD
    // ==========================================
    [BepInPlugin("com.modder.gt.verity", "Verity Ghost Mod", "1.0.0")]
    public class VerityMod : BaseUnityPlugin
    {
        private float timer = 0f;
        private bool isTriggered = false;

        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(VerityMod));
            Logger.LogInfo("Verity mod loaded.");
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > 15f && !isTriggered)
            {
                isTriggered = true;
                RenderSettings.fogDensity = 0.1f;
                RenderSettings.fogColor = new Color(0.3f, 0f, 0f);
                Logger.LogInfo("Verity has arrived...");
            }
        }
    }

    // ==========================================
    // MOD 2: SUPER SPEED MOD
    // ==========================================
    [BepInPlugin("com.modder.gt.speed", "Super Speed Mod", "1.0.0")]
    public class SpeedMod : BaseUnityPlugin
    {
        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Rigidbody rb = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = rb.velocity * 1.05f;
                }
            }
        }
    }
}
