using System;
using GTANetworkServer;
using GTANetworkShared;

using System.Collections.Generic;
 

public class html_create : Script
{ 	
	private long html_create_cd = UnixTime.getUnixTimeToS()+5;

	public html_create()
	{ 
		API.onUpdate += OnUpdate; 
		
	} 
	
	private void OnUpdate()
	{
		if(UnixTime.getUnixTimeToS()>html_create_cd)
		{
			//每半小时更新一次页面
			html_create_cd=UnixTime.getUnixTimeToS()+1800;
			create_road_html();
		}
	}
	
	private void create_road_html()
	{//生成赛道表
		string msg="";
		foreach(RaceRoad v in track.raceRoad)
		{
			
			var top=v.getRaceTopInTop(1);
			if(top!=null)
			{
				msg=msg+"<tr>";
				msg=msg+"<td>"+v.getRaceName()+"</td>";//赛道名
				msg=msg+"<td>"+top.rcarname+"</td>";//车辆
				msg=msg+"<td>"+(top.rtime/1000.0).ToString()+"s</td>";//时间
				msg=msg+"<td>"+top.playername+"</td>";//玩家
				msg=msg+"<td>"+top.team+"</td>";//车队	
				msg=msg+"</tr>";				
			}
		}
		foreach(RaceRoad v in track.raceRoad)
		{
			
			var top=v.getRaceTopInTop(1);
			if(top==null)
			{
				msg=msg+"<tr>";
				msg=msg+"<td>"+v.getRaceName()+"</td>";//赛道名
				msg=msg+"<td>无数据</td>";//车辆
				msg=msg+"<td>无数据</td>";//时间
				msg=msg+"<td>无数据</td>";//玩家
				msg=msg+"<td>无数据</td>";//车队	

				msg=msg+"</tr>";				
			}
		}
		var p=new HTML("index_origin.html","resources\\shadowscity\\html\\");
		//API.consoleOutput(p.Path());
		p.loadHtml();
		p.replace("$list$",msg);
		p.replace("$createtime$","页面生成于"+DateTime.Now.ToString());
		//API.consoleOutput(p.Html());
		p.saveHtml("index.html");
	}
	
}