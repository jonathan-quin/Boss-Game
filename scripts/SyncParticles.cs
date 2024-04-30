using Godot;
using System;

public partial class SyncParticles : CpuParticles3D
{

    /// <summary>
    /// Calling this function on any client calls it for all other clients.
    /// </summary>
    /// <param name="origin"></param>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void EmittRPC(int origin = -1)
    {
        if (origin == Multiplayer.GetUniqueId()) return;
        Emitting = true;
        if (origin == -1)
            Rpc("EmittRPC", Multiplayer.GetUniqueId());
    }
}
