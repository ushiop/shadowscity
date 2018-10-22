var Vehmenu=API.createMenu("交易大廳",0,0,6);
var Sellmenu=API.createMenu("選擇要出售的物品",0,0,6);
var linkItem=null
var sellCar=null,sellName="出售清單",sellMoney=1;
var sellList=[],sellState=false,upbut;
var open=null,show=false,show_money=0,show_name="",show_create="",show_item=[],show_nowid=0
var size=API.getScreenResolutionMantainRatio();
var dx_w=5,dx_h=0,dx_maxh=450
var slist=[];
var one=0;
 API.onResourceStart.connect(function(){
	linkItem = API.createMenuItem("交易大廳", "");
	resource.mainMenu.mainmenu.AddItem(linkItem);
	resource.mainMenu.mainmenu.BindMenuToItem(Vehmenu, linkItem);
	resource.mainMenu.menuPool.Add(Vehmenu); 
	resource.mainMenu.menuPool.Add(Sellmenu);
	Sellmenu.OnMenuClose.connect(function(menu){
		
		show_nowid=0
		sellState=false
		open=false
		Vehmenu.Visible=true;
	})
	linkItem.Activated.connect(function(menu,item){
		Vehmenu.RefreshIndex();
		if(one==0)
		{
			one=1;
			//createPayMenu(null)
		}
	})
	Vehmenu.OnMenuClose.connect(function(menu){
			show_nowid=0
			show_money=0
			show_name=""
			show_create=""
			show_item=[]
			open=false
	})
	createPayMenu(null)
});

 function AddListMenuItem(cname,name,money,id,slist){
		var tips="按回車購買,立即生效不可撤銷"
		if(cname==API.getPlayerName(API.getLocalPlayer())){
			tips="按回車撤銷此清單,立即生效不可撤銷"
		}
		var nameItem=API.createMenuItem(name,tips)

		nameItem.Activated.connect(function(menu,item){
			//購買 未製作
			if(API.getPlayerName(API.getLocalPlayer())==cname){
				API.triggerServerEvent("SC_PAY_SELL_QUIT",id);
			}else{
				API.triggerServerEvent("SC_PAY_SELL_BUY",id);
			}
			item.Enabled=false;
		})
		return nameItem;
 }

 function AddSellMenuItem(name,id,pz,msg,type,value){
	
		var nameItem=API.createMenuItem(pz+" ~o~"+name,"類型:"+type+"\n"+msg)
		nameItem.Activated.connect(function(menu,item){
			
			//TRIGGER SERVER GOTO Function
			upbut.Enabled=true;
			item.Enabled=false;
			item.Text=item.Text+" ~r~已添加至清單";
			sellList[sellList.length]=new itemInfo(name,pz,id,msg,type,value);
			show_money=sellMoney
			show_name=sellName
			show_create=API.getPlayerName(API.getLocalPlayer())
			show_item=sellList
			open=true
			//API.triggerServerEvent("SC_ITEM_USE",id);
			//API.sendNotification("你召喚了一輛 ~g~"+vehicleName); 
		});		 
		return nameItem;
 }
 
 
  function showSellMenu(d){ 
	sellList=[]; 
	Sellmenu.Clear();
	Sellmenu.Visible=true;
	
	var l=API.createMenuItem(sellName,"回車可更改出售清單的名字")

	l.Activated.connect(function(menu,item){
		var pn=API.getUserInput(sellName,8);
		if(pn!=sellName&&pn.length!=0){
			sellName=pn;
			item.Text=sellName;
		}
	})
	Sellmenu.AddItem(l);
	l=API.createMenuItem("出售價格:$"+sellMoney,"回車可設置出售價格,默認為1塊\n請務必修改!!")
	l.Activated.connect(function(menu,item){
		var pn=API.getUserInput(sellMoney+"",9);
		if(pn>0&&pn!=sellMoney){
			sellMoney=pn;
			item.Text="出售價格:$"+sellMoney;
		}
	})
	Sellmenu.AddItem(l);
	l=API.createMenuItem("放棄該清單","退出清單編輯,放棄本次清單的申請.")
	Sellmenu.AddItem(l);
	l.Activated.connect(function(menu,item){
		//放棄
		list=null;
		sellMoney=1
		sellName="出售清單";
		sellCar=null
		show_nowid=0
		sellState=false
		open=false
		Sellmenu.Visible=false;
		Vehmenu.Visible=true;		
		resource.item.linkItem.Description="";
		resource.item.linkItem.Enabled=true;
	})
	l=API.createMenuItem("提交該清單","將該清單提交至服務器\n其他玩家可以瀏覽並購買\n注:你下線後清單自動撤銷")
	l.Enabled=false;
	upbut=l;
	Sellmenu.AddItem(l);
	l.Activated.connect(function(menu,item){
		//提交
		API.triggerServerEvent("SC_PAY_CREATE_SELL",sellName,sellMoney,JSON.stringify(sellList));
		sellMoney=1
		sellName="出售清單";
		sellCar=null
		show_nowid=0
		sellState=false
		open=false
		Sellmenu.Visible=false;
		Vehmenu.Visible=true;
		resource.item.linkItem.Description="";
		resource.item.linkItem.Enabled=true;
	})
	l=API.createMenuItem("刷新物品列表","如果你獲得了新的物品請刷新\n~r~刷新後需要重新選擇出售物品.")
	Sellmenu.AddItem(l);
	l.Activated.connect(function(menu,item){
		//提交	
		show_nowid=0
		item.Enabled=false;
		open=false
		API.triggerServerEvent("SC_PAY_SELL_GET_ITEMLIST");
		API.setMenuSubtitle(Sellmenu,"正在拉取可出售物品列表..")
	})
	l=API.createMenuItem("------------------------------------","")
	l.Enabled=false;
	Sellmenu.AddItem(l)
	sellCar=API.createMenuItem("出售當前車輛","將你當前駕駛的車輛添加進出售清單.")
	sellCar.Enabled=false;
	sellCar.Activated.connect(function(menu,item){
		upbut.Enabled=true
		open=true
		veh=API.getPlayerVehicle(API.getLocalPlayer());
		var sqlID=API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_SQLID")
		var add=0;
		for( var f in sellList){
			var i=sellList[f] 
			if(i.itemType=="車輛"){
				if(i.itemID==sqlID){
					add=1;
				}
			}
		}
		if(add==0){
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
			var ss="車輛屬性\n極速 "+maxSP+"\n加速 "+maxJS+"\n牽引 "+maxQY+"\n煞車 "+maxSC;
			var sr=maxSC*100+maxQY*70+maxJS*215+maxSP*1.5
			sr=parseInt(sr) +clscore;
			if(sr>700){sr="~p~I("+sr+")"}
			if(sr>600&&sr<=700){sr="~y~X("+sr+")"}
			if(sr>500&&sr<=600){sr="~b~S("+sr+")"}
			if(sr>400&&sr<=500){sr="~g~A("+sr+")"}
			if(sr<=400){sr="~c~B("+sr+")"}
			sellList[sellList.length]=new itemInfo(API.getVehicleDisplayName(API.getEntityModel(veh)),sr+"",sqlID,ss,"車輛",sqlID);
			show_money=sellMoney
			show_name=sellName
			show_create=API.getPlayerName(API.getLocalPlayer())
			show_item=sellList	
		}
		
	})
	Sellmenu.AddItem(sellCar);
	
	if(d==null){
		API.triggerServerEvent("SC_PAY_SELL_GET_ITEMLIST");
	}else{
		for(var i in d)
		{
			Sellmenu.AddItem(AddSellMenuItem(d[i]["itemName"],d[i]["itemID"],d[i]["itemPz"],d[i]["itemMsg"],d[i]["itemType"],d[i]["itemValue"]))
		}
	 
	}

	Sellmenu.RefreshIndex();
  }

 function createPayMenu(d)
 {
	Vehmenu.Clear();
	var lj=API.createMenuItem("幫助","回車可查看交易幫助.")
	Vehmenu.AddItem(lj);
	lj.Activated.connect(function(menu,item){
		API.sendChatMessage("~g~---------交易幫助---------")
		API.sendChatMessage("~g~交易分為兩部分:出售與購買. 先講出售,選擇'出售物品'按鈕") 
		API.sendChatMessage("~g~然後會出現一個你擁有的並且可出售的物品列表")
		API.sendChatMessage("~g~選擇你想要出售的物品,然後回車,即可將該物品添加到待創建的出售清單中")
		API.sendChatMessage("~r~如果想要出售車輛,必須先坐在該車上")
		API.sendChatMessage("~g~你可以設置出售清單的名字,價格,全部設置完畢後按'提交清單'即可")
		API.sendChatMessage("~g~請注意:你創建的出售清單將會在你~r~離線~g~後~r~自動撤銷")
		API.sendChatMessage("~g~以上即為出售部分的幫助,~r~以下為購買部分的幫助")
		API.sendChatMessage("~g~在交易大廳中可以看見其他人提交的出售清單,選中即可瀏覽內容")
		API.sendChatMessage("~g~在選中的情況下,按~r~左箭頭~g~或~r~右箭頭~g~可以瀏覽清單中的其他物品")
		API.sendChatMessage("~g~按回車鍵購買,購買請求不可撤銷,請謹慎!")
	})
	lj=API.createMenuItem("出售物品","")
	Vehmenu.AddItem(lj);
	lj.Activated.connect(function(menu,item){
		API.setMenuSubtitle(Sellmenu,"正在拉取可出售物品列表..")
		resource.item.linkItem.Description="該功能暫時被禁用,若想啟用\n請點擊出售物品中的'放棄該清單'";
		resource.item.linkItem.Enabled=false;
		Vehmenu.Visible=false;
		showSellMenu(null);
		sellState=true
	})
	lj=API.createMenuItem("刷新出售清單","若自動刷新被關閉\n則需手動刷新")
	lj.Activated.connect(function(menu,item){
		API.triggerServerEvent("SC_PAY_GET_LISTCHANGE");
	})
	Vehmenu.AddItem(lj)
	var auto=true;
	if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_CONFIG:PAY_AUTO_LISTUPDATA")==false)
	{
		auto=false;
	}
	
	lj=API.createCheckboxItem("自動刷新","自動刷新清單列表\n在清單量極大的情況下建議關閉",auto)	
	lj.CheckboxEvent.connect(function (item, newChecked) {
		API.triggerServerEvent("SC_CONFIG_PLAYER_UPDATA","PAY_AUTO_LISTUPDATA",newChecked);
	});
	Vehmenu.AddItem(lj);
	lj=API.createMenuItem("------------------------","")
	lj.Enabled=false
	Vehmenu.AddItem(lj)
	if(d!=null){ 
		for(i in d)
		{ 
			Vehmenu.AddItem(AddListMenuItem(d[i]["sellPlayer"],d[i]["sellName"],d[i]["sellMoney"],d[i]["sellID"],d[i]["sList"]));
			
		}
	}
	Vehmenu.OnIndexChange.connect(function(menu,item){
		//被选择时显示物品
		//瀏覽,未製作
		if(menu.CurrentSelection>=5){
			var index=menu.CurrentSelection-5;
			show_nowid=0;
			show_money=slist[index]["sellMoney"]
			show_name=slist[index]["sellName"]
			show_create=slist[index]["sellPlayer"]
			show_item=slist[index]["sList"]
			open=true;
		}else{
			show_nowid=0
			show_money=0
			show_name=""
			show_create=""
			show_item=[]
			open=false
		}
	})
	Vehmenu.RefreshIndex();
 }

  API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_PAY_SELL_SEND_ITEMLIST"){ 
		list=JSON.parse(args[0])
		//API.sendChatMessage(list.length+"");
		showSellMenu(list)
		API.setMenuSubtitle(Sellmenu,"選擇要出售的物品")
	}
	if(eventName=="SC_PAY_LISTCHANGE"){ 
		if(args[1]=="手動刷新"||API.getEntitySyncedData(API.getLocalPlayer(),"SC_CONFIG:PAY_AUTO_LISTUPDATA")==true)
		{
			slist=JSON.parse(args[0])
			show_nowid=0
			show_money=0
			show_name=""
			show_create=""
			show_item=[]
			open=false
			createPayMenu(slist)
		}
	}
 
 });
 
 var y=(size.Height*0.258)
API.onUpdate.connect(function(){ 
		if(sellCar!=null){
			sellCar.Enabled=false;
			
			var player=API.getLocalPlayer(); 
			var veh=API.isPlayerInAnyVehicle(player);
			
			 
			if(veh){ 
				veh=API.getPlayerVehicle(player);
				var gv=API.getEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master")
				if(gv==API.getPlayerName(player)){
					sellCar.Enabled=true;
		 
				}	
			}
		}
		if(open==true){
			show=true; 
			if(dx_h<dx_maxh){
				dx_h=dx_h+20
			}else{ 
				if(dx_h>dx_maxh){
					dx_h=dx_h-5
				}else{	
					dx_h=dx_maxh
				}
				
				
				if(dx_w<300){
					dx_w=dx_w+20;
				}else{
					dx_w=300
				}
			}
		}
		if(open==false){
			if(dx_w>5){
				dx_w=dx_w-20
			}else{
				dx_w=5
				if(dx_h>0){
					dx_h=dx_h-20
				}else{
					dx_h=0
					open=null;
					show=false;
				}
			}
		}
		if(show==true){
			var x=(size.Width-435)-dx_w
			
			var dxw_mid=x+(dx_w/2)
			API.drawRectangle(x,y,dx_w,dx_h,0,0,0,195);
			if(dx_w>200){
	
					//浏览出售物品
					API.drawText("清單名稱", dxw_mid,y+5, 0.4, 255,255,255,195, 6,1, false, true, 0); 
					API.drawText(show_name, dxw_mid,y+35, 0.3, 255,255,255,195, 6,1, false, true, 0);
					API.drawText("出售者", dxw_mid,y+65, 0.4, 255,255,255,195, 6,1, false, true, 0); 
					API.drawText(show_create, dxw_mid,y+95, 0.3, 255,255,255,195, 6,1, false, true, 0); 
					API.drawText("清單價格", dxw_mid,y+125, 0.4, 255,255,255,195, 6,1, false, true, 0); 
					API.drawText("$"+show_money, dxw_mid,y+155, 0.3, 255,255,255,195, 6,1, false, true, 0); 
					API.drawRectangle(x,y+215,dx_w,2,255,255,255,195);
					if(show_item.length!=0){
						API.drawText((show_nowid+1)+"/"+show_item.length+"\n按左右箭頭鍵切換", dxw_mid,y+190, 0.3, 255,255,255,195, 6,1, false, true, 0); 
					
						var items=show_item[show_nowid]
						API.drawText(items.itemPz+"", dxw_mid,y+260, 0.5, 255,255,255,195, 6,1, false, true, 0); 
						API.drawText(items.itemName+"",dxw_mid,y+300, 0.4, 255,255,255,195, 6,1, false, true, 0); 
						API.drawText(items.itemMsg+"", dxw_mid,y+340, 0.3, 255,255,255,195, 6,1, false, true, 0); 
						var tmp=items.itemMsg.split('\n')
						dx_maxh=450+(tmp.length)*10
					}
			
			}
		}
}); 

 API.onKeyDown.connect(function(sender, keyEventArgs) {
	if(show==true)
	{
		if(keyEventArgs.KeyCode == Keys.Left){
			show_nowid=show_nowid-1
			if(show_nowid<0){
		
					show_nowid=show_item.length-1
					if(show_item.length==0){show_nowid=0}
			
			}
		}
		if(keyEventArgs.KeyCode == Keys.Right){
			show_nowid=show_nowid+1

					if(show_nowid>=show_item.length){				
						show_nowid=0
					}
			
			
		}	
	}
 })
 
 
 function itemInfo(iName,iPz,iID,iMsg,iType,iValue){
	var obj=new Object()
	obj.itemName=iName;
	obj.itemPz=iPz;
	obj.itemID=iID;
	obj.itemType=iType;
	obj.itemValue=iValue;
	obj.itemMsg=iMsg;
	return obj;
 }
 
 function sellInfo(cname,name,money,sli){
	var obj=new Object();
	obj.cname=cname
	obj.name=name
	obj.money=money
	obj.slist=sli
	return obj
 }
 
//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
