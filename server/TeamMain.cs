using System;
using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; 

public class TeamMain : Script
{ 	
	public static DataBase TeamDB;  
	public static List<TeamDataFunc> tList;
	public static long time;
	public static long cd;
	
	public TeamMain()
	{
		API.onUpdate += OnUpdate;
		API.onResourceStart += OnResourceStart;  
		API.onPlayerDisconnected += OnPlayerDisconnected; 
		API.onClientEventTrigger += OnClientEvent;  
		API.onEntityDataChange+= OnEntityDataChange;
	} 
	
	public void OnResourceStart()
	{ 
		TeamDB = new DataBase("Team.db","resources\\shadowscity_db\\");
		 if(TeamDB.isDataBase()==false)
		 {
			API.consoleOutput("车队庫不存在!!創建！！！");
			TeamDB.createDataBase();
			TeamDB.connectOpenToDataBase();
			API.consoleOutput("车队建主表！！！");
			var sql="PRAGMA synchronous = OFF";
			TeamDB.sqlCommand(sql);
			sql="CREATE TABLE Team( tID INTEGER PRIMARY KEY AUTOINCREMENT  ,tTime NOT NULL,tValue NOT NULL); ";
			TeamDB.sqlCommand(sql);
			
		 }else{
			TeamDB.connectOpenToDataBase();
			var sql="PRAGMA synchronous = OFF";
			TeamDB.sqlCommand(sql);
		 }
		 
		API.consoleOutput("车队读取！");
		tList=new List<TeamDataFunc>();
		DataBaseSdon[] rr=TeamDB.sqlCommandReturn("Team","","tID","tValue"); 
		int _tid;
		string _value="";
		foreach(DataBaseSdon v in rr)
		{
			_tid=Convert.ToInt32(v.Get("tID"));
			_value=v.Get("tValue"); 
			TeamDataFunc r=new TeamDataFunc(_tid,_value);
			r.updataTeamRank();
			r.checkPlayerState();
			tList.Add(r); 
		//	API.consoleOutput(r.findTeamPlayer("[Shadows]ushio_p~p~∑").ToString());
		}
		API.consoleOutput("车队读取完毕！");  
		time=UnixTime.getUnixTimeToS()+20;
		cd=UnixTime.getUnixTimeToS()+60;
    }
	
	private void OnUpdate()
	{		 
		if(time!=-1)
		{
			if(UnixTime.getUnixTimeToS()>=time)
			{
				time=-1;
				this.teamRoadBoss();
			}
		}
		if(UnixTime.getUnixTimeToS()>=cd)
		{
			cd=UnixTime.getUnixTimeToS()+60;
			sendTeamList();
		}
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_TEAM_CREATE")
		{
			var name=arguments[0].ToString();
			if(checkTeamName(name)==true)
			{
				var o=new JObject();
				o["tName"]=name;
				o["tMaster"]=Player.name;
				o["tCreateDate"]=DateTime.Now.ToString("yyyy-MM-dd");
				var t=new TeamDataFunc(-1,API.toJson(o));
				t.addTeamPlayer(Player);
				t.addTeamDynamic(Player.name+":創建了車隊.");
				t.setTeamGov("沒有公告");
				t.updataTeamRank();
				t.updataTeamInfo(Player);  
				t.saveTeam();
				addPlayerTeam3DT(Player);
				tList.Add(t);
				API.sendNotificationToPlayer(Player,"車隊:"+name+"~g~創建成功!");
			}else{
				API.sendNotificationToPlayer(Player,"車隊創建失敗\n"+name+"~r~已存在.");
				API.triggerClientEvent(Player,"SC_TEAM_CREATE_ERROR");
			}
		}
		if(eventName=="SC_TEAM_CHANGE_GOV")
		{
			var t=API.getEntityData(Player,"SC_TEAM");
			t.setTeamGov(arguments[0].ToString());
			t.addTeamDynamic(Player.name+"~w~:修改了公告"); 
			t.updataTeamGov();
			t.updataTeamDynamic();
			t.saveTeam();
		}
		if(eventName=="SC_TEAM_KICK_PLAYER")
		{ 
			var t=API.getEntityData(Player,"SC_TEAM");
			t.redTeamPlayer(arguments[0].ToString());
			t.addTeamDynamic(Player.name+"~w~:踢出了成員 "+arguments[0].ToString());
			t.updataTeamPlayerState();
			t.updataTeamDynamic();
			t.updataTeamRank();
			t.updataTeamRankToPlayer();
			t.saveTeam();
			var Target=API.getPlayerFromName(arguments[0].ToString());
			if(Target!=null)
			{
				redPlayerTeam3DT(Target);
			}
		}
		if(eventName=="SC_TEAM_QUIT")
		{
			var t=API.getEntityData(Player,"SC_TEAM");
			if(Player.name==t.getTeamMaster())
			{
				//解散
				t.deleteTeam();
			}else{
				t.redTeamPlayer(Player.name);
				t.addTeamDynamic(Player.name+"~w~:退出了車隊");
				t.updataTeamPlayerState();
				t.updataTeamDynamic();
				t.updataTeamRank();
				t.updataTeamRankToPlayer();
				t.saveTeam();			
				redPlayerTeam3DT(Player);
			}
		}
		if(eventName=="SC_TEAM_YAOQING_GET")
		{
			var p=new List<string>();
			foreach(var i in API.getAllPlayers())
			{
				if(API.hasEntityData(i,"SC_TEAM")==false)
				{
					p.Add(i.name);
				}
			}
			API.triggerClientEvent(Player,"SC_TEAM_YAOQING_PLAYER",API.toJson(p));
		}
		if(eventName=="SC_TEAM_YAOQING_SEND")
		{
			var name=arguments[0].ToString();
			var Target=API.getPlayerFromName(name);
			if(Target==null)
			{
				API.sendNotificationToPlayer(Player,"車隊邀請發送失敗,原因:"+name+"~w~未登錄");
			}else{
				if(API.hasEntityData(Target,"SC_TEAM")==true)
				{
					API.sendNotificationToPlayer(Player,"車隊邀請發送失敗,原因:"+name+"~w~已加入其他車隊");
				}else{
					API.triggerClientEvent(Target,"SC_TEAM_YAOQING_SENDGUI",API.getEntityData(Player,"SC_TEAM").getTeamName());
				}
			}
		}
		if(eventName=="SC_TEAM_YAOQING_ADD")
		{
			var tname=arguments[0].ToString();
			TeamDataFunc t=findTeam(tname);
			if(t!=null)
			{ 
				t.addTeamPlayer(Player);
				t.addTeamDynamic(Player.name+"~w~:加入了車隊."); 
				t.updataTeamInfo(Player); 
				t.updataTeamRank();
				t.updataTeamRankToPlayer(); 
				t.updataTeamPlayerState();
				t.updataTeamDynamic();
				t.saveTeam();
				addPlayerTeam3DT(Player);
			}else{
			
				API.sendNotificationToPlayer(Player,"車隊加入失敗,原因:"+tname+"~w~車隊不存在");
			}
		}
	}

	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录时尋找他的車隊
			var Player=API.getPlayerFromHandle(entity);
			TeamDataFunc team=null;
			foreach(var i in tList)
			{
				if(i.findTeamPlayer(Player.name)==true)
				{
					team=i;
					break;
				}
			}
			if(team!=null)
			{
				API.setEntityData(Player,"SC_TEAM",team);
				team.updataPlayerState(Player,true);
				//team.addTeamDynamic(Player.name+"~w~:登陸了游戲");
				team.updataTeamInfo(Player);  
				addPlayerTeam3DT(Player);
			}
		}
	}
 
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		if(API.hasEntityData(Player,"SC_TEAM")==true)
		{ 
			redPlayerTeam3DT(Player);
			var team=API.getEntityData(Player,"SC_TEAM");
			team.updataPlayerState(Player,false);  
			team.updataTeamPlayerState();
		}
	}
	
	private void addPlayerTeam3DT(Client Player)
	{//为指定玩家增加头顶车队名
		if(Player.hasData("SC_TEAM")==true)
		{
			var team=Player.getData("SC_TEAM");
			var text=API.createTextLabel(team.getTeamName(),new Vector3(),50f,1f);
			Player.setData("SC_TEAM_NAME",text);
			API.attachEntityToEntity(text,Player,"0",new Vector3(0,0,1.5f),new Vector3());
			API.triggerClientEvent(Player,"SC_TEAM_NAME_LOCAL_HIDE",text);
		}
	}
	
	public void redPlayerTeam3DT(Client Player)
	{//为玩家删除头顶名
		if(Player.hasData("SC_TEAM_NAME")==true)
		{
			Player.getData("SC_TEAM_NAME").delete();
		}
	}
	
	private void sendTeamList()
	{
		var l=new List<TeamInfo>();
		foreach(var i in tList)
		{
			l.Add(new TeamInfo(i.getTeamName(),i.getTeamBoss().ToString(),i.getTeamRank().ToString()));
		}
		API.triggerClientEventForAll("SC_TEAM_LIST",API.toJson(l));
	}
	
	private bool checkTeamName(string name)
	{
		var b=true;
		foreach(var i in tList)
		{
			if(i.getTeamName()==name)
			{
				b=false;
				return b;
			}
		}
		return b;
	}
	
	public static TeamDataFunc findTeam(string name)
	{
		TeamDataFunc p=null;
		foreach(var i in tList)
		{
			if(i.getTeamName()==name)
			{
				p=i;
				return p;
			}
		}
		return p;
	}
	
	public void teamRoadBoss()
	{//計算所有車隊的賽事統治率
		var p=new JObject();
		var roads=0;
		foreach(RaceRoad v in track.raceRoad)
		{
			var r=v.getRaceTopInTop(1);
			if(r!=null)
			{
				if(r.team!="")
				{
					p[r.team]=p[r.team]==null?1:Convert.ToInt32(p[r.team])+1;
				}
			}
			roads=roads+1;
		}
		foreach(var v in p)
		{
			string s1=v.ToString().Replace("[","");
			s1=s1.Replace("]","");
			string[] s=s1.Split(',');
			// s0 teamname
			// s1 team roads
			var t=TeamMain.findTeam(s[0]);
			var tops=Convert.ToInt32(s[1]);
			float acc=(float)tops/roads;
			if(t!=null)
			{ 
				t.setTeamTops(tops,Convert.ToInt32(acc*100)); 
			}
			 
		}
	}
}

public class TeamInfo{
	public string name;
	public string boss;
	public string rank;
	public TeamInfo(){}
	public TeamInfo(string _name,string _boss,string _rank)
	{
		name=_name;
		boss=_boss;
		rank=_rank;
	}
}