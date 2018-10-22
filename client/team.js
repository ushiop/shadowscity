var Vehmenu=API.createMenu("車隊",0,0,6);
var linkItem=null
var js=null;//車隊主要信息
 var show=false
 var size=API.getScreenResolutionMantainRatio(); 
 var dx=[];
 var g_gov=null,g_rank=null,g_boss=null,g_bosst=null,g_buff=null,g_bufft=null; 
 var g_playerlist_showindex=0,g_dylan_showindex=0,g_yaoqing=null,g_player=null,g_quit=null,g_teamlist=null;
 var select=0,old_select=-1,s_select=0,s_select_playername;
 var caption=null;
 API.onResourceStart.connect(function(){
	linkItem = API.createMenuItem("車隊", "點擊可創建車隊\n需要10W金幣+RANK分>=3500");
	resource.mainMenu.mainmenu.AddItem(linkItem);
	linkItem.Activated.connect(function(menu,item){
		
		var Player=API.getLocalPlayer()
		if(API.getEntitySyncedData(Player,"SC_money")>=100000&&API.getEntitySyncedData(Player,"SC_USERINFO:RACERANK")>=3500)
		{
				.....
				
			
			if(js==null)
			{
				//沒車隊時創建
				var n=API.getUserInput("車隊名不可重複,最多10字",15);
				if(n.length!=0&&n.length<=10)
				{
					 API.sendNotification("車隊創建申請已發送.");
					 API.triggerServerEvent("SC_TEAM_CREATE",n);
					 linkItem.Enabled=false;
				}
			}else{
				//有車隊時打開車隊界面
				resource.mainMenu.mainmenu.Visible=false;
				createGui();
			}
		}
	})
});

 
function createGui()
{
			show=true;
			select=0,old_select=-1,s_select=0,caption=true;
			if(API.getPlayerName(API.getLocalPlayer())!=js.tMaster)
			{
				caption=false;
				select=3;
			}
			var tmp=resource.dx_effect.dxEffect("text",mid_x,y,0,-10,0,0,-1,{'showtext':js.tName,'showindex':0,'showstyle':1,'showfont':6,'showfonts':1,'showalpha':255},"255:255:255").Enabled();	
			dx.push(tmp)
			g_gov=resource.dx_effect.dxEffect("text",mid_x,y,0,62,0,0,-1,{'showtext':js.tGovmsg,'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.5,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_gov)
			g_rank=resource.dx_effect.dxEffect("text",mid_x,y,0,130,0,0,-1,{'showtext':js.tRank+"",'showindex':0,'showstyle':1,'showfont':6,'showfonts':1.2,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_rank)
			tmp=resource.dx_effect.dxEffect("text",mid_x,y,0,230,0,0,-1,{'showtext':"車隊分數",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.2,'showalpha':255},"255:255:255").Enabled();	
			dx.push(tmp)
			tmp=resource.dx_effect.dxEffect("text",x,y,dx_w-5,140,0,0,-1,{'showtext':js.tMaster+"~w~ 隊長",'showindex':0,'showstyle':2,'showfont':6,'showfonts':0.6,'showalpha':255},"255:255:255").Enabled();	
			dx.push(tmp)
			tmp=resource.dx_effect.dxEffect("text",x,y,dx_w-5,200,0,0,-1,{'showtext':js.tCreateDate+" 創建日期",'showindex':0,'showstyle':2,'showfont':6,'showfonts':0.6,'showalpha':255},"255:255:255").Enabled();	
			dx.push(tmp)
			g_boss=resource.dx_effect.dxEffect("percentage",x+350,y+200,0,0,180,180,-1,{'showacc':js.tRoadBoss,'showindex':0},"0:0:0").Enabled();	
			dx.push(g_boss)
			g_bosst=resource.dx_effect.dxEffect("text",x,y,350,170,0,0,-1,{'showtext':js.tRoadBoss+"%\n賽道統治度",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.4,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_bosst)
			g_buff=resource.dx_effect.dxEffect("percentage",x+100,y+200,0,0,180,180,-1,{'showacc':js.tBuff,'showindex':0},"0:0:0").Enabled();	
			dx.push(g_buff)
			g_bufft=resource.dx_effect.dxEffect("text",x,y,100,170,0,0,-1,{'showtext':js.tBuff+"%\n額外賽事獎勵",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.4,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_bufft)
			g_player=resource.dx_effect.dxEffect("text",x,y,5,305,0,0,-1,{'showtext':"車隊成員",'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.8,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_player)	
			g_yaoqing=resource.dx_effect.dxEffect("text",x,y,380,315,0,0,-1,{'showtext':caption==true?"邀請成員":"",'showindex':0,'showstyle':2,'showfont':6,'showfonts':0.6,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_yaoqing)
			tmp=resource.dx_effect.dxEffect("text",x,y,605,305,0,0,-1,{'showtext':"車隊動態",'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.8,'showalpha':255},"255:255:255").Enabled();	
			dx.push(tmp)	
			tmp=resource.dx_effect.dxEffect("text",x+dx_w,y+dx_h,-15,-40,0,0,-1,{'showtext':"方向鍵上下選擇操作項目,左右選擇次級項目,回車確認執行,G鍵關閉菜單",'showindex':0,'showstyle':2,'showfont':6,'showfonts':0.4,'showalpha':255},"255:255:255").Enabled();	
			dx.push(tmp)			
			g_quit=resource.dx_effect.dxEffect("text",x,y+dx_h,5,-40,0,0,-1,{'showtext':caption==true?"解散車隊":"退出車隊",'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.4,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_quit)
			g_teamlist=resource.dx_effect.dxEffect("text",x,y+dx_h,110,-40,0,0,-1,{'showtext':"車隊排行榜",'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.4,'showalpha':255},"255:255:255").Enabled();	
			dx.push(g_teamlist)	
}
 
function deleteGui()
{
	for(var i in dx)
			{
				dx[i].Delete();
			}
			dx=[];
			show=false;
}
 
var dx_w=1200,dx_h=900
var x=(size.Width*0.5)-(dx_w/2)
var y=(size.Height*0.5)-(dx_h/2)
var mid_x=x+(dx_w/2)
API.onUpdate.connect(function(){  
	if(show==true)
	{
		API.drawRectangle(x,y,dx_w,dx_h,0,0,0,255);
		
		// 名字
		//API.drawText(js.tName,mid_x,y-10,1,255,255,255,255, 6,1, false, true, 0);
		
		API.drawRectangle(x,y+60,dx_w,2,255,255,255,255);
		 
		// 公告 
		//API.drawText(js.tGovmsg==""?"沒有公告":js.tGovmsg,mid_x,y+60,0.5,255,255,255,255, 6,1, false, true, 0);
		
		API.drawRectangle(x,y+110,dx_w,2,255,255,255,255);
		
		//數據
		//API.drawText(js.tRank+"",mid_x,y+130,1.2,255,255,255,255, 6,1, false, true, 0);
		//API.drawText("車隊分數",mid_x,y+230,0.2,255,255,255,255, 6,1, false, true, 0);
		
		//API.drawText(js.tMaster+" ~w~隊長",x+dx_w-15,y+140,0.7,255,255,255,255, 6,2, false, true, 0); 
		
		//API.drawText(js.tCreateDate+" 創建日期",x+dx_w-10,y+200,0.7,255,255,255,255, 6,2, false, true, 0); 
		
		API.drawRectangle(x,y+300,dx_w,2,255,255,255,255);
		
		//列表
		API.drawRectangle(x+600,y+300,2,560,255,255,255,255);
		 
		
		//成員
		var showid=0
		for(var i=g_playerlist_showindex;i<js.tList.length;i++)
		{
			if(showid<14)
			{//p.login==true?"~g~":"~c~"+
				var p=js.tList[i];
				API.drawText((p.login==true?"~g~":"~c~")+p.name,x+5,y+360+(showid*35),0.4,255,255,255,255, 6,0, false, true, 0);
				if(showid==s_select&&select==1)
				{
					s_select_playername=p.name;
					API.drawText(p.name==js.tMaster?"<踢不出>":"<踢出>",x+350,y+365+(showid*35),0.3,255,255,255,255, 6,1, false, true, 0);
				}
				API.drawText("Rank:"+p.rank,x+595,y+360+(showid*35),0.4,255,255,255,255, 6,2, false, true, 0);
				showid++
			}
		} 
		
		//動態
		showid=0
		 
		for(var i=(js.tDylan.length-g_dylan_showindex);i>0;i--)
		{
			if(showid<6)
			{
				var p=js.tDylan[i-1];
				API.drawRectangle(x+605,y+370+(showid*75),590,70,255,255,255,175);
				API.drawText(p.msg,x+610,y+370+(showid*75),0.3,0,128,255,255, 6,0, false, true, 0);
				API.drawText(p.date,x+dx_w-5,y+410+(showid*75),0.3,0,128,255,255, 6,2, false, true, 0);
				showid++
			}
		} 
		API.drawRectangle(x,y+dx_h-40,1200,2,255,255,255,255);
		
		if(select!=old_select)
		{
			if(old_select==0)
			{
				g_gov.r=255;
				g_gov.g=255;
				g_gov.b=255;
			}
			if(old_select==1)
			{
				g_player.r=255;
				g_player.g=255;
				g_player.b=255;
			}
			if(old_select==2)
			{
				g_yaoqing.r=255;
				g_yaoqing.g=255;
				g_yaoqing.b=255;				
			}
			if(old_select==3)
			{
				g_quit.r=255;
				g_quit.g=255;
				g_quit.b=255;				
			}
			if(old_select==4)
			{
				g_teamlist.r=255;
				g_teamlist.g=255;
				g_teamlist.b=255;				
			}			
			old_select=select;
			if(select==0)
			{
				g_gov.r=0;
				g_gov.g=255;
				g_gov.b=0;
			}
			if(select==1)
			{
				g_player.r=0;
				g_player.g=255;
				g_player.b=0;				
			}
			if(select==2)
			{
				g_yaoqing.r=0;
				g_yaoqing.g=255;
				g_yaoqing.b=0;				
			}	
			if(select==3)
			{
				g_quit.r=0;
				g_quit.g=255;
				g_quit.b=0;				
			}			
			if(select==4)
			{
				g_teamlist.r=0;
				g_teamlist.g=255;
				g_teamlist.b=0;	
			}
		}
		
	 
	}
})
 
 API.onKeyDown.connect(function(sender, keyEventArgs) {
	 if(show==true)
	 {
		if(keyEventArgs.KeyCode == Keys.Up){
				select=select-1
				if(caption==true)
				{
					if(select<0){
						select=0
					}
				}else{
					if(select<3){
						select=3
					}
				}
			}
		if(keyEventArgs.KeyCode == Keys.Down){
						select=select+1
						if(select>4){				
							select=4
						}
			}	
		if(keyEventArgs.KeyCode == Keys.Left){
				if(select!=1){return}
				s_select=s_select-1
				if(s_select<0){
					s_select=0
							if(g_playerlist_showindex>0)
							{
								g_playerlist_showindex--;
							}					
				}
			}
		if(keyEventArgs.KeyCode == Keys.Right){
					if(select!=1){return}
						s_select=s_select+1
						if(s_select>13){				
							s_select=13
							 
							if((g_playerlist_showindex+13)<js.tList.length-1)
							{
								g_playerlist_showindex++;
							}
						}
						if(s_select>=js.tList.length){				
							s_select=js.tList.length-1
						}
			}				
		if(keyEventArgs.KeyCode == Keys.Enter){
			if(select==0)
			{
				var n=API.getUserInput("公告最長30字",30);
				if(n.length!=0&&n.length<=30)
				{
					
					API.triggerServerEvent("SC_TEAM_CHANGE_GOV",n);
				}
			}
			if(select==1)
			{
				if(s_select_playername!=js.tMaster)
				{ 
					API.triggerServerEvent("SC_TEAM_KICK_PLAYER",s_select_playername);
				}
			}
			if(select==2)
			{
				deleteGui();
				resource.team_yaoqing.createGui();
			}
			if(select==3)
			{
				API.triggerServerEvent("SC_TEAM_QUIT");
			}
			if(select==4)
			{
				deleteGui();
				resource.team_top.show=true;
				resource.team_top.back=true;
			}
		}
		if(keyEventArgs.KeyCode == Keys.G){
			deleteGui()
		}
	 }
 })
 
 
 API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_TEAM_KICK"){
		//玩家被踢出
		js=null;
		if(show==true){
			deleteGui()
			}
		linkItem.Enabled=true;
		linkItem.Description="點擊可創建車隊\n需要10W金幣+RANK分>=3500"
	}
 
	if(eventName=="SC_TEAM_UPDATA"){ 
		//車隊信息被刷新
		js=JSON.parse(args[0]);   
		js.tList=sort(js.tList)
		linkItem.Enabled=true;
		linkItem.Description="點擊打開車隊界面"
	}
	
	if(eventName=="SC_TEAM_DYNAMIC_UPDATA"){ 
		//車隊動態被刷新
		if(js==null){return}
		js.tDylan=JSON.parse(args[0])
 
	}
	if(eventName=="SC_TEAM_BOSS_UPDATA")
	{//車隊增益,統治率,TOP數量更新
		if(js==null){return}
		js.tRoadBoss=args[0];
		js.tRoadTops=args[1];
		js.tBuff=args[2];
		if(show==true)
		{
			g_boss.set.showacc=js.tRoadBoss;
			g_boss.set.showindex=0;
			g_bosst.set.showtext=js.tRoadBoss+"%\n賽道統治度"
			g_bosst.set.showindex=0;
			g_buff.set.showacc=js.tBuff;
			g_buff.set.showindex=0;
			g_bufft.set.showtext=js.tBuff+"%\n額外賽事獎勵"
			g_bufft.set.showindex=0;
		}
	}
	if(eventName=="SC_TEAM_RANK_UPDATA")
	{
		if(js==null){return}
		js.tRank=args[0]
		if(show==true)
		{
			g_rank.set.showtext=js.tRank+"";
			g_rank.set.showindex=0;
		}
	}
	if(eventName=="SC_TEAM_GOV_UPDATA")
	{
		if(js==null){return}
		js.tGovmsg=args[0]
		if(show==true)
		{
			g_gov.set.showtext=js.tGovmsg==""?"沒有公告":js.tGovmsg;
			g_gov.set.showindex=0;
		}		
	}
	if(eventName=="SC_TEAM_PLAYERSTATE_UPDATA")
	{
		if(js==null){return}
		js.tList=sort(JSON.parse(args[0]))
	 
		if((g_playerlist_showindex+13)>js.tList.length-1&&js.tList.length>13)
							{
								g_playerlist_showindex--;
							}
				 
				 if(s_select>13){				
							s_select=13
						}
							if(s_select>=js.tList.length){				
							s_select=js.tList.length-1
						}
		
	}
	if(eventName=="SC_TEAM_CREATE_ERROR")
	{
		linkItem.Enabled=true;
		linkItem.Description="點擊可創建車隊\n需要10W金幣+RANK分>=3500"		
	}
	if(eventName=="SC_TEAM_NAME_LOCAL_HIDE")
	{
		API.setTextLabelColor(args[0],0,255,255,255);
	}
})


function sort(arr)
{
	var end=[]
	var online=[]
	var offline=[]
	for(var i in arr)
	{
		var p=arr[i]
		if(p.login==true)
		{
			online.push(p)
		}else{
			offline.push(p)
		}
	}
	for(var i in online)
	{
		for(var x=i;x<online.length;x++)
		{
			if(online[i].rank<online[x].rank)
			{
				var tmp=online[i];
				online[i]=online[x]
				online[x]=tmp;
			}
		}
	}
	for(var i in offline)
	{
		for(var x=i;x<offline.length;x++)
		{
			if(offline[i].rank<offline[x].rank)
			{
				var tmp=offline[i];
				offline[i]=offline[x]
				offline[x]=tmp;
			}
		}
	}	
	for(var i in online)
	{
		end.push(online[i]);
	}
	for(var i in offline)
	{
		end.push(offline[i]);
	}
	return end;
}

 
 