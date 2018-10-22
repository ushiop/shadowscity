var Vehmenu=API.createMenu("選擇賽道-使用/r亦可創建賽事",0,0,6);
var cd=0;
 
 
 API.onResourceStart.connect(function(){
	var linkItem = API.createMenuItem("創建比賽", "選擇一個賽道來創建比賽吧!");
	resource.racehouse.Vehmenu.AddItem(linkItem);
	resource.racehouse.Vehmenu.BindMenuToItem(Vehmenu, linkItem);
	resource.mainMenu.menuPool.Add(Vehmenu);
	
 

	Vehmenu.RefreshIndex();
});
 
 
function AddRaceRoad(raceName,raceIndex,raceCreatePlayer,raceTopPlayer,raceTopTime){
		var str="作者:"+raceCreatePlayer; 
		if(raceTopTime=="SC_NULL"){
			str=str+"";
		}else{
			str=str+"\n~w~TopTime>>~y~"+raceTopPlayer+"~y~("+raceTopTime/1000+"s)";
		}
		var nameItem=API.createMenuItem(raceName+" (RID:"+raceIndex+")",str)
		nameItem.Activated.connect(function(menu,item){
			//TRIGGER SERVER GOTO Function
			if(cd==0)
			{
				API.triggerServerEvent("SC_race_createhouse",raceName);
				API.sendNotification("比賽:"+raceName+"\n~g~創建比賽房間申請已發送.");
				cd=Math.round(new Date().getTime()/1000)+5;  
			}else{
				var nowtime=Math.round(new Date().getTime()/1000);
				if(nowtime>=cd)
				{
					API.triggerServerEvent("SC_race_createhouse",raceName);
					API.sendNotification("比賽:"+raceName+"\n~g~創建比賽房間申請已發送.");
					cd=Math.round(new Date().getTime()/1000)+5;  
				}else{
					API.sendChatMessage("~r~* 抱歉.你的創建申請太過頻繁,請在"+(cd-nowtime)+"秒後再試");
				}
			}
		});		
		return nameItem;
 

}

  
 API.onServerEventTrigger.connect(function (eventName, args) {
 
	if(eventName=="SC_tracklistchange"){ 
			if(Vehmenu.Visible==true)
			{
				Vehmenu.Visible=false;
				resource.racehouse.Vehmenu.Visible=true;
			}
			var p=JSON.parse(args[0]);
			Vehmenu.Clear();
			for(var i in p)
			{
				Vehmenu.AddItem(AddRaceRoad(p[i][0],p[i][1],p[i][2],p[i][3],p[i][4])); 
			}
			Vehmenu.CurrentSelection=0; 
		}
	if(eventName=="SC_race_toptimeupdata"){
		
					API.triggerServerEvent("SC_editrack_getlist");
	}
 });
 
 

 