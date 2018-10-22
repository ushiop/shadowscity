using System;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
 
public class acl : Script
{
	private static List<AclData> aclList=new List<AclData>();
	
	public acl()
	{
 
		API.onClientEventTrigger += OnClientEvent;
		API.onEntityDataChange+= OnEntityDataChange; 
		API.onPlayerFinishedDownload += OnPlayerFinshedDownload;
		API.onPlayerConnected += OnPlayerConnected;
		API.onPlayerDisconnected += OnPlayerDisconnected;
		aclList.Add(new AclData("Race_Edit","","賽道創建/編輯介面(F2)的使用權限."));
		aclList.Add(new AclData("Goto_Take_Vehicle","","傳送至玩家時是否允許帶車傳送的權限."));
		/*for(var i=1;i<67;i++)
		{生成meta.xml的批量文件声明
				API.consoleOutput(  "<file src=\"res\\img\\login_logo\\00"+ ((i<10)?"0"+i.ToString():i.ToString()) +".png\" />");
		}	*/		
	}
	
	private void OnPlayerConnected(Client Player)
	{
		API.triggerClientEventForAll("SC_ACL_playerlist_updata",PlayerListToJson());
		
	}
	
	private void OnPlayerFinshedDownload(Client Player)
	{
		
		API.triggerClientEvent(Player, "SC_ACL_cooooooooooooooooool","SC_ACL_HAS_Name:SC_Admin");
		API.triggerClientEvent(Player, "SC_ACL_playerlist_updata",PlayerListToJson() );
 	}
	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		API.triggerClientEventForAll("SC_ACL_playerlist_updata",PlayerListToJson(API.getPlayerName(Player)));

	}
	
	private string PlayerListToJson(string name=null)
	{
		var players=API.getAllPlayers();
		List<string> str=new List<string>();
 
			foreach(Client x in players)
			{
				if(API.getPlayerName(x)!=name){
					str.Add(API.getPlayerName(x));
				}
			}
	 
		return API.toJson(str.ToArray());
	}
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="SC_ACL_edit_on")
		{
			var pName=arguments[0].ToString();
			var target=API.getPlayerFromName(pName);
			if(target!=null)
			{
				if(API.getEntityData(target,"SC_ACL_edit_state")==null)
				{ 
					API.setEntityData(Player,"SC_ACL_edit_to",target);
					API.setEntityData(target,"SC_ACL_edit_state",1);
					API.triggerClientEvent(Player,"SC_ACL_edit_on_ok",pName,API.toJson(API.getEntityData(target,"SC_ACL")),API.toJson(aclList));
				}else{
					
					API.sendNotificationToPlayer(Player,"編輯 ~g~"+pName+"\n~w~權限表失敗\n原因 ~r~該玩家權限表正在被編輯",false);
				}
			}else{
				API.sendNotificationToPlayer(Player,"編輯 ~g~"+pName+"\n~w~權限表失敗\n 原因 ~r~該玩家不在綫",false);
				
			}
		}
		if(eventName=="SC_ACL_edit_off")
		{
			var target=API.getEntityData(Player,"SC_ACL_edit_to");
			API.resetEntityData(target,"SC_ACL_edit_state");
			API.resetEntityData(Player,"SC_ACL_edit_to");
			API.triggerClientEvent(Player,"SC_ACL_edit_off_ok",PlayerListToJson() );
 
		}
		if(eventName=="SC_ACL_edit_add_aclname")
		{
			//新增權限
			var name=arguments[0].ToString();
			var target=API.getEntityData(Player,"SC_ACL_edit_to");
			var acllist=API.getEntityData(target,"SC_ACL");
			acllist.Add(new AclData(name,"",""));
			API.triggerClientEvent(Player,"SC_ACL_edit_listupdata",API.toJson(acllist));
			var lg=new login();
			lg.SetPlayerAccess(target,"SC_ACL",API.toJson(acllist));
			
			API.setEntitySyncedData(target,"SC_ACL_HAS_Name:"+name,1);
			lg=null;
			acllist=null;

		}
		if(eventName=="SC_ACL_edit_delete")
		{
			var name=arguments[0].ToString();
			var target=API.getEntityData(Player,"SC_ACL_edit_to");
			var acllist=API.getEntityData(target,"SC_ACL");
			AclData p=null;
			foreach(AclData v in acllist)
			{
				if(v.aclName==name)
				{
					p=v;
				}
			}
			if(p!=null){
				acllist.Remove(p);
			}
			
			API.resetEntitySyncedData(Player,"SC_ACL_HAS_Name:"+name);
			API.triggerClientEvent(Player,"SC_ACL_edit_listupdata",API.toJson(acllist));
			var lg=new login();
			lg.SetPlayerAccess(Player,"SC_ACL",API.toJson(acllist));
			lg=null;
			acllist=null;
		}
	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_Login_Status")
		{//玩家登录时读取他的权限
			List<AclData> p=new List<AclData>();
			var lg=new login();
			var Player=API.getPlayerFromHandle(entity);
			if(lg.GetPlayerAccess(Player,"SC_ACL")!="SC_NULL")
			{
				var js=lg.GetPlayerAccess(Player,"SC_ACL"); 
				//對於有數組結構的json字符串，必須使用 JArray.Parse來轉換
				//需引用using Newtonsoft.Json.Linq;
				//GTA-N的API.fromJson暫時不支持帶數組結構的json字符串
				//服務端版本:v0.1.367.419
				JArray jsonVals = JArray.Parse(js);
				var allACL=0;
				foreach(var i in jsonVals)
				{
					if(i["aclName"].ToString()=="SC_Admin")
					{
						allACL=1;
					}
					var d=new AclData(i["aclName"].ToString(),"","");
					API.setEntitySyncedData(Player,"SC_ACL_HAS_Name:"+i["aclName"],1);
					p.Add(d);
					d=null;
				}
				if(allACL==1)
				{
					p=new List<AclData>();
					p.Add(new AclData("SC_Admin","","~g~警告:該權限為超管權限\n擁有該權限後就擁有了其餘所有權限"));
					API.setEntitySyncedData(Player,"SC_ACL_HAS_Name:SC_Admin",1);
					foreach(var i in aclList)
					{
						
						//API.consoleOutput(i);
						var d=new AclData(i.aclName.ToString(),"","");
						API.setEntitySyncedData(Player,"SC_ACL_HAS_Name:"+i.aclName,1);
						p.Add(d);
						d=null;
					}

				}
			}
			API.setEntityData(entity,"SC_ACL",p);
			lg=null;
			p=null;
		}

	}
}
 
 
public class AclData   
{ 	
	public string aclName="";//权限名
	public string aclLevel="";//权限等级
	public string aclTip="";//權限描述
	
	public AclData()
	{ 
	} 
	
	public AclData(string aName,string aLv,string aTip="")
	{
		aclName=aName;
		aclLevel=aLv;
		aclTip=aTip;
	}
}