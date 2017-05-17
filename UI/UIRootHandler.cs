using UnityEngine;

namespace MoeFramework {
	public class UIRootHandler : MonoBehaviour {
		void Awake() {
			GUIManager.Instance().m_CanvasRoot = this.gameObject;
		}
	}
}
