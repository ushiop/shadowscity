var Vehmenu=API.createMenu("選擇車輛",0,0,6);
var linkItem=null
 
 API.onResourceStart.connect(function(){
	linkItem = API.createMenuItem("召喚車輛", "");
	resource.mainMenu.mainmenu.AddItem(linkItem);
	resource.mainMenu.mainmenu.BindMenuToItem(Vehmenu, linkItem);
	resource.mainMenu.menuPool.Add(Vehmenu);
	createCallMenu(null);
});
 
 function AddVehicleListItem(vehicleName,vid,va,vm,vlock){
		var nameItem=API.createMenuItem("#"+vid+" "+vehicleName,"")
		if(vlock==true){
			nameItem.Text=nameItem.Text+" ~r~(出售中..)";
			nameItem.Enabled=false;
		}
		if(va==false){
			nameItem.Text=nameItem.Text+" ~r~(已損壞)"
			nameItem.Description="這輛車已經損壞了,若要召喚\n需要支付~y~$"+(vm*0.02).toFixed(0)+"的修理費用";
		}
		nameItem.Activated.connect(function(menu,item){
			//TRIGGER SERVER GOTO Function
			API.triggerServerEvent("SC_Vehicle_CALL_VEH",vid);
			API.sendNotification("你召喚了一輛 ~g~"+vehicleName); 
		});		
		return nameItem;
 }
 
 
 function createCallMenu(d)
 {
	Vehmenu.Clear();
	if(d!=null){
		
		 for(i in d)
		{ 
			Vehmenu.AddItem(AddVehicleListItem(API.getVehicleDisplayName(API.vehicleNameToModel(d[i]["vName"]+"")),d[i]["vID"],d[i]["vA"],d[i]["vM"],d[i]["vLock"]));
		}
	}

	Vehmenu.RefreshIndex();
 }
 
 API.onServerEventTrigger.connect(function (eventName, args) {
 
	if(eventName=="SC_Vehicle_CALL_LISTUPDATA"){ 
		var js=JSON.parse(args[0]); 
		createCallMenu(js);
	}
})

 
 
 