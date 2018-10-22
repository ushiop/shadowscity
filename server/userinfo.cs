using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json.Linq;

public class userinfo : Script
{ 	
 
	private Client player;
	private List<user_info> iList;
	
	public userinfo()
	{
		API.onClientEventTrigger += OnClientEvent; 
		API.onEntityDataChange+= OnEntityDataChange; 
	}
	
	public userinfo(Client Player)
	{
		iList=new List<user_info>();
		player=Player;
		API.setEntityData(player,"SC_USERINFO",this);
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_USERINFO_PLAYER_UPDATA")
		{  
			var us=API.getEntityData(Player,"SC_USERINFO");
			us.setUserInfo(arguments[0].ToString(),Convert.ToInt32(arguments[1]));
			if(arguments[0].ToString()=="PORTRAIT")
			{
				foreach(RaceRoad v in track.raceRoad)
				{
						var p=v.getPlayerInTopTime(Player.name);
						p.playerportrait=Convert.ToInt32(arguments[1]);
						v.saveTopTime();
				}
			}
		}
	}
	
	public List<user_info> getList()
	{
		return iList;
	}
	
	public user_info getUserInfo(string key)
	{//获取玩家的统计数据,如果数据不存在,则返回空
		var list=getList();
		foreach(var p in list)
		{
			if(p.infoName==key)
			{
				return p;
			}
		}
		user_info x=new user_info(key,0); 
		list.Add(x);
		return x;
	}
	
	public void setUserInfo(string Key,int Values)
	{//设置玩家的统计数据,如果数据不存在,则初始化为0
		
			API.setEntitySyncedData(player,"SC_USERINFO:"+Key,Values);
			var list=getList();
			var re=getUserInfo(Key);
			re.infoValue=Values;
			var lg=new login(); 
			lg.SetPlayerAccess(player,"SC_USERINFO",API.toJson(API.getEntityData(player,"SC_USERINFO").getList()));
		
	}

	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录时读取他的统计数据
			var Player=API.getPlayerFromHandle(entity);
			var lg=new login(); 
			var us=API.getEntityData(Player,"SC_USERINFO");
			if(lg.GetPlayerAccess(Player,"SC_USERINFO")=="SC_NULL")
			{//读取玩家统计数据的JSON串
			//没数据的话
				us.setUserInfo("MAXMONEY",0);//累计获得金钱
				us.setUserInfo("USEMONEY",0);//累计消耗金钱
				us.setUserInfo("RACEWINS",0);//累计赛车胜场(单人赛车不计算)
				us.setUserInfo("RACELOSES",0);//累计赛车负场
				us.setUserInfo("RACETOPS",0);//累计获得记录次数
				us.setUserInfo("SYSCARS",0);//累计获得车辆(从系统获得)
				us.setUserInfo("PAYS",0);//累计交易次数
				us.setUserInfo("RACEMISSION",0);//累计赛事任务完成次数
				us.setUserInfo("RACERANK",0);//赛事分
				us.setUserInfo("PORTRAIT",0);//头像
				API.setEntitySyncedData(Player,"SC__USERINFO_OK",1);
			}else{
				var js=lg.GetPlayerAccess(Player,"SC_USERINFO");
				//對於有數組結構的json字符串，必須使用 JArray.Parse來轉換
				//需引用using Newtonsoft.Json.Linq;
				//GTA-N的API.fromJson暫時不支持帶數組結構的json字符串
				//服務端版本:v0.1.367.419
				JArray jsonVals = JArray.Parse(js); 
				foreach(var i in jsonVals)
				{
					us.setUserInfo(i["infoName"].ToString(),Convert.ToInt32(i["infoValue"]));
				}
				API.setEntitySyncedData(Player,"SC__USERINFO_OK",1);
			}
 
			lg=null;
		}
 
	}
}
 
 
public class user_info
{
	public string infoName="";
	public int infoValue=0;
	
	public user_info(){}
	
	public user_info(string name,int values)
	{
		infoName=name;
		infoValue=values;
	}
}