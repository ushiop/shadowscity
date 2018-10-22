using System;
using GTANetworkServer;
using GTANetworkShared; 
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
 

public class track : Script
{ 	
	
	public static DataBase trackDB;
	public static List<RaceRoad> raceRoad=new List<RaceRoad>();
	
	public track()
	{
		API.onResourceStart += onResourceStart;
		API.onResourceStop += onResourceStop;
		API.onClientEventTrigger += OnClientEvent;  
		API.onPlayerFinishedDownload += OnPlayerFinshedDownload;
		API.onPlayerDisconnected += OnPlayerDisconnected; 
	} 

	public void onResourceStart()
	{ 
      API.consoleOutput("賽道讀取！！");
	  trackDB=new DataBase("Race.db","resources\\shadowscity_db\\");
	  if(trackDB.isDataBase()==false)
	  {
		  API.consoleOutput("赛道库不存在!!創建！！！");
		  trackDB.createDataBase();
		  trackDB.connectOpenToDataBase();
		  API.consoleOutput("赛道建主表！！！");
		  var sql="PRAGMA synchronous = OFF";
		  trackDB.sqlCommand(sql);
		  sql="CREATE TABLE Race( rID INTEGER PRIMARY KEY AUTOINCREMENT  ,rName NOT NULL,rPassword NOT NULL,rCreate NOT NULL,rTopTime,rTopTimePlayer); ";
		  trackDB.sqlCommand(sql);
		  sql="CREATE TABLE CP( cID INTEGER PRIMARY KEY AUTOINCREMENT,rID,rX NOT NULL,rY NOT NULL,rZ NOT NULL,rCreateTime NOT NULL); ";
		  trackDB.sqlCommand(sql);
	  }else{
		trackDB.connectOpenToDataBase();
		var sql="PRAGMA synchronous = OFF";
		trackDB.sqlCommand(sql);
	  }
	  //開始讀取賽道 
	  DataBaseSdon[] rr=trackDB.sqlCommandReturn("Race","","rID","rName","rPassword","rCreate","rTopTime","rTopTimePlayer"); 
	  int _rid,_rtoptime;
	  string _rname="",_rpas="",_rc="",_rtopp="";
	  foreach(DataBaseSdon v in rr)
	  {
		_rid=Convert.ToInt32(v.Get("rID"));
		_rname=v.Get("rName");
		_rpas=v.Get("rPassword");
		_rc=v.Get("rCreate");

		RaceRoad r=new RaceRoad(_rid,_rname,_rpas,_rc,"-ShadowsCity-",0); 
		if(v.Get("rTopTime").IndexOf("[")!=-1)
		{//找到记录数据
			//读取
				var js=v.Get("rTopTime");
				//對於有數組結構的json字符串，必須使用 JArray.Parse來轉換
				//需引用using Newtonsoft.Json.Linq;
				//GTA-N的API.fromJson暫時不支持帶數組結構的json字符串
				//服務端版本:v0.1.367.419
				JArray jsonVals = JArray.Parse(js); 
				foreach(var i in jsonVals)
				{
					r.setTimeInTopTime(i["team"]==null?"":i["team"].ToString(),Convert.ToInt32(i["rtime"]),i["playername"].ToString(),i["rcarname"].ToString(),i["rcarlevel"].ToString(),Convert.ToInt32(i["playerportrait"]),i["rcreatetime"].ToString());
				}
				r.sortTopTime();
		}
		DataBaseSdon[] cp=trackDB.sqlCommandReturn("CP","rID=='"+_rid.ToString()+"'","cID","rX","rY","rZ");
		foreach(DataBaseSdon z in cp)
		{
			r.addCheckPoint(new Vector3(Convert.ToSingle(z.Get("rX")),Convert.ToSingle(z.Get("rY")),Convert.ToSingle(z.Get("rZ"))),Convert.ToInt32(z.Get("cID")));
		}
		raceRoad.Add(r);
		cp=null;
		r=null;
	  }
 
	  onTrackListChange();
	   
    }
	
	public void onResourceStop()
	{ 
		trackDB.closeToDataBase(); 
		API.consoleOutput("賽道庫關閉！！！！！");
    }	
	
 
	
	public void OnClientEvent(Client Player, string eventName, params object[] arguments) //arguments param can contain multiple params
	{
		if(eventName=="track_createRace")
		{
			var raceName=arguments[0].ToString();
			var racePassword=arguments[1].ToString();
			var createState=-1;
			foreach(RaceRoad v in raceRoad)
			{ 
				if(raceName==v.getRaceName())
				{
					createState=1;
				}
			}
			if(createState==1)
			{
				API.sendNotificationToPlayer(Player,"賽道:"+raceName+"\n創建~r~失敗~w~\n~g~原因 ~w~有重名賽道");
			}else{
				var sql="INSERT INTO Race (rName,rPassword,rCreate,rTopTimePlayer,rTopTime) VALUES ('"+raceName+"','"+racePassword+"','"+API.getPlayerName(Player)+"','-ShadowsCity-','[]')";
				var _rid=0;
				trackDB.sqlCommand(sql);
				DataBaseSdon[] rr=trackDB.sqlCommandReturn("Race","rName='"+raceName+"' AND rPassword='"+racePassword+"'","rID","rName","rPassword"); 
				_rid=Convert.ToInt32(rr[0].Get("rID"));
				rr=null;
				RaceRoad r=new RaceRoad(_rid,raceName,racePassword,API.getPlayerName(Player),"",999999); 
				raceRoad.Add(r);
				r=null;
				onTrackListChange();
			}
		}
		if(eventName=="track_editRace")
		{
			var raceName=arguments[0].ToString();
			var racePassword=arguments[1].ToString();
			RaceRoad createState=null;
			foreach(RaceRoad v in raceRoad)
			{ 
				if(raceName==v.getRaceName()&&racePassword==v.getRacePassword())
				{
					createState=v;
				}
			}
			if(createState==null)
			{
				API.sendNotificationToPlayer(Player,"賽道:"+raceName+"\n編輯~r~失敗~w~\n~g~原因 ~w~找不到賽道/密碼錯誤");
			}else{
				if(createState.getEditState()==true){
					API.sendNotificationToPlayer(Player,"賽道:"+raceName+"\n編輯~r~失敗~w~\n~g~原因 ~w~該賽道正在被編輯");	
				}else{				 
					foreach(var i in Race.House)
					{
						if(i.getRaceRoad().getRaceRid()==createState.getRaceRid())
						{ 
							i.sendMessageToPlayer(" 賽事被關閉了!原因: 賽道正在被編輯");
							i.closeRaceHouse();
						}
					}
					API.setEntityData(Player,"SC_track_editrace",createState); 
					API.setEntitySyncedData(Player,"SC_track_editrace_state",1);
					API.triggerClientEvent(Player, "SC_track_oneditrace" );
					API.triggerClientEvent(Player, "SC_track_editracedata",createState.getRacePassword(),createState.getRaceTopPlayer(1),createState.getRaceTopTime(1),createState.getRaceRid().ToString(),createState.getRaceName() );
					createState.setEditState(true);
					RaceCheckPoint[] cp= createState.getAllCheckPoint(); 
					Vector3[] cPos=new Vector3[cp.Length]; 
					int[] cRid=new int[cp.Length];
					int[] cDBid=new int[cp.Length];
					var idx=0;
					foreach(var i in cp)
					{
						cPos[idx]=new Vector3(i.X,i.Y,i.Z);
						cRid[idx]=i.raceId;
						cDBid[idx]=i.dbId;
						idx++;
					}
					API.triggerClientEvent(Player,"SC_track_editracedata_cps",API.toJson(cRid),API.toJson(cDBid),cPos);
					cp=null; 
				}
				createState=null;
			}
		}
		if(eventName=="SC_editrack_off")
		{ 
			API.getEntityData(Player,"SC_track_editrace").setEditState(false); 
			API.resetEntityData(Player,"SC_track_editrace");
			API.resetEntitySyncedData(Player,"SC_track_editrace_state");		
		}
		if(eventName=="SC_editrack_changepass")
		{
			var np=arguments[0].ToString();
			var d=API.getEntityData(Player,"SC_track_editrace");
			d.setRacePassword(np);
			var sql="UPDATE Race SET rPassword = '"+np+"' WHERE rID = "+d.getRaceRid()+";";
			d=null;
			trackDB.sqlCommand(sql);
			
		}
		if(eventName=="SC_editrack_cleartoptime")
		{
			var d=API.getEntityData(Player,"SC_track_editrace");
			d.clearTopTime();
			d=null;
			new TeamMain().teamRoadBoss();
			onTrackListChange();
			
		}
		if(eventName=="SC_editrack_createCP")
		{
			var d=API.getEntityData(Player,"SC_track_editrace");
			Vector3 pos=(Vector3)arguments[0];
			var time=new Random().Next(0,10000000);
			var sql="INSERT INTO CP (rID,rX,rY,rZ,rCreateTime) VALUES ('"+d.getRaceRid().ToString()+"','"+pos.X.ToString()+"','"+pos.Y.ToString()+"','"+pos.Z.ToString()+"','"+time.ToString()+"')";
			var _cid=0;
			trackDB.sqlCommand(sql);
			DataBaseSdon[] rr=trackDB.sqlCommandReturn("CP","rCreateTime='"+time.ToString()+"'","cID"); 
			_cid=Convert.ToInt32(rr[0].Get("cID"));
			d.addCheckPoint(pos,_cid); 
			rr=null; 
			API.triggerClientEvent(Player, "SC_track_editracedata_cp",d.getRaceRid().ToString(),_cid,pos);
		}
		if(eventName=="SC_editrack_gotocp")
		{
			var d=API.getEntityData(Player,"SC_track_editrace");
			var cp=d.getCheckPoint(Convert.ToInt32(arguments[0]));
			if(cp!=null)
			{
				Vector3 targetPos=new Vector3(cp.X,cp.Y,cp.Z);
				//get player vehicle
				var veh=API.getPlayerVehicle(Player);
				if(API.isPlayerInAnyVehicle(Player)==false)
				{
					API.setEntityPosition(Player, targetPos);
				}else{
					var seat=API.getPlayerVehicleSeat(Player);
					if(seat==-1)
					{
						API.setEntityPosition(veh,targetPos);
					}else{
						API.setEntityPosition(Player, targetPos);
					}
				} 
			}
			
		}
		if(eventName=="SC_editrack_deleteCP")
		{
			var d=API.getEntityData(Player,"SC_track_editrace");
			var cp=Convert.ToInt32(arguments[0]);
			var sql="DELETE FROM CP WHERE cID='"+cp.ToString()+"';";
			d.removeCheckPoint(cp);
			trackDB.sqlCommand(sql);
			d=null;
			API.triggerClientEvent(Player, "SC_editrack_deleteCP_ok",cp);
		}
		if(eventName=="SC_editrack_deleteroad")
		{
			 
			var d=API.getEntityData(Player,"SC_track_editrace");
			d.setEditState(false); 
			var sql="DELETE FROM CP WHERE rID='"+d.getRaceRid()+"';"; 
			trackDB.sqlCommand(sql);
			sql="DELETE FROM Race WHERE rID='"+d.getRaceRid()+"';"; 
			trackDB.sqlCommand(sql);
			raceRoad.Remove(d);
			d=null;
			API.resetEntityData(Player,"SC_track_editrace");
			API.resetEntitySyncedData(Player,"SC_track_editrace_state");	
			new TeamMain().teamRoadBoss();
				onTrackListChange();
		}
		if(eventName=="SC_editrack_getlist")
		{
			onTrackListChange();
		}
	}
	
	private string trackListToJson()
	{
		string[,] pl=new string[raceRoad.Count,5];
		var i=0;
		foreach(RaceRoad x in raceRoad)
		{	 
			pl[i,0]=x.getRaceName();
			pl[i,1]=x.getRaceRid().ToString();
			pl[i,2]=x.getRaceCreatePlayer();
			pl[i,3]=x.getRaceTopPlayer(1);
			pl[i,4]=x.getRaceTopTime(1);
			i++;
		} 
		return API.toJson(pl);
	}	
	
	public   void onTrackListChange()
	{//當賽道列表刷新時觸發該函數(創建賽道或刪除賽道)
	 //將新的賽道列表分發給客戶端
		API.triggerClientEventForAll("SC_tracklistchange",trackListToJson());
	}
	
	
	
	private void OnPlayerFinshedDownload(Client Player)
	{ 
		API.triggerClientEvent(Player, "SC_tracklistchange",trackListToJson() );
	}
 
 	
	private void OnPlayerDisconnected(Client Player,string reason)
	{
		if(API.getEntityData(Player,"SC_track_editrace")!=null){
			API.getEntityData(Player,"SC_track_editrace").setEditState(false); 
			API.resetEntityData(Player,"SC_track_editrace");
			API.resetEntitySyncedData(Player,"SC_track_editrace_state");
		}
	}
	
 
}