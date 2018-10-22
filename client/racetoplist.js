
var toplist=null;
var open=null;
var show=false;
var size=API.getScreenResolutionMantainRatio();
var race_name=""

API.onResourceStart.connect(function(){
	//dxEffect("particle",size.Width*0.5,size.Height*0.5,5,5,20*1000,{'moveX':0,'moveY':1,'moveW':0,'moveH':0,'endshadow':false},"0:255:0").Enabled();
	//dxEffect("number",size.Width*0.5,size.Height*0.5,0,0,-1,{'shownumber':1,'showindex':100,'showstyle':0,'showfont':6,'showfonts':0.3,'showalpha':150},"0:255:0").Enabled();
	 
	//dxTopInfo(size.Width*0.5,size.Height*0.5,"1","testtesttesttesttest","86412345","86412345","196747873","~y~S(567)","3","1906/2/14","255:0:255").Enabled();
})

 API.onServerEventTrigger.connect(function (eventName, args) {
	 

	if(eventName=="SC_race_houseinfo_toplist")
	{
		if(args[0]==API.getEntitySyncedData(API.getLocalPlayer(),"SC_race_house_road"))
		{// 赛道名和记录赛道名相等
			
				
					race_name=args[0]
					toplist=JSON.parse(args[1]);					
			
			open=true
		}
	}
	 
	if(eventName=="SC_race_house_quitrace"){
		alivetime=0.5*1000
		open=false;
	}
});

var dx_h=0;
var dx_w=700;
var dx_mid=dx_w*0.5
var x=Math.round(size.Width*0.5)-dx_mid
var y=0
var dx=[]
var step=0;
var alivetime=-1
var dx_tagh=800
API.onUpdate.connect(function(){ 
	var nowtime=Math.round(new Date().getTime());
	if(open==true)
	{
		show=true; 
			if(dx_h<dx_tagh)
			{ 
				if(dx_h==0)
				{
					var tmp=resource.dx_effect.dxEffect("text",x,y,dx_mid,dx_h-10,0,0,-1,{'showtext':race_name,'showindex':0,'showstyle':1,'showfont':6,'showfonts':1.5,'showalpha':255},"0:0:0").Enabled();			 		
					dx.push(tmp);
				}
				if(dx_h==100)
				{
					//var tmp=resource.dx_effect.dxEffect("text",x,y,dx_mid,dx_h,0,0,-1,{'showtext':"Race Track Record",'showindex':0,'showstyle':1,'showfont':6,'showfonts':1,'showalpha':255},"0:0:0").Enabled();	
					//dx.push(tmp)
				}
				dx_h=dx_h+20
			}else{
				if(dx_h>dx_tagh)
				{
					dx_h=dx_h-10
				}
				if(step==0)
				{
					step=1
				}
		 
		}
	}
	if(open==false)
	{
		if(dx_h>=0)
		{
			step=8
			dx_h=dx_h-40
		}else{
			show=false;
			dx_tagh=800
			step=0
			open=null;
		}
	}
	if(show==true)
	{
		API.drawRectangle(x,y,dx_w,dx_h,255,255,255,150);
		if(step==1)
		{ 
			if(toplist==null||toplist.length==0)
			{
				alivetime=nowtime+3*1000;
				var tmp=resource.dx_effect.dxEffect("text",x,y,dx_mid,200,0,0,-1,{'showtext':"No Data    No Data    No Data\n	No Data		No Data\nNo Data	No Data		No Data\n\nNo Data	No Data\nNo Data    No Data    No Data",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.7,'showalpha':255},"0:0:0").Enabled();	
				dx.push(tmp)
				step=-1
			}else{
				step=2
 
			}
		}
		if(step==2)
		{
			var p=toplist[0]
			var tmp_x=x+dx_mid-210+120
			API.dxDrawTexture("res/img/portrait/"+p.playerportrait+".png",new Point(tmp_x,180),new Size(240,240),0 )
			var tmp=resource.dx_effect.dxEffect("text",tmp_x,y,200,220,0,0,-1,{'showtext':p.playername,'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.6,'showalpha':255},"255:255:0").Enabled();			 			
			dx.push(tmp)
			tmp=resource.dx_effect.dxEffect("text",x,y,dx_mid,100,0,0,-1,{'showtext':p.team,'showindex':0,'showstyle':1,'showfont':6,'showfonts':1,'showalpha':255},"255:255:0").Enabled();	
			dx.push(tmp)
			tmp=resource.dx_effect.dxEffect("text",tmp_x,y,200,190,0,0,-1,{'showtext':API.getVehicleDisplayName(parseInt(p.rcarname))+" "+p.rcarlevel,'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.5,'showalpha':255},"255:255:0").Enabled();			 		
			dx.push(tmp)
			tmp=resource.dx_effect.dxEffect("text",tmp_x,y,200,340,0,0,-1,{'showtext':p.rcreatetime,'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.5,'showalpha':255},"255:255:0").Enabled();			 		
			dx.push(tmp)
			tmp=resource.dx_effect.dxEffect("text",tmp_x,y,200,250,0,0,-1,{'showtext':(p.rtime/1000)+"s",'showindex':0,'showstyle':1,'showfont':6,'showfonts':1.2,'showalpha':255},"255:255:0").Enabled();			 		
			dx.push(tmp)
			step=3
			var onetime=p.rtime;
			var maxh=400
			for(var i=1;i<toplist.length;i++)
			{
				if(i<5)
				{
					p=toplist[i];
					tmp=resource.racetoplist_dx.dxTopInfo(x+dx_mid,350+(i*75),p.team,"#"+(i+1),p.playername,p.rtime,p.rtime-onetime,p.rcarname,p.rcarlevel,p.playerportrait,p.rcreatetime,(i==1)?"0:128:255":((i==2)?"0:255:64":"192:192:192")).Enabled()
					tmp.Move(30,x+25,350+(i*75))
					dx.push(tmp)
					maxh=maxh+80
				}else{break}
			}
			var player=API.getPlayerName(API.getLocalPlayer())
			var how=findtophow(player) 
			if(how!=-1&&how>=5)
			{ 
				p=toplist[how]
				tmp=resource.racetoplist_dx.dxTopInfo(x+dx_mid,350+(i*75),p.team,"#"+(how+1),p.playername,p.rtime,p.rtime-onetime,p.rcarname,p.rcarlevel,p.playerportrait,p.rcreatetime,"255:128:0").Enabled()
				tmp.Move(30,x+25,350+(5*75))
				dx.push(tmp)		
				maxh=maxh+80
			}
			dx_tagh=maxh
			alivetime=nowtime+7*1000
		}
		if(step==3)
		{
			var p=toplist[0]
			API.dxDrawTexture("res/img/portrait/"+p.playerportrait+".png",new Point(x+dx_mid-210-120,180),new Size(240,240),0 )

		} 
	}
	if(alivetime!=-1)
	{
		if(nowtime>alivetime)
		{
			alivetime=-1
			open=false;
			for(var i in dx)
			{
				dx[i].Delete();
			}
			dx=[];
		}
	}
})

function findtophow(name)
{
	var tmp=-1 
	for(var i in toplist)
	{
		if(toplist[i].playername==name)
		{
			tmp=i;
			break;
		}
	}
	return parseInt(tmp);
}

 function copy(list)
 {
	var obj=JSON.parse(JSON.stringify(list));
	return obj;
 }
 