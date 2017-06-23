using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoeFramework {
	public class GUIManager : MonoSingleton<GUIManager> {

		public enum PanelStatus {//面板状态
			Loading,
			Home,
			Sale,
			Purchase,
			Bank,
			Dairy,
			Hospital,
			Produce,
			Achievement,
			IngredientStore,
			SnackStore,
			GiftStore,
			MaterialStore,
			Map,
			GuLuMap,
			MuseumMap,
			PrimaryMap,
			MiddleMap,
			StreetMap
		}
		///当前显示的Panel
		public PanelStatus CurrentPanel;

		//获取Panel的画布
		public GameObject m_CanvasRoot;

		public List<UIBase> m_UIList = new List<UIBase>();

		private bool CheckCanvasRootIsNull() {
			if (m_CanvasRoot == null) {
				Debug.LogError("m_CanvasRoot is Null, Please in your Canvas add UIRootHandler.cs");
				return true;
			} else {
				return false;
			}
		}

		public T ShowUI<T>() where T : UIBase {
			if (CheckCanvasRootIsNull())
				return null;

			GameObject loadGo = Utility.AssetRelate.ResourcesLoadCheckNull<GameObject>(GameManager.UI_PREFAB_PATH + typeof(T));
			if (loadGo == null)
				return null;


			GameObject ui = Utility.GameObjectRelate.InstantiateGameObject(m_CanvasRoot, loadGo);

			ui.name = ui.name.Replace("(Clone)", "").Trim();
			T t = ui.AddComponent<T>();
			t.Init(t.transform);
			m_UIList.Add(t);
			return t;
		}


		public void CloseUI(UIBase ui) {
			m_UIList.Remove(ui);
			GameObject.Destroy(ui.gameObject);

		}

		public void CloseAllPanel() {
			for (int i = 0; i < m_UIList.Count; i++) {
				GameObject.Destroy(m_UIList[i].gameObject);
			}
			m_UIList.Clear();
		}

	}
}