using System.Collections;
using HarmonyLib;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Combat;
using MelonLoader;
using UnityEngine;
using SeneekiMod.Utils;

namespace SeneekiMod.Enhancements
{
    public static class EmployeeMove
    {
        private static readonly List<Employee> trackedEmployees = new();
        private static readonly HashSet<int> ragdolled = new();
        private static float nextCheckTime = 0f;
        
        public static void Init()
        {
            MelonEvents.OnUpdate.Subscribe(OnUpdate);
        }

        public static void TrackEmployee(Employee emp)
        {
            if (emp != null && !trackedEmployees.Contains(emp))
                trackedEmployees.Add(emp);
        }

        private static void OnUpdate()
        {
            bool keysPressed = Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl);
            if (!keysPressed)
                return;

            if (Time.time < nextCheckTime)
                return;

            nextCheckTime = Time.time + 0.2f;

            var player = Player.Local;
            if (player == null)
                return;

            foreach (var emp in trackedEmployees)
            {
                if (emp == null || !emp.gameObject.activeInHierarchy)
                    continue;

                int id = emp.gameObject.GetInstanceID();
                if (ragdolled.Contains(id))
                    continue;

                float dist = Vector3.Distance(emp.transform.position, player.transform.position);
                if (dist > 6f)
                    continue;

                MelonCoroutines.Start(RagdollPunch(emp, id));
            }
        }


        private static IEnumerator RagdollPunch(Employee emp, int id)
        {
            var impact = new Impact
            {
                HitPoint = emp.transform.position + Vector3.up,
                ImpactForceDirection = Vector3.down,
                ImpactForce = 150f,
                ImpactDamage = 0f,
                ImpactType = EImpactType.Punch,
                ImpactID = UnityEngine.Random.Range(1, 9999),
                ImpactSource = null
            };

            emp.ReceiveImpact(impact);
            emp.ProcessImpactForce(impact.HitPoint, impact.ImpactForceDirection, 50f);
            ragdolled.Add(id);

            yield return new WaitForSeconds(5f);
            ragdolled.Remove(id);
        }
    }

    [HarmonyPatch(typeof(Employee), "Awake")]
    public static class EmployeeAwakePatch
    {
        public static void Postfix(Employee __instance)
        {
            try { EmployeeMove.TrackEmployee(__instance); } catch { }
        }
    }

    [HarmonyPatch(typeof(Employee), "Start")]
    public static class EmployeeStartPatch
    {
        public static void Postfix(Employee __instance)
        {
            try { EmployeeMove.TrackEmployee(__instance); } catch { }
        }
    }

    [HarmonyPatch(typeof(UnityEngine.Object), "Instantiate", new System.Type[] { typeof(UnityEngine.Object) })]
    public static class InstantiatePatch
    {
        public static void Postfix(UnityEngine.Object __result)
        {
            try
            {
                GameObject? go = __result as GameObject;
                if (go == null) return;

                var emp = go.GetComponent<Employee>();
                if (emp != null)
                    EmployeeMove.TrackEmployee(emp);
            }
            catch { }
        }
    }
}
