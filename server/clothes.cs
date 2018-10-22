using System;
using GTANetworkServer;
using GTANetworkShared;

using System.Collections.Generic;
 

public class clothes : Script
{ 	 
	
	public clothes()
	{
		
		API.onClientEventTrigger += OnClientEvent;  
		API.onEntityDataChange+= OnEntityDataChange; 
		
		 
 
	} 
 
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_clothes_changesex_freeze")
		{
			API.setEntityInvincible(API.getPlayerVehicle(Player),(bool)arguments[0]);	
			API.freezePlayer(Player,(bool)arguments[0]);
		}
		if(eventName=="SC_clothes_changeskin_skin")
		{
			var money=10000;
			if(API.getEntitySyncedData(Player,"SC_SKIN_CHANGE")==1)
			{
				money=0;
			}
			var m=API.getEntityData(Player,"SC_money");
			if(m.getMoney()>=money)
			{
				m.addMoney(-money);
				var p=arguments[0].ToString();
				PedHash v=(PedHash)arguments[0];
				API.setPlayerSkin(Player,v);
				var lg=new login();
				lg.SetPlayerAccess(Player,"SC_SKIN",p);
				lg=null;
				API.resetEntitySyncedData(Player,"SC_SKIN_CHANGE");
			}else{
				API.sendChatMessageToPlayer(Player,"~r~朋友,你的金錢不夠呀!");
			}
			m=null;
		}		
	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录傳車輛列表 
			var lg=new login();
			var Player=API.getPlayerFromHandle(entity);
			if(lg.GetPlayerAccess(Player,"SC_SKIN")!="SC_NULL")
			{
				var p=lg.GetPlayerAccess(Player,"SC_SKIN");
				var id=Convert.ToInt64(p);
	 
				API.setPlayerSkin(Player,(PedHash)id);
				API.triggerClientEvent(Player,"SC_clothes_skinid",p);
			}else{ 
				if(API.getEntitySyncedData(Player,"SC_register")!=1)
					{
						API.setEntitySyncedData(Player,"SC_SKIN_CHANGE",1);
						API.sendChatMessageToPlayer(Player,"~g~你現在擁有~r~一~g~次免費換膚的機會,如需使用,請前往地圖上的衣服處");
					}else{
						lg.SetPlayerAccess(Player,"SC_SKIN","71929310");
					}
			}
			lg=null;
		}
	}
}