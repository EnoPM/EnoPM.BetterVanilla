using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;
using BetterVanilla.Options.Core.Serialization;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ShareHostOption)]
public sealed class HostOptionRpc : CustomRpcMessage
{
    private AbstractSerializableOption? Option { get; }
    
    public HostOptionRpc(BetterPlayerControl sender, AbstractSerializableOption option) : base(sender)
    {
        Option = option;
    }

    public HostOptionRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        var key = reader.ReadString();
        var option = HostOptions.Default.GetOptions().FirstOrDefault(x => x.Key == key);
        if (option == null)
        {
            Ls.LogWarning($"Unable to find host option {key} from rpc");
            return;
        }
        Option = option;
        Option.ReadValue(reader);
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        if (Option == null)
        {
            Ls.LogWarning($"Unable to serialize null host option");
            return;
        }
        writer.Write(Option.Key);
        Option.WriteValue(writer);
    }
    
    protected override void HandleMessage()
    {
        if (Option == null) return;
        Ls.LogInfo($"Received option value '{Option.GetValueAsString()}' for '{Option.Key}' from host rpc");
    }
}