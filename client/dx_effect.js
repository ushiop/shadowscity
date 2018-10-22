
var size=API.getScreenResolutionMantainRatio();
var dx=[];
//size.Width=size.Width*1.2;
//size.Height=size.Height*1.2;
//.Width,.Height
//	var p=API.getScreenResolutionMantainRatio();
//	API.sendChatMessage(p.Width+"="+p.H);

API.onResourceStart.connect(function(){
	//dxEffect("particle",size.Width*0.5,size.Height*0.5,5,5,20*1000,{'moveX':0,'moveY':1,'moveW':0,'moveH':0,'endshadow':false},"0:255:0").Enabled();
	//var tmp=dxEffect("number",size.Width*0.5,size.Height*0.5,0,0,-1,{'shownumber':1,'showindex':100,'showstyle':0,'showfont':6,'showfonts':0.3,'showalpha':150},"0:255:0").Enabled();
	//API.sendChatMessage(tmp.type)
	//dxEffect("percentage",500,500,-150,20,100,100,-1,{'showacc':70,'showindex':0},"0:0:0").Enabled();	
			
})

function dxEffect(dxType,dxstartX,dxstartY,dxoffestX,dxoffestY,dxstartW,dxstartH,dxTime,dxSet,dxRGB)
{
	/*
		dxType - DX特效的类型,可选项
		       - shadow "残影类型，在指定XY创建一个WH大小的方块，并在DXTIME后消失，消失过程渐隐"
			   - particle "粒子类型，在制定XY创建一个WH大小的方块，并在DXTIME后消失，过程渐隐，额外参数表由dxSet传入
	                       粒子类型的dxSet结构{
							                    moveX -X轴的加减度
												moveY -Y轴的加减度
												moveW -W轴的加减度
												moveH -H轴的加减度
												endshadow - 是否留下一个残影dx(true/false)
																	}
				- text "文本类型,在指定XY逐渐显示字符,不会自动消失,必须调用DELETE来删除,参数由dxset指定，结构如下
						文本类型特效的dxset结构
												{
													showtext - 显示的文本内容
													showindex - 当前显示到第几个字符 
													showstyle - 显示风格(0 从左朝右边显示,1 从中间朝两边显示,2 从右边往左边显示)
													showfont - 字体ID
													showfonts - 字体大小
													showalpha - 字体透明度
												}
				- number "数字类型,同上,逐渐显示至目标数字,不会自动消失
						  数字类型的dxset结构{
												shownumber - 目标数字
												showindex - 显示速率，越大完整显示的时间就越久
												showstyle - 同text
												showfont - 同text
												showfonts - 同text
												showalpha - 同text
											}
				- percentage "圓形統計圖類型,不會自動消失,在指定XY位置創建一個圓形統計圖并逐漸填充至目標百分比(0-100%)
							 ,w/h表示圖片大小,XY始終會是圓形統計圖的圓心位置
							 dxset結構{
								         showacc - 目標百分比
										 showindex - 當前百分比
										}
				
				
		dxstartX,Y,W,H - 初始的XY与HW
		dxTime - DX显示时间，单位为毫秒
		dxSet - 特殊DX类型的额外参数,由一个自定义的结构对象传入
		dxRGB - DX的颜色，由字符串格式 RR:GG:BB 传入
                                                						   
	*/
	
	var _dx=new Object();
	_dx.type=dxType;
	_dx.x=dxstartX;
	_dx.ox=dxoffestX;
	_dx.oy=dxoffestY;
	_dx.y=dxstartY;
	_dx.w=dxstartW;
	_dx.h=dxstartH;
	_dx.time=Math.round(new Date().getTime())+dxTime;
	_dx.tagtime=dxTime
	_dx.set=dxSet;
	_dx.show=false;
	dx.push(_dx);
	
	var tmp;
	tmp=dxRGB.split(":");
	_dx.r=parseInt(tmp[0]);
	_dx.g=parseInt(tmp[1]);
	_dx.b=parseInt(tmp[2]);
	
	_dx.Enabled=function(){
		this.show=!this.show;
		return this;
	}
	
	_dx.AnimEd=function(){}
	
	_dx.Delete=function(){
			var index=-1
			for(var i=0;i<dx.length;i++)
			{
				if(dx[i]==this)
				{
					index=i;
				}
			}
			dx.splice(index,1);
	}
	return _dx;
}


API.onUpdate.connect(function(){ 
	var nowtime=Math.round(new Date().getTime());
	for(var i in dx)
	{
		var p=dx[i];
 
		if(nowtime<p.time||p.tagtime==-1)
		{
			if(p.show==true)
			{
				var alpha=parseInt(255*(((p.time-nowtime)/p.tagtime)))
				if(p.type=="shadow")
				{//残影类型
					API.drawRectangle(p.x+p.ox,p.y+p.oy,p.w,p.h,p.r,p.g,p.b,alpha);
				} 
				if(p.type=="particle")
				{
					if(p.set.endshadow==true)
					{
						dxEffect("shadow",p.x,p.y,p.ox,p.oy,p.w,p.h,1*1000,"",p.r+":"+p.g+":"+p.b).Enabled();
					}
					p.x=p.x+p.set.moveX
					p.y=p.y+p.set.moveY
					p.w=p.w+p.set.moveW
					p.h=p.h+p.set.moveH
					API.drawRectangle(p.x+p.ox,p.y+p.oy,p.w,p.h,p.r,p.g,p.b,alpha);
				}
				if(p.type=="text")
				{
					var str;
					if(p.set.showindex<p.set.showtext.length)
					{
						p.set.showindex++;
						str=p.set.showtext.substr(0,p.set.showindex);
					}else{
						str=p.set.showtext;
					}
					API.drawText(str,p.x+p.ox,p.y+p.oy,p.set.showfonts,p.r,p.g,p.b,p.set.showalpha,p.set.showfont,p.set.showstyle, false, true, 0);
				}
				if(p.type=="number")
				{
					var str;
					if(p.w<p.set.showindex)
					{
						p.w++; 
						str=numMulti(p.set.shownumber,numDiv(p.w,p.set.showindex));
					}else{
						str=p.set.shownumber;
					}
					API.drawText(str+"",p.x+p.ox,p.y+p.oy,p.set.showfonts,p.r,p.g,p.b,p.set.showalpha,p.set.showfont,p.set.showstyle, false, true, 0);
				}
				if(p.type=="percentage")
				{
					if(p.set.showindex<p.set.showacc)
					{
						p.set.showindex=p.set.showindex+1
					}else{
						p.set.showindex=p.set.showacc;
					} 
					API.dxDrawTexture("res/img/percentage/round/"+p.set.showindex+".png",new Point(p.x-(p.w/2),p.y-(p.h/2)),new Size(p.w,p.h),0 )
				}
			}
		}else{
			p.Delete();
		}
	} 
 });
 
 //乘法误差修正
 function numMulti(num1, num2) {
    var baseNum = 0;
    try {
        baseNum += num1.toString().split(".")[1].length;
    } catch (e) {
    }
    try {
        baseNum += num2.toString().split(".")[1].length;
    } catch (e) {
    }
    return Number(num1.toString().replace(".", "")) * Number(num2.toString().replace(".", "")) / Math.pow(10, baseNum);
 }
	
//除法误差修正
function numDiv(num1, num2) {
    var baseNum1 = 0, baseNum2 = 0;
    var baseNum3, baseNum4;
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
    with (Math) {
        baseNum3 = Number(num1.toString().replace(".", ""));
        baseNum4 = Number(num2.toString().replace(".", ""));
        return (baseNum3 / baseNum4) * pow(10, baseNum2 - baseNum1);
    }
};
 
//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
