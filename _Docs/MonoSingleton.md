In ***MonoSingleton***, we need to do the following things:
1. Restrict the number of the instance object. 
2. Restrict the number of GameObject. 
3. Receive the MonoBehavior life-cycle.
4. Destroy singleton and its corresponding GameObject。

Be aware that the MonoSingleton has already had *DontDestroyOnLoad()* method, therefore **any class inheriting** from ***MonoSingleton***  do not need to include *DontDestroyOnLoad()* any more.
~~~
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
	protected static T _instance = null;
    
	public static T Instance() {
		if (_instance == null) {
			_instance = FindObjectOfType<T>();

		if (FindObjectsOfType<T>().Length > 1) {
			return _instance;
		}

		if (_instance == null) {
			string instanceName = "GodManager";
			GameObject instanceGO = GameObject.Find(instanceName);

			if (instanceGO == null) {
				instanceGO = new GameObject(instanceName);
				_instance = instanceGO.AddComponent<T>();
				DontDestroyOnLoad(instanceGO); 
			} 
		}

		return _instance;
	}

	protected virtual void OnDestroy() {
		_instance = null;
        }
}
~~~