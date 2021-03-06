﻿using MoeFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MoeFramework {
    /// <summary>
    /// 全局的事件管理器，管理所有的按钮点击事件
    /// </summary>
    public class EventManager : MonoSingleton<EventManager> {
	    private Dictionary<string, UnityEvent> eventDictionary;
	    void Init() {
		    if (eventDictionary == null) {
			    eventDictionary = new Dictionary<string, UnityEvent>();
		    }
	    }

	    public static void StartListening(string eventName, Action listener) {
		    UnityEvent thisEvent = null;
		    if (Instance().eventDictionary.TryGetValue(eventName, out thisEvent)) {
			    thisEvent += listener;
		    } else {
			    thisEvent = new UnityEvent();
			    thisEvent += listener;
			    Instance().eventDictionary.Add(eventName, thisEvent);
		    }
	    }

	    public static void StopListening(string eventName, Action listener) {
		    if (Instance() == null) return;
		    UnityEvent thisEvent = null;
		    if (Instance().eventDictionary.TryGetValue(eventName, out thisEvent)) {
			    thisEvent -= listener;
		    }
	    }

	    public static void TriggerEvent(string eventName) {
		    UnityEvent thisEvent = null;
		    if (Instance().eventDictionary.TryGetValue(eventName, out thisEvent)) {
			    thisEvent.Invoke();
		    }
	    }
	}

}