using System;
using GTANetworkServer;
using GTANetworkShared;

 

public class vehiclechangemod : Script
{ 	

	public vehiclechangemod()
	{
		API.onClientEventTrigger += OnClientEvent; 
	} 

	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{

		if(eventName=="SC_vehicle_changemod_go!")
		{	 
			//收到改装申请，保留改装JSON串
			var veh=API.getPlayerVehicle(Player);
			var js=arguments[0].ToString();
			var d=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
			var m=API.getEntityData(Player,"SC_money");
			var money=Convert.ToInt32(arguments[1]);
			m.addMoney(-money);
			d.setModJson(js);
			d.setJsonToVehicleMod(js);
			API.triggerClientEventForAll("SC_vehicle_mod_Set",veh,js);
			d=null;
		}
		if(eventName=="SC_vehicle_changemod_freeze")
		{
			API.setVehicleLocked(API.getPlayerVehicle(Player),(bool)arguments[0]);
			API.setEntityInvincible(API.getPlayerVehicle(Player),(bool)arguments[0]);	
			API.freezePlayer(Player,(bool)arguments[0]);
		}
		if(eventName=="SC_vehicle_changemod_clear_ex!")
		{
			
			var veh=API.getPlayerVehicle(Player);
			var p=API.getEntityData(veh,"SC_VehicleMain_VehicleData");
			p.clearSlot(arguments[0].ToString());
			
			API.triggerClientEvent(Player, "SC_vehicle_changemod_logined_json",p.getModJson(),p.getSlotJson() );
			p=null;
		}
	}
	
	 


}