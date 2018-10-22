using System;

using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class mission : Script
{ 	
	private long missioncd=UnixTime.getUnixTimeToS()+20;
	
	public mission()
	{
		API.onUpdate += OnUpdate;
		
		API.onClientEventTrigger += OnClientEvent; 
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_mission_compelete_givemoney")
		{
			API.setEntitySyncedData(Player,"SC_USERINFO:RACEMISSION",API.getEntitySyncedData(Player,"SC_USERINFO:RACEMISSION")+1);
			int mo=(int)arguments[0];
			if(mo!=-1)
			{
				var m=API.getEntityData(Player,"SC_money");
				m.addMoney(mo);
				m=null;
			}else{
 
				new item_drop().itemDrop(Player,"車輛碎片");
 
			}
		}
 
	}
	
	/*[Command("f")]
	public void fss(Client Player)
	{
		new item_drop().itemDrop(Player,"車輛裝飾插件");
	}*/
	
	private void OnUpdate()
	{
		if(UnixTime.getUnixTimeToS() > missioncd)
		{//每5分鐘發佈一次新任務
		//如果舊任務未完成或失敗,則不會發佈給該玩家
			missioncd=UnixTime.getUnixTimeToS()+80;//+80
			getRandomMission();
		}
	}
	
	private void getRandomMission()
	{
		var index=new Random().Next((track.raceRoad.Count)); 
		var p=track.raceRoad[index];
		var hard=(float)new Random().Next(5);
		hard=hard/10;
		hard=hard+(float)1.0;
		var name=p.getRaceName();//任務目標
		var toptime=(float)p.getRaceTopTimeToLong(1);
		
		var time=0.0;
		if(p.getRaceTopTimeToLong(1)!=-1){
			 time=(float)toptime*hard;//任務目標時間
			 
			time=time/1000;
			time=Math.Round(time,3); 
		}else{
			 time=999999;
		}
		var msx=(1.6-hard)*10;
		var money=(int)(500*msx);//任務獎勵
		if(hard<1.2)
		{//困難以上的任務獎勵變為碎片掉落 
			
				money=-1;
			
		}
		foreach(Client i in API.getAllPlayers())
		{
			if(API.getEntitySyncedData(i, "SC_Login_Status")==1)
			{//登錄了的玩家
				API.triggerClientEvent(i,"SC_MISSION_NEW",name,time.ToString(),money,hard,p.getRaceRid().ToString());
				//發佈新任務給所有登錄玩家
				//new item_drop().itemDrop(i);
			}
		}
	}
}
 