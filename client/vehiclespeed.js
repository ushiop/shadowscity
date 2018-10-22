
var size=API.getScreenResolutionMantainRatio();
var x=size.Width*0.68
var y=size.Height-140
var y1=size.Height-90
 API.onResourceStart.connect(function(){
	 
	
});


var inCar=false
API.onPlayerEnterVehicle.connect(function(veh) {
	//進入車輛
	inCar=true
}); 

API.onPlayerExitVehicle.connect(function(veh) {
	//離開車輛
	inCar=false
})

var changecd=0,show_speed,show_prm
API.onUpdate.connect(function(){ 
	if(inCar){ 
		var nowtime=Math.round(new Date().getTime()); 
		if(nowtime>=changecd)
		{
			changecd=nowtime+200;
			var player=API.getLocalPlayer();
			var veh=API.getPlayerVehicle(player);
			
			//Width,Height
			var speed=API.getEntityVelocity(veh);
			speed=Math.sqrt(speed.X * speed.X +speed.Y * speed.Y +speed.Z * speed.Z)*3.6;
			show_speed=parseInt(speed).toString()+"KM/H"
			//var RPM = (API.getVehicleRPM(veh) - 0.2 / 0.8 ) * 10000;
			var RPM = ((API.getVehicleRPM(veh) -0.2) / 0.8) * 10000;
			if(RPM<0){RPM=0}
			if(RPM>=7000){ 
				show_prm="~r~"+parseInt(RPM).toString()+"RPM"
			}else{
				show_prm=parseInt(RPM).toString()+"RPM"
			} 
		}
			API.drawText(show_speed,x,y,1,255,255,255,255,0,0,true,true,0);
			API.drawText(show_prm,x,y1,1,255,255,255,255,0,0,true,true,0);
			
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
