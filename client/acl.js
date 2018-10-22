var Gomenu=API.createMenu("權限編輯",0,0,6); 
var menuPool = API.getMenuPool();
var playerlist={};
var mods=0;
var menuName,aclList,aclNameList;
var addMenu;
var serverACLNAME="";
menuPool.Add(Gomenu);

 API.onResourceStart.connect(function(){
	updata(mods); 
});

 

API.onKeyDown.connect(function(sender, keyEventArgs) {
	
		if (keyEventArgs.KeyCode == Keys.F3) {
			if(API.getEntitySyncedData(API.getLocalPlayer(), serverACLNAME)==1){
				if(resource.menuOpen.MenuOpen()==true)
				{
						Gomenu.Visible =true;
				}
		}
	}
});

function updata(type)
{
	Gomenu.Clear();
	if(type==0){
		//選擇列表
		for(var i=0;i<playerlist.length;i++)
		{
			var nameItem=API.createMenuItem(playerlist[i],"")
			Gomenu.AddItem(nameItem);
			nameItem.Activated.connect(function(menu,item){
					//TRIGGER SERVER GOTO Function
				//API.triggerServerEvent("SC_plist_goto",item.Text);
				
				API.triggerServerEvent("SC_ACL_edit_on",item.Text);
				API.sendNotification("你已開始編輯 "+item.Text+" 權限表.");
			});
		}
	}else{
		//編輯列表
		var linkItem=API.createMenuItem(menuName+" ~r~的權限表","選擇某個權限並回車可以刪除.");
		linkItem.Enabled=false;
		Gomenu.AddItem(linkItem);
		linkItem=API.createMenuItem("----權限名----","所有的權限名");
		linkItem.Enabled=false;
		Gomenu.AddItem(linkItem);
		for(var i in aclNameList)
		{
			 
			linkItem=API.createMenuItem(aclNameList[i].aclName,aclNameList[i].aclTip);
			linkItem.Enabled=false;
			Gomenu.AddItem(linkItem);
		}
		linkItem=API.createMenuItem("新增權限","新增一個權限給該帳戶\n立即生效,請注意權限名是否正確");
		addMenu=linkItem;
		Gomenu.AddItem(linkItem);
		linkItem.Activated.connect(function(menu,item){
			var v=API.getUserInput("",12);
			if(v.length==0){return API.sendNotification("編輯權限遇到一個錯誤:權限名為空");}
			 
			if("SC_ACL_HAS_Name:"+v==serverACLNAME){return  API.sendNotification("編輯權限遇到一個錯誤:這個權限無法添加");}
			//API.sendNotification("為 "+menuName+"\n~g~新增權限~r~ "+v+" ~w~的請求已發送.")
			
			API.triggerServerEvent("SC_ACL_edit_add_aclname",v);
			addMenu.Enabled=false;
		})
		linkItem=API.createMenuItem("退出編輯","停止對該帳戶權限的編輯");
		Gomenu.AddItem(linkItem);
		linkItem.Activated.connect(function(menu,item){
			API.triggerServerEvent("SC_ACL_edit_off");
			API.sendNotification("你退出了權限編輯.")
			Gomenu.Clear();
		})
		linkItem=API.createMenuItem("----該帳戶擁有的權限----","該帳戶擁有的權限列表");
		linkItem.Enabled=false;
		Gomenu.AddItem(linkItem);
		for(var i in aclList)
		{
			var c="";
			if(aclList[i].aclName=="SC_Admin"){c="~y~";}
			linkItem=API.createMenuItem(c+aclList[i].aclName,aclList[i].aclTip);
			if(aclList[i].aclName=="SC_Admin"){linkItem.Enabled=false;}
			linkItem.Activated.connect(function(menu,item){
				
				API.triggerServerEvent("SC_ACL_edit_delete",item.Text);
			})
			Gomenu.AddItem(linkItem);
		}
	}
	Gomenu.RefreshIndex();
}


  
 API.onServerEventTrigger.connect(function (eventName, args) {
 
	if(eventName=="SC_ACL_playerlist_updata"){ 
		playerlist=JSON.parse(args[0]);
			if(mods==0){
				updata(mods);
			}
		}
	if(eventName=="SC_ACL_edit_on_ok"){ 
		menuName=args[0];
		aclList=JSON.parse(args[1]); 
		aclNameList=JSON.parse(args[2]);
		//開扁！！
		mods=1;
		updata(mods);
	}
	if(eventName=="SC_ACL_edit_listupdata"){
		//addMenu.Enabled=true; 
		aclList=JSON.parse(args[0]);
		updata(mods);
	//權限列表更新
	}
	if(eventName=="SC_ACL_edit_off_ok")
	{
		
		playerlist=JSON.parse(args[0]);
		mods=0;
		updata(mods);
	}
	if(eventName=="SC_ACL_cooooooooooooooooool")
	{
		serverACLNAME=args[0];
	}
 });

API.onUpdate.connect(function(sender, events) {
	menuPool.ProcessMenus();
});

 
