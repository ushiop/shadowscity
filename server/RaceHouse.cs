using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared; 
 

public class RaceHouse : Script
{ 	
	private string rHouseName="";//該比賽大廳的名字
	private bool rStart=false;//該比賽大廳的比賽狀態
	private Client rCreate;//該比賽大廳的創建者
	private List<Client> rPlayer=new List<Client>();//該比賽大廳內的玩家
	private RaceRoad race;//該比賽大廳的賽道
	private int hId;//該賽事大廳的唯一ID
	public long Count=-1;//倒計時
	public long lastMessage=-1;//最後一次公告時間 
	public long startTick;//啟動比賽時間
	private List<string> rankList=new List<string>();//结果列表
	private int startRacePlayer=0;
	
	
	public RaceHouse()
	{
		API.onUpdate += OnUpdate;
	} 
	
	public RaceHouse(string hName,Client hCreate,RaceRoad r)
	{//帶參構造
		rHouseName=hName;
		rCreate=hCreate;
		race=r;
		hId=System.Environment.TickCount+new Random().Next(0,1000);
	}	
	
	private void OnUpdate(){
		foreach(var i in Race.House)
		{
			if(i.Count!=-1)
			{
				if(i.getRaceHouseState()==true){
					
						if(UnixTime.getUnixTimeToS()>=i.Count)
						{//時間到  開始比賽
							i.sendMessageToPlayer("!!!~r~比賽開始~w~!!!");
							i.freezePlayer(false);
							i.startTick=UnixTime.getUnixTimeToMS(); 
							i.repairAllVehicle();
							foreach(var v in i.rPlayer)
							{
								
								API.setEntitySyncedData(v,"SC_race_house_start_state",1);
								API.triggerClientEvent(v,"SC_race_house_startrace",i.startTick.ToString(),i.getRaceRoad().getRaceName()); 
							}		
							i.Count=-1;
						}else{
		//API.consoleOutput(UnixTime.getUnixTime().ToString()+"="+i.lastMessage.ToString());
							if(UnixTime.getUnixTimeToS()>=i.lastMessage)
							{
								i.lastMessage=UnixTime.getUnixTimeToS()+1;
								i.sendMessageToPlayer("比賽將在~r~"+(i.Count-UnixTime.getUnixTimeToS()).ToString()+"~w~秒後開始..");
							}
						} 
				}
			}
		}
	}
	

	
	public void setRaceHouseState(bool v)
	{//設置比賽狀態
		rStart=v;
	}
	
	public bool getRaceHouseState()
	{//獲取比賽狀態
		return rStart;
	}
	
	public long getRaceHouseStartTick()
	{//獲取比賽開始時間
		return startTick;
	}
	
	public int getRaceHouseId()
	{//返回該賽事大廳的唯一ID
		return hId;
	}
	
	public string getRaceHouseName()
	{//返回該賽事大廳的名字
		return rHouseName;
	}
	
	public Client getRaceMaster()
	{//獲取該賽事大廳當前主人
		return rCreate;
	}
	
	public Client[] getRaceAllPlayer()
	{//返回該賽事大廳的所有玩家
		return rPlayer.ToArray();
	}
	
	public string[] getRaceAllPlayerName()
	{//返回該賽事大廳的所有玩家名字
		var d=getRaceAllPlayer();
		List<string> str=new List<string>();
		foreach(var i in getRaceAllPlayer())
		{
			str.Add(API.getPlayerName(i));
		};
		return str.ToArray();
	}
	
	public string[] getRaceHouseRankList()
	{//返回该赛事大厅的结果列表
		return rankList.ToArray();
	}
	
	public int finishRace(Client Player,long time,string showtime)
	{//某人完成比賽
		var wins=rankList.Count;
		var str="~r~#"+(wins+1).ToString()+" ~g~"+API.getPlayerName(Player)+" ~y~"+showtime.ToString()+"s";
		rankList.Add(str);
		updataPlayerHouseRankListInfo();
		//API.consoleOutput(API.getEntitySyncedData(API.getPlayerVehicle(Player),"SC_VehicleMain_VehicleData_Rank")+"?"+API.getEntitySyncedData(API.getPlayerVehicle(Player),"SC_VehicleMain_VehicleData_SQLID").ToString());
		var tmp=race.setTimeInTopTime(API.hasEntityData(Player,"SC_TEAM")==true?API.getEntityData(Player,"SC_TEAM").getTeamName():"",time,Player.name,API.getEntityModel(API.getPlayerVehicle(Player)).ToString(),API.getEntitySyncedData(API.getPlayerVehicle(Player),"SC_VehicleMain_VehicleData_Rank"),API.getEntityData(Player,"SC_USERINFO").getUserInfo("PORTRAIT").infoValue);
		if(tmp==true)
		{
			race.sortTopTime();
			race.saveTopTime();
			updataPlayerHouseAllTopList(Player);
		}
		return wins+1;
	}
	
	public RaceRoad getRaceRoad()
	{//返回該賽事大廳的賽道
		return race;
	}
	
	public void addPlayer(Client Player)
	{//讓某玩家加進該房間
		rPlayer.Add(Player);
		updataPlayerHouseAllInfo(Player);
		updataPlayerHouseRaceCP(Player);  
		updataPlayerHousePlayerListInfo(Player);
		updataPlayerHouseAllTopList(Player);
		if(API.isPlayerInAnyVehicle(Player)==false)
			{
				API.setEntityPosition(Player,new Vector3(race.getAllCheckPoint()[0].X,race.getAllCheckPoint()[0].Y,race.getAllCheckPoint()[0].Z+10));

			}else{
				var seat=API.getPlayerVehicleSeat(Player);
				if(seat!=-1)
				{
					API.setEntityPosition(Player,new Vector3(race.getAllCheckPoint()[0].X,race.getAllCheckPoint()[0].Y,race.getAllCheckPoint()[0].Z+10));

				}else{
					var veh=API.getPlayerVehicle(Player);
					API.setEntityPosition(veh,new Vector3(race.getAllCheckPoint()[0].X,race.getAllCheckPoint()[0].Y,race.getAllCheckPoint()[0].Z+10));

				}
			}
		API.triggerClientEvent(Player,"SC_race_house_joinrace",race.getRaceName());
		API.setEntitySyncedData(Player,"SC_race_house_road",race.getRaceName());
		//玩家進入賽事房間時觸發事件,參數為賽道名
		sendMessageToPlayer("歡迎 ~g~"+API.getPlayerName(Player)+"~w~ 來到比賽房間~~!");
	}
	
	public Client findRaceHouseMaster()
	{//尋找一個新的房主
		return rPlayer[0];
	}
	
	public int getRaceHouseRacePlayers()
	{//返回比賽房間內的比賽人數
		return startRacePlayer;
	}
	
	public int getRaceHousePlayers()
	{//返回當前房間內人數
		return rPlayer.Count;
	}
	

	
	public void removePlayer(Client Player)
	{//某玩家退出該房間
		API.resetEntitySyncedData(Player,"SC_race_house_start_state");
		API.resetEntitySyncedData(Player,"SC_race_house_road");
		if(rStart==true){
			API.freezePlayer(Player, false);
		}
		rPlayer.Remove(Player);

			if(Player==rCreate)
			{ 	
				sendMessageToPlayer("哎呀!房主退出了,搜索新的房主!");
				if(rPlayer.Count!=0)
				{ 
					rCreate=findRaceHouseMaster();
					
					sendMessageToPlayer("新的房主是:"+API.getPlayerName(rCreate));
					updataAllPlayerHouseMasterInfo();
				}else{
					API.consoleOutput("沒有人了。。！！");
				}
			}
		updataAllPlayerHousePlayerListInfo();
		
		API.triggerClientEvent(Player,"SC_race_house_quitrace",race.getRaceName());
		//玩家離開賽事房間時觸發事件,參數為賽道名
	}
	
	public void closeRaceHouse()
	{//關閉房間 
		foreach(var i in rPlayer)
		{
			API.triggerClientEvent(i,"SC_race_houseinfo_close","close");		
		}
	 
	}
	
	public void repairAllVehicle()
	{//修復所有該房玩家的汽車
		foreach(var i in rPlayer)
		{
			if(API.isPlayerInAnyVehicle(i)==true)
			{
				API.repairVehicle(API.getPlayerVehicle(i));
			}
		}		
	}
	
	public void startRace()
	{//開始比賽!
		List<Client> g =new List<Client>();
		foreach(var i in rPlayer)
		{
			if(API.isPlayerInAnyVehicle(i)==false)
			{
				g.Add(i);
			}else{
				var seat=API.getPlayerVehicleSeat(i);
				if(seat!=-1)
				{
					g.Add(i);
				}
			}
		}
		sendMessageToPlayer("比賽將在~r~5~w~秒後開始..");
		foreach(var i in g)
		{	
			sendMessageToPlayer( "~g~"+API.getPlayerName(i)+"~w~ 被踢出了比賽,原因:~r~不在車裏/不是駕駛");
			API.triggerClientEvent(i,"SC_race_houseinfo_close","close");
		}
		startRacePlayer=getRaceHousePlayers();
		Count=UnixTime.getUnixTimeToS()+5;
		lastMessage=Count-4;
		rStart=true;
		freezePlayer(true);
	}
	
	public void freezePlayer(bool f)
	{//設置房內所有玩家的凍結狀態
		foreach(var i in rPlayer)
		{
			API.freezePlayer(i, f);
		}
	}
	 
 
	
	
	public void updataAllPlayerHouseMasterInfo()
	{//更新該賽事大廳內的所有玩家關於該賽事大廳主人的信息
		foreach(var i in rPlayer)
		{
			API.triggerClientEvent(i,"SC_race_houseinfo_housemaster_updata",API.getPlayerName(rCreate));
		}				
	}
	
	public void updataAllPlayerHousePlayerListInfo()
	{//更新該賽事大廳所有玩家關於該賽事大廳玩家列表的信息
		foreach(var i in rPlayer)
		{
			API.triggerClientEvent(i,"SC_race_houseinfo_playerlist_updata",API.toJson(getRaceAllPlayerName()));
		}			
	}
	
	public void updataPlayerHousePlayerListInfo(Client Player)
	{//更新該賽事大廳所有玩家中除了Player以外的玩家關於該賽事大廳玩家列表的信息
		foreach(var i in rPlayer)
		{
			if(i!=Player){
				
				API.triggerClientEvent(i,"SC_race_houseinfo_playerlist_updata",API.toJson(getRaceAllPlayerName()));
			}
		}			
	}
	
	public void updataPlayerHouseRankListInfo()
	{//更新該賽事大廳所有玩家的排名列表
		foreach(var i in rPlayer)
		{
			 
				API.triggerClientEvent(i,"SC_race_houseinfo_ranklist_updata",API.toJson(getRaceHouseRankList()));
			 
		}			
	}
	
	public void updataPlayerHouseAllInfo(Client Player)
	{//更新指定玩家該賽事大廳的所有信息
		API.triggerClientEvent(Player,"SC_race_houseinfo_updata",rHouseName,hId,API.getPlayerName(rCreate),API.toJson(getRaceAllPlayerName()),race.getRaceName(),race.getRaceTopPlayer(1),race.getRaceTopTime(1));		
			
	}
	
	
	public void updataPlayerHouseAllTopList(Client Target=null)
	{//更新赛事大厅的记录表
		if(Target==null)
		{
			foreach(var i in rPlayer)
			{
				API.triggerClientEvent(i,"SC_race_houseinfo_toplist",race.getRaceName(),API.toJson(race.getRoadTopList()));
			}		
		}else{
			API.triggerClientEvent(Target,"SC_race_houseinfo_toplist",race.getRaceName(),API.toJson(race.getRoadTopList()));
		}
	}
	
	public void updataPlayerHouseRaceCP(Client Player)
	{//更新指定玩家該賽事大廳的檢查點信息
		RaceCheckPoint[] d= race.getAllCheckPoint() ;
		RaceNextCheckPoint[] nc=new RaceNextCheckPoint[d.Length];
		var max=d.Length-1;
		var idx=0;
		foreach(var i in d)
		{
			if(idx==max){
				nc[idx]=new RaceNextCheckPoint(d[idx].raceId,new Vector3(d[idx].X,d[idx].Y,d[idx].Z),null,d[idx].dbId);
			}else{
				nc[idx]=new RaceNextCheckPoint(d[idx].raceId,new Vector3(d[idx].X,d[idx].Y,d[idx].Z),new Vector3(d[idx+1].X,d[idx+1].Y,d[idx+1].Z),d[idx].dbId);
			}
			idx++;
		}
		API.triggerClientEvent(Player,"SC_race_houseinfo_CP_updata",API.toJson(nc));		
		
	}
	
	public void sendMessageToPlayer(string str)
	{//向房間內的所有玩家發送消息 
		foreach(var i in rPlayer)
		{
			API.sendChatMessageToPlayer(i,"賽事大廳",str); 
		}
	}
	
}