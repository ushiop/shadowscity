using System;
using System.IO;  
using System.Text;
using GTANetworkServer;
using GTANetworkShared;
 
public class HTML 
{ 	
	
	private string path=""; 
	private string htmlstr="";
	private string old_htmlname="";
	
	public HTML()
	{   
		/*
			打开并替换HTML内容的类
			HTML模板中，需要替换的内容用
			$*$标识
			替换完后保存到指定的文件中
		*/
	} 
	
	public HTML(string _html_origin,string filePath)
	{//带参数构造,_html_origin为html模板文件名,filePath为db文件路径
		old_htmlname=_html_origin;
		path=filePath;
	}
	
	public string Path()
	{//返回数据库文件路径
		return path;
	}
	
	public string Html()
	{
		return htmlstr;
	}
	
	public bool isHtml()
	{//检测数据库文件是否存在
		return File.Exists(path+old_htmlname);
	}
	
	public void loadHtml()
	{//读取html文件
		if(isHtml()==true)
		{
			htmlstr=File.ReadAllText(path+old_htmlname);
		}
	}
	
	public void replace(string keyname,string msg)
	{//替换已打开的html文件中的keyname为msg
		if(htmlstr=="")
		{
			loadHtml();
		}	
		htmlstr=htmlstr.Replace(keyname,msg);
	}
	
	public void saveHtml(string newfilename)
	{//生成一个新的HTML文件并保存到同一个目录下
		if(newfilename==old_htmlname)
		{
			newfilename="_"+newfilename;
		}
		 
		File.WriteAllText(path+newfilename,htmlstr, Encoding.UTF8);
	}
	 

}