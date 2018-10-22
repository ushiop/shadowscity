
 //客戶端所有菜單判斷均在此，如果有某一個菜單在打開狀態則返回false
function MenuOpen(){
	if(resource.mainMenu.mainmenu.Visible==true||
	   resource.playerlist.Gomenu.Visible==true||
	   resource.vehicle_call.Vehmenu.Visible==true||
	   resource.track.Gomenu.Visible==true||
	   resource.vehiclechangemod.Vehmenu.Visible==true||
	   resource.raceroad.Vehmenu.Visible==true||
	   resource.racehouse.Vehmenu.Visible==true||
	   resource.edittrack.Gomenu.Visible==true||
	   resource.racehouse.Housemenu.Visible==true||
	   resource.acl.Gomenu.Visible==true||
	   resource.clothes.Vehmenu.Visible==true||
	   resource.clothes.Clomenu.Visible==true||
	   resource.item.Vehmenu.Visible==true||
	   resource.pay.Vehmenu.Visible==true||
	   resource.pay.Sellmenu.Visible==true||
	   resource.car_shop.show==true||
	   resource.config.Vehmenu.Visible==true||
	   resource.team.show==true||
	   resource.team_top.show==true||
	   resource.team_yaoqing.show==true||
	   resource.team_yaoqing.show1==true){
		return false;
	}else{
		return true;
	}
}
 
//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
