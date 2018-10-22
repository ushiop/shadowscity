var show=false; 
var old_select_index=0
var select_index=0;
var list;

 API.onResourceStart.connect(function(){
	 //new Vector3(-716.3962,-162.2415,36.19393),new Vector3(-0.02342723,0.355593,29.58234)//cam
//new Vector3(-719.264,-158.2872,36.19803),new Vector3(-0.1573869,-0.3500315,-149.8079)//ped
	//blip=API.createBlip(new Vector3(297.213,-583.9615,42.76713));
	//API.setBlipSprite(blip,51);
	//mark=API.createMarker(1,new Vector3(297.213,-583.9615,38.76713),new Vector3(),new Vector3(),new Vector3(5,5,10),0,64,128,32); 
	//lab= API.createTextLabel("按~g~G~w~打開變性菜單\n請不要將車開進來\n否則你會很尷尬...",new Vector3(297.213,-583.9615,44.76713),100,1,false); 
 
	blip=API.createBlip(new Vector3(-1167.556,-2021.094,12.48617));
	API.setBlipSprite(blip,225);	
	mark=API.createMarker(1,new Vector3(-1167.556,-2021.094,12.48617),new Vector3(),new Vector3(),new Vector3(5,5,10),0,64,128,32); 
	lab= API.createTextLabel("按~g~G~w~打開商店\n請不要將車開進來\n否則你會很尷尬...\n如果打不開菜單請關閉F1菜單后再試",new Vector3(-1167.556,-2021.094,15.48617),100,1,false); 
});


  API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_CAR_SHOPLIST"){ 
		//獲取出售物品
		list=JSON.parse(args[0])
		
		//在开启菜单的情况下收到新的商品列表则关闭并重新生成菜单
		if(show==true)
		{
			switchShop()
			switchShop()
		}
	}
 
 })
 
 function switchShop()
 {
	 if(show==false)
	 {
								show=true;
						old_select_index=0;
						select_index=0;
						//創建出售物品 
						for(var i in list)
						{
							var p=list[i];
							var tmp=resource.car_shop_dx.dxShopInfo(x+1,y+40+(75*i),p.name,p.money,p.msg,p.exmsg,p.itemtype,p.itemaddtype)
							if(i==0){tmp.Switch()}
							dx.push(tmp);			
						}
				
	 }else{
		show=false;
		old_select_index=0;
		select_index=0;
		for(var i in dx)
		{
			dx[i].Delete();
		}
		dx=[]
	 }
 }
 
 

 
 API.onKeyDown.connect(function(sender, keyEventArgs) {
	if(API.getEntitySyncedData(API.getLocalPlayer(), "SC_Login_Status")==1){
		if (keyEventArgs.KeyCode == Keys.G) {	
			if(show==false)
			{
				if(API.isInRangeOf(API.getEntityPosition(API.getLocalPlayer()),new Vector3(-1167.556,-2021.094,12.48617),3))
					{
						if(API.isPlayerInAnyVehicle(API.getLocalPlayer())==false){		
					 
							
							if(resource.menuOpen.MenuOpen()==true)
							{
								//菜单的开启与关闭
								switchShop();
							}
					}
				}
			}else{			 
				switchShop();
			}		
		}

		if(show==true)
		{
			if(keyEventArgs.KeyCode==Keys.Enter&&!API.isChatOpen() ){
			 
				//購買
				var p=dx[select_index]
				var nowtime=Math.round(new Date().getTime()); 
				if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_money")>=p.money)
				{
					if(nowtime>=buycd)
					{
						buycd=Math.round(new Date().getTime())+1000;
						API.sendChatMessage("~g~*已購買"+p.name+"")	
						API.triggerServerEvent("SC_CAR_SHOP_BUY",p.itemtype,p.itemaddtype,p.money);  
					}
 					
				}else{
					API.sendChatMessage("~r~*這就很尷尬了,你的錢不夠購買"+p.name)
				}
			 
			}
			if(keyEventArgs.KeyCode == Keys.Up){
				select_index=select_index-1
				if(select_index<0){
			
						select_index=dx.length-1
						if(dx.length==0){select_index=0}
				
				}
			}
			if(keyEventArgs.KeyCode == Keys.Down){
						select_index=select_index+1

						if(select_index>=dx.length){				
							select_index=0
						}
			
			
			}	
		}
	}
});

var buycd=-1;
var dx=[]
var size=API.getScreenResolutionMantainRatio();
var x=size.Width*0.5-300
var y=size.Height*0.5-300

API.onUpdate.connect(function(){ 

	if(show==true)
	{
		API.drawRectangle(x,y,600,600,0,0,0,150);
		
		//title line
		API.drawRectangle(x,y,600,30,70,163,255,255);
		
		//title text
		API.drawText("碎片商店",x+300,y, 0.4,255,255,255,255, 6,1, false, true, 0);
		
		//tips left
		API.drawText("上下箭頭鍵選擇商品",x,y+560, 0.4,255,255,255,255, 6,0, false, true, 0);
		API.drawText("回車鍵確認購買,再按G可關閉",x+600,y+560, 0.4,255,255,255,255, 6,2, false, true, 0);
		
		if(select_index!=old_select_index)
		{ 
			dx[old_select_index].Switch()
			old_select_index=select_index
			dx[select_index].Switch()
			
		}
		API.drawRectangle(x+305,y+40,295,500,255,255,255,150);
		var tmp=strFromInt(10,dx[select_index].msg)
		var f=""
		for(var i=0;i<tmp.length;i++)
		{
			
			f=f+tmp[i]+"\n"
		}
		API.drawText(f+"",x+452,y+80, 0.4,70,163,255,255, 6,1, false, true, 0);
		
	}
	


})



 
 
function strFromInt(s,str)
{ 
	 var strArr = [];

	var n = s;

	for (var i = 0, l = str.length; i < l/n; i++) {

		var a = str.slice(n*i, n*(i+1));
 
		strArr.push(a);
	}
	return strArr;
}