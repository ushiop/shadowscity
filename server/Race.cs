using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

 

public class Race : Script
{ 	
	public static List<RaceHouse> House=new List<RaceHouse>();
	
	public Race()
	{
		API.onClientEventTrigger += OnClientEvent; 
		API.onPlayerFinishedDownload += OnPlayerFinshedDownload;
		API.onPlayerDisconnected += OnPlayerDisconnected; 	
		
	} 
	
	private void OnPlayerFinshedDownload(Client Player)
	{ 
		onRaceHouseUpdataToPlayer(Player,"手動刷新");
	}
 
 	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		if(API.getEntityData(Player,"SC_race_house")!=null){
			var h=API.getEntityData(Player,"SC_race_house");
			h.removePlayer(Player); 
			if(h.getRaceHousePlayers()==0)
			{
				House.Remove(h);	
				onRaceHouseUpdata();
				//API.consoleOutput("guan fanla");
			}
			API.resetEntityData(Player,"SC_race_house");
			API.resetEntitySyncedData(Player,"SC_race_house_state");
		}
	}
	
	[Command("r")]
	public void cmd_createRace(Client Player,int RID)
	{
		RaceRoad p=null;
		foreach(RaceRoad v in track.raceRoad)
		{
			if(v.getRaceRid()==RID)
			{
				p=v;
				break;
			}
		}
		if(p!=null)
		{
			if(API.getEntitySyncedData(Player,"SC_race_house_state")!=1)
			{
				OnClientEvent(Player,"SC_race_createhouse",p.getRaceName());
			}else{
				API.sendNotificationToPlayer(Player,"比賽:"+p.getRaceName()+"\n創建~r~失敗~w~\n~g~原因 ~w~你已經在一個房間裏了");	
			}
		}else{
			API.sendNotificationToPlayer(Player,"比賽創建~r~失敗~w~\n~g~原因 ~w~ 該賽道不存在");	
		}
		p=null;
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_race_house_get_listchange")
		{
			onRaceHouseUpdataToPlayer(Player,"手動刷新");
		}
		if(eventName=="SC_race_createhouse")
		{
			//createhousefunctoin
			var roadname=arguments[0].ToString(); 
			foreach(RaceRoad v in track.raceRoad)
			{
						
				if(v.getRaceName()==roadname)
				{
					if(v.getEditState()==true)
					{
						API.sendNotificationToPlayer(Player,"比賽:"+roadname+"\n創建~r~失敗~w~\n~g~原因 ~w~賽道正在被編輯");
						
						API.triggerClientEvent(Player,"SC_race_house_join_false");
						return;
					}
					if(v.getRaceCheckPointRows()<=3)
					{
						API.sendNotificationToPlayer(Player,"比賽:"+roadname+"\n創建~r~失敗~w~\n~g~原因 ~w~該賽道檢查點數量不足");
						API.triggerClientEvent(Player,"SC_race_house_join_false");
						return;
					}
						
						var time=new Random().Next(0,10000).ToString();
						var h=new RaceHouse("#"+time+" "+roadname,Player,v);
						House.Add(h);
						API.sendChatMessageToAll("賽事大廳",API.getPlayerName(Player)+"~w~ 建立了一場 ~g~"+roadname+"~w~ 的比賽,趕緊加入吧!");
						//傳遞給客戶端賽事大廳信息
						//房間名、房間ID、房間主人名、房間玩家列表名、賽道名
						h.addPlayer(Player);
						onRaceHouseUpdata();//,刷新所有客戶端的賽事大廳列表
						API.setEntityData(Player,"SC_race_house",h); 
						API.setEntitySyncedData(Player,"SC_race_house_state",1);
						h=null; 
						break;
				 
				}
			}
		}
		if(eventName=="SC_race_house_quit")
		{
			var h=API.getEntityData(Player,"SC_race_house");
			h.sendMessageToPlayer("~g~"+API.getPlayerName(Player)+"~w~ 離開了這個房間.");
			h.removePlayer(Player);
			API.resetEntitySyncedData(Player,"SC_race_house_state");
			API.resetEntityData(Player,"SC_race_house");
			if(h.getRaceHousePlayers()==0)
			{//房間關閉
				API.consoleOutput("人退光了，房間關閉");
				House.Remove(h);
				
			}
			onRaceHouseUpdata();
			h=null;
		}
		if(eventName=="SC_race_house_nocar_quit")
		{
			var h=API.getEntityData(Player,"SC_race_house");
			h.sendMessageToPlayer( "~g~"+API.getPlayerName(Player)+"~w~ 被踢出了比賽,原因:~r~不在車裏/不是駕駛");
			API.triggerClientEvent(Player,"SC_race_houseinfo_close","close");
			h=null;
		}
		if(eventName=="SC_race_house_start")
		{
			//race start!
			var h=API.getEntityData(Player,"SC_race_house");
			h.startRace();
		}
		if(eventName=="SC_race_house_join_house")
		{
			RaceHouse h=null;
			var hid=Convert.ToInt32( arguments[0]);
			var c=-1;
			foreach(RaceHouse v in House)
			{
				if(v.getRaceHouseId()==hid)
				{
					h=v;
					c=1;
				}
			}
			if(c==-1){
				API.sendNotificationToPlayer(Player,"房間加入~r~失敗~w~\n~g~原因 ~w~房間不存在\n請重新打開賽事大廳刷新列表");
				return;
			}
			if(h.getRaceHouseState()==true){
				API.sendNotificationToPlayer(Player,"房間:"+h.getRaceHouseName()+"\n加入~r~失敗~w~\n~g~原因 ~w~ 該房比賽已經開始了!");
				return ;
			}
			if(API.getEntitySyncedData(Player,"SC_race_house_state")==1){
				API.sendNotificationToPlayer(Player,"房間:"+h.getRaceHouseName()+"\n加入~r~失敗~w~\n~g~原因 ~w~你已經在一個房間裏了");
				return ;
			}
			h.addPlayer(Player);
			API.setEntityData(Player,"SC_race_house",h); 
			API.setEntitySyncedData(Player,"SC_race_house_state",1);
			h=null;
			
		}
		if(eventName=="SC_race_house_finisRace")
		{ 
			var h=API.getEntityData(Player,"SC_race_house");
			var time=(UnixTime.getUnixTimeToMS()-h.getRaceHouseStartTick());
			var nT=(float)time/1000;
			var vT=nT.ToString();
			var win=h.finishRace(Player,time,vT);
			var b=h.getRaceRoad().getTimeInTopTime(time);
			var scb=0;
			var maxTime=(h.getRaceRoad().getRaceTopTimeToLong(1)/1000)*1.25; 
			var topjl=0;
			var drop=new Random().Next(100);
			API.triggerClientEvent(Player,"SC_race_house_finishrace_mission",h.getRaceRoad().getRaceName(),vT);
			if(b==true){
				var us=API.getEntityData(Player,"SC_USERINFO");
				us.setUserInfo("RACERANK",us.getUserInfo("RACERANK").infoValue+50); 
				us.setUserInfo("RACETOPS",us.getUserInfo("RACETOPS").infoValue+1); 
				h.sendMessageToPlayer("~g~"+API.getPlayerName(Player)+"~w~ 完成 ~g~"+h.getRaceRoad().getRaceName()+" ~w~獲得第 ~y~"+win.ToString()+" ~w~名");	
				API.sendNotificationToAll("~y~"+h.getRaceRoad().getRaceName()+" ~g~記錄刷新!~y~\n"+API.getPlayerName(Player)+"("+vT+"s)" ,true);
 
				new TeamMain().teamRoadBoss();
				if(API.hasEntityData(Player,"SC_TEAM")==true)
				{
					var t=API.getEntityData(Player,"SC_TEAM");
					t.addTeamDynamic(Player.name+"~w~:以 ~y~"+vT+"s 的成績刷新了 ~y~"+h.getRaceRoad().getRaceName()+" .");
					t.updataTeamDynamic();
					t.saveTeam();
				}
				API.triggerClientEventForAll("SC_race_toptimeupdata");  
				topjl=1;
				drop=100;
			}else{
				h.sendMessageToPlayer("~g~"+API.getPlayerName(Player)+"~w~ 完成 ~g~"+h.getRaceRoad().getRaceName()+" ~w~獲得第 ~r~"+win.ToString()+" ~w~名");	
				//API.consoleOutput(vT);
			}  

			if(nT>=60)
			{
				//比賽時間在60秒以上時才能獲得scb;
				var timeAdd=(double)Convert.ToInt64(h.getRaceRoad().getRaceTopTime(1))/time;//基礎SCB獎勵,根據與記錄時間的比例來決定獲得多少
				timeAdd= timeAdd*800;
				var winsS=0.1;//名次SCB獎勵系數
				var winsAdd=0.0;
				if(h.getRaceHouseRacePlayers()>1)
				{
					if(API.hasEntityData(Player,"SC_TEAM")==true)
					{
						var t=API.getEntityData(Player,"SC_TEAM");
						t.addTeamDynamic(Player.name+"~w~:在 ~y~"+h.getRaceRoad().getRaceName()+" 的"+h.getRaceHouseRacePlayers().ToString()+" 人比賽中獲得了 ~y~#"+win.ToString()+"("+vT.ToString()+"s) .");
						t.updataTeamDynamic();
						t.saveTeam();
					}
					var us=API.getEntityData(Player,"SC_USERINFO");
					if(win==1)
					{
						us.setUserInfo("RACEWINS",us.getUserInfo("RACEWINS").infoValue+1);  
					}else{
						us.setUserInfo("RACELOSES",us.getUserInfo("RACELOSES").infoValue+1);   
					}
					var maxSCB=h.getRaceHouseRacePlayers()*1500; 
					if(win==1){ winsS=1;us.setUserInfo("RACERANK",us.getUserInfo("RACERANK").infoValue+40);}
					if(win==2){ winsS=0.7;us.setUserInfo("RACERANK",us.getUserInfo("RACERANK").infoValue-30);}
					if(win==3){ winsS=0.5;us.setUserInfo("RACERANK",us.getUserInfo("RACERANK").infoValue-35);}
					if(win>3){ winsS=0.3;us.setUserInfo("RACERANK",us.getUserInfo("RACERANK").infoValue-38);} 
					winsAdd=winsS*maxSCB;//名次SCB獎勵,與人數和排名有關
				}
				var heal=API.getVehicleHealth(API.getPlayerVehicle(Player))/1000;
				heal=heal*500; 
				if(nT<maxTime||maxTime==-1)
				{
					timeAdd=timeAdd+((nT*8)*winsS);
				}
				
				scb=(int)timeAdd+(int)winsAdd+(int)heal;
				if(topjl==1){
					scb=scb*2;
					API.sendNotificationToPlayer(Player,"~g~獎勵\n時間 ~y~"+Convert.ToInt32(timeAdd).ToString()+"~g~\n名次 ~y~"+Convert.ToInt32(winsAdd).ToString()+"\n~g~技術 ~y~"+Convert.ToInt32(heal).ToString()+"\n~g~記錄 ~y~"+Convert.ToInt32(scb/2).ToString()+"\n~g~合計 ~y~"+scb.ToString() ,false);
				}else{
					API.sendNotificationToPlayer(Player,"~g~獎勵\n時間 ~y~"+Convert.ToInt32(timeAdd).ToString()+"~g~\n名次 ~y~"+Convert.ToInt32(winsAdd).ToString()+"\n~g~技術 ~y~"+Convert.ToInt32(heal).ToString()+"\n~g~合計 ~y~"+scb.ToString() ,false);				
				}
				var m=API.getEntityData(Player,"SC_money");
				m.addMoney(scb);
				m=null; 
				if(drop>=60){
					new item_drop().itemDrop(Player);
				}
			}
			if(API.hasEntityData(Player,"SC_TEAM")==true)
			{	
				var t=API.getEntityData(Player,"SC_TEAM");
				t.updataPlayerRank(Player);
				t.updataTeamRank();
				t.updataTeamRankToPlayer();
				var acc=(float)t.getTeamBoss()/100;
				var mon=Convert.ToInt32(scb*acc);
				API.sendNotificationToPlayer(Player,"~g~車隊獎勵\n~g~金錢 ~y~+"+(acc*100).ToString()+"%\n~g~合計 ~y~"+mon.ToString() ,true);				
				var m=API.getEntityData(Player,"SC_money");
				m.addMoney(mon);
				m=null;
			}
			h=null;
		}
		if(eventName=="SC_race_house_getlist"){
			onRaceHouseUpdata();
		}
	}
	
	public void onRaceHouseUpdata()
	{//刷新所有玩家的賽事大廳列表 
	//創建房間時刷新
	//房間被刪時刷新
	//房主变更时刷新
		List<HouseListData> str=new List<HouseListData>();
		foreach(RaceHouse v in House)
		{
			if(v.getRaceHouseState()==false){
				str.Add(new HouseListData(v.getRaceHouseName(),v.getRaceHouseId() ,API.getPlayerName(v.getRaceMaster()),v.getRaceRoad().getRaceName()));
			}
		} 
		API.triggerClientEventForAll("SC_race_house_listchange",API.toJson(str.ToArray()),"");
	}
	
	public void onRaceHouseUpdataToPlayer(Client Player,string res="")
	{//刷新指定玩家的賽事大廳列表 ,一般用與進服
		List<HouseListData> str=new List<HouseListData>();
		foreach(RaceHouse v in House)
		{
			if(v.getRaceHouseState()==false){
				str.Add(new HouseListData(v.getRaceHouseName(),v.getRaceHouseId() ,API.getPlayerName(v.getRaceMaster()),v.getRaceRoad().getRaceName()));
			}
		} 
		API.triggerClientEvent(Player,"SC_race_house_listchange",API.toJson(str.ToArray()),res);
	}
}

public class HouseListData   
{ 	
	public string Name;
	public int Hid;
	public string Master;
	public string Race;
	
	public HouseListData()
	{ 
	} 
	
	public HouseListData(string hName,int hId,string hMaster,string hRace)
	{
		Name=hName;
		Hid=hId;
		Master=hMaster;
		Race=hRace;
	}
}
