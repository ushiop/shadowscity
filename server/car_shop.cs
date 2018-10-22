using System;

using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class car_shop : Script
{ 	
	private long cd=UnixTime.getUnixTimeToS()+600;
	private List<sell_info> sList=new List<sell_info>();
	
	public car_shop()
	{
		API.onUpdate += OnUpdate;
		API.onClientEventTrigger += OnClientEvent; 
		API.onEntityDataChange+= OnEntityDataChange; 
		createSellList();
	}
	
	private void OnUpdate()
	{
		if(UnixTime.getUnixTimeToS() > cd)
		{//每10分钟更新一次出售列表
			cd=UnixTime.getUnixTimeToS()+600;//+80
			createSellList();
			sendSellList();
		}
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_CAR_SHOP_BUY")
		{
			var m=API.getEntityData(Player,"SC_money");
			m.addMoney(-1*Convert.ToInt32(arguments[2]));
			new item_drop().itemDrop(Player,arguments[0].ToString(),arguments[1].ToString());	
		
		}
	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登陆后传递出售列表
			var Player=API.getPlayerFromHandle(entity);
			sendSellList(Player);
		}
	}
	

	private void createSellList()
	{
		sList.Clear();
		var random=new Random();
		sList.Add(new sell_info("隨機車輛碎片",random.Next(27500,50000),"隨機獲得一個車輛碎片,檢驗血統的時刻到了!","","車輛碎片",""));
		string[] veh_addtypes=new string[]{"FMJ","Prototipo","Adder","Bullet","EntityXF","LE7B","Nero","Nero2","Osiris","Pfister811","Sheava","SultanRS","T20","Tempesta","Tyrus","Italigtb","Italigtb2","Cheetah","Infernus","Penetrator","Reaper","Superd","Turismor","Vacca","Voltic","Zentorno","Voltic2"};
		string[] veh_addmsgs=new string[]{"FMJ","X80","靈蛇","子彈","本質","RE7B","尼羅","尼羅升級版","奧西里斯","811","皇霸天","王者RS","T20","泰皮斯達","泰勒斯","伊塔裏GTB","伊塔裏GTB升級版","獵豹","煉獄膜","摧花辣手","死神","金鑽耀星","披治 R","狂牛","狂雷","桑托勞","火箭狂雷"};
		
		var time=cd-UnixTime.getUnixTimeToS();
		DateTime p=DateTime.Now.AddSeconds(time);
		for(var i=0;i<4;i++)
		{
			var index=random.Next(i,veh_addtypes.Length-1);
			var tmp=veh_addtypes[index];
			var tmp1=veh_addmsgs[index];
			veh_addtypes[index]=veh_addtypes[i];
			veh_addmsgs[index]=veh_addmsgs[i];
			 sList.Add(new sell_info("碎片:"+tmp1,random.Next(55000,80000),"獲得車輛"+tmp1+"的碎片","限時出售至"+p.Hour.ToString()+":"+p.Minute.ToString(),"車輛碎片",tmp));	
		 }
		 
	}
	
	private void sendSellList(Client Player=null)
	{
		if(Player==null)
		{
			API.triggerClientEventForAll("SC_CAR_SHOPLIST",API.toJson(sList));
		}else{
			API.triggerClientEvent(Player,"SC_CAR_SHOPLIST",API.toJson(sList));
		}
	}
}
 
 
 
public class   sell_info
{
	public string name;
	public int money;
	public string msg;
	public string exmsg;
	public string itemtype;
	public string itemaddtype;
	
	public sell_info(){}
	
	public sell_info(string _name,int _money,string _msg,string _exmsg,string _itemtype,string _itemaddtype)
	{
		name=_name;
		money=_money;
		msg=_msg;
		exmsg=_exmsg;
		itemtype=_itemtype;
		itemaddtype=_itemaddtype;
	}
}