Using C# delegates and UnityEvents, ***EventManager*** implements a simple messaging system which will allow items in the project to subscribe to events, and have events trigger actions in our games. This will reduce dependencies and allow easier maintenance of our projects.

### Instruction

First, you need to bind an event with a function by assigning a string to the function using *EventManager.StartListening(string eventName, Action listener)*.
~~~
void OnEnable()
{
    EventManager.StartListening("Test", SomeFunction);
    EventManager.StartListening("Spawn", SomeOtherFunction);
    EventManager.StartListening("Destroy", SomeThirdFunction);
}
~~~
If you want to stop listening, just invoke *EventManager.StopListening(string eventName, Action listener)*.
~~~
void OnDisable()
{
    EventManager.StopListening("Test", SomeFunction);
    EventManager.StopListening("Spawn", SomeOtherFunction);
    EventManager.StopListening("Destroy", SomeThirdFunction);
}
~~~
Then in some other scripts you may define the specific operations in each funtion you need.
~~~
void SomeFunction()
{
    Debug.Log("Some Function was called!");
}
~~~
Also, you should use *TriggerEvent(string eventName)* to invoke an event. The following script below test the event by triggering events every 2 seconds.
~~~
void Awake()
{
    someListener = new Action(SomeFunction);
    StartCoroutine(invokeTest());
}

IEnumerator invokeTest()
{
    WaitForSeconds waitTime = new WaitForSeconds(2);
    while (true)
    {
        yield return waitTime;
        EventManager.TriggerEvent("test");
        yield return waitTime;
        EventManager.TriggerEvent("Spawn");
        yield return waitTime;
        EventManager.TriggerEvent("Destroy");
    }
}
~~~
