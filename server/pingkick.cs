using System;
using GTANetworkServer;
using GTANetworkShared;

public class pingkick : Script
{ 	
	
	private long lastTime=UnixTime.getUnixTimeToS();
	
	public pingkick()
	{
		API.onUpdate += OnUpdate;
	}


	private void OnUpdate(){
		if(UnixTime.getUnixTimeToS()-lastTime>=3)
		{
			lastTime=UnixTime.getUnixTimeToS();
			var p=API.getAllPlayers();
			foreach(var i in p)
			{ 
				if(i.name.Length>20)
				{
					API.sendChatMessageToAll("[ ~r~!~w~ ]" + i.name + " 因名稱過長被踢出了服務器.");
					API.kickPlayer(i,"name-so-long");				
				}
				if(API.getPlayerPing(i)>=350)
				{
				
					API.sendChatMessageToAll("[ ~r~!~w~ ]" + API.getPlayerName(i) + " 因延遲過高被踢出了服務器.");
					API.kickPlayer(i,"ping-high");
				}
			}
		}
	}
}
 