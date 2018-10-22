API.onPlayerEnterVehicle.connect(function(veh) {
	//進入車輛
	var gv=API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master")
	if(gv=="出售"){
			API.sendChatMessage("~g~哎呀!好運氣!這輛車不歸任何人");
			API.sendChatMessage("~g~你只需要花費~y~$"+API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Money")+"~g~就可以擁有它了!");
			API.sendChatMessage("~g~還猶豫什麼呢?趕快輸入~y~/bc\n~g~來購買這輛~y~"+API.getVehicleDisplayName(API.getEntityModel(veh))+"~g~!");
			API.sendChatMessage("~g~然後暢遊在洛聖都繁華的街道,讓沒有車的窮小子們羨慕去吧!");
		
	}
	else
	{ 
		var player = API.getLocalPlayer();                
		if(gv==API.getPlayerName(player))
		{
			API.sendChatMessage("~g~車輛控制:X 引擎開關,數字鍵1-6 車門開關,數字鍵:7 修復車輛(需要~y~$"+API.getEntitySyncedData(player, "SC_Repair_Vehicle_Money")+"~g~)");
			if(API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Neon")==1)
			{
				API.sendChatMessage("~g~車輛控制:8 霓虹燈開關");
			}
			API.triggerServerEvent("SC_GETVEHICLETIMELIMIT", veh);
		}
		else
		{
			API.sendChatMessage("~g~車主:~w~"+gv);
		}
	}
	updatacd=Math.round(new Date().getTime())+60*1000;
	updataCarRank()
	inCar=true
}); 

var inCar=false
 
API.onPlayerExitVehicle.connect(function(veh) {
	//離開車輛
	inCar=false
})

var updatacd;

function updataCarRank()
{
	var veh=API.getPlayerVehicle(API.getLocalPlayer());
		var cls=API.getVehicleClass(API.getEntityModel(veh));
	var clscore=25;
	if(cls==6){clscore=75}
	if(cls==7){clscore=125}
	var maxSC=API.returnNative("0xAD7E85FC227197C4",7,veh).toFixed(3);
											//0xA132FB5370554DB0 牽引力
											//0xAD7E85FC227197C4 刹車
											//0x5DD35C8D074E57AE 加速度
											//0x53AF99BAA671CA47 極速
				
	var maxQY=API.returnNative("0xA132FB5370554DB0",7,veh).toFixed(3);
	var maxJS=API.returnNative("0x5DD35C8D074E57AE",7,veh).toFixed(3);	
	var maxSP=API.returnNative("0x53AF99BAA671CA47",7,veh).toFixed(2);	 
	var sr=maxSC*100+maxQY*70+maxJS*215+maxSP*1.5
	sr=parseInt(sr) +clscore;
	API.triggerServerEvent("SC_VehicleMain_VehicleData_RankLevel",veh,sr); 
	if(sr>700){sr="~p~I("+sr+")"}
	if(sr>600&&sr<=700){sr="~y~X("+sr+")"}
	if(sr>500&&sr<=600){sr="~b~S("+sr+")"}
	if(sr>400&&sr<=500){sr="~g~A("+sr+")"}
	if(sr<=400){sr="~c~B("+sr+")"}
	API.triggerServerEvent("SC_VehicleMain_VehicleData_Rank",veh,sr);  
}



API.onUpdate.connect(function(){ 
	if(inCar==true)
	{//在车上
		var f=Math.round(new Date().getTime());
		if(f>=updatacd)
		{
			
			updatacd=f+60*1000;
			updataCarRank()
		}
	}

})

var time=0,ecd=0,d1cd=0,d2cd=0,d3cd=0,d4cd=0,d5cd=0,fixcd=0,d8cd=0;

  API.onKeyDown.connect(function(sender, keyEventArgs) { 
  var player=API.getLocalPlayer()
	if(API.getEntitySyncedData(player, "SC_Login_Status")==1){  
		 
		if(inCar==true){ 
			var veh=API.getPlayerVehicle(player);
			var gv=API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master")
			var f=Math.round(new Date().getTime());
			if(gv!="出售"){
					if(keyEventArgs.KeyCode == Keys.D8&&API.getPlayerVehicleSeat(player)==-1)
						{
							if(f>=d8cd){
						
							
								d8cd=f+1000;
								API.triggerServerEvent("SC_Vehicle_Control_Neon");
	
						}
					}
					if(keyEventArgs.KeyCode == Keys.D7)
						{
						if(f>=fixcd){
						
							if(API.getEntitySyncedData(player,"SC_race_house_start_state")!=1){
								fixcd=f+1000;
								API.triggerServerEvent("SC_Vehicle_NUMBER7_FIX");
							}
						}
					}
					if (keyEventArgs.KeyCode==Keys.X&&API.getPlayerVehicleSeat(player)==-1) {
						
						if(f>=time){
							API.triggerServerEvent("SC_Vehicle_Control_Engine");	
							time=f+1000;		
												
						} 
					}
					if(keyEventArgs.KeyCode==Keys.D1){ 			
						
						if(f>=ecd){
							ecd=f+500;
							API.triggerServerEvent("SC_Vehicle_Control_Door",0);	
									
						}
					}
					if(keyEventArgs.KeyCode==Keys.D2){ 			
						
						if(f>=d1cd){
							d1cd=f+500;
							API.triggerServerEvent("SC_Vehicle_Control_Door",1);	
						}
					}
					if(keyEventArgs.KeyCode==Keys.D3){ 			
						
						if(f>=d2cd){
							d2cd=f+500;
							API.triggerServerEvent("SC_Vehicle_Control_Door",2);	
						}
					}
					if(keyEventArgs.KeyCode==Keys.D4){ 			
						
						if(f>=d3cd){
							d3cd=f+500;
							API.triggerServerEvent("SC_Vehicle_Control_Door",3);	
						}
					}
					if(keyEventArgs.KeyCode==Keys.D5){ 			
						
						if(f>=d4cd){
							d4cd=f+500;
							API.triggerServerEvent("SC_Vehicle_Control_Door",4);	
						}
					}
					if(keyEventArgs.KeyCode==Keys.D6){ 			
						
						if(f>=d5cd){
							d5cd=f+500;
							API.triggerServerEvent("SC_Vehicle_Control_Door",5);	
						}
					}
				}
			}
		}
	});

