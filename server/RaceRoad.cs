using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;
using System.Linq;

 

public class RaceRoad : Script
{ 	
	private int raceId=0;
	private string raceName="";
	private string racePassword="";
	private string raceCreate="";
	//private string topPlayer="";
	//private long  topPlayerTime=999999; 
	private List<RaceCheckPoint> checkPoint=new List<RaceCheckPoint>();
	private bool editState=false;
	private List<RaceTop> topList;
	
	public RaceRoad()
	{
	} 
	
	public RaceRoad(int rID,string rName,string rPass,string rCreate,string rTopTimePlayer,int rTopTime)
	{//帶參構造
		raceName=rName;
		racePassword=rPass;
		raceId=rID;
		raceCreate=rCreate;
		//topPlayer=rTopTimePlayer;
		//topPlayerTime=rTopTime;
		topList=new List<RaceTop>();
	}
	
	public void setEditState(bool edit)
	{//設置編輯狀態
		editState=edit;
	}
	
	public bool getEditState()
	{//獲取編輯狀態
		return editState;
	}
	
	public void clearTopTime()
	{//清空該賽道的記錄
		topList=new List<RaceTop>();
		var sql="UPDATE Race SET 'rTopTime' = '"+API.toJson(topList)+"' WHERE rID = "+raceId.ToString()+";";
		track.trackDB.sqlCommand(sql);
	}
 
	public RaceTop getPlayerInTopTime(string PlayerName)
	{//获取某玩家是否在该赛道的记录中,不在则返回空
		foreach(var i in topList)
		{
			if(i.playername==PlayerName)
			{
				return i;
			}
		}
		return null;
	}
	
	private static int SortCompare(RaceTop AF1, RaceTop AF2)
	{
		int res = 0;
		if (AF1.rtime > AF2.rtime)
		{
			res = 1;
		}
		else if (AF1.rtime < AF2.rtime)
		{
			res = -1;
		}
		return res;
	}
	
	public bool getTimeInTopTime(long time)
	{//判断某时间是否为TOP1
		if(time<=getRaceTopTimeToLong(1)&&getRaceTopTimeToLong(1)!=-1)
		{
			return true;
		}
		return false;
	}
	
	public bool setTimeInTopTime(string _team,long time,string playername,string rcarname,string rcarlevel,int ptx,string ctime=null)
	{//設置某時間進入top表,true为进了，FALSE为没进
		RaceTop p=getPlayerInTopTime(playername);
		if(p==null)
		{
			 
			p=new RaceTop(_team,playername,time,rcarname,rcarlevel,ptx,ctime==null? DateTime.Now.ToString("yyyy-MM-dd"):ctime);
			topList.Add(p);
			return true;
		}else{
			if(time<p.rtime)
			{
				p.team=_team;
				p.rcarname=rcarname;
				p.rcarlevel=rcarlevel;
				p.playerportrait=ptx;
				p.rtime=time;
				p.rcreatetime=  DateTime.Now.ToString("yyyy-MM-dd");
				return true;
			}else{
				return false;
			}
		}
	}
	
	public void sortTopTime()
	{//对记录进行排序
		topList.Sort(SortCompare);
	}
	
	public void saveTopTime()
	{//将记录更新至数据库
		var sql="UPDATE Race SET 'rTopTime' = '"+API.toJson(topList)+"' WHERE rID = "+raceId.ToString()+";";
		API.consoleOutput(sql);
		track.trackDB.sqlCommand(sql);
	}
	
	public RaceTop getRaceTopInTop(int top)
	{//返回指定記錄的數據結構,不存在則返回NULL
		RaceTop p=null;
		top=top-1;
		if(top>topList.Count||topList.Any()==false)
		{
			return p;
		}
		p=topList[top];
		return p;		
	}
	
	public string getRaceTopPlayer(int top)
	{//返回指定记录名词的保持者,不存在则返回SC_NULL
		top=top-1;
		if(top>topList.Count||topList.Any()==false)
		{
			return "SC_NULL";
		}
		return topList[top].playername;
	}
	
	public string getRaceTopTime(int top)
	{//返回记录时间(字符串,不存在则返回SC_NULL
		top=top-1;
		if(top>topList.Count||topList.Any()==false)
		{
			return "SC_NULL";
		}
		return topList[top].rtime.ToString();
	}
	
	public long getRaceTopTimeToLong(int top)
	{//返回记录时间,长整数,不存在返回负数
		top=top-1;
		if(top>topList.Count||topList.Any()==false)
		{
			return -1;
		}
		return topList[top].rtime;
	}
	
	public List<RaceTop> getRoadTopList()
	{
		return topList;
	}
	
	public string getRaceCreatePlayer()
	{//返回創建者
		return raceCreate;
	}
	
	public int getRaceRid()
	{//返回該賽道在數據庫中的索引號
		return raceId;
	}
	
	public string getRaceName()
	{//獲取賽道名稱
		return raceName;
	}
	
	public void setRacePassword(string pas)
	{//設置賽道密碼
		racePassword=pas;
	}
	
	public string getRacePassword()
	{//獲取賽道密碼
		return racePassword;
	}
	
	public int getRaceCheckPointRows()
	{//獲取賽道檢查點格數
		return checkPoint.Count;
	}
  
	public void addCheckPoint(Vector3 pos,int dbID)
	{//添加一個檢查點給該賽道
		RaceCheckPoint r=new RaceCheckPoint(raceId,pos,dbID);
		checkPoint.Add(r);
		r=null; 
	}
	
	public RaceCheckPoint getCheckPoint(int cId)
	{//獲取指定索引的檢查點 
	//索引為數據庫索引而不是變量索引
		foreach(RaceCheckPoint i in checkPoint)
		{
			if(i.dbId==cId)
			{
				return i;
			}
		}
		return null;
	}
	
	public bool removeCheckPoint(int cId)
	{//刪除指定索引的檢查點，索引为數據庫索引
		foreach(RaceCheckPoint i in checkPoint)
		{
			if(i.dbId==cId)
			{
				checkPoint.Remove(getCheckPoint(cId));
				return true;
			}
		}
		return false;
	}
	
	public RaceCheckPoint[] getAllCheckPoint()
	{
		return checkPoint.ToArray();
	}
	
	
}


public class RaceTop
{
	public string team;
	public string playername;
	public long rtime;
	public string rcarname;
	public string rcarlevel;
	public int playerportrait;
	public string rcreatetime;
	
	public RaceTop(){}
	
	public RaceTop(string _team,string name,long time,string carname,string carlevel,int ptx,string rctime)
	{
		team=_team;
		playername=name;
		rtime=time;
		rcarname=carname;
		rcarlevel=carlevel;
		playerportrait=ptx;
		rcreatetime=rctime;
	}
}