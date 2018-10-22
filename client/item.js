var Vehmenu=API.createMenu("選擇物品(回車使用,立即生效)",0,0,6);
var linkItem=null
var list
var page,pagemax;
var size=API.getScreenResolutionMantainRatio();
var open=false;

 API.onResourceStart.connect(function(){
	linkItem = API.createMenuItem("物品列表", "");
	resource.mainMenu.mainmenu.AddItem(linkItem);
	resource.mainMenu.mainmenu.BindMenuToItem(Vehmenu, linkItem);
	resource.mainMenu.menuPool.Add(Vehmenu); 
	createCallMenu(null)
	Vehmenu.OnMenuClose.connect(function(menu){ 
		open=false;
	})
	linkItem.Activated.connect(function(menu,item){
		open=true
	})
});


 function AddVehicleListItem(name,id,pz,msg,type,value,ilock){
		var nameItem=API.createMenuItem(pz+" ~o~"+name,"")
		if(ilock==true){
			nameItem.Text=nameItem.Text+" ~r~出售中..";
			nameItem.Enabled=false;
		}
		nameItem.Activated.connect(function(menu,item){
			//TRIGGER SERVER GOTO Function
			item.Enabled=false;
			item.Text=item.Text+" ~r~使用中"; 
			API.triggerServerEvent("SC_ITEM_USE",id);
			//API.sendNotification("你召喚了一輛 ~g~"+vehicleName); 
		});		
		return nameItem;
 }
 

 function createCallMenu(d)
 {
	Vehmenu.Clear();
	if(d!=null){
		
		 for(i in d)
		{ 
			Vehmenu.AddItem(AddVehicleListItem(d[i]["itemName"],d[i]["itemID"],d[i]["itemPz"],d[i]["itemMsg"],d[i]["itemType"],d[i]["itemValue"],d[i]["itemLock"]));
		}
	}
 
	Vehmenu.RefreshIndex();
 }

  API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_ITEM_LISTCHANGE"){ 
		list=JSON.parse(args[0])
		//API.sendChatMessage(list.length+"");
		createCallMenu(list)
	}
 
 })
 //924032302138
 
var fx=0,state=0
var x=size.Width-735
var y=(size.Height*0.258)
API.onUpdate.connect(function(){ 
	if(open==true)
	{

		var index=Vehmenu.CurrentSelection;
		var pz=list[index]["itemPz"].split("~");
		var r=255,g=0,b=255;
		if(pz[1]=="c"){r=255,g=255,b=255}
		if(pz[1]=="g"){r=0,g=255,b=0;}
		if(pz[1]=="b"){r=0,g=150,b=255}
		if(pz[1]=="y"){r=255,g=255,b=0}
		if(pz[1]=="p"){r=128,g=0,b=255}
		API.drawRectangle(x,y,300,400,0,0,0,195);
		API.drawText(list[index]["itemName"],x+2,y+5, 0.4, r,g,b,195, 6,0, false, true, 0);
		API.drawRectangle(x,y+50,300,2,r,g,b,195);	
		API.drawText(list[index]["itemPz"],x+300,y+50, 0.3, r,g,b,195, 6,2, false, true, 0);
		API.drawText(list[index]["itemType"],x,y+50, 0.3, r,g,b,195, 6,0, false, true, 0);
		var tmp_msg=list[index]["itemMsg"]
		if(tmp_msg.length>=16)
		{
			var msg1=strFromInt(16,tmp_msg)
			var f="" 
			for(var i=0;i<msg1.length;i++)
			{
				
				f=f+msg1[i]+"\n"
			}
			tmp_msg=f;
		}
		API.drawText(tmp_msg,x+150,y+120, 0.3, 255,255,255,195, 6,1, false, true, 0);
		var lx=list[index]["itemType"].split(":")
		if(lx[1]=="霓虹燈顏色")  
		{		 
			var c=list[index]["itemValue"].split(":")
			API.drawText("將車輛的霓虹燈顏色\n設為下面字符的顏色",x+150,y+200, 0.3,255,255,255,195, 6,1, false, true, 0);
			API.drawRectangle(x+70,y+300,160,40,255-parseInt(c[0]),255-parseInt(c[1]),255-parseInt(c[2]),100);	
			API.drawText("霓虹燈顏色",x+150,y+300, 0.5, parseInt(c[0]),parseInt(c[1]),parseInt(c[2]),255, 6,1, false, true, 0);
		}
		API.drawRectangle(x,y,300,2,r,g,b,50);	
		API.drawRectangle(x,y+398,300,2,r,g,b,50);	
		API.drawRectangle(x,y,2,400,r,g,b,50);	
		API.drawRectangle(x+298,y,2,400,r,g,b,50);	
		if(state==0)
		{
			if(fx<290){fx=fx+10}else{fx=0,state=1}
			API.drawRectangle(x+fx,y,10,2,r,g,b,255);	
			API.drawRectangle(x+290-fx,y+398,10,2,r,g,b,255);	
		}
		if(state==1)
		{
			if(fx<390){fx=fx+10}else{fx=0,state=2}
			API.drawRectangle(x+298,y+fx,2,10,r,g,b,255);	
			API.drawRectangle(x,y+390-fx,2,10,r,g,b,255);	
		}
		if(state==2)
		{
			if(fx<290){fx=fx+10}else{fx=0,state=3}
			API.drawRectangle(x+fx,y,10,2,r,g,b,255);	
			API.drawRectangle(x+290-fx,y+398,10,2,r,g,b,255);	
		}
		if(state==3)
		{
			if(fx<390){fx=fx+10}else{fx=0,state=0}
			API.drawRectangle(x,y+390-fx,2,10,r,g,b,255);	
			API.drawRectangle(x+298,y+fx,2,10,r,g,b,255);	
		}
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
//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
