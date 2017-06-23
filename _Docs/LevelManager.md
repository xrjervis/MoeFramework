***LevelManager*** is responsible for the managment of all the levels in this game. 

It stores all the scene status by the *sceneStatus* enum type.
~~~
public enum SceneStatus{
	Home,
	Sale,
	Purchase
}

public SceneStatus CurrentScene;
~~~

It contains variables which redirect the name(string type) of each scene such that you can just use these variables to find each scene instead of use several strings. When you need to modify the name of a scene, you do not need to replace every string you have used, which brings convenience.
~~~
public string homeScene = "Home";
public string saleScene = "Sale";
public string purchaseScene = "Purchase";
~~~
And it jumps to another scene when you invoke the *LoadNext()* method. The method can jump to the correct scene according to the switch-case statements. You can also configure your loading strategies, like the ways or the orders to load the next level.
~~~
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
~~~
