var open=null,show=false,mission=false,misstime=-1;
var x=0,y=0,w=0,h=5,alh=0;
var size=API.getScreenResolutionMantainRatio();
//.Width,.Height
var title="";
var raceTime=0,raceName="",raceMoney=0,raceid=-1;
var msg=""
var money=""
var result="",startTime=-1,nowtime=-1;
var dx_x=size.Width
API.onUpdate.connect(function(){ 
	if(open==true){
		show=true;
		if(x<450){
			x=x+30
			w=w+30 
		}else{
			x=450;
			if(h<115)
			{
				h=h+10;
			}else{
				w=450;
				h=115;
				 
			}
		} 
	}
	if(open==false){
		if(h>5){
			h=h-10;
		}else{
			h=5;
			if(x>0){
				x=x-30;
				w=w-30;
				
			}else{
				x=0,w=0;
				open=null;
				show=false;
				mission=false;
			}
		}
		
	}
 
	if(show==true&&API.getEntitySyncedData(API.getLocalPlayer(),"SC_CONFIG:SHOW_RACE_MISSION")!=false){
		var sx=(size.Width)-x
		API.drawRectangle(sx,160,w,h,0,0,0,195);
		if(h>40){
			API.drawText(title, sx+10, 160, 0.5, 255,255,255,195, 6,0, false, true, 0);
			API.drawText(result, sx+x, 160, 0.5, 255,255,255,195, 6,2, false, true, 0);
			API.drawRectangle(sx,200,w,2,255,255,255,195);
		}
		if(h>60){
			var tmp=msg
			var newl=false;
			var newl2=false;
			if(raceTime!=999999){
				if(misstime==-1){
					nowtime=((Math.round(new Date().getTime())-startTime)/1000)
				}
				if(startTime!=-1){
					tmp=tmp+"\n當前時間 ~g~"+nowtime+"s"
					newl=true;
				}else{
					newl2=true; 
				}
			}else{
				newl2=true;
			}
			API.drawText(tmp, sx+10, 200, 0.3, 255,255,255,195, 6,0, false, true, 0);
			API.drawText(money,sx+x, 160+(h-30), 0.3, 255,255,255,195, 6,2, false, true, 0);
			if(newl2==true){
				API.drawText("退出~g~"+raceName+"~w~的賽事可~r~放棄任務",  sx+10, 160+(h-50), 0.3, 255,255,255,195, 6,0, false, true, 0);

			}
			if(newl==true){
				
				API.drawText("時間有一定誤差,以賽事大廳時間為准", sx+10, 160+(h-30), 0.3, 255,255,255,195, 6,0, false, true, 0);
			}
		}
	}
	if(misstime!=-1){
		var nowt=Math.round(new Date().getTime()/1000)
		if(nowt>=misstime)
		{
			misstime=-1;
			startTime=-1;
			nowtime=-1;
			open=false
		}
	}

}); 
 
  API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_MISSION_NEW"){ 
			if(mission==false){
				//沒有任務在進行
				result="未開始";
				var hard="~m~極簡單"
				if(args[3]<=1.4&&args[3]>1.3){hard="~c~簡單"}
				if(args[3]<=1.3&&args[3]>1.2){hard="~o~中等"}
				if(args[3]<=1.2&&args[3]>1.1){hard="~r~困難"}
				if(args[3]<=1&&args[3]>1.02){hard="~b~極困難"}
				if(args[3]<=1.02){hard="~p~極限"}
				title="賽事任務(難度:"+hard+"~w~)" 
				raceTime=parseFloat(args[1])
				raceName=args[0]
				raceMoney=args[2]
				raceid=args[4]
				if(raceTime==999999){
					
					msg="完成 ~g~"+raceName+"(RID:"+raceid+")"+" ~w~!";
				}else{
					msg="在 ~g~"+raceTime+"s(RID:"+raceid+")"+" ~w~內完成 ~g~"+raceName+" ~w~!";
				}
				money="~g~獎勵 ~y~"+raceMoney;
				if(raceMoney==-1)
				{
					money="~g~獎勵 ~y~車輛碎片"
				}
				open=true;
				mission=true;
			}
		}
	if(eventName=="SC_race_house_startrace"){
		if(args[1]==raceName){
			if(result=="~g~進行中"){
				startTime=Math.round(new Date().getTime())
			}
		}
	}
	if(eventName=="SC_race_house_quitrace"){
		if(args[0]==raceName){
			if(result=="~g~進行中"){
				if(result!="~y~完成"){
					result="~r~失敗"
					misstime=Math.round(new Date().getTime()/1000)+5
				}
			}
		}
	}
 	if(eventName=="SC_race_house_joinrace"){
		if(args[0]==raceName){
			result="~g~進行中" 
		}
	}
	if(eventName=="SC_race_house_finishrace_mission"){
		if(args[0]==raceName){
			if(result=="~g~進行中"){
				if(parseFloat(args[1])<=parseFloat(raceTime)){
					result="~y~完成";
					API.triggerServerEvent("SC_mission_compelete_givemoney",raceMoney);
				}else{
					result="~r~失敗";
				}
				misstime=Math.round(new Date().getTime()/1000)+5
			}
		}
	}
 
 });
 
 
//RPM 車輛轉速
//popVehicleTyre 車輛輪胎損壞控制，true 為損壞,false為未損壞
//breakVehicleTyre 車輛門損壞控制，同上
//breakVehicleWindow 車輛窗戶損壞控制，同上
//setVehiclePrimaryColor 設置車輛顏色(用GTA-V內置顏色ID）
//setVehicleSecondaryColor 設置車輛副色(同上)
//setVehicleCustomPrimaryColor 設置車輛顏色（用RGB三色(0-255)）
//setVehicleCustomSecondaryColor 設置車輛副色(同上)
