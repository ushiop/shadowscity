var Gomenu=API.createMenu("傳送至目標玩家",0,0,6);
var pl=["gtanetworkboom","gtanetworkboom1","gtanetworkboom2"]; 
var linkItem=null;
var open=false;
var size=API.getScreenResolutionMantainRatio();
var time=Math.round(new Date().getTime());
var lx=0
 API.onResourceStart.connect(function(){
	linkItem = API.createMenuItem("傳送功能", "");
	resource.mainMenu.mainmenu.AddItem(linkItem);
	resource.mainMenu.mainmenu.BindMenuToItem(Gomenu, linkItem);
	resource.mainMenu.menuPool.Add(Gomenu);
	linkItem.Activated.connect(function (menu, item) {
		open=true;
	}); 	
	Gomenu.OnMenuClose.connect(function(menu){
		tags=0;
		shows=0;
		open=false
	})
});

function updata()
{
	Gomenu.Clear();
		for(var i=0;i<pl.length;i++)
		{
			var nameItem=API.createMenuItem(pl[i]["name"],"")
			Gomenu.AddItem(nameItem);
			nameItem.Activated.connect(function(menu,item){
				//TRIGGER SERVER GOTO Function
				API.triggerServerEvent("SC_plist_goto",item.Text);
				API.sendNotification("你已開始嘗試傳送至 ~g~"+item.Text);
			});
		}
		
	//Gomenu.RefreshIndex();
}

 API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_plist"){ 
			pl=JSON.parse(args[0]);
			updata();
		}
	if(eventName=="SC_plist_false"){ 
			pl=JSON.parse(args[0]);
			updata();
			API.sendNotification("傳送至 ~g~"+args[1]+" ~w~失敗了\n請再試一次")
		}
 });
 
 
 var alpha=0;
 var step=0;
 var tags=0;
 var shows=0.00;
 var step1=0;
 var move=0;
 API.onUpdate.connect(function(sender, events) {
 
	
	if(open==true)
	{
		if(step==0)
		{
			alpha=alpha+10;
			if(alpha>255)
			{
				step=1;
			}
		}else{
			alpha=alpha-10;
			if(alpha<0)
			{
				step=0;
			}
		}
		if(step1==0)
		{
			move=move+10;
			if(move>300)
			{
				step1=1;
			}
		}else{
			move=move-10;
			if(move<0)
			{
				step1=0;
			}
		}
		if(pl.length!=0)
		{
			var x=parseInt((size.Width-735))
			var y=parseInt((size.Height*0.258))
			var index=Gomenu.CurrentSelection; 
			var user=pl[index]
			var portraitid=user["portrait"]
			API.drawRectangle(x,y,300,400,0,0,0,195);	
			API.dxDrawTexture("res/img/portrait/"+portraitid+".png",new Point(x+150-90,y-30),new Size(180,180),0 )
			API.drawText(user["name"],x+150,y+130, 0.4, 255,255,255,195, 6,1, false, true, 0);
			
				var rank_r=128,rank_g=128,rank_b=128;
				if(user["racerank"]>=1000&&user["racerank"]<2000){rank_r=97,rank_g=184,rank_b=71}
				if(user["racerank"]>=2000&&user["racerank"]<3500){rank_r=28,rank_g=177,rank_b=227}
				if(user["racerank"]>=3500&&user["racerank"]<5000){rank_r=237,rank_g=18,rank_b=18}
				if(user["racerank"]>=5000){rank_r=239,rank_g=189,rank_b=16}
				//RANK	
				API.drawRectangle(x,y+170,300,1,rank_r,rank_g,rank_b,50);
				API.drawText(user["racerank"]+"",x+150,y+170, 0.4,rank_r,rank_g,rank_b,195, 6,1, false, true, 0);
				API.drawText("RANK",x+150,y+200, 0.2, rank_r,rank_g,rank_b,125, 6,1, false, true, 0);	
				API.drawRectangle(x,y+220,300,1,rank_r,rank_g,rank_b,50);
				resource.dx_effect.dxEffect("shadow",x,y,move,170,1,1,0.3*1000,"",rank_r+":"+rank_g+":"+rank_b).Enabled();
				resource.dx_effect.dxEffect("shadow",x,y,300-move,220,1,1,0.3*1000,"",rank_r+":"+rank_g+":"+rank_b).Enabled();
				 
				
			var show_value=[]
			var show_name=[]
			if(lx==0)
			{
				show_value=[user["maxmoney"],user["usemoney"]*-1,user["pays"],user["syscars"]]
				show_name=["已獲得金錢","已使用金錢","交易次數","獲得車輛數(從系統)"]
			}else{
				show_value=[user["racewins"],user["raceloses"],user["racetops"],user["racemission"]]
				show_name=["勝場","負場","破記錄次數","賽事任務完成次數"]
			}
			
			//Value 0-3

			API.drawText(show_value[0]+"",x,y+220, 0.3, 255,255,255,195, 6,0, false, true, 0);
			API.drawText(show_name[0],x,y+240, 0.2, 255,255,255,125, 6,0, false, true, 0);	
			
			API.drawText(show_value[1]+"",x+300,y+220, 0.3, 255,255,255,195, 6,2, false, true, 0);
			API.drawText(show_name[1],x+300,y+240, 0.2, 255,255,255,125, 6,2, false, true, 0);	

			API.drawText(show_value[2]+"",x,y+400-40, 0.3, 255,255,255,195, 6,0, false, true, 0);
			API.drawText(show_name[2],x,y+400-20, 0.2, 255,255,255,125, 6,0, false, true, 0);	

			API.drawText(show_value[3]+"",x+300,y+400-40, 0.3, 255,255,255,195, 6,2, false, true, 0);
			API.drawText(show_name[3],x+300,y+400-20, 0.2, 255,255,255,125, 6,2, false, true, 0);		

			//勝率
			var maxs=user["racewins"]+user["raceloses"];
			var acc=(user["racewins"]/maxs).toFixed(2); 
			var fx=230+(400-230)/2
			tags=acc=="NaN"?0:acc;
			acc=parseInt(shows.toFixed(2)*100)  
			API.dxDrawTexture("res/img/percentage/round/"+acc+".png",new Point(x+150-65,y+fx+5-65),new Size(130,130),0 )
			API.drawText(acc+"%",x+150,y+fx-40, 0.6, 255,255,255,195, 6,1, false, true, 0);
			API.drawText("勝率",x+150,y+fx+10, 0.4, 255,255,255,125, 6,1, false, true, 0);					
		}
		if(shows.toFixed(2)!=tags)
		{
			if(shows>tags)
			{
				shows=shows-0.01;
			}else{
				shows=shows+0.01;
			} 
		} 
	}
	
	
	
	var nowtime=Math.round(new Date().getTime());
	if(nowtime>time)
	{
		lx=(lx==0)?1:0
		time=nowtime+4000;
	}
});

 