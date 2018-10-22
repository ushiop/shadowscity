var Vehmenu=API.createMenu("車輛改裝",0,0,6);
var linkItem;
var modName=["擾流板","前保險杠","後保險杠","側裙","排氣","框架","格柵","引擎罩","擋泥板","右側擋泥板","頂部","發動機","製動器","傳輸","喇叭","懸掛","護甲","渦輪","疝氣燈","板持有人(什麼鬼)","裝飾設計(??)","飾品(??)","錶盤設計(?)","方向盤(?)","換擋杆(?)","斑塊(??)","液壓傳動(??)","塗裝(部分車輛有效)","主要顏色","次要顏色","輪胎","輪胎類型","輪胎顏色","板(??)","窗戶色"];
var modId=[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,18,22,25,27,28,30,33,34,35,38,48,10000,10001,23,10006,10007,62,69];
var modMax=[30,30,30,30,30,30,30,30,30,30,30,5,5,5,51,5,5,1,1,50,50,50,50,50,50,50,5,30,157,157,57,25,157,50,5];
var carModList={};
var oldModList={};
var time=-1;
var button; 
var change=false;
var matserme=false;
var blip,mark,lab;
var money=0;
var moneyl={};
var slotlist=null;

 API.onResourceStart.connect(function(){
	blip=API.createBlip(new Vector3(-360.71038818359375,-114.17789459228516,33.02218246459961));
	API.setBlipSprite(blip,72);
	mark=API.createMarker(1,new Vector3(-360.71038818359375,-114.17789459228516,33.02218246459961 ),new Vector3(),new Vector3(),new Vector3(40,40,200),0,64,128,32); 
	lab= API.createTextLabel("按~g~G~w~打開改裝菜單\n無效則請~r~下車後\n~g~從駕駛位上車~w~再試",new Vector3(-368.7769,-118.7935,43.19925 ),100,1,false); 
	resource.mainMenu.menuPool.Add(Vehmenu);
 
	for(i in modId)
	{ 
		carModList[modId[i]]=-1;
		oldModList[modId[i]]=-1;
	}

	 
	Vehmenu.OnMenuClose.connect(function(menu){
		//结束改装，恢复到旧样式
		API.triggerServerEvent("SC_vehicle_changemod_freeze",false);	
		
		API.setMenuSubtitle(Vehmenu,"車輛改裝");	 
			money=0;
			moneyl={};
		carModList=copy(oldModList); 
		for(i in carModList)
		{  
			setVehMod(null,i,carModList[i]);
		}
		change=false;
		createModMenu();
	})
	
		createModMenu();
});
 

 function copy(list)
 {
	var obj=JSON.parse(JSON.stringify(list));
	return obj;
 }
 
  function AddVehicleModListItem(modName,modId,fid){
		 var list=new List(String);

			for(var z=0;z<modMax[fid]+1;z++)
			{
				list.Add(z.toString());
			}
			var select=0;
			if(carModList[modId]!=-1)
			{
				 
				select=carModList[modId];
			}else{
				select=0;
			} 
			 
			var modItem=API.createListItem(modName,"改裝完畢後請點‘確定改裝’\n否則其他玩家無法看見你的改裝",list,select);
			if(modId==10006){
				modItem.Enabled=false; 
				modItem.Description="該改裝項目暫時無法使用";
				//服务端暂时无法修改车轮类型，暂时禁用
			}
 
			modItem.OnListChanged.connect(function(menu,item){
				//TRIGGER SERVER GOTO Function 
				//API.sendNotification("你選擇改裝了 ~g~"+modName+" ~w~ ID:"+modId+" "+item.Index);
				//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
				//setVehicleSecondaryColor 設置車輛副色(同上)
				if(modId==11||modId==12||modId==13){
					
					moneyl[modId]=20000;
				}else{
					if(modId==18){
						moneyl[modId]=50000;
					}else{
						moneyl[modId]=1000;
					}
				} 
				setVehMod(null,modId,menu.Index)
				carModList[modId]=menu.Index;
				
				getModMoney();
			});	
	
		return modItem;  
 }

 
 function setVehMod(veh,modId,Index)
 { 
	modId=parseInt(modId); 
	if(veh==null){veh=API.getPlayerVehicle(API.getLocalPlayer())}
	 			if(modId<1000){
					if(modId==22||modId==18){
						if(Index==-1)
						{
							Index=0;
						}
					}
					API.setVehicleMod(veh,modId,Index);
				}else{ 
					if(Index==-1)
					{
						Index=0;
					}
					if(modId==10000){
						API.setVehiclePrimaryColor(veh,Index);
					}
					if(modId==10001){
						API.setVehicleSecondaryColor(veh,Index);// 設置車輛副色(同上)
					}
					if(modId==10006){
						//輪胎類型
						API.setVehicleWheelType(veh,Index);
					}
					if(modId==10007){
						API.setVehicleWheelColor(veh,Index);
					}
					
				}
 }
  
  
function getModMoney()
{
	money=0;
	for(var i in moneyl)
	{  
 
		if(oldModList[i]!=carModList[i]){
			money=money+moneyl[i];
		}
	}
 
}
  
  
  function AddExItem(z){
			var lk=API.createMenuItem("清空插件:"+z,"~r~不可恢复"); 
			Vehmenu.AddItem(lk);
			lk.Activated.connect(function(menu,item){	 
				API.triggerServerEvent("SC_vehicle_changemod_clear_ex!",z);		
			})  
  }
  
  function createModMenu()
  {
	  
	Vehmenu.Clear();
	 for(var i=0;i<modName.length;i++)
	{
		Vehmenu.AddItem(AddVehicleModListItem(modName[i],modId[i],i));
	} 
	if(slotlist!=null){
	for(var z in slotlist){
		if(slotlist[z]!=""){
			AddExItem(z)
		}
	}
	}

	button=API.createMenuItem("確定改裝","");
	Vehmenu.AddItem(button);
	button.Activated.connect(function(menu,item){
		var js=JSON.stringify(carModList);
		oldModList=copy(carModList);
		API.sendNotification("你的~g~改裝申請~w~已發送.");	
		API.triggerServerEvent("SC_vehicle_changemod_go!",js,money);		
		time=Math.round(new Date().getTime()/1000)+20;
		button.Enabled=false;
		button.Description="請不要頻繁提交改裝申請(剩餘20秒)";
	})
	Vehmenu.RefreshIndex();
  }
 
API.onPlayerEnterVehicle.connect(function(veh) {
	//進入車輛
	var gv=API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master")
	if(gv==API.getPlayerName(API.getLocalPlayer())){
		if(API.getPlayerVehicleSeat(API.getLocalPlayer())==-1)
		{
			/*API.sendChatMessage("~g~嘿!想要改裝你的愛車嗎?洛聖都改車廠重新開業啦!");
			API.sendChatMessage("~g~趕緊去小地圖上的噴漆點處按~r~G~g~進行改裝吧!");
			API.sendChatMessage("~g~如果在噴漆點內按~r~G~g~打不開改裝菜單,請下車後從駕駛位車門上車");*/
			matserme=true;
		}
	} 
}); 

API.onPlayerExitVehicle.connect(function(veh) {
	//離開車輛
	
	matserme=false		
	if(Vehmenu.Visible==true){
		
		Vehmenu.Visible=false;
		resource.mainMenu.mainmenu.Visible=true;
		} 
		if(change==true){
			money=0;
			moneyl={};
		change=false
			carModList=copy(oldModList); 
			
		API.setMenuSubtitle(Vehmenu,"車輛改裝");
		API.triggerServerEvent("SC_vehicle_changemod_freeze",false);	
			if(API.getEntityModel(veh) !=0){
				var gv=API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master")
				if(gv==API.getPlayerName(API.getLocalPlayer())){
					for(i in carModList)
					{   
						setVehMod(veh,i,carModList[i]);
					}
				}
			}
		createModMenu(); 
		}
			//结束改装，恢复到旧样式
		// 		API.triggerServerEvent("SC_vehicle_changemod_freeze",false);
				 
		
});


API.onUpdate.connect(function(){ 


	if(change==true){
			var nowtime=Math.round(new Date().getTime()/1000);
			 if(nowtime>=time){	  
		time=-1;
		button.Enabled=true;
		button.Description="";
	}else{
		var s=(time-nowtime); 
		//button.Description=">"+time+"\n>"+nowtime;
		button.Description="請不要頻繁提交改裝申請(剩餘"+s.toString()+"秒)";
	}
	if(time==-1)
	{
		if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_money")>=money&&money!=0)
		{
			button.Enabled=true; 
		}else{
			button.Enabled=false; 
		} 
		button.Description="本次改裝需要花費~y~$"+money;
		API.setMenuSubtitle(Vehmenu,"本次改裝需要花費~y~$"+money);
	}
		var player=API.getLocalPlayer();
		var veh=API.isPlayerInAnyVehicle(player);
		if(veh){ 
		
			veh=API.getPlayerVehicle(player);
			var maxSP=API.returnNative("0xAD7E85FC227197C4",7,veh);
										//0xA132FB5370554DB0 牽引力
										//0xAD7E85FC227197C4 刹車
										//0x5DD35C8D074E57AE 加速度
										//0x53AF99BAA671CA47 極速
			var size=API.getScreenResolutionMantainRatio();
			
			API.drawText("煞車:"+maxSP.toFixed(4),size.Width-700,(size.Height*0.3)+100,0.7,255,255,255,255,0,0,true,true,0);
			maxSP=API.returnNative("0xA132FB5370554DB0",7,veh);
			API.drawText("牽引:"+maxSP.toFixed(4),size.Width-700,(size.Height*0.3)+140,0.7,255,255,255,255,0,0,true,true,0);
			maxSP=API.returnNative("0x5DD35C8D074E57AE",7,veh);	
			API.drawText("加速:"+maxSP.toFixed(4),size.Width-700,(size.Height*0.3)+180,0.7,255,255,255,255,0,0,true,true,0);
			maxSP=API.returnNative("0x53AF99BAA671CA47",7,veh);	
			API.drawText("極速:"+maxSP.toFixed(2),size.Width-700,(size.Height*0.3)+220,0.7,255,255,255,255,0,0,true,true,0);
		}
	}
}); 

 API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_vehicle_changemod_logined_json"){ 
			API.setMenuSubtitle(Vehmenu,"車輛改裝");
			slotlist=null;
			for(i in modId)
			{ 
				carModList[modId[i]]=-1;
				oldModList[modId[i]]=-1;
			} 
			money=0;
			moneyl={};
			if(args[0]!=""){
				pl=JSON.parse(args[0]);
				var veh=API.getPlayerVehicle(API.getLocalPlayer());
				for(var i in pl)
				{ 
					carModList[i]=pl[i] ;
					
					oldModList[i]= pl[i];
					
					setVehMod(veh,i,pl[i]);
				}
			}
			if(args[1]!=""){
				slotlist=JSON.parse(args[1]);
			}
			
		}
 });
 
 API.onKeyDown.connect(function(sender, keyEventArgs) {

		if (keyEventArgs.KeyCode == Keys.G) {
				if(API.getEntitySyncedData(API.getLocalPlayer(), "SC_Login_Status")==1){
			 
				if(API.isInRangeOf(API.getEntityPosition(API.getLocalPlayer()),new Vector3(-360.71038818359375,-114.17789459228516,33.02218246459961 ),22))
				{
			//-378.43505859375,-120.35971069335938,38.01585388183594 
					if(matserme==true){
						if(resource.menuOpen.MenuOpen()==true){  
							createModMenu();
							Vehmenu.Visible =true;
								//开始改装，冻结
							API.triggerServerEvent("SC_vehicle_changemod_freeze",true);		
							oldModList=copy(carModList);
							change=true;
						}
					}
				}
		}
	}
});