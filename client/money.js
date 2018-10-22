var showtime= 	Math.round(new Date().getTime()/1000)+3 ;
var oldmoney=0;
var money=0;
var changemoney=0;
var shotadd=false;
var tips;
var x=API.getScreenResolutionMantainRatio().Width-15
 
 
 
API.onUpdate.connect(function(){ 
	if(showtime!=-1)
	{
		var now=Math.round(new Date().getTime()/1000);
		if(now<showtime)
		{
			//Width,Heightvar 
		 
	 
			var show=false;
			var player=API.getLocalPlayer()
			if(API.hasEntitySyncedData(player,"SC_money")==true){ 
				show=true;
				money=API.getEntitySyncedData(player,"SC_money")
					if(changemoney<0)
					{
						tips="~r~$"+changemoney;
					}else{
						
						tips="~g~$+"+changemoney;
					}
					if(changemoney!=oldmoney){
					if(oldmoney<0)
					{
						tips=tips+"~r~($"+oldmoney+")";
					}else{
						
						tips=tips+"~g~($"+oldmoney+")";
					}
					}
			} 
			
			if(show==true)
			{ 
					if(shotadd==true){
						API.drawText(tips,x, 95, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
					}
	 
				API.drawText("$" + money, x, 50, 1, 115, 186, 131, 255, 4, 2, false, true, 0);
			}
			//var RPM = (API.getVehicleRPM(veh) - 0.2 / 0.8 ) * 10000;
		}else{ 
			showtime=-1
			shotadd=false;
			oldmoney=0;
		}
	}
}); 


 API.onServerEventTrigger.connect(function (eventName, args) {
 
	if(eventName=="SC_money_loginok"){ 
		 showtime= 	Math.round(new Date().getTime()/1000)+3
		 money=parseInt(args[0]);
	}
	if(eventName=="SC_money_updata"){  
		 showtime= 	Math.round(new Date().getTime()/1000)+3
		 money=parseInt(args[0]);
		 oldmoney=oldmoney+parseInt(args[1]);
		 changemoney=args[1]
		 shotadd=true;
	}	
	
})


API.onKeyDown.connect(function(sender, keyEventArgs) { 
		if (keyEventArgs.KeyCode == Keys.Z) {
			showtime=Math.round(new Date().getTime()/1000)+3 
		} 
});
 
//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
