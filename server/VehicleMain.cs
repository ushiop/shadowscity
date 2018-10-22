using System;
using GTANetworkServer;
using GTANetworkShared;

using System.Collections.Generic;
 

public class VehicleMain : Script
{ 	
	
	public static DataBase VehDB; 
	private long time=-1;
	private int RepairVehicleMoney = 300;
	
	public VehicleMain()
	{
		API.onUpdate += OnUpdate;
		API.onResourceStart += OnResourceStart;
		API.onResourceStop += OnResourceStop; 
		API.onVehicleDeath += OnVehicleDeath;
		API.onPlayerConnected += OnPlayerConnected;
		API.onPlayerDisconnected += OnPlayerDisconnected;
		API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
		API.onClientEventTrigger += OnClientEvent;  
		API.onEntityDataChange+= OnEntityDataChange;
	} 
	
	private void OnUpdate()
	{
		//API.consoleOutput(UnixTime.getUnixTimeToMS().ToString());
		if(time!=-1)
		{
			if(UnixTime.getUnixTimeToS()>time)
			{
				time=UnixTime.getUnixTimeToS()+3600;
				onCreateSellVeh();
			}
		}
		 
	}
	
	private void OnPlayerEnterVehicle(Client player, NetHandle vehicle)
	{//玩家進車時將改裝串傳給玩家
		if(API.hasEntityData(vehicle,"SC_VehicleMain_VehicleData")==false)
		{
			API.consoleOutput("進車錯誤:"+API.getPlayerName(player)+"/Model:"+API.getEntityModel(vehicle).ToString());
			
		}
		var f=API.getEntityData(vehicle,"SC_VehicleMain_VehicleData");
      	API.triggerClientEvent(player, "SC_vehicle_changemod_logined_json",f.getModJson() ,f.getSlotJson());
		f=null;
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName == "SC_GETVEHICLETIMELIMIT") 
		{
			var veh = API.getPlayerVehicle(Player);
			var days =Convert.ToInt64(API.getEntitySyncedData(veh, "SC_VehicleMain_VehicleData_TimeLimit"));
			if(days != 0)
			{
				var nowtime = UnixTime.getUnixTimeToS();
				days = Convert.ToInt64((days - nowtime) / 86400);
				API.sendChatMessageToPlayer(Player, "~w~該栽具還有"+days.ToString()+"天到期.");
			}
		}
		if(eventName=="SC_VehicleMain_VehicleData_Rank")
		{
			var v=API.getEntityData((NetHandle)arguments[0],"SC_VehicleMain_VehicleData");
			v.setRank(arguments[1].ToString());
			API.setEntitySyncedData((NetHandle)arguments[0],"SC_VehicleMain_VehicleData_Rank",v.getRank());
		}
		if(eventName=="SC_VehicleMain_VehicleData_RankLevel")
		{
			var v=API.getEntityData((NetHandle)arguments[0],"SC_VehicleMain_VehicleData");
			v.setRankLevel(Convert.ToInt32(arguments[1]));
			API.setEntitySyncedData((NetHandle)arguments[0],"SC_VehicleMain_VehicleData_RankLevel",v.getRankLevel());
		}		
		if(eventName=="SC_vehicle_testetetete")
		{
			API.consoleOutput(arguments[0].ToString());
		}
		if(eventName=="SC_Vehicle_CALL_VEH")
		{ 
			var p=API.getEntityData(Player,"SC_VehicleList");
			foreach(var i in p)
			{
				if(i.getSQLID()==Convert.ToInt32(arguments[0]))
				{
					var f=false;
					if(i.getAlive()==false)
					{
						var m=API.getEntityData(Player,"SC_money");
						var fix=i.getMoney()*0.02;
						fix=(int)fix; 
						if(m.getMoney()<fix)
						{
							API.sendNotificationToPlayer(Player,"~g~召喚車輛大失敗！\n~g~原因 ~w~車輛損壞,無錢修理");
							return;
						}else{
							m.addMoney(-fix);
							i.setAlive(true);
							f=true;
							updataPlayerVehicleList(Player);
						}
					}
						var veh=i.getVeh();
						var targetPos=API.getEntityPosition(Player);
						var targetRot=API.getEntityRotation(Player);
						
						if(f==true)
						{ 
							i.respawnVeh();	
							veh=i.getVeh();
						}
						//API.repairVehicle(veh);
						if(API.isPlayerInAnyVehicle(Player)==false)
						{
							API.setEntityPosition(veh,targetPos);
							API.setEntityRotation(veh,targetRot);
							API.setPlayerIntoVehicle(Player,veh,-1);
						}else{
							if(veh!=API.getPlayerVehicle(Player))
							{
								API.setEntityPosition(veh,new Vector3(targetPos.X,targetPos.Y,targetPos.Z+10));
								API.setEntityRotation(veh,targetRot);
							}
						}
						break;
					
				}
			}
			p=null;
		}
		if(eventName=="SC_Vehicle_Control_Engine")
		{  
			var veh=API.getPlayerVehicle(Player);
			var d=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
			if(d.getMaster()==API.getPlayerName(Player))
			{
				if(d.getAlive()==true)
				{
					API.setVehicleEngineStatus(veh,!API.getVehicleEngineStatus(veh));
				}
			}else{
				API.sendChatMessageToPlayer(Player,"~r~*哎呀！你好像沒有鑰匙");
			}
			d=null; 
		}
		if(eventName=="SC_Vehicle_Control_Door")
		{  
			var veh=API.getPlayerVehicle(Player);
			var d=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
			var door=Convert.ToInt32(arguments[0]);
			if(d.getMaster()==API.getPlayerName(Player))
			{
				if(d.getAlive()==true)
				{
					API.setVehicleDoorState(veh,door,!API.getVehicleDoorState(veh,door));
				}
			}else{
				API.sendChatMessageToPlayer(Player,"~r~*哎呀！你好像沒有遙控器");
			}
			d=null; 
		} 
		if(eventName == "SC_Vehicle_NUMBER7_FIX")
		{
			var m = API.getEntityData(Player,"SC_money");
			if(m.getMoney() < RepairVehicleMoney)
			{
				API.sendChatMessageToAll("[ ~r~!~w~ ] 你的錢不夠啊.");
			}
			else
			{
				API.repairVehicle(API.getPlayerVehicle(Player));
				m.addMoney(-RepairVehicleMoney);
				API.sendChatMessageToPlayer(Player,"你修復了這輛載具.");
			}
		}
		if(eventName=="SC_Vehicle_Control_Neon")
		{
			var veh=API.getPlayerVehicle(Player);
			var d=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
			var js=API.fromJson(d.getSlotJson());
			if(js.Property("霓虹燈")!=null)
			{
				var v=js["霓虹燈"].ToString();
				if(v.Length!=0){
					string[] type=v.Split(':');
					foreach(var i in type){
						var id=Convert.ToInt32(i);
						API.setVehicleNeonState(veh,id,!API.getVehicleNeonState(veh,id)); 
					}
				}
			}
		}
	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录傳車輛列表
			updataPlayerVehicleList(API.getPlayerFromHandle(entity));
		}
		if(key=="SC_girf_vehicle")
		{
			updataPlayerVehicleList(API.getPlayerFromHandle(entity));
		}

	}
	
	private void OnVehicleDeath(NetHandle vehicle)
	{									 
		var d=API.getEntityData(vehicle,"SC_VehicleMain_VehicleData");
		if(d.getMaster()=="出售")
		{
			//API.consoleOutput("車輛死亡:"+d.getName());
			API.deleteEntity(vehicle);
		}else{
			API.setVehicleEngineStatus(vehicle,false);
			d.setAlive(false);
			var P=API.getPlayerFromName(d.getMaster());
			if(P!=null)
			{
				updataPlayerVehicleList(P);
			}
		}
		d=null;
	}	
	
	private void OnPlayerConnected(Client Player)
	{
		if(API.getPlayerName(Player)=="出售")
		{
			API.kickPlayer(Player,"這個名字不允許使用");
			return;
		}
		var d=new List<VehicleDataFunc>();
		foreach(var i in API.getAllVehicles())
		{
			var c=API.getEntityData(i,"SC_VehicleMain_VehicleData");
			if(c.getMaster()==API.getPlayerName(Player))
			{
				d.Add(c);
			}
			c=null;
		}
		API.setEntityData(Player, "SC_VehicleList", d);
		API.setEntitySyncedData(Player, "SC_Repair_Vehicle_Money", RepairVehicleMoney);
	}
	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		API.resetEntityData(Player, "SC_VehicleList");
	}
	
	public void OnResourceStart()
	{ 
		VehDB = new DataBase("Vehicle.db","resources\\shadowscity_db\\");
		 if(VehDB.isDataBase()==false)
		 {
			API.consoleOutput("車輛庫不存在!!創建！！！");
			VehDB.createDataBase();
			VehDB.connectOpenToDataBase();
			API.consoleOutput("車輛建主表！！！");
			var sql="PRAGMA synchronous = OFF";
			VehDB.sqlCommand(sql);
			sql="CREATE TABLE Veh( vID INTEGER PRIMARY KEY AUTOINCREMENT  ,vTime NOT NULL,vValue NOT NULL); ";
			VehDB.sqlCommand(sql);
			
		 }else{
			VehDB.connectOpenToDataBase();
			var sql="PRAGMA synchronous = OFF";
			VehDB.sqlCommand(sql);
		 }
		 
		API.consoleOutput("车辆读取！");
		DataBaseSdon[] rr=VehDB.sqlCommandReturn("Veh","","vID","vValue"); 
		int _vid;
		string _value="";
		foreach(DataBaseSdon v in rr)
		{
			_vid=Convert.ToInt32(v.Get("vID"));
			_value=v.Get("vValue");
			var js=API.fromJson(_value);
			VehicleDataFunc r=new VehicleDataFunc(js["vehName"].ToString(),(VehicleHash)js["vehHash"] ,js["vehMaster"].ToString(),js["vehModJson"].ToString(),
			Convert.ToInt32(js["vehMoney"]),new Vector3(Convert.ToSingle(js["vehX"]),Convert.ToSingle(js["vehY"]),Convert.ToSingle(js["vehZ"])),
			new Vector3(Convert.ToSingle(js["vehRX"]),Convert.ToSingle(js["vehRY"]),Convert.ToSingle(js["vehRZ"])),0,0,Convert.ToBoolean(js["vehAlive"]),
			Convert.ToInt32(js["vehTimeLimit"])); 
			r.setSQLID(Convert.ToInt32(js["sqlID"]));
			if(js.Property("vRankLevel")!=null){
				r.setRankLevel(Convert.ToInt32(js["vRankLevel"]));
			}			
			if(js.Property("vRank")!=null){
				r.setRank(js["vRank"].ToString());
			}			
			if(js.Property("vehSlot")!=null){
				r.setSlot(js["vehSlot"].ToString());
			}
			var veh=r.createVeh();
			API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_RankLevel",r.getRankLevel());
			API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Rank",r.getRank());
			r=null;
			API.sleep(100);
		}
		API.consoleOutput("车辆读取完毕！"); 
		time=UnixTime.getUnixTimeToS();
    }
	
	public void OnResourceStop()
	{ 
		foreach(var i in API.getAllVehicles())
		{
			var d=API.getEntityData(i,"SC_VehicleMain_VehicleData");
			if(d.getMaster()!="出售")
			{//保存所有非出售車輛的最後座標和旋轉。	
				d.updataVehPos();
				d.pushVeh();
			}
			d=null;
		}
		VehDB.closeToDataBase();
		VehDB = null;
    }
	
	[Command("createvehicleTL", GreedyArg = true)]
	public void cmd_createvehicleTL(Client player, string owner, string s_days)
	{
		if(API.getEntitySyncedData(player, "SC_ACL_HAS_Name:SC_Admin") == 1)
		{
			var days = Convert.ToInt32(s_days);
			if(days < 1 || days > 1095)
			{
				API.sendChatMessageToPlayer(player, "[ ~r~!~w~ ] the days is too long.(hahah)");
			}
			else
			{
				long tl = UnixTime.getUnixTimeToS() + (86400 * days);
				Vector3 PlayerPos = API.getEntityPosition(player);
				new VehicleDataFunc("Elegy",API.vehicleNameToModel("Elegy"),owner,"",285000,new Vector3(PlayerPos.X,PlayerPos.Y,PlayerPos.Z),new Vector3(0,0,0),0,0,true,tl).createVeh();
			}
		}
		else
		{
			API.sendChatMessageToPlayer(player, "[ ~r~!~w~ ] you don't have the authority to use this command.");
		}
	}
	
	[Command("bc")]
	public void buycar(Client Player)
	{
		if(API.isPlayerInAnyVehicle(Player)==true)
		{
			var seat=API.getPlayerVehicleSeat(Player);
			if(seat==-1)
			{
				var veh=API.getPlayerVehicle(Player);
				var d=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
				if(d.getMaster()=="出售")
				{
					if(API.getEntityData(Player,"SC_money").getMoney()>=d.getMoney())
					{
						var us=API.getEntityData(Player,"SC_USERINFO");
						us.setUserInfo("SYSCARS",us.getUserInfo("SYSCARS").infoValue+1);
						API.getEntityData(Player,"SC_money").addMoney(-d.getMoney());
						API.sendChatMessageToPlayer(Player,"~y~這輛車現在歸你了!撒丫子狂奔吧!");
						d.changeMaster(API.getPlayerName(Player));
						d.pushVeh();
						API.setVehicleEngineStatus(veh,true);
						API.setEntityInvincible(veh,false);				 
						var l=API.getEntityData(Player,"SC_VehicleList");
						l.Add(d); 
						updataPlayerVehicleList(Player);
						l=null;
					}else{	
						API.sendChatMessageToPlayer(Player,"~r~嘿!你的金錢不足~y~$"+d.getMoney().ToString()+"~r~!");
					} 
				}else{
					API.sendChatMessageToPlayer(Player,"~r~嘿!這可是非賣品!");
				}
				d=null;
			}else{
				API.sendChatMessageToPlayer(Player,"嘿!這個指令需要你~r~是司機~w~才能使用!");
			}
		}else{
			API.sendChatMessageToPlayer(Player,"嘿!這個指令需要~r~在車上~w~才能使用!");
		}
	}
	

	
	public bool getSellVehicle(string vname)
	{//獲取某種車是否有被出售
	
		var vh=API.vehicleNameToModel(vname);
		foreach(var i in API.getAllVehicles())
		{
			var b=API.getEntityData(i,"SC_VehicleMain_VehicleData");
			if(b.getMaster()=="出售"&&b.getHash()==vh)
			{
				return true; 
			}
		}
		return false;
	}
	
	public void onCreateSellVeh()
	{//瞎逼生成出售汽車
	//189.7448, -857.1785, 31.5500
		API.consoleOutput("生成随机出售车！");
		if(getSellVehicle("Guardian")==false)
		{
			new VehicleDataFunc("Guardian",API.vehicleNameToModel("Guardian"),"出售","",350000,new Vector3(-840.8125,-760.54,21.62859),new Vector3(-0.2773531,6.421905,-90.03515),0,0,true).createVeh();
		}
		if(getSellVehicle("Dominator")==false)
		{
			new VehicleDataFunc("Dominator",API.vehicleNameToModel("Dominator"),"出售","",100000,new Vector3(-840.8566,-764.3352,21.2302),new Vector3(0.0209913,5.949872,-88.69661),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Gauntlet")==false)
		{
			new VehicleDataFunc("Gauntlet",API.vehicleNameToModel("Gauntlet"),"出售","",85000,new Vector3(-840.7903,-768.0631,20.90033),new Vector3(-0.01971078,5.074128,-88.95302),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Nightshade")==false)
		{
			new VehicleDataFunc("Nightshade",API.vehicleNameToModel("Nightshade"),"出售","",650000,new Vector3(-840.9588,-771.7653,20.58943),new Vector3(-0.1544096,4.760806,-90.39618),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Vigero")==false)
		{
			new VehicleDataFunc("Vigero",API.vehicleNameToModel("Vigero"),"出售","",120000,new Vector3(-829.6426,-768.1426,20.89247),new Vector3(0.09399778,-5.105069,87.91321),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Stalion")==false)
		{
			new VehicleDataFunc("Stalion",API.vehicleNameToModel("Stalion"),"出售","",100000,new Vector3(-829.6463,-764.5139,21.21085),new Vector3(-0.178012,-5.819916,90.36237),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Brawler")==false)
		{
			new VehicleDataFunc("Brawler",API.vehicleNameToModel("Brawler"),"出售","",400000,new Vector3(-829.6716,-760.5746,21.60937),new Vector3(0.01453253,-5.79729,90.35735),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Mesa3")==false)
		{
			new VehicleDataFunc("Mesa3",API.vehicleNameToModel("Mesa3"),"出售","",250000,new Vector3(-829.6956,-756.9705,21.93623),new Vector3(-0.04528392,-5.140606,91.04927),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Baller4")==false)
		{
			new VehicleDataFunc("Baller4",API.vehicleNameToModel("Baller4"),"出售","",125000,new Vector3(-822.2299,-756.965,21.9278),new Vector3(-0.09642363,5.463419,-89.91291),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Seminole")==false)
		{
			new VehicleDataFunc("Seminole",API.vehicleNameToModel("Seminole"),"出售","",90000,new Vector3(-822.1362,-760.6603,21.58334),new Vector3(-0.1199874,5.779522,-89.23751),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Stanier")==false)
		{
			new VehicleDataFunc("Stanier",API.vehicleNameToModel("Stanier"),"出售","",85000,new Vector3(-822.6038,-764.3098,21.228),new Vector3(-0.1911422,5.815838,-89.84666),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Alpha")==false)
		{
			new VehicleDataFunc("Alpha",API.vehicleNameToModel("Alpha"),"出售","",100000,new Vector3(-822.2722,-768.1317,20.89244),new Vector3(-0.1154281,5.116807,-90.59661),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Futo")==false)
		{
			new VehicleDataFunc("Futo",API.vehicleNameToModel("Futo"),"出售","",185000,new Vector3(-810.3134,-768.2221,20.88384),new Vector3(-0.2196786,-5.114316,91.30293),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Kuruma")==false)
		{
			new VehicleDataFunc("Kuruma",API.vehicleNameToModel("Kuruma"),"出售","",250000,new Vector3(-810.3484,-764.3441,21.22149),new Vector3(-0.2820641,-5.581238,92.01154),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Ninef")==false)
		{
			new VehicleDataFunc("Ninef",API.vehicleNameToModel("Ninef"),"出售","",158000,new Vector3(-810.3736,-760.6966,21.56098),new Vector3(-0.09174464,-5.509927,91.33854),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Omnis")==false)
		{
			new VehicleDataFunc("Omnis",API.vehicleNameToModel("Omnis"),"出售","",1000000,new Vector3(-810.5027,-757.0791,21.89684),new Vector3(-0.1010506,-5.611928,90.8251),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Elegy2")==false)
		{
			new VehicleDataFunc("Elegy2",API.vehicleNameToModel("Elegy2"),"出售","",235000,new Vector3(-810.4974,-753.4069,22.25035),new Vector3(-0.3221333,-5.727067,92.28563),0,0,true).createVeh();//pos
		}
		if(getSellVehicle("Elegy")==false)
		{
			new VehicleDataFunc("Elegy",API.vehicleNameToModel("Elegy"),"出售","",285000,new Vector3(-816.4288,-742.3093,23.32326),new Vector3(-0.1859518,-3.237555,89.6736),0,0,true).createVeh();//pos
		}
	} 
	
	[Command("t")]
	public void c(Client Player , string tips)
	{
		var veh=API.getPlayerVehicle(Player);
		var pos=API.getEntityPosition(veh);
		var rot=API.getEntityRotation(veh);
		API.consoleOutput("new VehicleDataFunc(\"待選\",API.vehicleNameToModel(\"待選\"),\"出售\",\"\",999999,new Vector3("+pos.X+","+pos.Y+","+pos.Z+"),new Vector3("+rot.X+","+rot.Y+","+rot.Z+"),0,0,true).createVeh();//"+tips);
	}
	
	
	public void updataPlayerVehicleList(Client Player)
	{
		var l=API.getEntityData(Player,"SC_VehicleList");
		var d=new List<VehicleInfo>();
		foreach(var i in l)
		{ 
			d.Add(new VehicleInfo(i.getName(),i.getSQLID(),i.getAlive(),i.getMoney(),i.getLock()));
		} 
		API.triggerClientEvent(Player, "SC_Vehicle_CALL_LISTUPDATA",API.toJson(d) );
		d=null;
		l=null;
	}
  
}

public class VehicleInfo
{
	public string vName="";
	public int vID=-1;
	public bool vA;
	public int vM;
	public bool vLock;
	public VehicleInfo(){}
	public VehicleInfo(string n,int i,bool a,int mo,bool vl){
		vName=n;
		vID=i;
		vA=a;
		vM=mo;
		vLock=vl;
	}
}