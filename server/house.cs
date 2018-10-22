using System;
using GTANetworkServer;
using GTANetworkShared;

using System.Collections.Generic;
 
public class house : Script
{
	//o = outside, i = inside
	public static DataBase housedb;
	public static List<HouseList> h_List = new List<HouseList>();
	public house()
	{
		API.onResourceStart += OnResourceStart;
		API.onResourceStop += OnResourceStop;
	}
	
	public void OnResourceStart()
	{
		housedb = new DataBase("House.db","resources\\shadowscity\\db\\");
		if(housedb.isDataBase() == false)
		{
			API.consoleOutput("house database not found.");
		}
		else
		{
			API.requestIpl("apa_v_mp_h_08_c"); 
			housedb.connectOpenToDataBase();
			API.consoleOutput("house loading..");
			DataBaseSdon[] hall = housedb.sqlCommandReturn("House","","ID","Name","Owner","Price","oX","oY","oZ","iX","iY","iZ"); 
			foreach(DataBaseSdon v in hall)
			{
				HouseList h = new HouseList(Convert.ToInt32(v.Get("ID")), v.Get("Name"), v.Get("Owner"), Convert.ToInt32(v.Get("Price")), Convert.ToSingle(v.Get("oX")), Convert.ToSingle(v.Get("oY")),
				Convert.ToSingle(v.Get("oZ")), Convert.ToSingle(v.Get("iX")), Convert.ToSingle(v.Get("iY")), Convert.ToSingle(v.Get("iZ"))); 
				h_List.Add(h);
				
			}
		}
    }
	
	public void OnResourceStop()
	{ 
		housedb.closeToDataBase();
		housedb = null;
		API.consoleOutput("house database close.");
    }
	
	[Command("setpos")]
	public void cmd_setpos(Client player)
	{
		API.requestIpl("apa_v_mp_h_08_c"); 
		API.setEntityPosition(player, new Vector3(-786.9756,	315.723,	187.9134));
	}
	
	[Command("getpos")]
	public void cmd_getpos(Client player)
	{
		Vector3 PlayerPos = API.getEntityPosition(player);
		Vector3 PlayerAngle = API.getEntityRotation(player);
		API.sendChatMessageToPlayer(player, "X: " + PlayerPos.X + " Y: " + PlayerPos.Y + " Z: " + PlayerPos.Z);
		API.sendChatMessageToPlayer(player, "RX: " + PlayerAngle.X + " RY: " + PlayerAngle.Y + " RZ: " + PlayerAngle.Z);
	}
	
	[Command("enter")]
	public void cmd_enter(Client player)
	{
		API.triggerClientEvent(player, "house_enter", API.toJson(h_List));
	}
	
	[Command("exit")]
	public void cmd_exit(Client player)
	{
		API.triggerClientEvent(player, "house_exit", API.toJson(h_List));
	}
}

public class HouseList : Script
{
	//o = outside, i = inside
	public int h_ID = 0;
	public string h_Name = "";
	public string h_Owner = "";
	public int h_Price = 0;
	public float h_oX;
	public float h_oY;
	public float h_oZ;
	public float h_iX;
	public float h_iY;
	public float h_iZ;
	public NetHandle h_Marker;

	public HouseList()
	{
		
	}
	
	public HouseList(int id, string name, string owner, int price, float ox, float oy, float oz, float ix, float iy, float iz)
	{
		h_ID = id;
		h_Name = name;
		h_Owner = owner;
		h_Price = price;
		h_oX = ox;
		h_oY = oy;
		h_oZ = oz;
		h_iX = ix;
		h_iY = iy;
		h_iZ = iz;
		h_Marker = API.createMarker(1, new Vector3(ox, oy, oz - 1.5), new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 255, 255, 255);
		API.consoleOutput(Convert.ToString(h_Marker));
	}
}