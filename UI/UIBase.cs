using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoeFramework {

	public class UIBase : MonoBehaviour {

		public void Init(Transform tf) {
			//if (tf.gameObject.activeInHierarchy) {
			this.GetNodeComponent(tf.name, tf.gameObject);
			Button btn = tf.GetComponent<Button>();
			if (btn != null) {
				btn.onClick.AddListener(() => {
					this.OnButtonClick(btn.gameObject);
				});
			}
			//广度优先遍历
			for (int i = 0; i < tf.childCount; i++) {
				this.Init(tf.GetChild(i));
			}
			//}
		}

		void Start() {
			this.OnEnter();
		}

		void OnDestroy() {
			this.OnExit();
		}

		public void Close() {
			GUIManager.Instance().CloseUI(this);
		}

		public virtual void OnEnter() {//UI启动完成
			Debug.Log(this.name + " Enter");
		}
		public virtual void OnExit() {
			Debug.Log(this.name + " Exit");
		}

		public virtual void OnButtonClick(GameObject obj) {
			switch (obj.name) {
				case "GoHomeButton":
					if (LevelManager.Instance().CurrentScene == LevelManager.SceneStatus.Sale) {
						LevelManager.Instance().LoadNext(obj.name);
						//ui.Close();
					} else if (LevelManager.Instance().CurrentScene == LevelManager.SceneStatus.Purchase) {
						LevelManager.Instance().LoadNext(obj.name);
					} else {
						this.Close();
					}
					LevelManager.Instance().CurrentScene = LevelManager.SceneStatus.Home;
					break;

				case "BackButton":
					this.Close();
					break;
				case "CancelButton":
					this.Close();
					break;
			}
		}

		public virtual void GetNodeComponent(string name, GameObject obj) {
		}
	}
}

