
var dx=[]
var size=API.getScreenResolutionMantainRatio();

API.onResourceStart.connect(function(){
	//dxEffect("particle",size.Width*0.5,size.Height*0.5,5,5,20*1000,{'moveX':0,'moveY':1,'moveW':0,'moveH':0,'endshadow':false},"0:255:0").Enabled();
	//dxEffect("number",size.Width*0.5,size.Height*0.5,0,0,-1,{'shownumber':1,'showindex':100,'showstyle':0,'showfont':6,'showfonts':0.3,'showalpha':150},"0:255:0").Enabled();
	 
	//var tmp=dxTopInfo(size.Width*0.3,size.Height*0.2,"啊啊啊啊啊啊啊啊啊啊","#88888","testtesttesttesttest","86412345","86412345","196747873","~y~S(567)","3","1906/2/14","255:0:255").Enabled();
	/*tmp.OpenEd=function(){
		this.Move(200,size.Width*0.3,size.Height*0.3)
	}*/
})

 
 
  
function dxTopInfo(top_x,top_y,top_teamname,top_index,top_name,top_time,top_subtime,top_carname,top_carlv,top_ptx,top_date,top_rgb)
{
	/*
		宽度固定为650px,不可修改。
		index TOP几
		name 玩家名
		time 时间
		carname 车名
		carlv 车评级 ~c~LV(评分) 格式
		ptx 玩家头像ID
		date 记录创建日期
		rgb 记录榜颜色 格式 RR:GG:BB
	*/
	//记录榜效果

	var _dx=new Object();
	_dx.index=top_index;
	_dx.team=top_teamname;
	_dx.name=top_name;
	_dx.time=top_time/1000; 
	_dx.subtime=top_subtime/1000;
	_dx.carname=top_carname;
	_dx.ptx=top_ptx;
	_dx.date=top_date;

	var tmp;
	tmp=top_rgb.split(":");
	_dx.r=parseInt(tmp[0]);
	_dx.g=parseInt(tmp[1]);
	_dx.b=parseInt(tmp[2]);

	tmp=top_carlv.split("(");
	_dx.carlv=tmp[1].substr(0,tmp[1].length-1);
	_dx.carrank=tmp[0];
	 
	_dx.open=false;
	_dx.show=false;
	_dx.step=0;
	_dx.acc=0.0;
				
	_dx.x=Math.round(top_x)
	 
	_dx.y=Math.round(top_y) 
	_dx.eff=[];
	_dx.alive=-1;
	
	_dx.movestate=false;
	_dx.tagindex=0;
	_dx.tagx=0.0;
	_dx.tagy=0.0;

	_dx.Enabled=function(){
		this.open=true;
		this.show=!this.show;
		return this;
	}
	
	_dx.Move=function(targetindex,targetX,targetY){
		this.tagindex=targetindex
		this.tagx=(targetX-this.x)/targetindex
		this.tagy=(targetY-this.y)/targetindex
		this.movestate=true;
	}
	
	_dx.MoveEd=function(){};
	_dx.OpenEd=function(){};
	
	_dx.Delete=function(ac){
			if(ac!=null)
			{
				var index=-1
				for(var i=0;i<dx.length;i++)
				{
					if(dx[i]==this)
					{
						index=i;
					}
				}
				dx.splice(index,1);
			}else{
				this.open=false;
			}
	}
	
	_dx.clear=function(){
		var ef=this.eff;
		for(var i in ef)
		{
			ef[i].Delete();
		} 
	}
	
	dx.push(_dx);
	return _dx;
}
 
API.onUpdate.connect(function(){ 
	var nowtime=Math.round(new Date().getTime()); 
	for(var i in dx)
	{
		var p=dx[i];
		if(p.show==true)
		{ 
			if(p.movestate==true)
			{
				if(p.tagindex>0)
				{
					p.tagindex=p.tagindex-1
					p.x=Math.round(numAdd(p.x,p.tagx))
					p.y=Math.round(numAdd(p.y,p.tagy))
					for(var i in  p.eff)
					{
						p.eff[i].x=p.x
						p.eff[i].y=p.y
					}
				}else{
					p.MoveEd();
					p.movestate=false;
				}
			}
			if(p.open==true)
			{				
				if(p.step==0)
				{
					API.drawRectangle(p.x,p.y,4,50*p.acc,p.r,p.g,p.b,255);
					if(p.acc<1){p.acc=numAdd(p.acc,0.05)}else{p.step=1,p.acc=0.0}
				}
				if(p.step==1)
				{
					API.drawRectangle(p.x,p.y,650*p.acc,50,p.r,p.g,p.b,255);
					if(p.acc<1){p.acc=numAdd(p.acc,0.1)}else{p.step=2,p.acc=0.0}
				}
				if(p.step==2)
				{
					API.drawRectangle(p.x,p.y,650*p.acc,25,0,0,0,100);
					API.drawRectangle(p.x+650*p.acc,p.y,650*(1-p.acc),25,p.r,p.g,p.b,255); 
					//resource.dx_effect.dxEffect("particle",p.x,p.y,650*p.acc,0,5,5,0.3*1000,{'moveX':30,'moveY':0,'moveW':0,'moveH':0,'endshadow':false},p.r+":"+p.g+":"+p.b).Enabled();
					//resource.dx_effect.dxEffect("particle",p.x,p.y,650*p.acc,12,5,5,0.5*1000,{'moveX':30,'moveY':0,'moveW':0,'moveH':0,'endshadow':false},p.r+":"+p.g+":"+p.b).Enabled();
					//resource.dx_effect.dxEffect("particle",p.x,p.y,650*p.acc,20,5,5,0.5*1000,{'moveX':30,'moveY':0,'moveW':0,'moveH':0,'endshadow':false},p.r+":"+p.g+":"+p.b).Enabled();
					//resource.dx_effect.dxEffect("particle",p.x,p.y,650*p.acc,25,5,5,0.3*1000,{'moveX':30,'moveY':0,'moveW':0,'moveH':0,'endshadow':false},p.r+":"+p.g+":"+p.b).Enabled();

					API.drawRectangle(p.x,p.y+25,650,25*p.acc,0,0,0,100);
					API.drawRectangle(p.x,p.y+25+(25*p.acc),650,23*(1-p.acc)+2,p.r,p.g,p.b,255);
					if(p.acc<1){p.acc=numAdd(p.acc,0.1)}else{p.step=3,p.acc=0.0}
				}
				if(p.step==3)
				{
					p.step=4
					API.drawRectangle(p.x,p.y,650,48,0,0,0,100);
					API.drawRectangle(p.x,p.y+48,650,2,p.r,p.g,p.b,255);
					
					//top index
					API.drawText(""+p.index,p.x+370,p.y, 0.7,p.r,p.g,p.b,255, 6,1, false, true, 0);
					
					//top tx
					//API.sendChatMessage(p.x+"");
					API.dxDrawTexture("res/img/portrait/"+p.ptx+".png",new Point(p.x,p.y-20),new Size(80,80),0 )
					
					//top name & carlv													
					var tmp=resource.dx_effect.dxEffect("text",p.x,p.y,90,50-31,0,0,-1,{'showtext':p.name,'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.4,'showalpha':214},p.r+":"+p.g+":"+p.b).Enabled();			 			
 
					p.eff.push(tmp)
					tmp=resource.dx_effect.dxEffect("text",p.x,p.y,110,5,0,0,-1,{'showtext':API.getVehicleDisplayName(parseInt(p.carname)),'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.3,'showalpha':214},p.r+":"+p.g+":"+p.b).Enabled();			 		
					p.eff.push(tmp)
					tmp=resource.dx_effect.dxEffect("number",p.x,p.y,86,-6,0,0,-1,{'shownumber':p.carlv,'showindex':50,'showstyle':0,'showfont':6,'showfonts':0.25,'showalpha':255},"255:255:255").Enabled();			 		
					p.eff.push(tmp)
					tmp=resource.dx_effect.dxEffect("text",p.x,p.y,650,25,0,0,-1,{'showtext':p.date,'showindex':0,'showstyle':2,'showfont':6,'showfonts':0.3,'showalpha':150},"255:255:255").Enabled();			 		
					p.eff.push(tmp); 
					tmp=resource.dx_effect.dxEffect("number",p.x,p.y,650-12,0,0,0,-1,{'shownumber':p.time,'showindex':50,'showstyle':2,'showfont':6,'showfonts':0.4,'showalpha':255},p.r+":"+p.g+":"+p.b).Enabled();			 		
					p.eff.push(tmp);
					if(p.subtime!=0)
					{
						tmp=resource.dx_effect.dxEffect("text",p.x,p.y,650-200,25,0,0,-1,{'showtext':"+"+p.subtime+"s",'showindex':0,'showstyle':0,'showfont':6,'showfonts':0.3,'showalpha':255},"255:0:0").Enabled();			 		
						p.eff.push(tmp); 
					}
					tmp=resource.dx_effect.dxEffect("text",p.x,p.y,370,-10,0,0,-1,{'showtext':p.team+"",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.3,'showalpha':255},p.r+":"+p.g+":"+p.b).Enabled();			 		
					p.eff.push(tmp); 
					p.OpenEd();
					p.alive=nowtime+6*1000;
					p.acc=0.0;
				}
				if(p.step==4)
				{
					API.drawRectangle(p.x,p.y,650,48,0,0,0,100);
					API.drawRectangle(p.x,p.y+48,650,2,p.r,p.g,p.b,255);
					
					//top index
					API.drawText(""+p.index,p.x+370,p.y,0.7,p.r,p.g,p.b,214, 6,1, false, true, 0);
					
					//top tx 
					API.dxDrawTexture("res/img/portrait/"+p.ptx+".png",new Point(p.x,p.y-20),new Size(80,80),0 )
					
					//car rank lv
					API.drawText(p.carrank,p.x+90,p.y+6,0.3,p.r,p.g,p.b,214, 6,0, false, true, 0);
					
					//time s
					API.drawText("s",p.x+650,p.y,0.4,p.r,p.g,p.b,255, 6,2, false, true, 0);
				}
			}
			if(p.open==false)
			{
				if(p.step==4)
				{
					API.drawRectangle(p.x,p.y,650,48,0,0,0,100);
					API.drawRectangle(p.x,p.y+48,650,2,p.r,p.g,p.b,255);
					p.clear();
					p.step=5
				}
				if(p.step==5)
				{
					API.drawRectangle(p.x,p.y,650,48,0,0,0,100);
					API.drawRectangle(p.x,p.y+(50*(1-p.acc)),650,2+(48*p.acc),p.r,p.g,p.b,255);				
					if(p.acc<1){p.acc=numAdd(p.acc,0.2)}else{p.step=6,p.acc=0.0}
				}
				if(p.step==6)
				{
					API.drawRectangle(p.x+(650*p.acc),p.y,650*(1-p.acc),50,p.r,p.g,p.b,255);	
					//resource.dx_effect.dxEffect("particle",p.x,p.y,650*p.acc,0,5,15,0.3*1000,{'moveX':60*(1-p.acc),'moveY':0,'moveW':0,'moveH':0,'endshadow':false},p.r+":"+p.g+":"+p.b).Enabled();
					//resource.dx_effect.dxEffect("particle",p.x,p.y,650*p.acc,25,5,15,0.3*1000,{'moveX':60*(1-p.acc),'moveY':0,'moveW':0,'moveH':0,'endshadow':false},p.r+":"+p.g+":"+p.b).Enabled();
				 			
					if(p.acc<1){p.acc=numAdd(p.acc,0.2)}else{p.step=7,p.acc=0.0}				
				}
				if(p.step==7)
				{
					p.Delete(1);
				}
			}
		}
		if(p.alive!=-1)
		{
			if(nowtime>p.alive)
			{
				p.open=false;
			}
		}
	} 
 });
 
 
//浮点数相加误差修正
 function numAdd(num1, num2) {
    var baseNum, baseNum1, baseNum2;
    try {
        baseNum1 = num1.toString().split(".")[1].length;
    } catch (e) {
        baseNum1 = 0;
    }
    try {
        baseNum2 = num2.toString().split(".")[1].length;
    } catch (e) {
        baseNum2 = 0;
    }
    baseNum = Math.pow(10, Math.max(baseNum1, baseNum2));
    return (num1 * baseNum + num2 * baseNum) / baseNum;
};