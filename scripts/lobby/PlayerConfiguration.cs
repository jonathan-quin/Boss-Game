using Godot;
using System;

public partial class PlayerConfiguration : Node
{

	public bool wantsToBeBoss;
	public string nametag;
	public int id;

    private static readonly Random random = new Random();
    private static readonly string[] randomNames = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa" };

    public PlayerConfiguration(bool wantsToBeBoss, string nametag, int id)
    {
        this.wantsToBeBoss = wantsToBeBoss;
        this.id = id;

        if (nametag.Length < 3)
        {
            this.nametag = nametag + " " + GetRandomName();
        }
    }

    private string GetRandomName()
    {
        return randomNames[random.Next(randomNames.Length)];
    }

}
