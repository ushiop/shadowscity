using System;
using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
 

public class TeamDataFunc : Script
{ 	 
	private TeamData team;
	
	public TeamDataFunc(){} 
	
	public TeamDataFunc(int _tSQLID,string _tValue)
	{
		var js=API.fromJson(_tValue);  
		team=new TeamData(_tSQLID,js);
	}
	
	public int getTeamRank()
	{//返回車隊評分
		var score=0;
		foreach(var i in team.tList)
		{
			score=score+i.rank;
		}
		return score;
	}
	
	public void updataPlayerRank(Client Player)
	{//更新指定玩家記錄在車隊内的評分
		foreach(var i in team.tList)
		{
			if(i.name==Player.name)
			{
				i.rank=API.getEntitySyncedData(Player,"SC_USERINFO:RACERANK");
				break;
			}
		}
	}
	
	public void updataPlayerState(Client Player,bool f)
	{
		foreach(var i in team.tList)
		{
			if(i.name==Player.name)
			{
				i.login=f;
				break;
			}
		}
	}
	
	public void checkPlayerState()
	{
		
		foreach(var i in team.tList)
		{
				var p=API.getPlayerFromName(i.name);
				if(p==null)
				{
					i.login=false;
				}else{
					i.login=true;
				}
				
		}
	}
	
	public void setTeamTops(int tops,int boss)
	{
		team.tRoadTops=tops;
		team.tRoadBoss=boss; 
		team.tBuff=boss;
		updataTeamBoss();
	}
	
	public int getTeamBoss()
	{//
		return team.tRoadBoss;
	}
	
	public void setTeamGov(string gov)
	{//設置 車隊公告
		team.tGovmsg=gov;
	}
	
	public void updataTeamRank()
	{//更新車隊評分
		team.tRank=getTeamRank(); 
	}
	
	public string getTeamName()
	{//返回車隊名字
		return team.tName;
	}
	
	public string getTeamMaster()
	{//返回車隊隊長
		return team.tMaster;
	}
	
	public void addTeamPlayer(Client Player)
	{//添加某玩家進該車隊
		team.tList.Add(new TeamPlayerData(Player.name, API.getEntitySyncedData(Player,"SC_USERINFO:RACERANK"),true));
		API.setEntityData(Player,"SC_TEAM",this);
	}
	
	public void redTeamPlayer(string name)
	{//踢出某成員
		var Player=API.getPlayerFromName(name);
		TeamPlayerData p=null;
		foreach(var i in team.tList)
		{
			if(i.name==name)
			{
				p=i;
				break;
			}
		}
		if(p!=null)
		{
			team.tList.Remove(p);
		}
		if(Player!=null)
		{
			API.resetEntityData(Player,"SC_TEAM");
			API.triggerClientEvent(Player,"SC_TEAM_KICK");
		}
	}
	
	public bool findTeamPlayer(string Name)
	{//該車隊是否存在該成員
		var b=false;
		foreach(var i in team.tList)
		{
			if(i.name==Name)
			{
				b=true;
				return b;
			}
		}
		return b;
	}
	
	public void addTeamDynamic(string msg)
	{//給車隊添加一個新動態
		team.tDylan.Add(new TeamDynamic(msg,DateTime.Now.ToString()));
	}
	
	public void updataTeamRankToPlayer(Client Target=null)
	{
		if(Target==null)
		{
			var p=getTeamAllPlayers();
			foreach(var i in p)
			{
				API.triggerClientEvent(i, "SC_TEAM_RANK_UPDATA",team.tRank);
			}
		}else{
			API.triggerClientEvent(Target, "SC_TEAM_RANK_UPDATA",team.tRank);
		}
	}
	
	public void updataTeamBoss(Client Target=null)
	{
		if(Target==null)
		{
			var p=getTeamAllPlayers();
			foreach(var i in p)
			{
				API.triggerClientEvent(i, "SC_TEAM_BOSS_UPDATA",team.tRoadBoss,team.tRoadTops,team.tBuff);
			}
		}else{
			API.triggerClientEvent(Target, "SC_TEAM_BOSS_UPDATA",team.tRoadBoss,team.tRoadTops,team.tBuff);
		}
	}
	
	public void updataTeamGov()
	{
		
			var p=getTeamAllPlayers();
			foreach(var i in p)
			{
				API.triggerClientEvent(i, "SC_TEAM_GOV_UPDATA",team.tGovmsg);
			}
	}
	
	public void updataTeamPlayerState()
	{
		
			var p=getTeamAllPlayers();
			foreach(var i in p)
			{
				API.triggerClientEvent(i, "SC_TEAM_PLAYERSTATE_UPDATA",API.toJson(team.tList));
			}
	}
	
	
	
	public void updataTeamInfo(Client Target)
	{//給指定玩家更新車隊所有信息
		API.triggerClientEvent(Target, "SC_TEAM_UPDATA",API.toJson(team));
	}
	
	public void updataTeamDynamic(Client Target=null)
	{//更新車隊所有在綫玩家的車隊動態信息
		if(Target==null)
		{
			var p=getTeamAllPlayers();
			foreach(var i in p)
			{
				API.triggerClientEvent(i, "SC_TEAM_DYNAMIC_UPDATA",API.toJson(team.tDylan));
			}
		}else{
			API.triggerClientEvent(Target, "SC_TEAM_DYNAMIC_UPDATA",API.toJson(team.tDylan));
		}
	}
	
 	
	
	public List<Client> getTeamAllPlayers()
	{//獲取車隊所有在綫玩家
		var p=new List<Client>(); 
		foreach(var i in team.tList)
		{
			var l=API.getPlayerFromName(i.name);
			if(l!=null)
			{
				p.Add(l);
			}
		}
		return p;
	}
	
	public void saveTeam()
	{//将车队写入数据库
		var sql="";
		if(team.tSQLID==-1)
		{//新增
			var t=UnixTime.getUnixTimeToMS().ToString();
			sql="INSERT INTO Team (tTime,tValue) VALUES ('"+t+"','"+API.toJson(team)+"')";
			var _tid=0;
			TeamMain.TeamDB.sqlCommand(sql);
			DataBaseSdon[] rr=TeamMain.TeamDB.sqlCommandReturn("Team","tTime='"+t+"'","tID"); 
			_tid=Convert.ToInt32(rr[0].Get("tID"));
			rr=null;
			team.tSQLID=_tid;
			saveTeam();
		}else{
		 //更新
			//API.consoleOutput(API.toJson(this));
			sql="UPDATE Team SET tValue = '"+API.toJson(team)+"' WHERE tID = "+team.tSQLID+";";
			TeamMain.TeamDB.sqlCommand(sql);
		}
	}
	
	public void deleteTeam()
	{//刪除車隊
		var p=getTeamAllPlayers();
		foreach(var i in p)
		{
			new TeamMain().redPlayerTeam3DT(i);
			API.resetEntityData(i,"SC_TEAM");
			API.triggerClientEvent(i,"SC_TEAM_KICK");
		}
		team.tList.Clear();
		team.tMaster="出售";
		addTeamDynamic("車隊解散");
		saveTeam();
	}
}

public class TeamData
{   
	public int tSQLID;//车队数据库ID
	public string tName;//车队名字 
	public string tMaster;//车队队长
	public string tCreateDate;//车队成立时间
	public string tGovmsg;//车队公告
	public List<TeamPlayerData> tList;//車隊成員列表
	public List<TeamDynamic> tDylan;//車隊動態列表
	/* 
		上述屬性均爲數據庫讀取而來 
		下述屬性均爲服務端計算而來
	*/
	public int tRank=0;//車隊評分
	public int tRoadBoss=0;//車隊賽道統治率
	public int tRoadTops=0;//車隊當前的所有TOP1數量
	public int tBuff=0;//車隊增益狀態率
	
	public TeamData(){}
	
	public TeamData(int _tSQLID,JObject js)
	{
		
		tSQLID=_tSQLID;
		tName=js["tName"].ToString();
		tMaster=js["tMaster"].ToString();
		tCreateDate=js["tCreateDate"].ToString();
		
		if(js.Property("tGovmsg")!=null)
		{
			tGovmsg=js["tGovmsg"].ToString();
		}	
		//對於有數組結構的json字符串，必須使用 JArray.Parse來轉換
		//需引用using Newtonsoft.Json.Linq;
		//GTA-N的API.fromJson暫時不支持帶數組結構的json字符串
		//服務端版本:v0.1.367.419
		tList=new List<TeamPlayerData>();
		if(js.Property("tList")!=null)
		{
			JArray jsonVals = JArray.Parse(js["tList"].ToString());
			foreach(var i in jsonVals)
			{ 
				tList.Add(new TeamPlayerData(i["name"].ToString(),Convert.ToInt32(i["rank"]),false));
			}
		}
		
		tDylan=new List<TeamDynamic>();
		if(js.Property("tDylan")!=null)
		{
			JArray jsonVals = JArray.Parse(js["tDylan"].ToString());
			foreach(var i in jsonVals)
			{ 
				tDylan.Add(new TeamDynamic(i["msg"].ToString(),i["date"].ToString()));
			}
		}		
	}
}

public class TeamPlayerData
{
	public string name;//車隊成員名字
	public int rank;//車隊成員rank分數
	public bool login;//登陸狀態，true為登陸
		
	public TeamPlayerData(){}
	
	public TeamPlayerData(string _name,int _rank,bool _login)
	{
		name=_name;
		rank=_rank;
		login=_login;
	}
}

public class TeamDynamic
{
	public string msg;//車隊動態内容
	public string date;//車隊動態時間
		
	public TeamDynamic(){}
	
	public TeamDynamic(string _msg,string _date)
	{
		msg=_msg;
		date=_date;
	}
}

