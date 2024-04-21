using Godot;
using System;

public partial class PlayerUI : Control
{
	

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void displayMessage(string message,long id)
	{
		GD.Print("we are in the display message code. ", id);

		if (GetMultiplayerAuthority() != id)
		{
			GD.Print(GetMultiplayerAuthority(), " recieved a message and redirected to ", id);
			RpcId(id, "displayMessage", message);
			return;
		}

		Tween tween = GetTree().CreateTween();

		Label label = GetNode<Label>("%messageLabel");

		label.Text = message;

		label.VisibleCharacters = 0;

		tween.TweenProperty(label, "VisibleCharacters", message.Length, 0.5);
        tween.TweenProperty(label, "VisibleCharacters", 0, 0.5).SetDelay(1.0);

		GD.Print("The messaage is: ", message);

    }

	

}