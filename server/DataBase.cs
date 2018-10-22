using System;
using System.IO;
using System.Data.SQLite;
using GTANetworkServer;
using GTANetworkShared;  

 
/*
SQlite.dll下载地址:
http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki
版本选择:Precompiled Statically-Linked Binaries for 64-bit Windows (.NET Framework 4.0)
		 sqlite-netFx40-static-binary-bundle-x64-2010-1.0.104.0.zip(2.66 MiB) 		
*/

public class DataBase : Script
{ 	
	
	private string path="";
	private SQLiteConnection con;
	private SQLiteCommand cmd;
	
	public DataBase()
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
	
	public DataBase(string dbFileName,string filePath)
	{//带参数构造,dbFileName为db文件名,filePath为db文件路径
		path=filePath+dbFileName;
	}
	
	public string Path()
	{//返回数据库文件路径
		return path;
	}
	
	public bool isDataBase()
	{//检测数据库文件是否存在
		return File.Exists(path);
	}
	
	public void   createDataBase()
	{//创建数据库文件
		SQLiteConnection.CreateFile(path);
	}
	
	public void connectOpenToDataBase()
	{//连接到一个数据库并打开他
		con=new SQLiteConnection("Data Source="+path+";Version=3");
		con.Open();
	}
	
	public void closeToDataBase()
	{//关闭一个数据库连接
		con.Close();
	}
	
	public void sqlCommand(string sql)
	{//执行一个sql命令,无返回
		var s=new SQLiteCommand(sql,con);
		s.ExecuteNonQuery();
	}
	
	public DataBaseSdon[] sqlCommandReturn(string tablename ,string wheres,params string[] rowsName)
	{//执行一个sql查询命令,并以sdon字符串返回结果
	//参数rowsName为要查询的列名称
	//tablename为查询的表名
	//where为条件语句,需要手写
	//有一个bug,不要执行查询不存在的列的sql语句否则会报错
		var rowsN="";
		for(var i=0;i<rowsName.Length;i++)
		{ 
			rowsN=rowsN+rowsName[i]+",";
		}
		rowsN=rowsN.Substring(0,rowsN.Length-1);
		var wherefor=""; 
		if(wheres.Length!=0){wherefor="WHERE "+wheres;}
		var sql="SELECT "+rowsN+" FROM " + tablename + " "+wherefor+";";		
		//API.consoleOutput(sql);
		var s=new SQLiteCommand(sql,con);
		var reader = s.ExecuteReader(); 
		var count=0; 
		if(reader.HasRows)
		{	
			while(reader.Read())
			{
				count++;
			}
		}
		reader.Close();
		reader = s.ExecuteReader(); 
		var m=0;
		DataBaseSdon[] p=new DataBaseSdon[count];
		if(reader.HasRows)
		{
			while(reader.Read())
			{
				p[m]=new DataBaseSdon(tablename,m);
				for(var i=0;i<rowsName.Length;i++)
				{
					p[m].Add(rowsName[i],reader[rowsName[i]].ToString());	
				}
				m++;
			}
		}
		reader.Close();
		return p;
	}
	

}