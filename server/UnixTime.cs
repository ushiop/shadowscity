using System;
using GTANetworkServer;
using GTANetworkShared;

 

public class UnixTime : Script
{ 	

	public UnixTime()
	{
	}

	public static  long getUnixTimeToS()
	{//返回基于unix时间戳的秒数
		DateTime timeStamp=new DateTime(1970,1,1);  //得到1970年的时间戳  
		long a=(DateTime.UtcNow.Ticks-timeStamp.Ticks)/10000000;  //注意这里有时区问题，用now就要减掉8
		return a;
	}
	
	public static  long getUnixTimeToMS()
	{//返回基于unix时间戳的毫秒数
		DateTime timeStamp=new DateTime(1970,1,1);  //得到1970年的时间戳  
		long a=(DateTime.UtcNow.Ticks-timeStamp.Ticks)/10000;  //注意这里有时区问题，用now就要减掉8
		return a;
	}
	
}