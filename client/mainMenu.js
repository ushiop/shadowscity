var mainmenu=API.createMenu("-ShadowsCity-",0,0,6);
var menuPool = API.getMenuPool();
var lock=0
menuPool.Add(mainmenu);
 
 

API.onKeyDown.connect(function(sender, keyEventArgs) {
	
		if (keyEventArgs.KeyCode == Keys.F1) {
			if(API.getEntitySyncedData(API.getLocalPlayer(), "SC_Login_Status")==1){
				if(resource.menuOpen.MenuOpen()==true){  
					mainmenu.Visible =true;
				 
				}
		}
	}
});

API.onUpdate.connect(function(sender, events) {
	menuPool.ProcessMenus();
});
