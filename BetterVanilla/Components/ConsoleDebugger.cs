using System.Linq;
using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class ConsoleDebugger : MonoBehaviour
{
    public static Transform? Target { get; set; }

    private void Update()
    {
        if (Target == null) return;
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            var pos = Target.position;
            pos.z += 0.1f;
            Target.position = pos;
            Ls.LogMessage($"Console position {Target.gameObject.active} [+] {Target.position.x} {Target.position.y} {Target.position.z}");
        }
        else if (Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            var pos = Target.position;
            pos.z -= 0.1f;
            Target.position = pos;
            Ls.LogMessage($"Console position {Target.gameObject.active} [-] {Target.position.x} {Target.position.y} {Target.position.z}");
        }
        else if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            var parent = Target.parent;
            if (parent != null)
            {
                Ls.LogMessage($"Console parent {parent.gameObject.active}: {parent.gameObject.name}");
            }
            
        }
    }
}