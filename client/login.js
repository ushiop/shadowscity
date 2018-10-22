 
var show=false 
var size=API.getScreenResolutionMantainRatio();
var select=0; 
 
 
 API.onResourceStart.connect(function(){
	show=true
	API.setCanOpenChat(false)
	API.setChatVisible(false)
});
 
var index=1
var dx_w=1020,dx_h=350
var x=parseInt((size.Width*0.5)-(dx_w/2)),y=parseInt((size.Height*0.5)-(dx_h/2))
var ox=x+220 
var password=""
API.onUpdate.connect(function(){ 
   
	if(show==true)
	{
		if(index<66)
		{
			index=index+1
		}else{
			index=1
		}
  
		API.dxDrawTexture("res/img/login_logo/0001.png",new Point(x,y),new Size(220,dx_h),0) 
		var tmp_index= index<10?"0"+index:index; 
		API.dxDrawTexture("res/img/login_logo/00"+tmp_index+".png",new Point(x,y),new Size(220,dx_h),0) 
		API.drawRectangle(ox,y,800,dx_h,255,255,255,214);
		
		//welcome msg
		API.drawText("歡迎來到ShadowsCity\n這是一個賽車生涯式服務器",ox+400,y+25, 0.4,0,168,255,255, 6,1, false, true, 0);
		
		//API.drawText("現在,在下方輸入框輸入密碼,完成注冊或登陸吧",ox+200,y+50, 0.4,0,168,255,255, 6,1, false, true, 0);
		
		
		//input line
		API.drawRectangle(ox+50,y+160,700,4,0,128,255,255);
		
		//tips 
		API.drawText("回車鍵:確認執行\n上箭頭鍵:選擇上一個選項\n下箭頭鍵:選擇下一個選項",ox,y+dx_h-75, 0.3,0,168,255,255, 6,0, false, true, 0);

		//version tips
		API.drawText("ver(70307102)",ox+800,y+dx_h-30, 0.3,0,168,255,255, 6,2, false, true, 0);

	
		//password 
		API.drawText(password==""?"請輸入密碼":password,ox+400,y+127, 0.4,0,168,255,(select==0)?255:150, 6,1, false, true, 0);
		
		//connect button
 
		API.drawRectangle(ox+400-100-1,y+199,202,42,0,0,0,255);
		API.drawRectangle(ox+400-100,y+200,200,40,0,168,255,150);
		API.drawText("CONNECT",ox+400,y+200, 0.4,255,255,255,(select==1)?255:150, 1,1, false, true, 0);
	} 
 });
 
 
  API.onKeyDown.connect(function(sender, keyEventArgs) {
	if(show==true)
	{
		if(keyEventArgs.KeyCode == Keys.Up){
			select=select-1
			if(select<0){
		
					select=0
					 
			
			}
		}
		if(keyEventArgs.KeyCode == Keys.Down){
					select=select+1

					if(select>1){				
						select=1
					}
			
			
		}	
		if(keyEventArgs.KeyCode == Keys.Enter){
			if(select==0)
			{
				password=API.getUserInput(password==""?"":password,16);
			}
			if(select==1)
			{
				if(password!="")
				{
					if(enabled==true)
					{
						enabled=false
						API.triggerServerEvent("SC_LOGIN_REGISTER",password);  
					}
				}else{
					if(servertips!=null){servertips.Delete()}
					servertips=resource.dx_effect.dxEffect("text",ox,y,400,240,0,0,2000,{'showtext':"密碼不能為空",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.4,'showalpha':255},"0:128:255").Enabled();	
	 
				}
			}
		}
	}
 })
 
 var enabled=true
 var servertips=null;
 API.onServerEventTrigger.connect(function (eventName, args) {
 
	if(eventName=="SC_LOGIN_REGISTER_RETURN"){ 
	
		var s=args[0]
		if(s=="login_ok")
		{
			show=false;
			API.showShard("登陸成功,按F1打開基礎面板,祝你游戲愉快~")
				API.setCanOpenChat(true)
			API.setChatVisible(true)
		}
		if(servertips!=null){servertips.Delete()}
		if(s=="login_cd")
		{
			
			servertips=resource.dx_effect.dxEffect("text",ox,y,400,240,0,0,2000,{'showtext':"請稍後再試",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.4,'showalpha':255},"0:128:255").Enabled();	
 
		}
		if(s=="login_password_error")
		{
			servertips=resource.dx_effect.dxEffect("text",ox,y,400,240,0,0,2000,{'showtext':"密碼錯誤",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.4,'showalpha':255},"0:128:255").Enabled();	
 	
		}
		if(s=="register_scid_faild")
		{
			servertips=resource.dx_effect.dxEffect("text",ox,y,400,240,0,0,2000,{'showtext':"該SCID已和某個昵稱綁定\n請更換SCID或昵稱",'showindex':0,'showstyle':1,'showfont':6,'showfonts':0.4,'showalpha':255},"0:128:255").Enabled();	
 					
		}
		enabled=true
	}
 })