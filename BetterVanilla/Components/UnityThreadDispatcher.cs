using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class UnityThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action?> ExecutionQueue = new();

    public static void RunOnMainThread(Action action)
    {
        lock (ExecutionQueue)
        {
            ExecutionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        while (ExecutionQueue.Count > 0)
        {
            Action? action;
            lock (ExecutionQueue)
            {
                action = ExecutionQueue.Dequeue();
            }
            action?.Invoke();
        }
    }
}