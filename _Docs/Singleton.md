If you need 10 various manager, you need to copy the same code 10 times causing a huge mess! Then you need to bring in generic type.

~~~
public abstract class Singleton<T> where T : Singleton<T> {
    protected static T _instance = null;
    protected Singleton() {	}

    public static T Instance() {
    	if (_instance == null) {
        	ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            if (ctor == null)
            	throw new Exception("Non-public ctor() not found!");
            _instance = ctor.Invoke(null) as T;
		}

		return _instance;
	}
}
~~~

The script below shows how Singleton works.
~~~
public class XXXManager : Singleton<XXXManager> {  
    private XXXManager() {
        // to do ...
    }
}

public static void main(string[] args)  
{
    XXXManager.Instance().MyMethod();
}
~~~