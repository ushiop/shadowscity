using System;
using GTANetworkServer;
using GTANetworkShared;

public class login : Script
{ 	
	public static DataBase logindb;
	
	public login()
	{
		API.onPlayerConnected += OnPlayerConnected;
		API.onPlayerDisconnected += OnPlayerDisconnected;
		API.onPlayerFinishedDownload += OnPlayerFinshedDownload;
		API.onResourceStart += OnResourceStart;
		API.onResourceStop += OnResourceStop;
		API.onClientEventTrigger += OnClientEvent;
	}

	public void OnResourceStart()
	{ 
		logindb = new DataBase("Users.db","resources\\shadowscity_db\\");
		if(logindb.isDataBase() == false)
		{
			API.consoleOutput("用户数据库未找到");
		}
		else
		{
			logindb.connectOpenToDataBase();
			API.consoleOutput("用户数据库打开");
			API.requestIpl("apa_v_mp_h_01_a"); 
		}
    }
	
	public void OnResourceStop()
	{ 
		logindb.closeToDataBase();
		logindb = null;
    }
	
	private void OnPlayerConnected(Client Player)
	{
		API.setEntityData(Player, "SC_Login_Status", 0);
		API.setEntityData(Player, "SC_Login_Timmmmmmmme", UnixTime.getUnixTimeToS() + 10);
		API.sendNotificationToAll("~b~"+API.getPlayerName(Player)+" ~w~進入了服務器");
	}
	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		API.resetEntityData(Player,"SC_money");
		API.sendNotificationToAll("~r~"+API.getPlayerName(Player)+" ~w~離開了服務器");
	}
	
	private void OnPlayerFinshedDownload(Client Player)
	{
		API.setEntityPosition(Player, new Vector3(-786.5013, 343.5338, 216.8386));
		API.setEntityRotation(Player, new Vector3(0, 0, -25.60212));
		API.freezePlayer(Player, true);
		API.sendChatMessageToPlayer(Player,"歡迎來到-~p~ShadowsCity~w~-"); 
		//API.sendChatMessageToPlayer(Player, "使用命令: ~r~/register [密碼] ~w~註冊賬號, ~r~/login [密碼] ~w~登錄賬號");
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_LOGIN_REGISTER")
		{
			var ps=arguments[0].ToString();
			var f=cmd_register(Player,ps);
			if(f=="register_faild"||f=="register_ok")
			{
				var s=cmd_login(Player,ps);
				API.triggerClientEvent(Player,"SC_LOGIN_REGISTER_RETURN",s);	
			}else{
				API.triggerClientEvent(Player,"SC_LOGIN_REGISTER_RETURN",f);		
			}
		}
	}
	
	 
	public string cmd_login(Client Player, string password)
	{
		if(GetPlayerLoginStatus(Player) == 1)
		{
			return "[ ~r~!~w~ ] 妳已經登錄了.";
		}
		else
		{
			if(IsUserAlreadyRegister(API.getPlayerName(Player)) == 0)
			{
				return "login_register";
			}
			else
			{
				if(UnixTime.getUnixTimeToS() < API.getEntityData(Player, "SC_Login_Timmmmmmmme"))
				{
					var timme = API.getEntityData(Player, "SC_Login_Timmmmmmmme") - UnixTime.getUnixTimeToS();
					return "login_cd";
				}
				else
				{
					DataBaseSdon[] v = logindb.sqlCommandReturn("Users","Name='"+API.getPlayerName(Player)+"'","Password");
					var dbpassword = v[0].Get("Password");
					if(password == dbpassword)
					{
						
						DataBaseSdon[] uid = logindb.sqlCommandReturn("Users","Name='"+API.getPlayerName(Player)+"'","UID");
						API.setEntityData(Player, "SC_Login_UID", uid[0].Get("UID"));
						DataBaseSdon[] scname = logindb.sqlCommandReturn("Users","Name='"+API.getPlayerName(Player)+"'","SCName");
						if(v[0].Get("SCName") == "0")
						{
							var sql = "UPDATE Users SET SCName = '"+Player.socialClubName+"' WHERE UID = '"+uid[0].Get("UID")+"';";
							logindb.sqlCommand(sql);  
						}	
						new userinfo(Player);
						new config(Player);
						new item(Player);
						var m=new money(Player);
						//API.sendChatMessageToPlayer(Player,"登錄成功, 按~g~F1~w~來打開基礎功能面板, 祝你遊戲愉快~");
						
						API.setEntityPosition(Player, new Vector3(189.7448, -857.1785, 31.5500));
						API.setEntityRotation(Player, new Vector3(0, 0, 18.4499));
						API.freezePlayer(Player, false);

						
						
						API.setEntityData(Player,"SC_money",m);
						API.triggerClientEvent(Player,"SC_money_loginok",m.getMoney());						
						
						if(API.getEntitySyncedData(Player,"SC_register")==1)
						{
							m.addMoney(15000);
							//Gauntlet
							var d=new VehicleDataFunc("Gauntlet",API.vehicleNameToModel("Gauntlet"),API.getPlayerName(Player),"",1,new Vector3(180.9991,-847.4241,30.22109),new Vector3(0.9849584,0.3272536,-19.41356),0,0,true);
							d.createVeh(); 
							d.pushVeh();
							var l=API.getEntityData(Player,"SC_VehicleList");
							l.Add(d); 
							API.setEntitySyncedData(Player,"SC_girf_vehicle",1);
							
							SetPlayerAccess(Player,"SC_girf","1");
							API.getEntityData(Player,"SC_item").showItemDx("新手禮包","~r~每個帳號限領一次","獲得~y~15000~w~金幣\n獲得一輛~g~Gauntlet");
								var us=API.getEntityData(Player,"SC_USERINFO");
								us.setUserInfo("SYSCARS",us.getUserInfo("SYSCARS").infoValue+1);
							l=null;
							d=null;
						}else{
							if(GetPlayerAccess(Player,"SC_girf")=="SC_NULL")
							{
								SetPlayerAccess(Player,"SC_girf","1");
								m.addMoney(25000); 
								API.getEntityData(Player,"SC_item").showItemDx("補償禮包","~r~每個帳號限領一次","獲得~y~25000~w~金幣\n獲得一輛~g~Dukes");
								var d=new VehicleDataFunc("Dukes",API.vehicleNameToModel("Dukes"),API.getPlayerName(Player),"",1,new Vector3(180.9991,-847.4241,30.22109),new Vector3(0.9849584,0.3272536,-19.41356),0,0,true);
								var l=API.getEntityData(Player,"SC_VehicleList");
								d.createVeh(); 	
								d.pushVeh();
								l.Add(d); 
								API.setEntitySyncedData(Player,"SC_girf_vehicle",1);
														var us=API.getEntityData(Player,"SC_USERINFO");
								us.setUserInfo("SYSCARS",us.getUserInfo("SYSCARS").infoValue+1);
								l=null;
								d=null;
							}
						}
						API.setEntityData(Player, "SC_Login_Status", 1); 
						API.setEntitySyncedData(Player, "SC_Login_Status", 1);
						
						m=null;
						return "login_ok";
					}
					else
					{
						return "login_password_error";
					}
				}
			}
		}
	}

 
	public string cmd_register(Client Player, string password)
	{
		if(GetPlayerLoginStatus(Player) == 1)
		{
             return  "[ ~r~!~w~ ] 妳已經登錄了.";
		}
		else
		{
			if(IsUserAlreadyRegister(API.getPlayerName(Player)) == 1)
			{
				return "register_faild";
			}
			else
			{
				DataBaseSdon[] v = logindb.sqlCommandReturn("Users","SCName='"+Player.socialClubName+"'","SCName");
				if(v.Length== 0)
				{
					var sql = "INSERT INTO Users (Name,Password,CreateDate,SCName) VALUES ('"+API.getPlayerName(Player)+"','"+password+"','"+DateTime.Now.ToString()+"','"+Player.socialClubName+"');";
					logindb.sqlCommand(sql);
					//API.sendChatMessageToPlayer(Player, "註冊成功, 請牢記妳的密碼["+password+"]");
					API.setEntitySyncedData(Player, "SC_register",1);//注册事件
					return "register_ok";
				}
				else
				{
					return  "register_scid_faild";
				}
			}
		}
	}
	
	public int GetPlayerLoginStatus(Client Player)
	{
		return API.getEntityData(Player, "SC_Login_Status");
	}
	
	public int IsUserAlreadyRegister(string playername)
	{
		DataBaseSdon[] v = logindb.sqlCommandReturn("Users","Name='"+playername+"'","Name");
		if(v.Length == 0) return 0;
		return 1;
	}
	
	public  int SetPlayerAccess(Client Player, string keyname, string values)
	{
		var playername=API.getPlayerName(Player);
		DataBaseSdon[] uid = logindb.sqlCommandReturn("Users","Name='"+playername+"'","UID");
		if(uid.Length == 0) return 0;
		var sql="";
		if(GetPlayerAccess(Player,keyname)=="SC_NULL")
		{
			
			sql = "INSERT INTO Access (UID,keyname,value) VALUES ('"+uid[0].Get("UID")+"','"+keyname+"','"+values+"');";
			
		}else{
			 
			sql = "UPDATE Access SET value = '"+values+"' WHERE UID = '"+uid[0].Get("UID")+"' AND keyname='"+keyname+"';";
		}
		 
		logindb.sqlCommand(sql);  
		return 1;
	}
	
	public  string GetPlayerAccess(Client Player, string keyname)
	{
		DataBaseSdon[] v = logindb.sqlCommandReturn("Access","UID='"+API.getEntityData(Player, "SC_Login_UID")+"' AND keyname='"+keyname+"'","value");
		var value = "SC_NULL";
		if(v.Length == 0) return value;
		value = v[0].Get("value");
		return value;
	}
}
 