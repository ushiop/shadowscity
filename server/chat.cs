using System;
using GTANetworkServer;
using GTANetworkShared;

 

public class chat : Script
{ 	

	public chat()
	{
		API.onClientEventTrigger += OnClientEvent; 
	} 

	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{

		if(eventName=="SC_chat_send")
		{	 
			API.sendChatMessageToAll(API.getPlayerName(Player),"~w~"+arguments[0].ToString());
			API.consoleOutput(API.getPlayerName(Player)+" : "+arguments[0].ToString());
		}
 
	}
	
	 


}