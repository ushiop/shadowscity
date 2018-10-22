var Gomenu=API.createMenu("創建/編輯賽道",0,0,6); 
var menuPool = API.getMenuPool();
var lock=0
menuPool.Add(Gomenu);
var trackName="",trackPassword="";
var upbutton,editbutton;
var uptime=0,edittime=0;

 API.onResourceStart.connect(function(){
	var linkItem = API.createMenuItem("請輸入賽道名稱", "最多12個字符");
	Gomenu.AddItem(linkItem);
	linkItem.Activated.connect(function(menu,item){
		if(item.Text=="請輸入賽道名稱"){
			var name=API.getUserInput("",12);
		}else{
			var name=API.getUserInput(item.Text,12);
		}
		if(name.length!=0)
		{
			item.Text=name;
			trackName=name;
			if(trackPassword.length!=0){upbutton.Enabled=true,editbutton.Enabled=true;}
		}else{
			
			API.sendNotification("遇到了一個~r~錯誤 ~w~ 賽道名稱長度為零");
		}
	});
	linkItem=API.createMenuItem("請輸入賽道密碼","最多12個字符\n請記住該密碼並不要告知他人\n編輯賽道需要該密碼");
	Gomenu.AddItem(linkItem); 
	linkItem.Activated.connect(function(menu,item){ 
		if(item.Text=="請輸入賽道密碼"){
			var name=API.getUserInput("",12);
		}else{
			var name=API.getUserInput(item.Text,12);
		}
		if(name.length!=0)
		{

			item.Text=name;
			trackPassword=name;
			if(trackName.length!=0){upbutton.Enabled=true,editbutton.Enabled=true;}
		}else{
			API.sendNotification("遇到了一個~r~錯誤 ~w~ 賽道密碼長度為零");
		}
	});
	editbutton=API.createMenuItem("編輯賽道","編輯賽道會立即關閉該賽道的所有比賽\n同一條賽道同一時間只能一人編輯");
	Gomenu.AddItem(editbutton);
	editbutton.Enabled=false;
	editbutton.Activated.connect(function(menu,item)
	{
		if(trackName.length!=0&&trackPassword.length!=0)
		{
			API.triggerServerEvent("track_editRace",trackName,trackPassword);
			edittime=Math.round(new Date().getTime()/1000)+20;
			editbutton.Enabled=false;
		}
	});
	upbutton=API.createMenuItem("創建賽道","發送創建賽道的申請\n若有同名賽道存在則無法創建");
	Gomenu.AddItem(upbutton);
	upbutton.Enabled=false;
	upbutton.Activated.connect(function(menu,item)
	{
		if(trackName.length!=0&&trackPassword.length!=0)
		{
			API.triggerServerEvent("track_createRace",trackName,trackPassword);
			API.sendNotification("賽道:"+trackName+"\n密碼:"+trackPassword+"\n~g~創建申請~w~已提交,請~g~記住密碼");
			uptime=Math.round(new Date().getTime()/1000)+20;
			upbutton.Enabled=false;
 
		}
	});
	Gomenu.RefreshIndex();
});


API.onKeyDown.connect(function(sender, keyEventArgs) {
	
		if (keyEventArgs.KeyCode == Keys.F2) {
			if(API.getEntitySyncedData(API.getLocalPlayer(), "SC_ACL_HAS_Name:Race_Edit")==1){
			if(resource.menuOpen.MenuOpen()==true)
			{
				if(API.getEntitySyncedData(API.getLocalPlayer(),"SC_track_editrace_state")==1)
				{ 
					resource.edittrack.Gomenu.Visible=true;
				}else{
					
					Gomenu.Visible =true;
				}
			}
		}
	}
});


  
 API.onServerEventTrigger.connect(function (eventName, args) {
 
	if(eventName=="SC_track_oneditrace"){ 
			if(Gomenu.Visible==true)
			{
				Gomenu.Visible=false;
				resource.edittrack.updataMenu();
				resource.edittrack.Gomenu.Visible=true;
				//resource.edittrack.Gomenu.CurrentSelection=4;
			}

		}
 });

 

API.onUpdate.connect(function(){ 
	menuPool.ProcessMenus();
	var nowtime=Math.round(new Date().getTime()/1000);
	if(uptime!=0){
		
		if(nowtime>=uptime){	 
			uptime=0;
			upbutton.Enabled=true;
			upbutton.Description="發送創建賽道的申請\n若有同名賽道存在則無法創建";
		}else{
			var s=(uptime-nowtime); 
			//button.Description=">"+time+"\n>"+nowtime;
			upbutton.Enabled=false;
			upbutton.Description="請不要頻繁提交創建申請(剩餘"+s.toString()+"秒)";
		}
	}
	if(edittime!=0){
		
		if(nowtime>=edittime){	 
			edittime=0;
			editbutton.Enabled=true;
			editbutton.Description="編輯賽道會立即關閉該賽道的所有比賽\n同一條賽道同一時間只能一人編輯";
		}else{
			var s=(edittime-nowtime); 
			//button.Description=">"+time+"\n>"+nowtime;
			editbutton.Enabled=false;
			editbutton.Description="請不要頻繁提交編輯申請(剩餘"+s.toString()+"秒)";
		}
	}
}); 
