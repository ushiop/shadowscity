var js=null;
 var size=API.getScreenResolutionMantainRatio(); 
 var show=false;
 var back=false;

 API.onResourceStart.connect(function(){
	
});

 
 

 
var dx_w=600,dx_h=900
var x=(size.Width*0.5)-(dx_w/2)
var y=(size.Height*0.5)-(dx_h/2)
var mid_x=x+(dx_w/2)
var select=0,g_show=0
API.onUpdate.connect(function(){  
	if(show==true)
	{
		API.drawRectangle(x,y,dx_w,dx_h,0,0,0,255);
		
		// 名字
		API.drawText("車隊排行榜",mid_x,y-10,1,255,255,255,255, 6,1, false, true, 0);
		API.drawText("G鍵關閉",x,y-10,0.4,255,255,255,255, 6,0, false, true, 0);
		API.drawText("上下鍵瀏覽",x+dx_w,y-10,0.4,255,255,255,255, 6,2, false, true, 0);
		if(js==null)
		{
			API.drawText("正在等待車隊數據...\n車隊榜每60秒刷新一次",mid_x,y+60,0.5,255,255,255,255, 6,1, false, true, 0);
		}else{
		
			//動態
			showid=0
			 
			for(var i=g_show;i<js.length;i++)
			{
				if(showid<11)
				{
					var p=js[i];
					if(select==showid)
					{
						API.drawRectangle(x+5,y+70+(showid*75),590,70,255,255,255,214);
					}else{
						
						API.drawRectangle(x+5,y+70+(showid*75),590,70,255,255,255,120);
					}
					API.drawText(p.name,x+5,y+70+(showid*75),0.5,0,128,255,255, 6,0, false, true, 0);
					API.drawText(p.boss+"%\n賽事統治度",mid_x,y+65+(showid*75),0.5,0,128,255,255, 6,1, false, true, 0);
					API.drawText(p.rank+"\n車隊分數",x+585,y+65+(showid*75),0.5,0,128,255,255, 6,2, false, true, 0);
					showid++
				}
			}  
		}
		
	
		

	 
	}
})
 
 API.onKeyDown.connect(function(sender, keyEventArgs) {
	 if(show==true)
	 {
		if(keyEventArgs.KeyCode == Keys.Up){
				select--;
				if(select<0)
				{
					select=0
					if(g_show>0)
					{
						g_show--
					}
				}
			}
		if(keyEventArgs.KeyCode == Keys.Down){
				select++;
				if(select>10)
				{
					select=10
					if(g_show+10<js.length-1)
					{
						g_show++
					}
				}
				if(select>=js.length){				
							select=js.length-1
						}
			}	
		if(keyEventArgs.KeyCode == Keys.G){
	  
			show=false;
			if(back==true)
			{
				back=false;
				resource.team.createGui();
			}
		}
	 }
 })
 
 
 API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_TEAM_LIST"){
		//玩家被踢出
		 js=JSON.parse(args[0]); 
		js=sort(js);
	}

})


function sort(arr)
{  
	var f=[]
	for(var i in arr)
	{
		f.push(arr[i])
	}
	for(var i in f)
	{
		for(var x=i;x<f.length;x++)
		{
			if(parseInt(f[i].boss)<parseInt(f[x].boss))
			{
				
				var tmp=f[i];
				f[i]=f[x]
				f[x]=tmp;
			}
		}
	}
	return f;
}

 
 