var Gomenu=API.createMenu("編輯賽道",0,0,6); 
var checkpoint=null;
var mks=new Array();
var blips=new Array();
var raceCid=new Array();
var raceRid=new Array();
var cps=0;
var createBut,deleteBut,deleteSelectBut;
var raceID;


 API.onResourceStart.connect(function(){
	resource.track.menuPool.Add(Gomenu);
});
 
 
 function AddMenuItem(itemName,itemTip,itemarg){
	var linkItem=API.createMenuItem(itemName,itemTip)
	if(itemarg=="cleartoptime")
	{
		if(itemTip=="沒有賽道記錄")
		{
			linkItem.Enabled=false;
		}
	}
	if(itemarg=="deleteCP")
	{
		deleteBut=linkItem;
		if(checkpoint==null)
		{
			linkItem.Enabled=false;
		}
	}
	if(itemarg=="createCP")
	{
		createBut=linkItem;
		linkItem.Enabled=false;
	}
	if(itemarg=="tips")
	{
		linkItem.Enabled=false;
	}
	linkItem.Activated.connect(function(menu,item){
			//TRIGGER SERVER GOTO Function
			if(itemarg=="quitedit"){
				for(var i in mks)
				{
					if(raceCid[i]!=-123){
						API.deleteEntity(mks[i])
						API.deleteEntity(blips[i])
					}
				}
				mks=new Array();
				blips=new Array();
				raceCid=new Array();
				raceRid=new Array();
				checkpoint=null;
				raceID=null;
				cps=0;
				createBut=null;
				deleteBut=null;
				deleteSelectBut=null;
				API.triggerServerEvent("SC_editrack_off");
				API.sendNotification("你退出了賽道編輯模式");	
				Gomenu.Visible=false; 
				resource.track.Gomenu.Visible=true;
			}
			if(itemarg=="changepass"){
				var pa=API.getUserInput(password,12);
				if(pa.length!=0){
					if(password!=pa){
						password=pa; 
						item.Text="賽道密碼:"+password; 
						API.triggerServerEvent("SC_editrack_changepass",password);
					}else{
						API.sendNotification("遇到了一個~r~錯誤 ~w~ 賽道密碼與舊的一樣");
					}
				}else{
					API.sendNotification("遇到了一個~r~錯誤 ~w~ 賽道密碼長度為零");
				}
			}
			if(itemarg=="cleartoptime"){ 
				linkItem.Description="沒有賽道記錄";
				linkItem.Enabled=false;
				API.triggerServerEvent("SC_editrack_cleartoptime");
				
			}
			if(itemarg=="deleteRoad"){
				for(var i in mks)
				{
					if(raceCid[i]!=-123){
						API.deleteEntity(mks[i])
						API.deleteEntity(blips[i])
					}
				}
				mks=new Array();
				blips=new Array();
				raceCid=new Array();
				raceRid=new Array();
				checkpoint=null;
				raceID=null;
				cps=0;
				createBut=null;
				deleteBut=null;
				deleteSelectBut=null; 
				Gomenu.Visible=false; 
				resource.track.Gomenu.Visible=true;
				API.triggerServerEvent("SC_editrack_deleteroad"); 
			}
			if(itemarg=="deleteCP")
			{
				linkItem.Enabled=false;
				deleteSelectBut.Enabled=false;
				API.triggerServerEvent("SC_editrack_deleteCP",raceCid[checkpoint]);
			}
			if(itemarg=="createCP")
			{
				linkItem.Enabled=false;
				createCheckPoint(API.getEntityPosition(API.getLocalPlayer()));
			}
		});		
		return linkItem;
 }
 
  function AddMenuCPItem(itemName,itemTip,itemarg,cpindex){
	var linkItem=API.createMenuItem(itemName,itemTip)
	linkItem.Activated.connect(function(menu,item){
			//TRIGGER SERVER GOTO Function 
				//API.triggerServerEvent("SC_editrack_off");
				deleteSelectBut=item;
				deleteBut.Enabled=true;
				deleteBut.Description="確定要刪除檢查點 "+raceCid[cpindex]+"嗎?\n~r~注意:該操作不可逆!";
				checkpoint=cpindex;
				API.triggerServerEvent("SC_editrack_gotocp",raceCid[checkpoint]);
				  
		});		
		return linkItem;
 }
 
 function updataMenu()
 {
	 /*API.triggerClientEvent(Player, "SC_track_editracedata",
	 createState.getRacePassword(),
	 createState.getRaceTopPlayer(),
	 createState.getRaceTopTime(),
	 createState.getRaceRid().ToString() );*/
	API.setMenuSubtitle(Gomenu,"編輯賽道-正在獲取賽道資料...");
 }
 
 
  API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_track_editracedata"){ 
			
			var pass=args[0],topp=args[1],topt=args[2],rid=args[3],rname=args[4];
			password=pass,raceID=rid; 
			var str="沒有賽道記錄"
			if(topt!="SC_NULL"){
				str="當前紀錄保持者:~y~"+topp+"\n~w~時間~y~ "+(topt/1000)+"s\n~w~請勿頻繁清空";
			}
			API.setMenuSubtitle(Gomenu,"編輯賽道("+rname+")");	
			Gomenu.Clear(); 
			Gomenu.AddItem(AddMenuItem("賽道密碼:"+pass,"點擊修改賽道密碼(請勿頻繁修改)","changepass")); 
			Gomenu.AddItem(AddMenuItem("清空賽道記錄",str,"cleartoptime"));
			Gomenu.AddItem(AddMenuItem("~r~刪除賽道","~r~無法恢復\n立即生效","deleteRoad"));
			Gomenu.AddItem(AddMenuItem("創建CP點","在當前位置創建CP點\n立即生效","createCP"));
			Gomenu.AddItem(AddMenuItem("刪除CP點","先選定一個CP點\n立即生效","deleteCP"));
			Gomenu.AddItem(AddMenuItem("退出編輯","結束對該賽道的編輯\n請務必先'提交編輯'","quitedit"));
			Gomenu.AddItem(AddMenuItem("----CP List----","比賽時的順序為從上往下\n檢查點序號不影響比賽時跑點順序","tips"));
			Gomenu.CurrentSelection=6;
		}
	if(eventName=="SC_track_editracedata_cp"){
		setCheckPoint(args[0],args[1],args[2]);
		createBut.Enabled=true;
	}
	
	if(eventName=="SC_track_editracedata_cps"){
		var rid=JSON.parse(args[0]),dbid=JSON.parse(args[1]),pos=args[2];
		for(var z in rid)
		{
			setCheckPoint(rid[z],dbid[z],pos[z]);
		}
		createBut.Enabled=true;
	}	
	if(eventName=="SC_editrack_deleteCP_ok")
	{
		deleteBut.Description="先選定一個CP點\n立即生效";
		deleteSelectBut.Text=deleteSelectBut.Text+"~r~ 已刪除";
		for(var i in raceCid)
			{
				if(raceCid[i]==args[0])
				{
					raceCid[i]=-123;
					API.deleteEntity(mks[i])
					API.deleteEntity(blips[i])
					
				}
			}
	}
 });
 
 
function setCheckPoint(rid,cid,pos)
{ 
	var marker=API.createMarker(1,pos,new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(10,10,2),241, 247, 57, 180);
	mks[cps]=marker;
	var blipss= API.createBlip(pos);
	API.setBlipColor(blipss, 66);
    API.setBlipScale(blipss, 0.6);
	blips[cps]=blipss;
	raceCid[cps]=cid;
	raceRid[cps]=rid;
	Gomenu.AddItem(AddMenuCPItem("檢查點 "+ cid.toString(),"點擊可傳送至檢查點 "+ cid.toString()+"\n賽道ID:"+rid+" 檢查點ID:="+cid,"cp",cps));
	Gomenu.CurrentSelection=3; 
	cps++;
}

function createCheckPoint(pos)
{ 
	API.triggerServerEvent("SC_editrack_createCP",pos);
}
 
