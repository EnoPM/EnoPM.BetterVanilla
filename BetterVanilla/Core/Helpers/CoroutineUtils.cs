using System;
using System.Collections;
using UnityEngine;

namespace BetterVanilla.Core.Helpers;

public static class CoroutineUtils
{
    public static IEnumerator CoAssertWithTimeout(Func<bool> assertion, Action onTimeout, float timeoutInSeconds)
    {
        var failed = true;
        for (var timer = 0f; timer < timeoutInSeconds; timer += Time.deltaTime)
        {
            if (assertion())
            {
                failed = false;
                break;
            }
            yield return null;
        }
        if (!failed) yield break;
        onTimeout?.Invoke();
    }
    
    public static IEnumerator RandomWait(float min = 1f, float max = 10f)
    {
        var time = UnityEngine.Random.RandomRange(1f, 10f);
        yield return new WaitForSeconds(time);
    }
}