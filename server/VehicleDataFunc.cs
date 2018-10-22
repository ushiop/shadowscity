using System;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
 

public class VehicleDataFunc : Script
{ 	
	private VehicleData p;
	private Vehicle veh;
	
	public VehicleDataFunc()
	{
	}

	public VehicleDataFunc(string vName,VehicleHash vHash,string vMaster,string vModJson,int vMoney,Vector3 vPos,Vector3 vRot,int vC,int vC1,bool alive, long timelimit = 0)
	{
		p=new VehicleData(vName,vHash,vMaster,vModJson,vMoney,vPos,vRot,vC,vC1,alive, timelimit);
	}
	
	public bool getLock()
	{
		return p.vehLock;
	}
	
	public void setLock(bool f)
	{
		if(f==true)
		{//设为真时表示车子被锁定,隐藏+锁引擎+无碰撞
			var ps=API.getVehicleOccupants(veh);
			foreach(var i in ps)
			{
				API.setEntityPosition(i,i.position);
			}
			API.setEntityCollisionless(veh,true);
			API.setEntityTransparency(veh,0);
			API.setVehicleEngineStatus(veh,false);
		}else{
			//设为假时表示被解除锁定,恢复碰撞和隐藏
			API.setEntityCollisionless(veh,false);
			API.setEntityTransparency(veh,255);			
		}
		p.vehLock=f;
		var Player=API.getPlayerFromName(getMaster());
		if(Player!=null)
		{
			var z=new VehicleMain();
			z.updataPlayerVehicleList(Player);
		}
	}
	
	public int getRankLevel()
	{
		return p.vRankLevel;
	}
	
	public void setRankLevel(int lv)
	{
		p.vRankLevel=lv;
	} 
	public string getRank()
	{
		return p.vRank;
	}
	
	public void setRank(string lv)
	{
		p.vRank=lv;
	}
	
	
	public int getSQLID()
	{
		return p.sqlID;
	}
	
	public void setSQLID(int s)
	{
		p.sqlID=s;
	}

	public void setSlot(string js)
	{
		p.vehSlot=js;
	}
	
	public string getModJson()
	{
		return p.vehModJson;
	}
	
	public string getSlotJson()
	{
		return p.vehSlot;
	}
	
	public void setModJson(string js)
	{
		p.vehModJson=js;
		updataVehPos();
		pushVeh();
	}
	
	public void clearSlot(string name)
	{
		if(p.vehSlot==""){ return;} 
		var js=API.fromJson(p.vehSlot);
		if(name=="霓虹灯"){
			API.resetEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Neon");
		}
		js[name]="";
		p.vehSlot=API.toJson(js);
		setSlotValueToVeh();
		setJsonToVehicleMod(getModJson());
		
		js=null; 
	}
	
	public void addSlotValue(string type,string values)
	{
		JObject js;
		if(p.vehSlot!="")
		{
			
			js=API.fromJson(p.vehSlot); 
		}else{
			js=new JObject();
		}
		if(type=="極速"){
			if(js.Property("極速")==null){
				js[type]=Convert.ToSingle(values);
			}else{ 
				js[type]=Convert.ToSingle(js[type].ToString()==""?"0":js[type].ToString())+Convert.ToSingle(values);
			}
		}
		if(type=="霓虹燈"||type=="霓虹燈顏色"){
			if(type=="霓虹燈"){
					API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Neon",1);
			}
								
			js[type]=values;
		}
		p.vehSlot=API.toJson(js);
		setSlotValueToVeh();
		updataVehPos();
		pushVeh();
	}
	
	public void setSlotValueToVeh()
	{
		if(p.vehSlot==""){ return;}
		var js=API.fromJson(p.vehSlot);
		if(js.Property("極速")!=null)
		{
			//API.consoleOutput(API.getVehicleEnginePowerMultiplier(veh).ToString());
			if(js["極速"].ToString().Length!=0){
				API.setVehicleEnginePowerMultiplier(veh,Convert.ToSingle(js["極速"]));
			}else{
				API.setVehicleEnginePowerMultiplier(veh,0);
			}
		}
		if(js.Property("霓虹燈")!=null)
		{
			//API.consoleOutput(API.getVehicleEnginePowerMultiplier(veh).ToString());
			var v=js["霓虹燈"].ToString();
			API.setVehicleNeonState(veh,0,false);
			API.setVehicleNeonState(veh,1,false);
			API.setVehicleNeonState(veh,2,false);
			API.setVehicleNeonState(veh,3,false); 
			if(v.Length!=0){
				string[] type=v.Split(':');
				foreach(var i in type){
					API.setVehicleNeonState(veh,Convert.ToInt32(i),true); 
				}
				if(js.Property("霓虹燈顏色")==null){
					API.setVehicleNeonColor(veh,255,255,255); 
				}else{
					var c=js["霓虹燈顏色"].ToString();
					if(c.Length==0){
						API.setVehicleNeonColor(veh,255,255,255); 
					}
				}
			}
		}
		if(js.Property("霓虹燈顏色")!=null)
		{		
			var v=js["霓虹燈顏色"].ToString();
			API.setVehicleNeonColor(veh,255,255,255);
			if(v.Length!=0){
				string[] type=v.Split(':'); 
				API.setVehicleNeonColor(veh,Convert.ToInt32(type[0]),Convert.ToInt32(type[1]),Convert.ToInt32(type[2])); 			
			}
		}
	}
	
	public string getMaster()
	{
		return p.vehMaster;
	}
	
	public bool getAlive()
	{
		return p.vehAlive;
	}
	
	public void setAlive(bool d)
	{

		p.vehAlive=d;
		if(d==false)
		{
			var ps=API.getVehicleOccupants(veh);
			foreach(var i in ps)
			{
				API.setEntityPosition(i,i.position);
			}
			API.setEntityCollisionless(veh,true);
			API.setEntityTransparency(veh,0);
		}
	}
	
	public int getMoney()
	{
		return p.vehMoney;
	}
	
	public VehicleHash getHash()
	{
		return p.vehHash;
	}
	
	public string getName()
	{
		return p.vehName;
	}
	
	public Vehicle getVeh()
	{
		return veh;
	}
	
	public void setMoney(int m)
	{//在車輛當前價格的基礎上增加m的價格
		p.vehMoney=getMoney()+m;
		
		API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Money",p.vehMoney);
	}
	
	public Vehicle createVeh()
	{//按數據標準生成車輛,自動進行數據綁定
 
			veh=API.createVehicle(p.vehHash,new Vector3(p.vehX,p.vehY,p.vehZ),new Vector3(p.vehRX,p.vehRY,p.vehRZ),p.vehColor,p.vehColor1);
			if(p.vehMaster=="出售")
			{
				API.setEntityInvincible(veh,true);	
 
			}
			if(getAlive()==false)
			{
				API.setEntityCollisionless(veh,true);
				API.setEntityTransparency(veh,0);
			}
			API.setVehicleEngineStatus(veh,false);
			API.setEntityData(veh,"SC_VehicleMain_VehicleData",this); 
			API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master",p.vehMaster);
			API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Money",p.vehMoney);
			API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_SQLID",p.sqlID);
			//API.consoleOutput("T:"+p.vehTimeLimit.ToString());
			API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_TimeLimit",p.vehTimeLimit.ToString());
			//API.consoleOutput("T:"+ API.getEntitySyncedData(veh, "SC_VehicleMain_VehicleData_TimeLimit").ToString());
			if(getSlotJson()!=""){
				var slot=API.fromJson(getSlotJson()); 
				if(slot.Property("霓虹燈")!=null){
					
					API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Neon",1);
				}
				slot=null;
			}
			setJsonToVehicleMod(getModJson());
			setSlotValueToVeh();  
			API.setVehicleNumberPlate(veh,"Shadows");
			return veh;
		
			
	}
	
	private string ModJsonReturnModId(string modjson)
	{
		var s1=modjson.IndexOf('"');
		var s2=modjson.LastIndexOf('"')-1;
		return modjson.Substring(s1+1,s2-s1);
	} 
	
	
	public void setJsonToVehicleMod(string json)
	{
			if(json==""){return;}
			var modlist=API.fromJson(json);
			var wheelID=0;
			foreach(var x in modlist)
			{//x="modID:indexID"
				var modId=ModJsonReturnModId(x.ToString());
				var indexId=Convert.ToInt32(modlist[modId].ToString());

				modId=Convert.ToInt32(modId);
				if(modId<1000){					
					if(modId==22||modId==18){
						if(indexId==-1)
						{
							indexId=0;
						}
					}
					API.setVehicleMod(veh,modId,indexId);
					if(modId==23){
						wheelID=indexId;
					}
					
				}else{ 
					if(indexId==-1)
					{
						indexId=0;
					}
					if(modId==10000){
						API.setVehiclePrimaryColor(veh,indexId);
					}
					if(modId==10001){
						API.setVehicleSecondaryColor(veh,indexId);// 設置車輛副色(同上)
					}			
					if(modId==10006){ 
						//輪胎類型
						API.setVehicleWheelType(veh,indexId);	 
						API.setVehicleMod(veh,23,wheelID);
						 
					}
					if(modId==10007){
						API.setVehicleWheelColor(veh,indexId);
					}
				}
			} 
	}
	
	public Vehicle respawnVeh()
	{//重生車輛在當前車輛的位置
		updataVehPos();
		API.deleteEntity(veh);
		return createVeh();
	}
	
	public void updataVehPos()
	{
		var pos=API.getEntityPosition(veh);
		var rot=API.getEntityRotation(veh);
		p.vehX=pos.X;
		p.vehY=pos.Y;
		p.vehZ=pos.Z;
		p.vehRX=rot.X;
		p.vehRY=rot.Y;
		p.vehRZ=rot.Z;
	}
	
	public void changeMaster(string newMaster,Client Player=null,Client Target=null)
	{//使車輛的主人進行更換
		p.vehMaster=newMaster;
		API.setEntitySyncedData(veh,"SC_VehicleMain_VehicleData_Master",p.vehMaster);
		if(Player!=null&&Target!=null)
		{
			p.vehLock=false;
			API.setEntityCollisionless(veh,false);
			API.setEntityTransparency(veh,255);	
			API.getEntityData(Target,"SC_VehicleList").Add(this);
			API.getEntityData(Player,"SC_VehicleList").Remove(this);
			
			
			var l=API.getEntityData(Target,"SC_VehicleList");
			var d=new List<VehicleInfo>();
			foreach(var i in l)
			{ 
				d.Add(new VehicleInfo(i.getName(),i.getSQLID(),i.getAlive(),i.getMoney(),i.getLock()));
			} 
			API.triggerClientEvent(Target, "SC_Vehicle_CALL_LISTUPDATA",API.toJson(d) );
			
			l=API.getEntityData(Player,"SC_VehicleList");
			d=new List<VehicleInfo>();
			foreach(var i in l)
			{ 
				d.Add(new VehicleInfo(i.getName(),i.getSQLID(),i.getAlive(),i.getMoney(),i.getLock()));
			} 
			API.triggerClientEvent(Player, "SC_Vehicle_CALL_LISTUPDATA",API.toJson(d) );
			d=null;
			l=null;
		}
	}
	
	public void pushVeh()
	{//將車輛保存進數據庫
	//vID,vTime,vValue
		var sql="";
		if(p.sqlID==-1)
		{//新增
	
			
			var t=UnixTime.getUnixTimeToMS().ToString();
			sql="INSERT INTO Veh (vTime,vValue) VALUES ('"+t+"','"+API.toJson(p)+"')";
			var _vid=0;
			VehicleMain.VehDB.sqlCommand(sql);
			DataBaseSdon[] rr=VehicleMain.VehDB.sqlCommandReturn("Veh","vTime='"+t+"'","vID"); 
			_vid=Convert.ToInt32(rr[0].Get("vID"));
			rr=null;
			p.sqlID=_vid;
			p.vehSlot="";
			pushVeh();
		}else{
		 //更新
			//API.consoleOutput(API.toJson(this));
			sql="UPDATE Veh SET vValue = '"+API.toJson(p)+"' WHERE vID = "+p.sqlID+";";
			VehicleMain.VehDB.sqlCommand(sql);
		}
		
	}
	
}

public class VehicleData 
{ 	
	
	public string vehName="";//車輛名字(內部名)
	public VehicleHash vehHash;//車輛HASH
	public string vehMaster="";//車輛主人名字 
	public string vehModJson="";//車輛改裝JS串
	public int vehMoney=0;//車輛價值
	public float vehX;//車輛X座標
	public float vehY;//車輛Y座標
	public float vehZ;//車輛Z座標
	public float vehRX;//車輛X旋轉
	public float vehRY;//車輛Y旋轉
	public float vehRZ;//車輛Z旋轉
	public bool vehAlive;//車輛是否存活
	public int vehColor=0;//車輛顏色
	public int vehColor1=0;//車輛顏色1
	public int sqlID=-1;//數據庫ID 
	public string vehSlot="";//車輛強化JSON串
	public bool vehLock=false;//车辆是否被锁定,用于出售判定,不从数据库中读取.
	public int vRankLevel=0;//车辆评分
	public string vRank="~u~C";//车辆评级
	public long vehTimeLimit = 0;//车辆限时
	
	public VehicleData(){}
	
	public VehicleData(string vName,VehicleHash vHash,string vMaster,string vModJson,int vMoney,Vector3 vPos,Vector3 vRot,int vC,int vC1,bool alive, long timelimit)
	{
		vehName=vName;
		vehHash=vHash;
		vehMaster=vMaster;
		vehModJson=vModJson;
		vehMoney=vMoney;
		vehX=vPos.X;
		vehY=vPos.Y;
		vehZ=vPos.Z;
		vehRX=vRot.X;
		vehRY=vRot.Y;
		vehRZ=vRot.Z;
		vehColor=vC;
		vehColor1=vC1;
		vehAlive=alive;
		vehLock=false;
		vehTimeLimit = timelimit;
	}	
	
}