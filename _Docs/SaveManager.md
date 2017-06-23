SaveManager supports most of the data type in unity with C# including List\<aClass\>.
On Windows, the save data file can be found under **C:\Users\YourUserName\AppData\LocalLow\MS\YourAppName**

### Instruction

First, you need to create the data instance.
~~~
data = new SaveData(fileName);
~~~

Then you may add keys with significant names and values.
~~~
data["Name"] = "Rui Xie";
data["Dude"] = "Siri";
data["Key"] = true;
data["HealthPotions"] = 10;
data["Position"] = new Vector3(20, 3, -5);
data["Rotation"] = new Quaternion(0.1f,0.1f,0.1f,1);
~~~

Save the data.
~~~
data.Save();
~~~

Load the data we just saved.
~~~
data = SaveData.Load(Application.streamingAssetsPath+"\\"+fileName+".uml");
~~~

Use data.
~~~
Debug.Log("Name : "+ data.GetValue<string>("Name"));
Debug.Log("Has health potions : " + data.TryGetValue<int>("HealthPotions", out potions));
Debug.Log("Has buddy : " + data.HasKey("Dude"));
Debug.Log("Buddy's name : " + data.GetValue<string>("Dude"));
Debug.Log("Current position : " + data.GetValue<Vector3>("Position"));
Debug.Log("Has key : " + data.GetValue<bool>("Key"));
Debug.Log("Rotation : " + data.GetValue<Quaternion>("Rotation"));
~~~