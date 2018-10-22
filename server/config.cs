using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json.Linq;

public class config : Script
{ 	
 

	private Client player;
	private List<config_class> iList;

	
	public config()
	{
		API.onClientEventTrigger += OnClientEvent; 
		API.onEntityDataChange+= OnEntityDataChange; 
	}
	
	public config(Client Player)
	{
		iList=new List<config_class>();
		player=Player;
		API.setEntityData(player,"SC_CONFIG",this);
	}
	
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_CONFIG_PLAYER_UPDATA")
		{  
			var us=API.getEntityData(Player,"SC_CONFIG");
			us.setConfig(arguments[0].ToString(),Convert.ToBoolean(arguments[1]));
		}
	}
	
	public List<config_class> getList()
	{
		return iList;
	}
	
	private config_class getConfig(string key)
	{//获取玩家的设置,如果设置不存在,则返回空
		var list=getList();
		foreach(var p in list)
		{
			if(p.cfgName==key)
			{
				return p;
			}
		}
		config_class x=new config_class(key,true); 
		list.Add(x);
		return x;
	}
	
	private void setConfig(string Key,bool Values)
	{//设置玩家的设置,如果设置不存在,则初始化为true
			API.setEntitySyncedData(player,"SC_CONFIG:"+Key,Values);
			var list=getList();
			var re=getConfig(Key);
			re.cfgValue=Values;
			var lg=new login(); 
			lg.SetPlayerAccess(player,"SC_CONFIG",API.toJson(API.getEntityData(player,"SC_CONFIG").getList()));
	}

	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录时读取他的统计数据
			var Player=API.getPlayerFromHandle(entity);
			var lg=new login();
			var us=API.getEntityData(Player,"SC_CONFIG");
			 
			if(lg.GetPlayerAccess(Player,"SC_CONFIG")=="SC_NULL")
			{//读取玩家设置的JSON串
			//没数据的话
				us.setConfig("PAY_AUTO_LISTUPDATA",true);//是否自动刷新交易大厅
				us.setConfig("RACEHOUSE_AUTO_LISTUPDATA",true);//是否自动刷新赛事大厅
				us.setConfig("SHOW_RACE_MISSION",true);//是否顯示賽事任務
			}else{
				var js=lg.GetPlayerAccess(Player,"SC_CONFIG");
				//對於有數組結構的json字符串，必須使用 JArray.Parse來轉換
				//需引用using Newtonsoft.Json.Linq;
				//GTA-N的API.fromJson暫時不支持帶數組結構的json字符串
				//服務端版本:v0.1.367.419
				JArray jsonVals = JArray.Parse(js); 
				foreach(var i in jsonVals)
				{
					us.setConfig(i["cfgName"].ToString(),Convert.ToBoolean(i["cfgValue"]));
				}
			}
 
			lg=null;
		}
	}
}
 
 
public class config_class
{
	public string cfgName="";
	public bool cfgValue=true;
	
	public config_class(){}
	
	public config_class(string name,bool values)
	{
		cfgName=name;
		cfgValue=values;
	}
}