using System;
using GTANetworkServer;
using GTANetworkShared;

using System.Collections.Generic;
 

public class weather : Script
{ 	
	private long weathertimeset = UnixTime.getUnixTimeToS();
	private long timeset = UnixTime.getUnixTimeToS();
	private int weatherid = 0;
	public weather()
	{
		weatherid = 0;
		API.onUpdate += OnUpdate;
		API.setWeather(weatherid);
	} 
	
	private void OnUpdate()
	{
		if(UnixTime.getUnixTimeToS() > timeset)
		{
			timeset = UnixTime.getUnixTimeToS() + 60;
			DateTime dt = DateTime.Now;
			int h =	Convert.ToInt32(dt.Hour.ToString()), m = Convert.ToInt32(dt.Minute.ToString());
			API.setTime(h, m);
		}
		if(UnixTime.getUnixTimeToS() > weathertimeset)
		{
			weathertimeset = UnixTime.getUnixTimeToS() + 1800;
			weatherid ++;
			if(weatherid > 6)
			{
				weatherid = 0;
			}
			API.setWeather(weatherid);	
		} 
	}
}