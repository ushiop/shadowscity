API.onServerEventTrigger.connect(function (eventName, args) 
{
	if(eventName == "house_enter")
	{
		var player = API.getLocalPlayer();
		var house_pos = JSON.parse(args[0]);
		var value = -1;
		for(var i in house_pos)
		{
			if(API.isInRangeOf(API.getEntityPosition(player), new Vector3(house_pos[i]["h_oX"], house_pos[i]["h_oY"], house_pos[i]["h_oZ"]), 1.0) == true)
			{
				value = i;
				break;
			}
		}
		if(value != -1)
		{
			API.setEntityPosition(player, new Vector3(house_pos[i]["h_iX"],house_pos[i]["h_iY"],house_pos[i]["h_iZ"]));
		}
	}
	if(eventName == "house_exit")
	{
		var player = API.getLocalPlayer();
		var house_pos = JSON.parse(args[0]);
		var value = -1;
		for(var i in house_pos)
		{
			if(API.isInRangeOf(API.getEntityPosition(player), new Vector3(house_pos[i]["h_iX"], house_pos[i]["h_iY"], house_pos[i]["h_iZ"]), 1.0) == true)
			{
				value = i;
				break;
			}
		}
		if(value != -1)
		{
			API.setEntityPosition(player, new Vector3(house_pos[i]["h_oX"],house_pos[i]["h_oY"],house_pos[i]["h_oZ"]));
		}
	}
});