var Vehmenu=API.createMenu("賽事大廳",0,0,6);
var Housemenu=API.createMenu("房間",0,0,6);
var houseName="",houseIndex="",houseCreate="",houseRid="",houseTopPlayer="",houseTopTime="";
var startrace,masterbut=null;
var cps;//CP點列表
var startTime=null,nowCP,maxCP; //開始時間,當前CP點,最大CP點
var nextCPPoint,nowCPPoint=null;
var finishRaceT=null;
var PlayerList=null,RankList=null;
var size=API.getScreenResolutionMantainRatio();	
 API.onResourceStart.connect(function(){
	var linkItem = API.createMenuItem("賽事大廳", "");
	resource.mainMenu.mainmenu.AddItem(linkItem); 
	resource.mainMenu.menuPool.Add(Vehmenu);
	resource.mainMenu.menuPool.Add(Housemenu);
	linkItem.Activated.connect(function(menu,item){
		if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_race_house_state")==1)
			{  
				resource.mainMenu.mainmenu.Visible=false;
				Housemenu.Visible =true;
			}else{
				resource.mainMenu.mainmenu.Visible=false;
				//createHouse(null);
				Vehmenu.Visible =true;
			}
 
	});
	Vehmenu.OnMenuClose.connect(function(menu){ 
		resource.mainMenu.mainmenu.Visible=true;
	});
	Housemenu.OnMenuClose.connect(function(menu){ 
		resource.mainMenu.mainmenu.Visible=true;
	});	
	createHouse(null);
	Vehmenu.RefreshIndex();
	
});


function createHouse(list)
{
			Vehmenu.Clear();
			//API.sendChatMessage("@@")
			var linkItem = API.createMenuItem("創建比賽", "選擇一個賽道來創建比賽吧!");
			Vehmenu.AddItem(linkItem);
			Vehmenu.BindMenuToItem(resource.raceroad.Vehmenu, linkItem);  
			linkItem=API.createMenuItem("刷新比賽列表", "若自動刷新被關閉,則需手動刷新");
			linkItem.Activated.connect(function(menu,item){
				API.triggerServerEvent("SC_race_house_get_listchange");
			})
			Vehmenu.AddItem(linkItem);
			var auto=true;
			if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_CONFIG:RACEHOUSE_AUTO_LISTUPDATA")==false)
			{
				auto=false;
			}
			var lj=API.createCheckboxItem("自動刷新","自動刷新比賽列表\n在比賽量極大的情況下建議關閉",auto)	
			lj.CheckboxEvent.connect(function (item, newChecked) {
				API.triggerServerEvent("SC_CONFIG_PLAYER_UPDATA","RACEHOUSE_AUTO_LISTUPDATA",newChecked);
			});
			Vehmenu.AddItem(lj);
			if(list!=null)
			{
				//API.sendChatMessage(list.length+"")
				for(var v in list)
				{
						Vehmenu.AddItem(AddHouseListItem(list[v]["Name"],list[v]["Master"],list[v]["Race"],list[v]["Hid"]));
				}
			}
}

function AddRaceHouseItem(hName,hIndex,hCreatePlayer,hRaceName,hPlayerList,hTopp,hTopt,listname,listname1){
		Housemenu.Clear(); 
		var nameItem=API.createMenuItem(hName,"賽道    :"+hRaceName+"\n房間ID:"+hIndex);
		nameItem.Enabled=false;
		Housemenu.AddItem(nameItem);
		nameItem=API.createMenuItem("房主:"+hCreatePlayer,"");
		masterbut=nameItem;
		nameItem.Enabled=false;
		Housemenu.AddItem(nameItem);
		if(hTopt!="SC_NULL"){
			nameItem=API.createMenuItem("賽道記錄 ~y~"+hTopp,"~w~時間 ~y~"+(hTopt/1000)+"s");
			nameItem.Enabled=false;
			Housemenu.AddItem(nameItem);
		}	 
		nameItem=API.createMenuItem("開始比賽","立即開始比賽\n開始後他人無法加入");
		startrace=nameItem;
		Housemenu.AddItem(nameItem);
		if(API.getPlayerName(API.getLocalPlayer())!=hCreatePlayer){
			nameItem.Enabled=false;
		}
		nameItem.Activated.connect(function(menu,item){ 
			startrace.Enabled=false;
			Housemenu.Visible=false;
			API.triggerServerEvent("SC_race_house_start");
		});
		nameItem=API.createMenuItem("退出房間","退出比賽房間\n如果你是房主,房主將移至下一位玩家");
		Housemenu.AddItem(nameItem);
		nameItem.Activated.connect(function(menu,item){ 
									//離開比賽房間后啓用召喚車輛和傳送
									resource.vehicle_call.linkItem.Enabled=true;
									resource.vehicle_call.linkItem.Description="";
									resource.playerlist.linkItem.Enabled=true;
									resource.playerlist.linkItem.Description=""
			houseName="",houseIndex="",houseCreate="",houseRid="",houseTopPlayer="",houseTopTime="";
			if(nowCPPoint!=null){nowCPPoint.deleteCP();}
			if(nextCPPoint!=null){nextCPPoint.deleteCP();}
			nowCPPoint=null;
			nextCPPoint=null;
			Housemenu.Visible=false;
			startrace=null;
			masterbut=null;
			cps=null;
			startTime=null;
			nowCP=null;
			finishRaceT=null;
			PlayerList=null;
			RankList=null;
			API.triggerServerEvent("SC_race_house_quit");
			
				if(resource.menuOpen.MenuOpen()==true){
					Vehmenu.Visible=true;
				}
		});
		//玩家列表
		if(listname1!=null)
		{
			nameItem=API.createMenuItem("----Rank List----","");
			nameItem.Enabled=false;
			Housemenu.AddItem(nameItem);		
			for(var i in listname1)
			{			 
				nameItem=API.createMenuItem(listname1[i],"");
				nameItem.Enabled=false;
				Housemenu.AddItem(nameItem);
	 
			} 			
		}
		if(hPlayerList!=null)
		{
			nameItem=API.createMenuItem("----Player List----","");
			nameItem.Enabled=false;
			Housemenu.AddItem(nameItem);		
			for(var i in hPlayerList)
			{			 
				nameItem=API.createMenuItem(hPlayerList[i],"");
				nameItem.Enabled=false;
				Housemenu.AddItem(nameItem);
	 
			} 
		}

		resource.raceroad.Vehmenu.Visible=false;
		Vehmenu.Visible=false;
		if(finishRaceT!=null){
			startrace.Enabled=false;
			if(finishRaceT==true){
				Housemenu.CurrentSelection=4;
				if(resource.menuOpen.MenuOpen()==true){
					Housemenu.Visible=true;
				}
			}
		}
}

function AddHouseListItem(hName,hMaster,hRace,hID)
{ 
	var nameItem=API.createMenuItem(hName,"賽道:"+hRace+"\n房主:"+hMaster);
	nameItem.Activated.connect(function(menu,item){
		API.triggerServerEvent("SC_race_house_join_house",hID);
	});
	return nameItem;
}
 
 API.onServerEventTrigger.connect(function (eventName, args) {
	 
	if(eventName=="SC_race_houseinfo_updata"){
		//房間名、房間ID、房間主人名、房間玩家列表名、賽道名、記錄名、記錄時間
		houseName=args[0],houseIndex=args[1],houseCreate=args[2],houseRid=args[4],houseTopPlayer=args[5],houseTopTime=args[6];
		//"----Player List----"
		PlayerList=JSON.parse(args[3]);
		AddRaceHouseItem(houseName,houseIndex,houseCreate,houseRid,PlayerList,houseTopPlayer,houseTopTime,PlayerList,RankList);
				if(resource.menuOpen.MenuOpen()==true){
					Housemenu.Visible=true;
				}		
	} 
 
 if(eventName=="SC_race_houseinfo_playerlist_updata"){
	 
		 PlayerList=JSON.parse(args[0]);
		 AddRaceHouseItem(houseName,houseIndex,houseCreate,houseRid,PlayerList,houseTopPlayer,houseTopTime,PlayerList,RankList);
 }
 if(eventName=="SC_race_houseinfo_ranklist_updata"){
	 RankList=JSON.parse(args[0]);
	 AddRaceHouseItem(houseName,houseIndex,houseCreate,houseRid,PlayerList,houseTopPlayer,houseTopTime,PlayerList,RankList);
 }
	if(eventName=="SC_race_house_listchange"){
		
		if(args[1]=="手動刷新"||API.getEntitySyncedData(API.getLocalPlayer(),"SC_CONFIG:RACEHOUSE_AUTO_LISTUPDATA")==true)
		{
			var list=JSON.parse(args[0]); 
			createHouse(list);
		}
	} 
	if(eventName=="SC_race_houseinfo_housemaster_updata"){
		var a=args[0];
		houseCreate=args[0];
		if(finishRaceT==null){
			if(a==API.getPlayerName(API.getLocalPlayer())){
				startrace.Enabled=true;
			}
		}
		if(masterbut!=null){
			masterbut.Text="房主:"+a;
		}
		API.triggerServerEvent("SC_race_house_getlist");
	}
	if(eventName=="SC_race_houseinfo_CP_updata"){
		cps=JSON.parse(args[0]);
		maxCP=cps.length;
		nowCP=0;
		if(maxCP!=1){
			nowCPPoint=new checkPoint(1,new Vector3(cps[0]["X"],cps[0]["Y"],cps[0]["Z"]),new Vector3(cps[0]["nX"],cps[0]["nY"],cps[0]["nZ"]),false,1);
			if(maxCP==2){
				
				nextCPPoint=new checkPoint(4,new Vector3(cps[1]["X"],cps[1]["Y"],cps[1]["Z"]),null,false,2);
			}else{
				nextCPPoint=new checkPoint(1,new Vector3(cps[1]["X"],cps[1]["Y"],cps[1]["Z"]),new Vector3(cps[1]["nX"],cps[1]["nY"],cps[1]["nZ"]),false,2);

			}
		}else{
			nowCPPoint=new checkPoint(4,new Vector3(cps[0]["X"],cps[0]["Y"],cps[0]["Z"]),null,false,1);
		} 
	}
	if(eventName=="SC_race_house_startrace"){
		
		/*
		比賽時禁用召喚車輛和傳送
		*/
									resource.vehicle_call.linkItem.Enabled=false;
									resource.vehicle_call.linkItem.Description="處於比賽狀態時該功能不允許使用";
									if(resource.vehicle_call.Vehmenu.Visible==true){
										resource.vehicle_call.Vehmenu.Visible=false;
									}
									
									resource.playerlist.linkItem.Enabled=false;
									resource.playerlist.linkItem.Description="處於比賽狀態時該功能不允許使用"
									if(resource.playerlist.Gomenu.Visible==true){
										resource.playerlist.Gomenu.Visible=false;
									}
									
		if(Housemenu.Visible==true){
			Housemenu.Visible=false;
		}
		finishRaceT=false;
		nowCPPoint.startCop();
	} 
	if(eventName=="SC_race_houseinfo_close"){
									//離開比賽房間后啓用召喚車輛和傳送
									resource.vehicle_call.linkItem.Enabled=true;
									resource.vehicle_call.linkItem.Description="";
									resource.playerlist.linkItem.Enabled=true;
									resource.playerlist.linkItem.Description=""
		
			houseName="",houseIndex="",houseCreate="",houseRid="",houseTopPlayer="",houseTopTime=""; 
			if(nowCPPoint!=null){nowCPPoint.deleteCP();}
			if(nextCPPoint!=null){nextCPPoint.deleteCP();}
			nowCPPoint=null;
			nextCPPoint=null;
			Housemenu.Visible=false;
			startrace=null;
			masterbut=null;
			cps=null;
			startTime=null;
			nowCP=null;
			finishRaceT=null;
			PlayerList=null;
			RankList=null;
			if(args[0]!="system"){
				API.triggerServerEvent("SC_race_house_quit");
			}
	}
 });
  
  function finishRace(){
	//比賽完成！！上傳結果至服務器
	API.triggerServerEvent("SC_race_house_finisRace");
	finishRaceT=true;
  }
  //nextCPPoint,nowCPPoint;
  
  function checkPoint(type,pos,topos,enabled,cpindexsss){
	  var cp=new Object();
	  var mak=API.createMarker(1,new Vector3(pos.X,pos.Y,pos.Z-4),new Vector3(),new Vector3(),new Vector3(10,10,60),255, 255,128,128); 
	  cp.Mark=mak;
	  var blips=API.createBlip(pos);
      API.setBlipColor(blips, 66);
	  API.setBlipScale(blips, 0.6);
	  cp.Blips=blips;
	  cp.Enabled=enabled; 
	  cp.Next=false;
	  cp.Pos=pos;
	  cp.Type=type;
	  cp.Index=cpindexsss;
	 if(cpindexsss==maxCP){ 
		  var nextmak=API.createMarker(1,new Vector3(pos.X,pos.Y,pos.Z-4),new Vector3(),new Vector3(),new Vector3(10,10,60),255, 0,0,196); 
		  cp.NextPos=nextmak;
		  cp.Next=true;
		}
	  if(topos!=null)
	  { 
		
	  } 
	
	  
	  cp.deleteCP=function()
	  {
		  
					API.deleteEntity(this.Mark)
					API.deleteEntity(this.Blips)
					if(this.Next==true)
					{
						API.deleteEntity(this.NextPos);
					} 
	  }
	  
	  cp.startCop=function()
	  { 
	    //247, 57, 180
		this.Enabled=true;
		API.setMarkerColor(this.Mark,125,255,128,0);
	  }
	  
	   
	  
	  return cp;
  }
   API.onUpdate.connect(function(){ 
		if(nowCPPoint!=null){
			if(nowCPPoint.Enabled==true){	
					if(API.isInRangeOf(API.getEntityPosition(API.getLocalPlayer()),nowCPPoint.Pos,7)==true)
						{ 
							
							API.playSoundFrontEnd("HUD_MINI_GAME_SOUNDSET", "CHECKPOINT_NORMAL");
							if(nowCPPoint.Index==maxCP)
							{
								
								if(nowCPPoint!=null){nowCPPoint.deleteCP();}
								if(nextCPPoint!=null){nextCPPoint.deleteCP();}
								nowCPPoint=null;
								nextCPPoint=null;
								finishRace();							
							}else{ // 4(0,1,2,3)  
								//		1,2,3,4
								var cpindex=nowCPPoint.Index-1;
								nowCPPoint.deleteCP();
								nowCPPoint=nextCPPoint;
								nowCPPoint.startCop();
								nextCPPoint=null; 
								if(cpindex+2<maxCP)
								{
									
									nextCPPoint=new checkPoint(1,new Vector3(cps[cpindex+2]["X"],cps[cpindex+2]["Y"],cps[cpindex+2]["Z"]),null,false,cpindex+3);
								}
							}
							 
						} 
					if(nowCPPoint!=null){
						if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_RADAR_BIGMAP_ENABLED")!=true){
							API.drawText("檢查點",size.Width*0.17+25,size.Height-100,0.7,255,255,255,255,0,0,true,true,0);
							API.drawText((nowCPPoint.Index-1)+"/"+(maxCP),size.Width*0.17+40,size.Height-130,0.6,255,255,255,255,0,0,true,true,0);
						}else{
							API.drawText("檢查點",size.Width*0.27+25,size.Height-100,0.7,255,255,255,255,0,0,true,true,0);
							API.drawText((nowCPPoint.Index-1)+"/"+(maxCP),size.Width*0.27+40,size.Height-130,0.6,255,255,255,255,0,0,true,true,0);
						}
					}
					var player=API.getLocalPlayer();
					veh=API.isPlayerInAnyVehicle(player);
					if(!veh){   
						houseName="",houseIndex="",houseCreate="",houseRid="",houseTopPlayer="",houseTopTime="";
						if(nowCPPoint!=null){nowCPPoint.deleteCP();}
						if(nextCPPoint!=null){nextCPPoint.deleteCP();}
						nowCPPoint=null;
						nextCPPoint=null;
						Housemenu.Visible=false;
						startrace=null;
						masterbut=null;
						cps=null;
						startTime=null;
						finishRaceT=null;
						nowCP=null;
						PlayerList=null;
						RankList=null;
						API.triggerServerEvent("SC_race_house_nocar_quit");
					}
			}
		};
	});


 
 