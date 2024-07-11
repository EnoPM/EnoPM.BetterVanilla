using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AmongUsDevKit.Extensions;

internal static partial class ILProcessorExtensions
{
    public static void Append(this ILProcessor ilProcessor, List<Instruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            ilProcessor.Append(instruction);
        }
    }

    public static void Prepend(this ILProcessor ilProcessor, Instruction instruction)
    {
        if (ilProcessor.Body.Instructions.Count > 0)
        {
            ilProcessor.InsertBefore(ilProcessor.Body.Instructions[0], instruction);
        }
        else
        {
            ilProcessor.Append(instruction);
        }
    }

    public static void Prepend(this ILProcessor ilProcessor, List<Instruction> instructions)
    {
        for (var i = instructions.Count - 1; i >= 0; i--)
        {
            ilProcessor.Prepend(instructions[i]);
        }
    }
}