using System;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class item : Script
{ 	
 
	private Client Player;
	private List<itemInfo> iList=new List<itemInfo>();
 
	public item()
	{
		API.onClientEventTrigger += OnClientEvent;

		API.onEntityDataChange+= OnEntityDataChange; 		
	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录时读取他的物品
			var Player=API.getPlayerFromHandle(entity);
			var p=API.getEntityData(Player,"SC_item");
			var lg=new login();
			p.loadItem();
			API.setEntitySyncedData(Player,"SC_item_seted",1);
			if(lg.GetPlayerAccess(Player,"SC_VEHICLE_EX")=="SC_NULL"){
				
				lg.SetPlayerAccess(Player,"SC_VEHICLE_EX","1");
				if(API.getEntitySyncedData(Player,"SC_register")!=1)
				{
					p.showItemDx("補償禮包","~r~僅限老玩家領取","車輛強化插件"+"\n~y~傳說 ~w~車輛極速+5% 1件\n~b~稀有 ~w~車輛極速+2.5% 2件\n~g~良好 ~w~車輛極速+1% 3件\n~c~普通 ~w~車輛極速+0.5% 4件");
					p.addItem("補償禮包-車輛強化插件","~y~傳說","車輛極速+5%","車輛強化插件:極速","5");
					p.addItem("補償禮包-車輛強化插件","~b~稀有","車輛極速+2.5%","車輛強化插件:極速","2.5");
					p.addItem("補償禮包-車輛強化插件","~b~稀有","車輛極速+2.5%","車輛強化插件:極速","2.5");
					p.addItem("補償禮包-車輛強化插件","~g~良好","車輛極速+1%","車輛強化插件:極速","1");
					p.addItem("補償禮包-車輛強化插件","~g~良好","車輛極速+1%","車輛強化插件:極速","1");
					p.addItem("補償禮包-車輛強化插件","~g~良好","車輛極速+1%","車輛強化插件:極速","1");
					p.addItem("補償禮包-車輛強化插件","~c~普通","車輛極速+0.5%","車輛強化插件:極速","0.5");
					p.addItem("補償禮包-車輛強化插件","~c~普通","車輛極速+0.5%","車輛強化插件:極速","0.5");
					p.addItem("補償禮包-車輛強化插件","~c~普通","車輛極速+0.5%","車輛強化插件:極速","0.5");
					p.addItem("補償禮包-車輛強化插件","~c~普通","車輛極速+0.5%","車輛強化插件:極速","0.5");
				}
			}
			p=null;
		}
	}
	
	public item(Client p)
	{
		Player=p;
		
		API.setEntityData(Player,"SC_item",this);
		
	}
	
 
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_ITEM_USE")
		{
			itemInfo p=null;
			foreach(itemInfo i in API.getEntityData(Player,"SC_item").getItemList())
			{
				if(i.itemID==arguments[0].ToString())
				{
					p=i;
				}
			}
			if(p!=null){
				string[] type=p.itemType.Split(':');
				if(type[0]=="車輛強化插件"||type[0]=="車輛裝飾插件"){
					if(API.isPlayerInAnyVehicle(Player)==true){
						var veh=API.getPlayerVehicle(Player);
						var vn=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
						if(vn.getMaster()==API.getPlayerName(Player))
						{
							vn.addSlotValue(type[1], p.itemValue);
							API.triggerClientEvent(Player, "SC_vehicle_changemod_logined_json",vn.getModJson() ,vn.getSlotJson());
							API.getEntityData(Player,"SC_item").redItem(arguments[0].ToString());
						}else{
							API.sendNotificationToPlayer(Player,"物品:"+p.itemName+"使用~r~失敗\n~w~原因:不是你的車子");
							 
						 						
						}
					}else{
						API.sendNotificationToPlayer(Player,"物品:"+p.itemName+"使用~r~失敗\n~w~原因:這個物品必須在車上使用");
						 
						 
					}
				}
				if(type[0]=="武器"){
					if(type[1]=="禮炮")
					{
						API.removePlayerWeapon(Player,(WeaponHash)2138347493);
						API.givePlayerWeapon(Player,(WeaponHash)2138347493,20,true,true);
						
					}
				}
				if(type[0]=="福袋"){
					API.getEntityData(Player,"SC_item").redItem(arguments[0].ToString());
					new item_drop().itemDrop(Player);
					new item_drop().itemDrop(Player);
					new item_drop().itemDrop(Player);
					new item_drop().itemDrop(Player);
					new item_drop().itemDrop(Player);
					
				}
				if(type[0]=="車輛碎片"){
					var v=p.itemValue.Split(':');
					if(Convert.ToInt32(v[1])==Convert.ToInt32(v[2]))
					{
						API.getEntityData(Player,"SC_item").redItem(arguments[0].ToString());
						if(API.hasEntityData(Player,"SC_TEAM")==true)
						{
							var t=API.getEntityData(Player,"SC_TEAM");
							t.addTeamDynamic(Player.name+"~w~:通過車輛碎片兌換了一輛 ~y~"+p.itemMsg.Split('~')[2]);
							t.updataTeamDynamic();
							t.saveTeam();
						}
						API.sendNotificationToAll("厲害了!"+API.getPlayerName(Player)+"\n通過車輛碎片兌換了一輛~g~"+p.itemMsg.Split('~')[2]);	
						var veh=new VehicleDataFunc(v[0],API.vehicleNameToModel(v[0]),Player.name,"",2650000,Player.position,Player.rotation,0,0,true);
						veh.createVeh();
						veh.pushVeh();
						API.getEntityData(Player,"SC_VehicleList").Add(veh);
						var us=API.getEntityData(Player,"SC_USERINFO");
						us.setUserInfo("SYSCARS",us.getUserInfo("SYSCARS").infoValue+1);
						new VehicleMain().updataPlayerVehicleList(Player);
						 
					}else{
						API.sendNotificationToPlayer(Player,"車輛~r~兌換失敗~w~!碎片數量不足"+v[1]);
					}
				}
				API.getEntityData(Player,"SC_item").sendItemList();
			}
			p=null;
		}			
	}
	
	public List<itemInfo> getItemList()
	{
		return iList;
	}
	
	public void changeItemMaster(string itemid,Client Player,Client Target)
	{
		var p=API.getEntityData(Player,"SC_item");
		var t=API.getEntityData(Target,"SC_item");
		itemInfo z=null;
		foreach(var i in p.getItemList())
		{
			if(i.itemID==itemid)
			{
				z=i;
				break;
			}
		} 
		z.itemLock=false;
		
		p.redItem(itemid);
		t.addItem(z);
	}
	
	public void setItemLock(string itemid,bool f)
	{
		var x=0;
		foreach(var i in iList)
		{
			if(i.itemID==itemid)
			{
				i.itemLock=f;
				x=1;
				break;
			}
		}
		if(x==1){
			sendItemList();
		}
	}
	
	public void addItem(string name,string pz,string msg,string type,string values)
	{//給該玩家添加一個物品
		itemInfo i=new itemInfo(name,pz,msg,type,values,(UnixTime.getUnixTimeToMS()+new Random().Next(1000)).ToString());
		
		
		//更新玩家物品列表,函數待做
		var v=type.Split(':');
		if(v[0]=="車輛碎片")
		{
			itemAdd(i);
		}else{
			iList.Add(i);
		}
		saveItem();
		sendItemList();
		i=null;
	}
	
	public void addItem(itemInfo i)
	{ 
		var v=i.itemType.Split(':');
		if(v[0]=="車輛碎片")
		{
			itemAdd(i);
		}else{
			iList.Add(i);
		}
		saveItem();
		sendItemList();
	}
	
	public void addItem(string name,string pz,string msg,string type,string values,string id)
	{//从数据库中读取物品并添加给玩家。
		itemInfo i=new itemInfo(name,pz,msg,type,values,id);
		var v=type.Split(':');
		if(v[0]=="車輛碎片")
		{
			itemAdd(i);
		}else{
			
			iList.Add(i);
		}
		i=null;
	}
	
	public void itemAdd(itemInfo p)
	{
		itemInfo z=null;
		foreach(var i in iList)
		{
			if(i.itemType==p.itemType)
			{//veh_addtype+":50:1";
				z=i;
			}
		}
		if(z!=null)
		{
			var iV=z.itemValue.Split(':');
			var pV=p.itemValue.Split(':');
			var iNum=Convert.ToInt32(iV[2]);
			var pNum=Convert.ToInt32(pV[2]);
			//API.consoleOutput(iNum.ToString()+"="+pNum.ToString());
			if((iNum+pNum)>=50)
			{
				var s=50-(iNum);
				p.itemValue=pV[0]+":50:"+(pNum-s).ToString();
				z.itemValue=iV[0]+":50:50";
				var mV=z.itemMsg.Split('~')[2];
				z.itemMsg="碎片已集齊,使用該物品即可兌換~g~"+mV;
				mV=p.itemMsg.Split('/')[0];
				p.itemMsg=mV+"/"+(pNum-s).ToString()+")";
				if((pNum-s)!=0){
					iList.Add(p);
				}
			}else{
				z.itemValue=iV[0]+":50:"+(iNum+pNum).ToString();
				var mV=z.itemMsg.Split('~')[2];
				z.itemMsg="集齊碎片來兌換~g~"+mV+"~w~(50/"+(iNum+pNum).ToString()+")";
			}
			z=null;
		}else{
			iList.Add(p);
		}
	}
	
	public void redItem(string id)
	{
		//刪除玩家的一個物品
		itemInfo p=null;
		foreach(itemInfo i in iList)
			{
				if(i.itemID==id)
				{
					p=i;
				}
			}
		if(p!=null){
			iList.Remove(p);
		}
		sendItemList();
		
		saveItem(); 
	}
	
	public void sendItemList()
	{//更新指定玩家的物品列表
				API.triggerClientEvent(Player,"SC_ITEM_LISTCHANGE",API.toJson(iList));
	}
	
	public void saveItem()
	{
			var lg=new login();
			lg.SetPlayerAccess(Player,"SC_ITEM",API.toJson(iList));
			lg=null;
	}
	
	public void loadItem()
	{
			var lg=new login();
			if(lg.GetPlayerAccess(Player,"SC_ITEM")!="SC_NULL")
			{
				var js=lg.GetPlayerAccess(Player,"SC_ITEM"); 
				//對於有數組結構的json字符串，必須使用 JArray.Parse來轉換
				//需引用using Newtonsoft.Json.Linq;
				//GTA-N的API.fromJson暫時不支持帶數組結構的json字符串
				//服務端版本:v0.1.367.419
				JArray jsonVals = JArray.Parse(js); 
				var s="";
				foreach(var i in jsonVals)
				{
					//API.consoleOutput(jsonVals.Count.ToString());
					if(i["itemType"].ToString().Split(':')[0]=="車輛碎片")
					{
						if(i["itemPz"].ToString()=="")
						{
							s="碎片";
						}else{
							s=i["itemPz"].ToString();
						}
					}else{
												s=i["itemPz"].ToString();
					}
					API.getEntityData(Player,"SC_item").addItem(i["itemName"].ToString(),s,i["itemMsg"].ToString(),i["itemType"].ToString(),i["itemValue"].ToString(),i["itemID"].ToString());
	
				} 
				sendItemList();
			}
			lg=null;
	}
	
	public  void showItemDx(string title,string title2,string msg,int r=255,int g=255,int b=255)
	{
				API.triggerClientEvent(Player,"SC_ITEM_DX",title,title2,msg,r,g,b);
				//發佈新任務給所有登錄玩家
	}
}

public class itemInfo
{
	public string itemName="";//物品名稱
	public string itemPz="";//物品品質
	public string itemMsg="";//物品描述
	public string itemType="";//物品類型
	public string itemValue="";//物品屬性
	public string itemID="";//物品唯一ID
	public bool itemLock=false;//物品锁定状态,TRUE为出售中
	
	public itemInfo(string name,string pz,string msg,string type,string values,string id)
	{
		itemName=name;
		itemPz=pz;
		itemMsg=msg;
		itemType=type;
		itemValue=values;
		itemID=id;
		itemLock=false;
	}
}
 