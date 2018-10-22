var js=null;
 var size=API.getScreenResolutionMantainRatio(); 
 var show=false;
 var back=false;

 API.onResourceStart.connect(function(){
	
});

 
 function createGui()
 {
	 show=true
	 back=true;
	 API.triggerServerEvent("SC_TEAM_YAOQING_GET");
 }
 
 
 var show1=false;
 var teamname=null;
 var show_y=(size.Height*0.5)-100
 var cd;
 
var dx_w=600,dx_h=900
var x=(size.Width*0.5)-(dx_w/2)
var y=(size.Height*0.5)-(dx_h/2)
var mid_x=x+(dx_w/2)
var select=0,g_show=0
var selectname=null
API.onUpdate.connect(function(){  
	if(show==true)
	{
		API.drawRectangle(x,y,dx_w,dx_h,0,0,0,255);
		
		// 名字
		API.drawText("邀請玩家",mid_x,y-10,1,255,255,255,255, 6,1, false, true, 0);
		API.drawText("G鍵關閉\n回車邀請",x,y-10,0.4,255,255,255,255, 6,0, false, true, 0);
		API.drawText("上下鍵瀏覽",x+dx_w,y-10,0.4,255,255,255,255, 6,2, false, true, 0);
		if(js==null)
		{
			API.drawText("正在等待玩家數據...",mid_x,y+60,0.5,255,255,255,255, 6,1, false, true, 0);
		}else{
			if(js.length==0)
			{
				API.drawText("沒有人可以邀請",mid_x,y+60,0.5,255,255,255,255, 6,1, false, true, 0);
				return 
			}
			//動態
			showid=0
			 
			for(var i=g_show;i<js.length;i++)
			{
				if(showid<23)
				{
					var p=js[i];
					if(select==showid)
					{
						selectname=p;
						API.drawRectangle(x+5,y+70+(showid*35),590,30,255,255,255,214);
					}else{
						
						API.drawRectangle(x+5,y+70+(showid*35),590,30,255,255,255,120);
					}
					API.drawText(p.name,x+200,y+60+(showid*35),0.5,0,128,255,255, 6,1, false, true, 0); 
					if(p.lock==true)
					{
						API.drawText("已邀請",x+590,y+60+(showid*35),0.5,0,128,255,255, 6,2, false, true, 0); 
					}
					showid++
				}
			}  
		}
	}
	
	if(teamname!=null)
	{
		if(show1==false)
		{
			
			var nowtime=Math.round(new Date().getTime());
			if(nowtime>=cd)
			{
				cd=nowtime+1000
				if(resource.menuOpen.MenuOpen()==true)
				{
					show1=true;
				}
			}
		}
	}
	
	if(show1==true)
	{
		API.drawRectangle(x,show_y,dx_w,200,0,0,0,175);
		API.drawRectangle(x,show_y,dx_w,60,0,128,255,214);
		API.drawText("車隊邀請",mid_x,show_y,0.7,255,255,255,255, 6,1, false, true, 0);
		API.drawText("車隊\n"+teamname+"\n邀請你加入\n按下回車加入,按G可忽略并關閉該界面",mid_x,show_y+70,0.4,255,255,255,255, 6,1, false, true, 0);
	}
})
 
 API.onKeyDown.connect(function(sender, keyEventArgs) {
	 if(show==true)
	 {
		if(keyEventArgs.KeyCode == Keys.Up){
				select--;
				if(select<0)
				{
					select=0
					if(g_show>0)
					{
						g_show--
					}
				}
			}
		if(keyEventArgs.KeyCode == Keys.Down){
				select++;
				if(select>22)
				{
					select=22
					if(g_show+22<js.length-1)
					{
						g_show++
					}
				}
				if(select>=js.length){				
							select=js.length-1
						}
			}	
		if(keyEventArgs.KeyCode == Keys.Enter)
		{
			if(selectname!=null)
			{
				API.sendNotification("車隊邀請已發送給"+selectname.name)
				selectname.lock=true;
				API.triggerServerEvent("SC_TEAM_YAOQING_SEND",selectname.name)
			}
		}
		if(keyEventArgs.KeyCode == Keys.G){
	  
			show=false;
			selectname=null
			if(back==true)
			{
				back=false;
				resource.team.createGui();
			}
		}
	 }
	 if(show1==true)
	 {
		 if(keyEventArgs.KeyCode == Keys.G){
			 
			 show1=false;
			 teamname=null;
		 }
		 if(keyEventArgs.KeyCode == Keys.Enter){
			 
			 API.triggerServerEvent("SC_TEAM_YAOQING_ADD",teamname)
			 show1=false;
			 teamname=null;
		 }
	 }
 })
 
 
 API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_TEAM_YAOQING_PLAYER"){
		// 
		 js=JSON.parse(args[0]);  
		 var f=[]
		 for(var i in js)
		 {
			 f.push(info(js[i]))
		 } 
		 js=f;
	}
	if(eventName=="SC_TEAM_YAOQING_SENDGUI")
	{
		teamname=args[0];
		cd=Math.round(new Date().getTime())+1000
		if(show1==false)
		{
			if(resource.menuOpen.MenuOpen()==true)
			{
				show1=true;
			}else{
				API.sendChatMessage("~g~*你收到了一份車隊邀請,關閉現在的界面即可打開邀請界面");
			}
		}
	}

})
 

function info(_name){
	var p=new Object();
	p.name=_name
	p.lock=false;
	return p
}
 
 