using System;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class pay : Script
{ 	
	private List<sellInfo> sellList=new List<sellInfo>();
 
	public pay()
	{
		API.onClientEventTrigger += OnClientEvent;
 
		API.onPlayerFinishedDownload += OnPlayerFinshedDownload;
		API.onPlayerConnected += OnPlayerConnected;
		API.onPlayerDisconnected += OnPlayerDisconnected;		
	}
	
	private void OnPlayerFinshedDownload(Client Player)
	{
		sendSellList(Player);
	}
	
	private void OnPlayerConnected(Client Player)
	{ 
		API.setEntityData(Player,"SC_PAY_PLAYER_SELLLIST",new List<sellInfo>());
		
	}
	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		//API.triggerClientEventForAll("SC_plist",PlayerListToJson(API.getPlayerName(Player)));
		quitSell(Player);
	}
 
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_PAY_SELL_GET_ITEMLIST")
		{
			//發送玩家的物品列表
			List<itemInfo> p=new List<itemInfo>();
			var l=API.getEntityData(Player,"SC_item").getItemList();
			foreach(var i in l)
			{
				if(i.itemLock==false)
				{
					p.Add(i);
				}
			}
			API.triggerClientEvent(Player,"SC_PAY_SELL_SEND_ITEMLIST",API.toJson(p));
		}
		if(eventName=="SC_PAY_CREATE_SELL")
		{// 创建出售清单
			var name=arguments[0].ToString();
			var money=Convert.ToInt32(arguments[1]);
			var js=arguments[2].ToString();
			var pItem=API.getEntityData(Player,"SC_item");
			var pVeh=API.getEntityData(Player,"SC_VehicleList");
			JArray jsonVals = JArray.Parse(js); 
			var sInfo=new sellInfo(Player.name,name,money);
			foreach(var i in jsonVals)
			{ 
				var iInfo=new itemInfo(i["itemName"].ToString(),i["itemPz"].ToString(),i["itemMsg"].ToString(),i["itemType"].ToString(),i["itemValue"].ToString(),i["itemID"].ToString());
				if(i["itemType"].ToString()!="車輛"){
					pItem.setItemLock(i["itemID"].ToString(),true);
				}else{
					foreach(var v in pVeh){
						if(v.getSQLID().ToString()==i["itemID"].ToString()){
							v.setLock(true);
						}
					}
				}
				sInfo.sList.Add(iInfo);
				//API.getEntityData(Player,"SC_item").addItem(i["itemName"].ToString(),i["itemPz"].ToString(),i["itemMsg"].ToString(),i["itemType"].ToString(),i["itemValue"].ToString(),i["itemID"].ToString());
			} 
			API.getEntityData(Player,"SC_PAY_PLAYER_SELLLIST").Add(sInfo);
			sellList.Add(sInfo);
			sendSellList();
		}
		if(eventName=="SC_PAY_SELL_QUIT")
		{
			var list=API.getEntityData(Player,"SC_PAY_PLAYER_SELLLIST");
			var id=arguments[0].ToString();
			sellInfo p=null;
			foreach(var i in list)
			{
				if(i.sellID==id)
				{
					p=i;
					break;
				}
			}
			if(p!=null)
			{
				if(p.sellEd==false)
				{
					quitSell(Player,id);
				}else{
					API.sendNotificationToPlayer(Player,"清單:"+p.sellName+"撤銷~r~失敗\n~w~原因:清單已被購買,正在結算");
				}
			}else{
				API.sendNotificationToPlayer(Player,"清單已失效");

			}
		}
		if(eventName=="SC_PAY_SELL_BUY")
		{
			var id=arguments[0].ToString();
			var money=API.getEntityData(Player,"SC_money");
			sellInfo p=null;
			foreach(var i in sellList)
			{
				if(i.sellID==id)
				{
					p=i;
					break;
				}
			}	
			if(p!=null)
			{
				if(p.sellEd==false)
				{
					var tar=API.getPlayerFromName(p.sellPlayer);
					if(tar!=null)
					{
						if(money.getMoney()>=p.sellMoney){
							p.sellEd=true;
							money.addMoney(-p.sellMoney);
							API.getEntityData(tar,"SC_money").addMoney(p.sellMoney);
							API.sendNotificationToPlayer(tar,"清單:"+p.sellName+" ~g~已出售");
							API.sendNotificationToPlayer(Player,"清單:"+p.sellName+" ~g~已購買");
							var us=API.getEntityData(Player,"SC_USERINFO");
							us.setUserInfo("PAYS",us.getUserInfo("PAYS").infoValue+1);
							us=API.getEntityData(tar,"SC_USERINFO");
							us.setUserInfo("PAYS",us.getUserInfo("PAYS").infoValue+1);
							var js=p.sList;
							var z=API.getEntityData(tar,"SC_item");
							var pv=API.getEntityData(tar,"SC_VehicleList");
							List<VehicleDataFunc> vehl=new List<VehicleDataFunc>();
							foreach(var i in js)
							{
								if(i.itemType!="車輛"){  
									z.changeItemMaster(i.itemID,tar,Player);
								}else{
									foreach(var v in pv){
										if(v.getSQLID().ToString()==i.itemID){
											vehl.Add(v);
										}
									}
								} 
							}
							foreach(var v in vehl)
							{
											v.changeMaster(Player.name,tar,Player);
							}
							sellList.Remove(p);
						}else{
							API.sendNotificationToPlayer(Player,"清單:"+p.sellName+"購買~r~失敗\n~w~原因:金錢不足");
						}
					}else{
						API.sendNotificationToPlayer(Player,"清單:"+p.sellName+"購買~r~失敗\n~w~原因:該清單已失效");
					}
				}else{
					API.sendNotificationToPlayer(Player,"清單:"+p.sellName+"購買~r~失敗\n~w~原因:清單已被購買,正在結算");
				}			
			}else{
				API.sendNotificationToPlayer(Player,"清單:"+p.sellName+"購買~r~失敗\n~w~原因:該清單已失效");
			}
			sendSellList();
		}
		if(eventName=="SC_PAY_GET_LISTCHANGE")
		{
			sendSellList(Player,"手動刷新");
		}
	}
	
	public void quitSell(Client Player,string id=null)
	{
		if(API.hasEntityData(Player,"SC_PAY_PLAYER_SELLLIST")==true)
		{
			var l=API.getEntityData(Player,"SC_PAY_PLAYER_SELLLIST");
			if(id==null)
			{
				foreach(var i in l){ 
					sellList.Remove(i);
				}
				API.resetEntityData(Player,"SC_PAY_PLAYER_SELLLIST");
			}else{
				sellInfo p=null;
				foreach(var i in l){
					if(i.sellID==id){
						p=i;
						break;
					}
				}
				if(p!=null){
					var pItem=API.getEntityData(Player,"SC_item");
					var pVeh=API.getEntityData(Player,"SC_VehicleList");	
					var js=p.sList;
					foreach(var i in js)
					{ 
						if(i.itemType!="車輛"){
							pItem.setItemLock(i.itemID,false);
						}else{
							foreach(var v in pVeh){
								if(v.getSQLID().ToString()==i.itemID){
									v.setLock(false);
								}
							}
						} 
					//API.getEntityData(Player,"SC_item").addItem(i["itemName"].ToString(),i["itemPz"].ToString(),i["itemMsg"].ToString(),i["itemType"].ToString(),i["itemValue"].ToString(),i["itemID"].ToString());
					} 
					l.Remove(p);
					sellList.Remove(p);
				}
			}
			sendSellList();
		}
	}
	
	
	public void sendSellList(Client Player=null,string res="")
	{
		if(Player==null){
			API.triggerClientEventForAll("SC_PAY_LISTCHANGE",API.toJson(sellList.ToArray()),res);
		}else{
			API.triggerClientEvent(Player,"SC_PAY_LISTCHANGE",API.toJson(sellList.ToArray()),res);
		}
	}
	
}

public class sellInfo
{
	public string sellPlayer;//出售者
	public string sellName;
	public int sellMoney;//出售价格
	public bool sellEd=false;//出售状态,FALSE为未出售
	public string sellID;
	public List<itemInfo> sList;//出售物品列表
	
	public sellInfo(string p,string n,int money)
	{
		sellPlayer=p;
		sellName=n;
		sellMoney=money;
		sellEd=false;
		sellID=UnixTime.getUnixTimeToMS().ToString()+new Random().Next(10000).ToString();
		sList=new List<itemInfo>();
		//API.getEntityData(p,"SC_PAY_PLAYER_SELLLIST").Add(this);
	}
}
 