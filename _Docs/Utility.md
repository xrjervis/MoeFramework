***Utility*** is just like a tool box providing various useful methods for developers. It includes instantiating objects, loading resoureces, check availability and so forth. We annouce these method to static and seperate them by their function using "Struct", which will make the codes easier to read.


How to invoke —— Utility.[Category].[Method]. For example:

~~~
Utility.GameObjectRelate.ClearChildren(gameObject.transform);
~~~

Also, under TestRelate struct, you may put all your test data here. Therefore, you can simply invoke *GenerateTestData()* in ***GameManager*** to get all your test data.
