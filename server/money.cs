using System;

using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class money : Script
{ 	
	private Client player;
	private int Money=0;
	private login lg;
	
	public money()
	{
	}

	public money(Client Player)
	{ 
		player=Player;
		lg=new login();
		loadMoney();
    }
	
	public int getMoney()
	{
		return Money;
	}
 
	public void addMoney(int v)
	{//增加金钱,负数为减少
		API.consoleOutput(API.hasEntityData(player,"SC_USERINFO").ToString());
		var us=API.getEntityData(player,"SC_USERINFO");
		if(v>0)
		{//获得金钱,统计数据增加
			us.setUserInfo("MAXMONEY",us.getUserInfo("MAXMONEY").infoValue+v);
		}else
		{//消耗金钱,统计数据增加
			us.setUserInfo("USEMONEY",us.getUserInfo("USEMONEY").infoValue+v); 
			
		}
		Money=Money+v;
		API.setEntitySyncedData(player,"SC_money",Money);
		
		API.triggerClientEvent(player, "SC_money_updata",Money,v);
		pushMoney();
	}
	
	public void loadMoney()
	{
		if(lg.GetPlayerAccess(player,"SC_money")!="SC_NULL")//读取玩家在数据库内的改装串,不为空则记录,等待dy的读取函数
		{
				var js=lg.GetPlayerAccess(player,"SC_money");
				Money=Convert.ToInt32(js);
				API.setEntitySyncedData(player,"SC_money",js);
		}else{
			Money=0;
			API.setEntitySyncedData(player,"SC_money",Money);
		}
	}
	
	public void pushMoney()
	{//将金钱保存至数据库 
		lg.SetPlayerAccess(player,"SC_money",Money.ToString());
	}
 
}
 