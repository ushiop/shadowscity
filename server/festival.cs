using System;

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class festival : Script
{ 	
 
	public festival()
	{

		API.onEntityDataChange+= OnEntityDataChange; 		
	}
	
	private void OnEntityDataChange(NetHandle entity, string key, object oldValue)
	{
		if(key=="SC_item_seted")
		{//玩家登录後物品讀取完畢時發放禮物
			var Player=API.getPlayerFromHandle(entity);
			var p=API.getEntityData(Player,"SC_item");
			var lg=new login();
			if(lg.GetPlayerAccess(Player,"SC_Festival_2017_3.8")=="SC_NULL"){
				
				lg.SetPlayerAccess(Player,"SC_Festival_2017_3.8","1");
				p.showItemDx("節日禮包","雖然并沒有女孩子玩這個服務器","獲得了物品\n金幣3888 1份\n~y~傳說 車輛極速+3.8% 1件\n~y~傳說 霓虹燈顏色(粉) 1件",255,0,128);
				API.getEntityData(Player,"SC_money").addMoney(3888);
				p.addItem("婦女節 極速卡","~y~傳說","車輛極速+3.8%","車輛強化插件:極速","3.8");
				p.addItem("婦女節 霓虹燈顏色","~y~傳說","開啟全部霓虹燈","車輛裝飾插件:霓虹燈顏色","255:128:255");
				
			}
			p=null;
		}
	}
	
	
}
 