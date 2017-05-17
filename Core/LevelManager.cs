using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoeFramework {
	public class LevelManager : MonoSingleton<LevelManager>{
		public enum SceneStatus{
			Home,
			Sale,
			Purchase
		}
		public SceneStatus CurrentScene;


		public string homeScene = "Home";
		public string saleScene = "Sale";
		public string purchaseScene = "Purchase";

		public void Init() {
			SceneManager.LoadScene(homeScene);
		}

		public void LoadNext(string button) {
			switch (button) {
				case "MapButton":
					SceneManager.LoadScene(saleScene);
					CurrentScene = SceneStatus.Sale;
					break;
				case "GoToPurchaseButton":
					SceneManager.LoadScene(purchaseScene);
					CurrentScene = SceneStatus.Purchase;
					break;
				case "GoHomeButton":
					SceneManager.LoadScene(homeScene);
					CurrentScene = SceneStatus.Home;
					break;
			}
		}
	}

}

