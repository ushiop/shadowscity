
API.onKeyDown.connect(function(sender, keyEventArgs) {

		if (keyEventArgs.KeyCode == Keys.Y) {
				if(API.getEntitySyncedData(API.getLocalPlayer(), "SC_Login_Status")==1){
					if(resource.menuOpen.MenuOpen()==true)
					{ 
						var p=API.getUserInput("",20);
						if(p.length!=0){
							API.triggerServerEvent("SC_chat_send",p);
						}
					}else{
						API.sendChatMessage("[~r~!~w~] 請關閉其他窗口後再試");
					}
				}
	}
});


 
