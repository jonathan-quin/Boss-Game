using Godot;
using System;


public partial class PlayerUI : Control
{
	

	//id is -1 if the is the final destination
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void displayMessage(string message,long id)
	{
		GD.Print(GetMultiplayerAuthority(), " is in the display message code. ", id);

		if (id != -1)
		{
			GD.Print(GetMultiplayerAuthority(), " recieved a message and redirected to ", id);
			RpcId(id, "displayMessage", message,-1);
			return;
		}

		GD.Print("running display message ", GetMultiplayerAuthority());

		Tween tween = GetTree().CreateTween();

		Label label = GetNode<Label>("%messageLabel");

		label.Text = message;

		label.VisibleCharacters = 0;

		tween.TweenProperty(label, "visible_characters", message.Length, 0.5);
        tween.TweenProperty(label, "visible_characters", 0, 0.5).SetDelay(3.0);

		GD.Print("The messaage is: ", message);

    }

	

}
