var Vehmenu=API.createMenu("個性設置",0,0,6); 
var open=false
var size=API.getScreenResolutionMantainRatio();
var dx_w=5,dx_h=0,dx_maxh=450
var select_index=0;

var portrait_max=7;//頭像數量上限
var portrait_gui=null;

 API.onResourceStart.connect(function(){
	var linkItem = API.createMenuItem("個性設置", "");
	resource.mainMenu.mainmenu.AddItem(linkItem);
	resource.mainMenu.mainmenu.BindMenuToItem(Vehmenu, linkItem);
	resource.mainMenu.menuPool.Add(Vehmenu); 
	linkItem.Activated.connect(function(menu,item){
		createPayMenu()
		open=true
	})
	Vehmenu.OnMenuClose.connect(function(menu){
		open=false
	})
	createPayMenu()
});



 
 
 
 function createPayMenu()
 {
	Vehmenu.Clear();
	var list=new List(String);
	for(var z=0;z<portrait_max;z++)
	{
		list.Add(z.toString());
	} 
	portrait_gui=API.createListItem("設置頭像","頭像可顯示在賽道記錄,個人信息中\n回車確認頭像",list,	API.hasEntitySyncedData(API.getLocalPlayer(),"SC_USERINFO:PORTRAIT")==true ? API.getEntitySyncedData(API.getLocalPlayer(),"SC_USERINFO:PORTRAIT") : 0);
	Vehmenu.AddItem(portrait_gui); 

	portrait_gui.Activated.connect(function(menu,item){
		API.triggerServerEvent("SC_USERINFO_PLAYER_UPDATA","PORTRAIT",item.Index);
	})

	Vehmenu.OnIndexChange.connect(function(menu,item){
		//被选择时显示物品
		//瀏覽,未製作
		if(menu.CurrentSelection==0)
		{
			open=true
		}else{
			open=false;
		}
	})
	var auto=true;
	if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_CONFIG:SHOW_RACE_MISSION")==false)
	{
		auto=false;
	}
	var lj=API.createCheckboxItem("顯示賽事任務信息框","顯示右邊的賽事任務信息框\n取消勾選可屏蔽顯示,但任務仍然有效",auto)	
	lj.CheckboxEvent.connect(function (item, newChecked) {
		API.triggerServerEvent("SC_CONFIG_PLAYER_UPDATA","SHOW_RACE_MISSION",newChecked);
	});
	Vehmenu.AddItem(lj);
	Vehmenu.RefreshIndex();
 }
 
API.onUpdate.connect(function(){ 
		if(open==true){
			var x=parseInt((size.Width-735))
			var y=parseInt((size.Height*0.258))
			var index=portrait_gui.Index;
			API.drawRectangle(x,y,300,400,0,0,0,195);	
			API.dxDrawTexture("res/img/portrait/"+index+".png",new Point(x+150-90,y-30),new Size(180,180),0 )
			API.drawText(API.getPlayerName(API.getLocalPlayer()),x+150,y+150, 0.4, 255,255,255,195, 6,1, false, true, 0);
		}
})

//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
