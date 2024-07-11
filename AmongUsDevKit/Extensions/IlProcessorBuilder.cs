using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AmongUsDevKit.Extensions;

internal static partial class ILProcessorExtensions
{
    public static Instruction CreateCallMethod(this ILProcessor ilProcessor, MethodReference methodToCall)
    {
        return ilProcessor.Create(OpCodes.Call, methodToCall);
    }
    
    public static Instruction CreateVirtualCallMethod(this ILProcessor ilProcessor, MethodReference methodToCall)
    {
        return ilProcessor.Create(OpCodes.Callvirt, methodToCall);
    }

    public static void AppendBeforeReturn(this ILProcessor il, List<Instruction> instructions)
    {
        var ret = il.Body.Instructions.LastOrDefault(x => x.OpCode == OpCodes.Ret);
        if (ret == null)
        {
            throw new Exception($"Unable to append before return in {il.Body.Method.FullName}. They are no return instruction!");
        }
        foreach (var instruction in instructions)
        {
            il.InsertBefore(ret, instruction);
        }
    }

    public static void AddCallOnTop(this MethodDefinition method, MethodReference methodToCall)
    {
        var il = method.Body.GetILProcessor();
        il.Prepend(il.CreateCallMethod(methodToCall));
    }
    
    public static void AddCallBeforeReturn(this MethodDefinition method, MethodReference methodToCall)
    {
        var il = method.Body.GetILProcessor();
        var ret = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Ret);
        il.InsertBefore(ret, il.Create(OpCodes.Call, methodToCall));
    }
}