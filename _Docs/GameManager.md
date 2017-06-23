The details in ***GameManager*** script can be very flexible. It deals with global variables and defines the attributes needed. You may also declare the instance of SaveData and write the specific operations in *Load()* and *Save()* methods.

***GameManager*** inherits from MonoSingleton and receives unity lift-cycle.

If save data exists, then *Load()* method will be executed to retrieve the data from the existing file. But if not, the script will load initial data from .csv file or generate test data from ***Utility*** script.
~~~
if (File.Exists(Application.persistentDataPath + "\\" + _saveDataFileName + ".uml") == false) {
    Utility.TestRelate.GenerateTestData();
    //Or Load data from .csv file
}

Load();
~~~

It contains const strings to indicate the root path of resources.
~~~
public const string BACKGROUND_TEXTURE_PATH = "Textures/BackgroundTexture/";
public const string MATERIAL_TEXTURE_PATH = "Textures/MaterialTexture/";
public const string INGREDIENT_TEXTURE_PATH = "Textures/IngredientTexture/";
public const string GIFT_TEXTURE_PATH = "Textures/GiftTexture/";
public const string SNACK_TEXTURE_PATH = "Textures/SnackTexture/";
public const string DRUG_TEXTURE_PATH = "Textures/DrugTexture/";
public const string UI_PREFAB_PATH = "Prefabs/UI/";
public const string CASH_TEXTURE_PATH = "Textures/CashTexture/";
public const string MAP_TEXTURE_PATH = "Textures/MapTexture/";
~~~

***GameManager*** also contains some other methods like *RemoveFromList(), AddToPlayerList(), AddToPlayerAttribute(),* etc. which are invoked by other controller scripts outside the MoeFramework. This means that you can only modify the global variables through these methods in ***GameManager*** script which can protect the security to some extents.