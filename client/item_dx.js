var open=null,show=false,misstime=-1;
var x=0,y=0,w=0,h=5,maxh=300,alh=0;
var size=API.getScreenResolutionMantainRatio();
var line_w=0,line_h=0,line_w1=0,line_mid=0,line_mid1=0;
var eff=1;
//size.Width=size.Width*1.2;
//size.Height=size.Height*1.2;
//.Width,.Height
//	var p=API.getScreenResolutionMantainRatio();
//	API.sendChatMessage(p.Width+"="+p.H);
var title=[];
var title2=[]
var msg=[]
var rgb=[];
var startId=0
var maxId=0
API.onUpdate.connect(function(){ 

	if(open==true){
		show=true;
		if(x<200){
			x=x+50
			w=w+100 
		}else{
			x=200;
			if(h<maxh)
			{
				h=h+80;
			}else{
				w=400;
				h=maxh;
				if(line_w<(w/2)){
					line_w=line_w+40;
				}else{
					line_w=(w/2);
					if(line_h<h){
						line_h=line_h+60;
					}else{
						line_h=h;
						if(line_w1<(w/2)){
							line_w1=line_w1+40;
						}else{
							line_w1=(w/2);
							if(line_mid<(w/2)-30){
								line_mid=line_mid+20;
							}else{
								line_mid=(w/2)-30;
								if(line_mid1<15){
									line_mid1=line_mid1+2;
									alh=alh+28;
								}else{
									line_mid1=15;
									if(misstime==-1){
									misstime=Math.round(new Date().getTime()/1000) +6;}
								}
							}
				
						}
					}
				}
	
			}
		} 
	}
	if(open==false){
		if(line_mid1>0){
			line_mid1=line_mid1-2;
			alh=alh-28;
		}else{
			line_mid1=0;
			if(line_mid>0){
				line_mid=line_mid-20;
			}else{
				line_mid=0;
				if(line_w1>0){
					line_w1=line_w1-40;
				}else{
					line_w1=0;
					if(line_h>0){
						line_h=line_h-60;
					}else{
						line_h=0;
						if(line_w>0){
							line_w=line_w-40;
						}else{
							line_w=0;
							if(h>5){
								h=h-60;
							}else{
								h=5;
								if(x>0){
									x=x-50;
									w=w-100;
									
								}else{
									x=0,w=0;
									show=false;   
									open=null;
									if(startId!=maxId){
										startId++
										if(startId!=maxId){
											open=true;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
	}
	if(show==true){
		var sx
		var sh=(size.Height*0.5)-(h/2) 
		if(resource.car_shop.show==true)
		{
		 sx=(size.Width*0.5)+600-x 
		}else{
		 sx=(size.Width*0.5)-x
		}
		var mid_w=sx+(w/2)
		API.drawRectangle(sx,sh,w,h,0,0,0,195);
		if(line_w!=0){	
			API.drawRectangle(mid_w-line_w,sh,line_w,3,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
			API.drawRectangle(mid_w,sh,line_w,3,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
		}
		
		if(line_h!=0){
			API.drawRectangle(sx+(w-2),sh,2,line_h,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
			API.drawRectangle(sx,sh,2,line_h,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
			
		}
		if(line_w1!=0){
			API.drawRectangle(sx,sh+h,line_w1,2,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
			API.drawRectangle(sx+w-line_w1,sh+h,line_w1,2,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
		}

		if(h>100){
			API.drawText(title[startId], mid_w, sh+20, 0.5, 255,255,255,alh, 6,1, false, true, 0); 
			API.drawText(title2[startId], mid_w, sh+70, 0.3, 255,255,255,alh, 6,1, false, true, 0); 
			
			//API.drawRectangle((size.Width*0.5)-x,((size.Height*0.5)-(h/2))+100,w,2,255,255,255,195);
			if(line_mid!=0){
				API.drawRectangle(sx,sh+100,line_mid,2,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);
				API.drawRectangle(sx+w-line_mid,sh+100,line_mid,2,rgb[startId][0],rgb[startId][1],rgb[startId][2],255);			

			}
			if(line_mid1!=0){
					API.drawRectangle(sx+line_mid,sh+100,line_mid1,2,rgb[startId][0],rgb[startId][1],rgb[startId][2],alh/2);
					API.drawRectangle(sx+w-line_mid-line_mid1,sh+100,line_mid1,2,rgb[startId][0],rgb[startId][1],rgb[startId][2],alh/2);
			
			}
			
		}
	
			var tmp=msg[startId].split('\n')
				 
			if(tmp.length<5){
				maxh=300
			}else{
				maxh=300+(tmp.length*15)
			}
			for(i in tmp){
				var showh=140+(30*i) 
				if(h>showh){
					API.drawText(tmp[i],mid_w,sh+(showh-30), 0.4, 255,255,255,alh, 6,1, false, false, 0); 
				}
			}
		
	}
	if(misstime!=-1){
		var nowt=Math.round(new Date().getTime()/1000)
		if(nowt>=misstime)
		{
			open=false;
			misstime=-1; 
			
		}
	}
}); 
 
  API.onServerEventTrigger.connect(function (eventName, args) {
	if(eventName=="SC_ITEM_DX"){ 
		title[maxId]=args[0];
		title2[maxId]=args[1];
		msg[maxId]=args[2];
		rgb[maxId]=new Array()
		rgb[maxId][0]=parseInt(args[3])
		rgb[maxId][1]=parseInt(args[4])
		rgb[maxId][2]=parseInt(args[5])
		maxId++;
		if(show==false){
			open=true;
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
