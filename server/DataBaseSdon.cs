using System;
using System.IO;
using System.Data.SQLite;
using GTANetworkServer;
using GTANetworkShared; 
using System.Collections.Generic;

 
/*
SQlite.dll下载地址:
http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
版本选择:Precompiled Statically-Linked Binaries for 64-bit Windows (.NET Framework 4.0)
		 sqlite-netFx40-static-binary-bundle-x64-2010-1.0.104.0.zip(2.66 MiB) 		
*/

public class DataBaseSdon : Script
{ 	
	private string tablename;//该结果所属的表名 
	private int rId;//结果集中的ID 
	private List<string> Sdon=new List<string>();//结果集中的键:值对
	
	public DataBaseSdon()
	{ 
		//毛都没有 
		/*if(isDataBase("test.db","resources\\shadowscity\\db\\")==true)
		{
			API.consoleOutput("存在啦！");
		}else{
			API.consoleOutput("不存在啦!");
			DataBase.createDataBase("test.db","resources\\shadowscity\\db\\");
			var con=DataBase.connectOpenToDataBase("test.db","resources\\shadowscity\\db\\");
			
		}*/
	} 
	
	public   DataBaseSdon(string tableName,int rID)
	{//带参构造函数,用于表明该对象为结果集中的哪一个ID
		tablename=tableName;
		rId=rID;
	}
	
	public string GetKeyName(string sdon)
	{//解析sdon并获取keyname
		var s1=sdon.IndexOf(':'); 
		return sdon.Substring(0,s1);
	}
	
	public string GetValues(string sdon)
	{//解析sdon并获取values
		var s1=sdon.IndexOf(':'); 
		return sdon.Substring(s1+1);		
	}
	
	public void Add(string to,string value_)
	{//为该对象添加一个key:name,自动增加索引
		Sdon.Add(to+":"+value_); 
	} 
	
	public string Get(string keyname)
	{//搜索key为keyname的values并用字符串返回内容
		var str="";
		foreach(var p in Sdon)
		{
			if(GetKeyName(p)==keyname)
			{
				str=GetValues(p);
			}
		}
		return str;
	}
	

}