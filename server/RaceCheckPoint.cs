using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

 

public class RaceCheckPoint : Script
{ 	
	public int raceId=0;//該檢查點屬於哪個賽道	
	public int dbId=0;//該檢查點的數據庫索引，用於協助排序
	public float X;
	public float Y;
	public float Z;
	public RaceCheckPoint()
	{
	}
	public RaceCheckPoint(int rID,Vector3 rPos,int dID)
	{//帶參構造
		raceId=rID;
		X=rPos.X;
		Y=rPos.Y;
		Z=rPos.Z;
		dbId=dID;
	}
}

public class RaceNextCheckPoint : Script
{ 	
	public int raceId=0;//該檢查點屬於哪個賽道	
	public int dbId=0;//該檢查點的數據庫索引，用於協助排序
	public float X;
	public float Y;
	public float Z;
	public float nX;
	public float nY;
	public float nZ;
	public RaceNextCheckPoint()
	{
	}	
	public RaceNextCheckPoint(int rID,Vector3 rPos,Vector3 nPos,int dID)
	{//帶參構造
		raceId=rID;
		X=rPos.X;
		Y=rPos.Y;
		Z=rPos.Z;
		dbId=dID;
		if(nPos!=null)
		{
			Vector3 newDir = null;
            Vector3 dir =nPos.Subtract(rPos);
            dir.Normalize();
            newDir = dir;
			nX=newDir.X;
			nY=newDir.Y;
			nZ=newDir.Z;
        }

	}
}