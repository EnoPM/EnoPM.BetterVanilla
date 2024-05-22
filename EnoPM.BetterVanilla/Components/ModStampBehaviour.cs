using System.Collections;
using System.Reflection;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Extensions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class ModStampBehaviour : MonoBehaviour
{
    private void Start()
    {
        this.StartCoroutine(CoShowModStamp());
    }

    private static IEnumerator CoShowModStamp()
    {
        while (!DestroyableSingleton<ModManager>.InstanceExists)
        {
            yield return new WaitForSeconds(1f);
        }
        DestroyableSingleton<ModManager>.Instance.ShowModStamp();
    }
}