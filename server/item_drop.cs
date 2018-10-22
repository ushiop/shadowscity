using System;
using GTANetworkServer;
using GTANetworkShared;

 

public class item_drop : Script
{ 	

	public item_drop()
	{
		//API.consoleOutput(colorRGBtoHx16(222,125,235));
	}

	public  void itemDrop(Client Player,string iType=null,string iAddtype=null)
	{//为Player随机生成一份物品掉落
		var random=new Random();
		var itemType="";
		if(iType==null)
		{
			string[] itemTypes=new string[]{"車輛強化插件","車輛裝飾插件","車輛碎片"};
			itemType=itemTypes[random.Next(itemTypes.Length)];
		}else{
			itemType=iType;
		}
		var p=API.getEntityData(Player,"SC_item");
		var pzs=random.Next(100);
		var pz="";
		int dxr=255;
		int dxg=255;
		int dxb=255;
		if(pzs<60){ pz="~c~普通";dxr=255;dxg=255;dxb=255;}
		if(pzs>=60&&pzs<90){pz="~g~良好";dxr=0;dxg=255;dxb=0;}
		if(pzs>=90&&pzs<97){pz="~b~稀有";dxr=0;dxg=150;dxb=255;}
		if(pzs>=97){pz="~y~傳說";dxr=255;dxg=255;dxb=0;} 
		if(itemType=="車輛強化插件"){
			string[] veh_addtypes=new string[]{"極速"};
			string[] veh_addmsgs=new string[]{"車輛極速+"};
			var veh_addtype=veh_addtypes[random.Next(veh_addtypes.Length)];
			var veh_addmsg=veh_addmsgs[random.Next(veh_addmsgs.Length)];
			var values=""; 

			if(pz=="~c~普通"){ 
				values=Math.Round(random.NextDouble() * (0.5 - 0.1) + 0.1,2).ToString();
			}
			if(pz=="~g~良好"){
				values=Math.Round(random.NextDouble() * (1 - 0.5) + 0.5,2).ToString();
			}
			if(pz=="~b~稀有"){
				values=Math.Round(random.NextDouble() * (2.5 - 1) + 1,2).ToString();
			}
			if(pz=="~y~傳說"){
				values=Math.Round(random.NextDouble() * (5 - 2.5) + 2.5,2).ToString();
			} 
			veh_addmsg=veh_addmsg+values+"%";
			
			p.addItem(itemType+":"+veh_addtype,pz,veh_addmsg,itemType+":"+veh_addtype,values);
			p.showItemDx("獲得物品",pz,"~r~"+itemType+":"+veh_addtype+"\n\n~r~"+veh_addmsg+"\n\n~w~按F1,選擇物品列表進行使用吧!",dxr,dxg,dxb);
		}
		if(itemType=="車輛裝飾插件"){
			string[] veh_addtypes=new string[]{"霓虹燈","霓虹燈顏色"};
			string[] veh_addmsgs=new string[]{"開啟霓虹燈","將霓虹燈顏色設為(紅:藍:綠)\n"};
			var index=random.Next(veh_addtypes.Length);
			var veh_addtype=veh_addtypes[index];
			var veh_addmsg=veh_addmsgs[index];
			var values=""; 
			if(veh_addtype=="霓虹燈")
			{
				if(pz=="~c~普通"){ 
					values="0";
					veh_addmsg="開啟左邊霓虹燈";
				}
				if(pz=="~g~良好"){
					values="0:1";
					veh_addmsg="開啟左邊和右邊霓虹燈";
				}
				if(pz=="~b~稀有"){
					values="0:1:2";
					veh_addmsg="開啟左邊、右邊、前方霓虹燈";
				}
				if(pz=="~y~傳說"){
					values="0:1:2:3";
					veh_addmsg="開啟全部霓虹燈";
				}
				p.showItemDx("獲得物品",pz,"~r~"+itemType+":"+veh_addtype+"\n\n~r~"+veh_addmsg+"\n\n~w~按F1,選擇物品列表進行使用吧!",dxr,dxg,dxb);	
				
			}
			if(veh_addtype=="霓虹燈顏色")
			{
				var tips="將霓虹燈顏色設為\n周圍邊框的顏色\n";
				if(pz=="~c~普通"){  
					values="60:60:60"; 
					dxr=60;dxg=60;dxb=60;
					veh_addmsg="顏色(RGB) ";
				}
				if(pz=="~g~良好"){
					var r=random.Next(255); 
					values=r.ToString()+":0:0"; 
					dxr=r;dxg=0;dxb=0;
					veh_addmsg="顏色(RGB) ";
				}
				if(pz=="~b~稀有"){
					var r=random.Next(255);
					var g=random.Next(255); 
					values=r.ToString()+":"+g.ToString()+":0"; 
					dxr=r;dxg=g;dxb=0;
					veh_addmsg="顏色(RGB) ";
				}
				if(pz=="~y~傳說"){
					var r=random.Next(255);
					var g=random.Next(255); 
					var b=random.Next(255); 
					values=r.ToString()+":"+g.ToString()+":"+b.ToString();
					dxr=r;dxg=g;dxb=b;					
					veh_addmsg="顏色(RGB) ";
				}	
				p.showItemDx("獲得物品",pz,"~r~"+itemType+":"+veh_addtype+"\n\n~r~"+tips+veh_addmsg+"\n"+values+"\n\n~w~按F1,選擇物品列表進行使用吧!",dxr,dxg,dxb);	
				veh_addmsg=veh_addmsg+values;
			}
		
			p.addItem(itemType+":"+veh_addtype,pz,veh_addmsg,itemType+":"+veh_addtype,values);
		}
		if(itemType=="車輛碎片")
		{

			string[] veh_addtypes=new string[]{"FMJ","Prototipo","Adder","Bullet","EntityXF","LE7B","Nero","Nero2","Osiris","Pfister811","Sheava","SultanRS","T20","Tempesta","Tyrus","Italigtb","Italigtb2","Cheetah","Infernus","Penetrator","Reaper","Superd","Turismor","Vacca","Voltic","Zentorno","Voltic2"};
			string[] veh_addmsgs=new string[]{"FMJ","X80","靈蛇","子彈","本質","RE7B","尼羅","尼羅升級版","奧西里斯","811","皇霸天","王者RS","T20","泰皮斯達","泰勒斯","伊塔裏GTB","伊塔裏GTB升級版","獵豹","煉獄膜","摧花辣手","死神","金鑽耀星","披治 R","狂牛","狂雷","桑托勞","火箭狂雷"};
			var index=random.Next(veh_addtypes.Length);
			var veh_addtype="";
			var veh_addmsg="";
			if(iAddtype==null||iAddtype=="")
			{
				veh_addtype=veh_addtypes[index];
				 veh_addmsg=veh_addmsgs[index];
			}else{
				 veh_addtype=iAddtype;
				 veh_addmsg="";
				for(var i=0;i<veh_addtypes.Length;i++)
				{
					if(veh_addtypes[i]==veh_addtype)
					{
						index=i;
						veh_addmsg=veh_addmsgs[i];
						break;
					}
				}
			}
			var values=""; 
			veh_addmsg="集齊碎片50個來兌換\n~g~"+veh_addmsgs[index];
			p.showItemDx("獲得物品","~p~車輛碎片","~r~"+itemType+":"+veh_addmsgs[index]+"\n\n~r~"+veh_addmsg+"\n\n~w~按F1,選擇物品列表進行使用吧!",255,0,255);	
			veh_addmsg="集齊碎片來兌換~g~"+veh_addmsgs[index]+"~w~(50/1)";
			values=veh_addtype+":50:1";
			p.addItem(veh_addmsgs[index],"碎片",veh_addmsg,itemType+":"+veh_addtype,values);		
				
				
		}
		p=null;
		random=null; 		
	}
	

	
	public static string colorRGBtoHx16(int R, int G, int B)
       {
            return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(R, G, B));
       }
	
 
	  
}