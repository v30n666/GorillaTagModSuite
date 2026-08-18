using BepInEx;
using UnityEngine;
using HarmonyLib;
using System.Collections;

namespace LocalVerityMonsterMod
{
    [BepInPlugin("com.modder.gt.veritylocalmonster", "Local Verity Monster Mod", "1.0.0")]
    public class VerityMonsterPlugin : BaseUnityPlugin
    {
        private GameObject spawnedMonster = null;
        private bool isGrabbing = false;
        private Rigidbody monsterRb = null;

        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(VerityMonsterPlugin));
            Logger.LogInfo("Verity Monster Mod has started up successfully.");
            StartCoroutine(SpawnMonsterOnStartup());
        }

        IEnumerator SpawnMonsterOnStartup()
        {
            // Wait until Gorilla Tag player instance is fully loaded in-game
            while (GorillaLocomotion.Player.Instance == null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);
            SpawnMonster();
        }

        void Update()
        {
            // Press 'G' to spawn or teleport the monster right onto your position
            if (Input.GetKeyDown(KeyCode.G))
            {
                SpawnMonster();
            }

            // Simple Grabbable / Pull mechanic using the Left Mouse Button or Trigger simulation
            if (Input.GetMouseButtonDown(1)) // Right click to grab/pull monster closer
            {
                isGrabbing = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                isGrabbing = false;
            }

            if (isGrabbing && spawnedMonster != null && GorillaLocomotion.Player.Instance != null)
            {
                Vector3 targetPos = GorillaLocomotion.Player.Instance.transform.position + GorillaLocomotion.Player.Instance.transform.forward * 2f;
                spawnedMonster.transform.position = Vector3.Lerp(spawnedMonster.transform.position, targetPos, Time.deltaTime * 10f);
                if (monsterRb != null)
                {
                    monsterRb.velocity = Vector3.zero;
                    monsterRb.angularVelocity = Vector3.zero;
                }
            }
        }

        void SpawnMonster()
        {
            if (GorillaLocomotion.Player.Instance == null) return;

            // If a monster already exists, destroy it first so a new one takes its place
            if (spawnedMonster != null)
            {
                Destroy(spawnedMonster);
            }

            // Create a primitive 3D shape to act as our scary custom monster model
            spawnedMonster = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            spawnedMonster.name = "VerityMonster_Local";

            // Spawn it directly on top of the player's position
            Vector3 spawnPosition = GorillaLocomotion.Player.Instance.transform.position + Vector3.up * 1f;
            spawnedMonster.transform.position = spawnPosition;
            spawnedMonster.transform.localScale = new Vector3(1.2f, 1.8f, 1.2f);

            // Give it a dark, spooky red/black horror material color
            Renderer rend = spawnedMonster.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.shader = Shader.Find("Standard");
                rend.material.color = new Color(0.15f, 0.0f, 0.0f); // Dark blood red/black
            }

            // Add physics Rigidbody so it can roll, tumble, and interact with physics
            monsterRb = spawnedMonster.AddComponent<Rigidbody>();
            monsterRb.mass = 50f;
            monsterRb.useGravity = true;
            monsterRb.freezeRotation = false; // Allows it to tumble and roll wildly around the map!

            // Add the AI chasing behavior script onto the spawned monster object
            spawnedMonster.AddComponent<VerityAIController>();

            Logger.LogInfo("Verity Monster spawned directly on player and is now rolling/chasing!");
        }
    }

    // AI Controller script that handles the rolling and chasing behavior towards the player
    public class VerityAIController : MonoBehaviour
    {
        private Rigidbody rb;
        private Transform playerTransform;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (playerTransform == null && GorillaLocomotion.Player.Instance != null)
            {
                playerTransform = GorillaLocomotion.Player.Instance.transform;
            }

            if (playerTransform != null && rb != null)
            {
                // Calculate direction towards the player
                Vector3 direction = (playerTransform.position - transform.position);
                float distance = direction.magnitude;
                direction.Normalize();

                // If the monster is far away, roll/push towards the player aggressively
                if (distance > 1.5f)
                {
                    float rollSpeed = 350f;
                    rb.AddForce(direction * rollSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
                }
            }
        }
    }
}
