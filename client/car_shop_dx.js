 
var dx=[]
var size=API.getScreenResolutionMantainRatio();

 API.onResourceStart.connect(function(){
	 //new Vector3(-716.3962,-162.2415,36.19393),new Vector3(-0.02342723,0.355593,29.58234)//cam
//new Vector3(-719.264,-158.2872,36.19803),new Vector3(-0.1573869,-0.3500315,-149.8079)//ped
	//blip=API.createBlip(new Vector3(297.213,-583.9615,42.76713));
	//API.setBlipSprite(blip,51);
	//mark=API.createMarker(1,new Vector3(297.213,-583.9615,38.76713),new Vector3(),new Vector3(),new Vector3(5,5,10),0,64,128,32); 
	//lab= API.createTextLabel("按~g~G~w~打開變性菜單\n請不要將車開進來\n否則你會很尷尬...",new Vector3(297.213,-583.9615,44.76713),100,1,false); 
	//dxShopInfo(size.Width*0.5,size.Height*0.5,"測試一下測試一下測試一下",999999,"這就是個測試","ceshiceshi")
});
 
function dxShopInfo(s_x,s_y,shop_name,shop_money,shop_msg,shop_exmsg="",_itemtype,_itemaddtype)
{
	/*
		商店物品框
		_name 物品名
		_money 價格
		_msg 詳細描述
		
		固定大小300*57
	*/
	var _dx=new Object();
	_dx.x=s_x
	_dx.y=s_y
	_dx.name=shop_name;
	_dx.money=shop_money;
	_dx.msg=shop_msg;
	_dx.exmsg=shop_exmsg
	_dx.itemtype=_itemtype;
	_dx.itemaddtype=_itemaddtype;
	
	_dx.enabled=false;
	_dx.r=255
	_dx.g=255
	_dx.b=255
	
	_dx.Delete=function(ac){
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
	
	_dx.Switch=function()
	{
		if(this.enabled==false)
		{
			this.enabled=true;
			this.r=0
			this.g=255
			this.b=0
		}else{
			this.enabled=false;
			this.r=255
			this.g=255
			this.b=255
		}
	}
	
	
	dx.push(_dx);
	return _dx;
	
}


API.onUpdate.connect(function(){  
	for(var i in dx)
	{
		var p=dx[i];
		
		API.drawRectangle(p.x,p.y,300,57,0,0,0,255);
					
		API.drawText(p.name,p.x+2,p.y+4, 0.3,255,255,255,255, 6,0, false, true, 0);
		
		
		API.drawRectangle(p.x,p.y+30,300,2,p.r,p.g,p.b,255);
		
		API.drawText("~y~$"+p.money,p.x+300,p.y+32, 0.3,255,255,255,255, 6,2, false, true, 0);
		
		API.drawText(p.exmsg,p.x,p.y+32, 0.3,255,255,255,255, 6,0, false, true, 0);
	}
})