using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AC_Timer : MonoBehaviour {
	//把这个放在GameManager里面作为常量，方便修改
	private const string timeURL = "http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1";
	//把这个变量放在Utility中作为工具变量使用
	public DateTime curTime;


	//把这个方法放在Utility里面作为工具类调用
	IEnumerator GetTime() {
		if(Application.internetReachability == NetworkReachability.NotReachable) {
			print("请检查网络连接");
		} else {
			WWW www = new WWW(timeURL);
			yield return www;

			string timeStr = www.text.Substring(2);
			//此处开始完成了时间字符串到DateTime类型的转换
			curTime = DateTime.MinValue;
			DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			curTime = startTime.AddMilliseconds(Convert.ToDouble(timeStr));
			print(curTime);
		}
		

	}

	private void Start() {
		//需要获取时间时，请先启动GetTime协程，之后通过curTime获取时间
		StartCoroutine("GetTime");

	}
}
