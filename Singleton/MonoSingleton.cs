using UnityEngine;

/// <summary>
/// 需要使用Unity生命周期的单例模式
/// </summary>
namespace MoeFramework {
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
		protected static T _instance = null;

		public static T Instance() {
			if (_instance == null) {
				_instance = FindObjectOfType<T>();

				if (FindObjectsOfType<T>().Length > 1) {
					//QPrint.FrameworkError("More than 1!");
					return _instance;
				}

				if (_instance == null) {
					string instanceName = "GodManager";
					//QPrint.FrameworkLog("Instance Name: " + instanceName);
					GameObject instanceGO = GameObject.Find(instanceName);

					if (instanceGO == null)
						instanceGO = new GameObject(instanceName);
					_instance = instanceGO.AddComponent<T>();
					DontDestroyOnLoad(instanceGO);  //保证实例不会被释放
					//QPrint.FrameworkLog("Add New Singleton " + instance.name + " in Game!");
				} else {
					//QPrint.FrameworkLog("Already exist: " + instance.name);
				}
			}

			return _instance;
		}


		protected virtual void OnDestroy() {
			_instance = null;
		}
	}

}