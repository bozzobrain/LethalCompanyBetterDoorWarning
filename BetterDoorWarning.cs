using HarmonyLib;
using UnityEngine;

namespace BetterDoorWarning
{
	[HarmonyPatch]
	internal class BetterDoorWarningFunctions
	{
		private bool lastCheckEnemy = false;

		[HarmonyPatch(typeof(EntranceTeleport), "Update")]
		private static class Patch
		{
			private static BetterDoorWarningFunctions betterDoorWarningFunctions = new();

			[HarmonyPrefix]
			private static void Update(EntranceTeleport __instance)
			{
				__instance.enemyNearLastCheck = true;
				if ((Object)__instance.triggerScript == (Object)null || !__instance.isEntranceToBuilding)
					return;
				if ((double)__instance.checkForEnemiesInterval <= 0.0)
				{
					if (!__instance.gotExitPoint)
					{
						if (!__instance.FindExitPoint())
							return;
						__instance.gotExitPoint = true;
					}
					else
					{
						__instance.checkForEnemiesInterval = 1f;
						bool flag = false;
						bool flag2 = false;
						for (int index = 0; index < RoundManager.Instance.SpawnedEnemies.Count; ++index)
						{
							if ((double)Vector3.Distance(RoundManager.Instance.SpawnedEnemies[index].transform.position, __instance.exitPoint.transform.position) < 7.6999998092651367 && !RoundManager.Instance.SpawnedEnemies[index].isEnemyDead)
							{
								flag = true;
								break;
							}
							if ((double)Vector3.Distance(RoundManager.Instance.SpawnedEnemies[index].transform.position, __instance.exitPoint.transform.position) < 7.6999998092651367 && RoundManager.Instance.SpawnedEnemies[index].isEnemyDead)
							{
								flag2 = true;
								break;
							}
						}
						if (flag && !betterDoorWarningFunctions.lastCheckEnemy)
						{
							betterDoorWarningFunctions.lastCheckEnemy = true;
							__instance.triggerScript.hoverTip = "[Near activity detected - He Alive]";
						}
						else if (flag2 && !betterDoorWarningFunctions.lastCheckEnemy)
						{
							betterDoorWarningFunctions.lastCheckEnemy = true;
							__instance.triggerScript.hoverTip = "[Near activity detected - He Dead]";
						}
						else
						{
							if (!__instance.enemyNearLastCheck)
								return;
							betterDoorWarningFunctions.lastCheckEnemy = false;
							__instance.triggerScript.hoverTip = "Enter: [LMB]";
						}
					}
				}
				else
					__instance.checkForEnemiesInterval -= Time.deltaTime;
				return;
			}
		}
	}
}