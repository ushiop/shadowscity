using System;
using GTANetworkServer;
using GTANetworkShared;

using System.Collections.Generic;
 

public class playerlist : Script
{ 	

	private long missioncd=UnixTime.getUnixTimeToS()+10;
	
	public playerlist()
	{

		API.onPlayerDisconnected += OnPlayerDisconnected;

		API.onClientEventTrigger += OnClientEvent; 
		API.onUpdate += OnUpdate;
		API.onEntityDataChange+= OnEntityDataChange; 		
	} 
	
	private void OnUpdate()
	{
		if(UnixTime.getUnixTimeToS() > missioncd)
		{//每隔60s更新玩家信息到所有客戶端
			missioncd=UnixTime.getUnixTimeToS()+10;//+80
			API.triggerClientEventForAll("SC_plist",PlayerListToJson());
		}
	}
	
	private string PlayerListToJson(string name=null)
	{
		var players=API.getAllPlayers();
		List<listinfo> str=new List<listinfo>();
 
			foreach(Client x in players)
			{
				if(API.getEntitySyncedData(x,"SC__USERINFO_OK")==1)
					{
						if(API.getPlayerName(x)!=name){
								listinfo p=new listinfo();
								var us=API.getEntityData(x,"SC_USERINFO");
								p.name=x.name;
								p.maxmoney=us.getUserInfo("MAXMONEY").infoValue;
								p.usemoney=us.getUserInfo("USEMONEY").infoValue;
								p.racewins=us.getUserInfo("RACEWINS").infoValue;
								p.raceloses=us.getUserInfo("RACELOSES").infoValue;
								p.racetops=us.getUserInfo("RACETOPS").infoValue;
								p.syscars=us.getUserInfo("SYSCARS").infoValue;
								p.pays=us.getUserInfo("PAYS").infoValue;
								p.racemission=us.getUserInfo("RACEMISSION").infoValue;
								p.racerank=us.getUserInfo("RACERANK").infoValue;
								p.portrait=us.getUserInfo("PORTRAIT").infoValue; 
 
								str.Add(p);
						}
					}
			}
	 
		return API.toJson(str.ToArray());
	}
	
	private Client getPlayerByName(string targetName)
	{
 
		return API.getPlayerFromName(targetName);
	}
	
	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		API.triggerClientEventForAll("SC_plist",PlayerListToJson(API.getPlayerName(Player)));

	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC__USERINFO_OK")
		{ 
			API.triggerClientEventForAll( "SC_plist",PlayerListToJson() );
		}
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_plist_goto"){
			Client target=getPlayerByName(arguments[0].ToString());
			if(target!=null){
				Vector3 targetPos=API.getEntityPosition(target);
				//get player vehicle
				var veh=API.getPlayerVehicle(Player);
				if(API.isPlayerInAnyVehicle(Player)==false)
				{
					API.setEntityPosition(Player, targetPos);
				}else{
					var seat=API.getPlayerVehicleSeat(Player);
					if(seat==-1)
					{
						if(API.getEntitySyncedData(Player, "SC_ACL_HAS_Name:Goto_Take_Vehicle")==1)
						{
							API.setEntityPosition(veh,targetPos);
						}else{
							
							API.setEntityPosition(Player, targetPos);
						}
					}else{
						API.setEntityPosition(Player, targetPos);
					}
				}
			}else{
				//TriggerClient 
				API.triggerClientEvent(Player, "SC_plist_false",PlayerListToJson(),arguments[0].ToString() );
			}
		}
	}
}

public class listinfo
{
	public string name;
	public int dataok=0;
	public int portrait;
	public int racerank;
	public int maxmoney;
	public int usemoney;
	public int racewins;
	public int raceloses;
	public int racetops;
	public int syscars;
	public int pays;
	public int racemission;
	public listinfo(){}
}