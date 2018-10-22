using System;
using GTANetworkServer;
using GTANetworkShared;

 

public class test : Script
{ 	

	public test()
	{ 
	} 

	[Command("ts")]
	public void OnClientEvent(Client Player) //arguments param can contain multiple params
	{

		API.sendNativeToPlayer(Player,0xB5BA80F839791C0F,API.getPlayerVehicle(Player),255,0,0); 
 
	}
	
	[Command("st")]
	public void cl(Client Player,string name)
	{
		API.sendNativeToPlayer(Player,0x068E835A1D0DC0E3,name); 
	}
	
	[Command("sta")]
	public void cl1(Client Player)
	{
		API.sendNativeToPlayer(Player,0xB4EDDC19532BFB85); 
	}	
 


}