using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkServer;
using GTANetworkShared;
using System.Threading;

public class InteriorsTeleportScript : Script{
	public InteriorsTeleportScript(){}
	double [,] InteriorPosition = new double [ , ]{
		{-786.8663,315.7642,217.6385},
		{-786.9563,315.6229,187.9136},
		{-774.0126,342.0428,196.6864},
		{-787.0749,315.8198,217.6386},
		{-786.8195,315.5634,187.9137},
		{-774.1382,342.0316,196.6864},
		{-786.6245,315.6175,217.6385},
		{-786.9584,315.7974,187.9135},
		{-774.0223,342.1718,196.6863},
		{-787.0902,315.7039,217.6384},
		{-787.0155,315.7071,187.9135},
		{-773.8976,342.1525,196.6863},
		{-786.9887,315.7393,217.6386},
		{-786.8809,315.6634,187.9136},
		{-774.0675,342.0773,196.6864},
		{-787.1423,315.6943,217.6384},
		{-787.0961,315.815,187.9135},
		{-773.9552,341.9892,196.6862},
		{-787.029,315.7113,217.6385},
		{-787.0574,315.6567,187.9135},
		{-774.0109,342.0965,196.6863},
		{-786.9469,315.5655,217.6383},
		{-786.9756,315.723,187.9134},
		{-786.9756,315.723,187.9134},
		{-774.0349,342.0296,196.6862},
		
		{-1579.756,-565.0661,108.523},
		{-1579.678,-565.0034,108.5229},
		{-1579.583,-565.0399,108.5229},
		{-1579.702,-565.0366,108.5229},
		{-1579.643,-564.9685,108.5229},
		{-1579.681,-565.0003,108.523},
		{-1579.677,-565.0689,108.5229},
		{-1579.708,-564.9634,108.5229},
		{-1579.693,-564.8981,108.5229},
		
		{1107.04,-3157.399,-37.51859},
		{998.4809,-3164.711,-38.90733},
		{1009.5,-3196.6,-38.99682},
		{1051.491,-3196.536,-39.14842},
		{1093.6,-3196.6,-38.99841},
		{1121.897,-3195.338,-40.4025},
		{1165,-3196.6,-39.01306},
		{1094.988,-3101.776,-39.00363},
		{1056.486,-3105.724,-39.00439},
		{1006.967,-3102.079,-39.0035},
		//no ipl apartments
		{261.4586,-998.8196,-99.00863},
		{-35.31277,-580.4199,88.71221},
		{-1477.14,-538.7499,55.5264},
		{-18.07856,-583.6725,79.46569},
		{-1468.14,-541.815,73.4442},
		{-915.811,-379.432,113.6748},		
		{-614.86,40.6783,97.60007},
		{-773.407,341.766,211.397},
		{-169.286,486.4938,137.4436},
		{340.9412,437.1798,149.3925},
		{373.023,416.105,145.7006},
		{-676.127,588.612,145.1698},
		{-763.107,615.906,144.1401},
		{-857.798,682.563,152.6529},
		{120.5,549.952,184.097},
		{-1288,440.748,97.69459},
		//no ipl misc
		{402.5164,-1002.847,-99.2587},
		{405.9228,-954.1149,-99.6627},
		{136.5146,-2203.149,7.30914},
		{-1005.84,-478.92,50.02733},
		{-1908.024,-573.4244,19.09722},
		{2331.344,2574.073,46.68137},
		{-1427.299,-245.1012,16.8039},
		{152.2605,-1004.471,-98.99999},
		{1401.21,1146.954,114.3337},
		{-1044.193,-236.9535,37.96496},
		{1273.9,-1719.305,54.77141},
		{134.5835,-749.339,258.152},
		{134.573,-766.486,234.152},
		{134.635,-765.831,242.152},
		{117.22,-620.938,206.1398}
		
	};
	string [,] InteriorName = new string [ , ]{
		{"Modern 1 Apartment","apa_v_mp_h_01_a"},
		{"Modern 2 Apartment","apa_v_mp_h_01_c"},
		{"Modern 3 Apartment","apa_v_mp_h_01_b"},
		{"Mody 1 Apartment","apa_v_mp_h_02_a"},
		{"Mody 2 Apartment","apa_v_mp_h_02_c"},
		{"Mody 3 Apartment","apa_v_mp_h_02_b"},
		{"Vibrant 1 Apartment","apa_v_mp_h_03_a"},
		{"Vibrant 2 Apartment","apa_v_mp_h_03_c"},
		{"Vibrant 3 Apartment","apa_v_mp_h_03_b"},
		{"Sharp 1 Apartment","apa_v_mp_h_04_a"},
		{"Sharp 2 Apartment","apa_v_mp_h_04_c"},
		{"Sharp 3 Apartment","apa_v_mp_h_04_b"},
		{"Monochrome 1 Apartment","apa_v_mp_h_05_a"},
		{"Monochrome 2 Apartment","apa_v_mp_h_05_c"},
		{"Monochrome 3 Apartment","apa_v_mp_h_05_b"},
		{"Seductive 1 Apartment","apa_v_mp_h_06_a"},
		{"Seductive 2 Apartment","apa_v_mp_h_06_c"},
		{"Seductive 3 Apartment","apa_v_mp_h_06_b"},
		{"Regal 1 Apartment","apa_v_mp_h_07_a"},
		{"Regal 2 Apartment","apa_v_mp_h_07_c"},
		{"Regal 3 Apartment","apa_v_mp_h_07_b"},
		{"Aqua 1 Apartment","apa_v_mp_h_08_a"},
		{"Aqua 2 Apartment","apa_v_mp_h_08_c"},
		{"Aqua 3 Apartment","apa_v_mp_h_08_b"},
		
		{"Executive Rich","ex_sm_13_office_02b"},
		{"Executive Cool","ex_sm_13_office_02c"},
		{"Executive Contrast","ex_sm_13_office_02a"},
		{"Old Spice Warm","ex_sm_13_office_01a"},
		{"Old Spice Classical","ex_sm_13_office_01b"},
		{"Old Spice Vintage","ex_sm_13_office_01c"},
		{"Power Broker Ice","ex_sm_13_office_03a"},
		{"Power Broker Conservative","ex_sm_13_office_03b"},
		{"Power Broker Polished","ex_sm_13_office_03c"},
		
		{"Clubhouse 1","bkr_biker_interior_placement_interior_0_biker_dlc_int_01_milo"},
		{"Clubhouse 2","bkr_biker_interior_placement_interior_1_biker_dlc_int_02_milo"},
		{"Warehouse 1","bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo"},
		{"Warehouse 2","bkr_biker_interior_placement_interior_3_biker_dlc_int_ware02_milo"},
		{"Warehouse 3","bkr_biker_interior_placement_interior_4_biker_dlc_int_ware03_milo"},
		{"Warehouse 4","bkr_biker_interior_placement_interior_5_biker_dlc_int_ware04_milo,1121.897"},
		{"Warehouse 5","bkr_biker_interior_placement_interior_6_biker_dlc_int_ware05_milo"},
		{"Warehouse Small","ex_exec_warehouse_placement_interior_1_int_warehouse_s_dlc_milo"},
		{"Warehouse Medium","ex_exec_warehouse_placement_interior_0_int_warehouse_m_dlc_milo"},
		{"Warehouse Large","ex_exec_warehouse_placement_interior_2_int_warehouse_l_dlc_milo"},
	//	Apartments NO IPL
		{"Low End Apartment","None"},
		{"4 integrity way apt 30","None"},
		{"del perro heights apt7","None"},
		{"4 integrity way apt 28","None"},
		{"del perro heights apt4","None"},
		{"richard majestic apt 2","None"},		
		{"tinsel towers apt42","None"},
		{"eclipse towers apt 3","None"},
		{"3655 wild oats drive","None"},
		{"2044 north conker avenue","None"},
		{"2045 north conker avenue","None"},
		{"2862 hillcrest avenue","None"},		
		{"2868 hillcrest avenue","None"},
		{"2874 hillcrest avenue","None"},
		{"whispymound drive 3677","None"},
		{"2113 mad wayne thunder","None"},
	//misc no ipl	
		{"charcreator","None"},
		{"mission carpark","None"},
		{"torture room","None"},
		{"solomons office","None"},
		{"psychiatrists office","None"},
		{"omegas garage","None"},
		{"movie theatre","None"},
		{"motel","None"},
		{"mandrazos ranch","None"},
		{"live invader office","None"},
		{"lesters house","None"},
		{"fbi top floor","None"},
		{"fbi floor 47","None"},
		{"fbi floor 49","None"},
		{"iaa office","None"}
		//67 44没窗户,68墨西哥老大
	};
	[Command("goint")]
	public void GoToInterior(Client sender, int interiorID){
		if(interiorID >= InteriorName.GetLength(0) | interiorID < 0){
			API.sendChatMessageToPlayer(sender,"~r~錯誤~w~沒有找到內飾");
		}
		if(InteriorName[interiorID,1] == "None"){
			var pos = API.getEntityPosition(sender.handle);
			API.setEntityPosition(sender.handle,new Vector3(InteriorPosition[interiorID,0],InteriorPosition[interiorID,1],InteriorPosition[interiorID,2]));
			API.sendChatMessageToPlayer(sender,"~w~你傳送到了~p~"+InteriorName[interiorID,0]);
		}
		else{
			for(int i = 0;i < InteriorName.GetLength(0);i ++){
				API.removeIpl(InteriorName[i,1]);				
			}
			var pos = API.getEntityPosition(sender.handle);
			API.requestIpl(InteriorName[interiorID,1]);
			API.setEntityPosition(sender.handle,new Vector3(InteriorPosition[interiorID,0],InteriorPosition[interiorID,1],InteriorPosition[interiorID,2]));
			API.sendChatMessageToPlayer(sender,"~w~你傳送到了~p~"+InteriorName[interiorID,0]);
		}
	}
	
	[Command("neno")]
	public void SetVehicleNeno(Client player,int slot,bool turnedOn){
		API.setVehicleNeonState(player.vehicle, slot, turnedOn);
	}
	[Command("nenoc")]
	public void SetVehicleNenoC(Client player,int r,int g,int b){
		API.setVehicleNeonColor(player.vehicle, r, g, b);
	}
	[Command("stopfs")]
	public void stopfs(Client sender)
	{
		API.stopResource("TeleportToInteriors");
	}
	[Command("startfs")]
	public void startfs(Client sender)
	{
		API.startResource("TeleportToInteriors");
	}
	
	[Command("settime2")]
	public void setPlayerTime(Client player,int hour){
		API.triggerClientEvent(player, "SC_SET_CLIENT_TIME", hour);
	}
	[Command("resettime2")]
	public void resetPlayerTime(Client player,int hour){
		API.triggerClientEvent(player, "SC_RESET_CLIENT_TIME");
	}
	[Command("setweather2")]
	public void setPlayerWeather(Client player,int weather){
		if (weather < 0 || weather > 13)
			player.sendChatMessage("~r~Invalid weather ID.");
		else{
			API.triggerClientEvent(player, "SC_SET_CLIENT_WEATHER", weather);
		}		
	}
	[Command("resetweather2")]
	public void resetPlayerWeather(Client player,int weather){
		API.triggerClientEvent(player, "SC_RESET_CLIENT_WEATHER");
	}
}