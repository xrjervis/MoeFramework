using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BaseClass;
using Random = UnityEngine.Random;

namespace MoeFramework {
    /// <summary>
    /// 游戏进程管理
    /// 数据定义（属性）
    /// </summary>
    public class GameManager : MonoSingleton<GameManager> {
        ///存档文件名（后缀固定为.uml)
        private string _saveDataFileName = "saveData";

        ///声明存档管理器
        private SaveManager data;

        ///游戏时间的1天与真实时间的秒数对应（可修改）
        public float dayPerSec = 10.0f;

        ///定义玩家仓库有的原料，玩具，小吃数据结构
        public List<BaseClass.Material> player_materials = new List<BaseClass.Material>();

        public List<Ingredient> player_ingredients = new List<Ingredient>();
        public List<Gift> player_gifts = new List<Gift>();
        public List<Snack> player_snacks = new List<Snack>();


        ///定义商店正在出售的物品
        public List<BaseClass.Material> store_materials = new List<BaseClass.Material>();

        public List<Ingredient> store_ingredients = new List<Ingredient>();
        public List<Gift> store_gifts = new List<Gift>();
        public List<Snack> store_snacks = new List<Snack>();

        ///医院出售的药品
        public List<Drug> drugs = new List<Drug>();

        ///银行的宝箱（现金）
        public List<Cash> cashs = new List<Cash>();

        ///地图列表
        public List<Map> maps = new List<Map>();

        ///机器列表
        public List<Machine> machines = new List<Machine>();

        /// 生产失败的概率，索引为原料个数
        public List<int> produceFailProbability = new List<int>();


        ///当前执行的操作总花费（购买物品，解锁地图等）
        public int CurMoneyCost = 0;

        ///选中物品在List中的索引 Range[0, itemCount]
        public int CurItemListIndex;

        ///选中物品的ID
        public string CurItemID = "";

        ///物品的数量
        public int CurAmountCost = 0;

        ///未解锁Item的索引
        public int CurUnlockIndex = 1;

	    ///UI_Produce界面当前生产成功的机器的索引
	    public int CurCompleteMachine = 0;

        //定义Prefab和Texture的根路径
        //注意，此处路径不需要再添加Resources，但是要在Project菜单下有Resources文件夹
        public const string BACKGROUND_TEXTURE_PATH = "Textures/BackgroundTexture/";
        public const string MATERIAL_TEXTURE_PATH = "Textures/MaterialTexture/";
        public const string INGREDIENT_TEXTURE_PATH = "Textures/IngredientTexture/";
        public const string GIFT_TEXTURE_PATH = "Textures/GiftTexture/";
        public const string SNACK_TEXTURE_PATH = "Textures/SnackTexture/";
        public const string DRUG_TEXTURE_PATH = "Textures/DrugTexture/";
        public const string UI_PREFAB_PATH = "Prefabs/UI/";
        public const string CASH_TEXTURE_PATH = "Textures/CashTexture/";
        public const string MAP_TEXTURE_PATH = "Textures/MapTexture/";

        #region 属性定义

        /// <summary>
        /// 玩家金钱
        /// </summary>
        public int Money {
            get { return data.GetValue<int>("Money"); }
            set { data["Money"] = value; }
        }



        /// <summary>
        /// 玩家体力值上限
        /// </summary>
        public int MaxStaminaVal {
            get { return data.GetValue<int>("MaxStaminaVal"); }
            set { data["MaxStaminaVal"] = value; }
        }

        /// <summary>
        /// 玩家体力值
        /// </summary>
        public int StaminaVal {
            get { return data.GetValue<int>("StaminaVal"); }
            set { data["StaminaVal"] = value; }
        }

        /// <summary>
        /// 玩家累积消耗体力值
        /// </summary>
        public int TotalStaminaVal {
            get { return data.GetValue<int>("TotalStaminaVal"); }
            set { data["TotalStaminaVal"] = value; }
        }

        /// <summary>
        /// 玩家状态值消耗速率
        /// </summary>
        public int StaminaRate {
            get { return data.GetValue<int>("StaminaRate"); }
            set { data["StaminaRate"] = value; }
        }

        /// <summary>
        /// 玩家智力值
        /// </summary>
        public int IntelligenceVal {
            get { return data.GetValue<int>("IntelligenceVal"); }
            set { data["IntelligenceVal"] = value; }
        }

        /// <summary>
        /// 玩家爱心值
        /// </summary>
        public int HotHeartVal {
            get { return data.GetValue<int>("HotHeartVal"); }
            set { data["HotHeartVal"] = value; }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public int CurrentDay {
            get { return data.GetValue<int>("CurrentDay"); }
            set { data["CurrentDay"] = value; }
        }

        public int HighQualityProbability {
            get { return data.GetValue<int>("HighQualityProbability"); }
            set { data["HighQualityProbability"] = value; }
        }

	    public string LaTiaoName {
		    get { return data.GetValue<string>("LaTiaoName"); }
			set { data["LaTiaoName"] = value; }
	    }

        /// <summary>
        /// 存档管理器
        /// </summary>
        public SaveManager Data {
            get {
                return data;
            }
        }
        #endregion

        #region 方法定义
        /// <summary>
        /// 减少金钱并保存
        /// </summary>
        public void DecreaseMoney() {
            Money -= CurMoneyCost;
            Save();
        }
        /// <summary>
        /// 增加玩家金钱并保存
        /// </summary>
        /// <param name="x">增加数量</param>
        public void IncreaseMoney(int x) {
            Money += x;
            Save();
        }
        /// <summary>
        /// 增加玩家体力值并保存
        /// </summary>
        /// <param name="x">增加数量</param>
        private void IncreaseStamina(int x) {
            StaminaVal += x;
            //体力值不超过上限
            if (StaminaVal > MaxStaminaVal)
                StaminaVal = MaxStaminaVal;
            Save();
        }
        /// <summary>
        /// 增加玩家体力值上限并保存
        /// </summary>
        /// <param name="x">增加数量</param>
        private void IncreaseMaxStaminaVal(int x) {
            MaxStaminaVal += x;
            Save();
        }

        /// <summary>
        /// 体力恢复满
        /// </summary>
        public void RecoverStamina() {
            StaminaVal = MaxStaminaVal;
            Save();
        }

        /// <summary>
        /// 每卖出一个商品，体力减少StaminRate点，累积体力消耗增加
        /// </summary>
        public void DecreaseStamina() {
            StaminaVal = StaminaVal - StaminaRate;
            TotalStaminaVal = TotalStaminaVal + StaminaRate;
            Save();
        } 

        /// <summary>
        /// 每卖出一件商品，HotHeartVal加一
        /// </summary>
        public void IncreaseHotHeartVal() {
            HotHeartVal = HotHeartVal + 1;
        }

        private void DecreaseStaminaRate(int x) { }
        private void IncreaseStaminaRate(int x) { }
        private void DecreaseIntelligence(int x) { }
        private void IncreaseIntelligence(int x) { }
        #endregion

        protected override void OnDestroy() {
            base.OnDestroy();
            Save();
        }

        void Start() {
            LevelManager.Instance().Init();
            data = new SaveManager();

            //TODO 之后会从excel读取初始数据
            //如果存档文件不存在，就生成测试数据
            if (File.Exists(Application.persistentDataPath + "\\" + _saveDataFileName + ".uml") == false) {
                Utility.TestRelate.GenerateTestData();
            }

            Load();

            //TODO 游戏时间根据体力值消耗修改
        }

        public void Save() {
            data["Player_Materials"] = player_materials;
            data["Player_Ingredients"] = player_ingredients;
            data["Player_Snacks"] = player_snacks;
            data["Player_Gifts"] = player_gifts;
            data["Store_Materials"] = store_materials;
            data["Store_Ingredients"] = store_ingredients;
            data["Store_Snacks"] = store_snacks;
            data["Store_Gifts"] = store_gifts;
            data["Drugs"] = drugs;
            data["Cashs"] = cashs;
            data["Maps"] = maps;
            data["Machines"] = machines;
            data["ProduceFailProbability"] = produceFailProbability;

            data.Save();
        }

        public void Load() {
            //TODO 在将商品名称作为Key的时候，最好传变量而不是直接传字符串常量,可以用一个NameList来实现
            //persistentDataPath路径在 C:\Users\用户\AppData\LocalLow\MS\LTZHD
            data = SaveManager.Load(Application.persistentDataPath + "\\" + _saveDataFileName + ".uml");

            //导入商店数据
            if (data.HasKey("Store_Materials"))
                store_materials = data.GetValue<List<BaseClass.Material>>("Store_Materials");
            if (data.HasKey("Store_Ingredients"))
                store_ingredients = data.GetValue<List<Ingredient>>("Store_Ingredients");
            if (data.HasKey("Store_Snacks"))
                store_snacks = data.GetValue<List<Snack>>("Store_Snacks");
            if (data.HasKey("Store_Gifts"))
                store_gifts = data.GetValue<List<Gift>>("Store_Gifts");

            //导入玩家仓库数据
            if (data.HasKey("Player_Materials"))
                player_materials = data.GetValue<List<BaseClass.Material>>("Player_Materials");
            if (data.HasKey("Player_Ingredients"))
                player_ingredients = data.GetValue<List<Ingredient>>("Player_Ingredients");
            if (data.HasKey("Player_Snacks"))
                player_snacks = data.GetValue<List<Snack>>("Player_Snacks");
            if (data.HasKey("Player_Gifts"))
                player_gifts = data.GetValue<List<Gift>>("Player_Gifts");

            //导入药品数据
            if (data.HasKey("Drugs"))
                drugs = data.GetValue<List<Drug>>("Drugs");

            //导入银行宝箱数据
            if (data.HasKey("Cashs"))
                cashs = data.GetValue<List<Cash>>("Cashs");

            //导入地图数据
            if (data.HasKey("Maps"))
                maps = data.GetValue<List<Map>>("Maps");

            //导入生产机器数据
            if (data.HasKey("Machines"))
                machines = data.GetValue<List<Machine>>("Machines");

            //导入生产失败概率表
            if (data.HasKey("ProduceFailProbability"))
                produceFailProbability = data.GetValue<List<int>>("ProduceFailProbability");
        }

        /// <summary>
        /// 从商店List减少商品数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storeList"></param>
        private void RemoveFromList<T>(List<T> list) where T : Goods {
            list[CurItemListIndex].Amount -= CurAmountCost;
            if (list[CurItemListIndex].Amount == 0) { list.RemoveAt(CurItemListIndex); }
        }

        private void RemoveFromList<T>(List<T> list, T item) where T : Goods {
            T temp = list.Find(x => (x.ID == item.ID));
            temp.Amount -= item.Amount;
            if (temp.Amount == 0) {
                list.Remove(temp);
            }
        }

        private void RemoveFromList<T>(List<T> list1, List<T> list2) where T : Goods {
            foreach (var item in list2) {
                RemoveFromList(list1, item);
            }
        }

        /// <summary>
        /// 给玩家List添加该商品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playerList"></param>
        private void AddToPlayerList<T>(List<T> playerList, List<T> storeList) where T : Goods {
            //先从playerList里查找，若找到，则增加数量，没找的则在List里添加该商品类
            T temp = playerList.Find(x => (x.ID == CurItemID));
            if (temp != null) {
                temp.Amount += CurAmountCost;
            } else {
                T goods = (T)storeList.Find(x => (x.ID == CurItemID)).Clone();
                goods.Amount = CurAmountCost;
                playerList.Add(goods);
            }
        }

        /// <summary>
        /// 玩家购买药品，增加玩家的对应数值
        /// </summary>
        private void AddToPlayerAttribute() {
            Drug d = drugs.Find(x => (x.ID == CurItemID));
            IncreaseStamina(d.StaminaIncreaseVal);
            IncreaseMaxStaminaVal(d.MaxStaminaIncreaseVal);
        }

        /// <summary>
        /// 响应UI_PurchaseConfirm界面里的确认购买按钮
        /// </summary>
        public void Purchase() {
            DecreaseMoney();
            //Debug.Log(gridName + gridNum)
            switch (GUIManager.Instance().CurrentPanel) {
                case GUIManager.PanelStatus.IngredientStore:
                    //玩家购买调味料，添加玩家List数据，移除商店List数据
                    AddToPlayerList(player_ingredients, store_ingredients);
                    RemoveFromList(store_ingredients);
                    break;
                case GUIManager.PanelStatus.SnackStore:
                    //玩家购买小吃，添加玩家List数据，移除商店List数据
                    AddToPlayerList(player_snacks, store_snacks);
                    RemoveFromList(store_snacks);
                    break;
                case GUIManager.PanelStatus.GiftStore:
                    //玩家购买礼品，添加玩家List数据，移除商店List数据
                    AddToPlayerList(player_gifts, store_gifts);
                    RemoveFromList(store_gifts);
                    break;
                case GUIManager.PanelStatus.MaterialStore:
                    //玩家购买原料，添加玩家List数据，移除商店List数据
                    AddToPlayerList(player_materials, store_materials);
                    RemoveFromList(store_materials);
                    break;
                case GUIManager.PanelStatus.Hospital:
                    //玩家购买药品，增加玩家属性，移除药品List数据
                    AddToPlayerAttribute();
                    RemoveFromList(drugs);
                    break;
            }
            Save();
        }

        /// <summary>
        /// 响应UI_Produce_SelectItem里的生产辣条按钮事件
        /// </summary>
        public void Produce(BaseClass.Material m, List<Ingredient> i_list) {
            machines[CurUnlockIndex].TotalProduceTime = new TimeSpan(0, 0, 0, 10);
            machines[CurUnlockIndex].EndTime = DateTime.Now + machines[CurUnlockIndex].TotalProduceTime;


            //根据概率生成辣条放到Machine的Product里
	        if (Random.Range(1, 101) > produceFailProbability[i_list.Count]) {
		        //决定辣条口味
		        int Sour = 0;
		        int Sweet = 0;
		        int Salty = 0;
		        int Spicy = 0;
		        int MaxVal;
		        string flavor;
		        foreach (var i in i_list) {
			        Sour += i.SourVal;
			        Sweet += i.SweetVal;
			        Salty += i.SaltyVal;
			        Spicy += i.SpicyVal;
		        }
		        MaxVal = Sour;
		        flavor = "Sour";
		        if (Sweet > MaxVal) {
			        MaxVal = Sweet;
			        flavor = "Sweet";
		        }
		        if (Salty > MaxVal) {
			        MaxVal = Salty;
			        flavor = "Salty";
		        }
		        if (Spicy > MaxVal) {
			        MaxVal = Spicy;
			        flavor = "Spicy";
		        }

		        Debug.Log("生产成功");
		        Debug.Log(CurUnlockIndex);
				if (Random.Range(1, 101) < HighQualityProbability) {
			        Debug.Log("高品质辣条");
			        machines[CurUnlockIndex].Product = new LaTiao("High_" + flavor + "_LaTiao", LaTiaoName, m.Amount, 100,
				        flavor, "High", Sour, Sweet, Salty, Spicy);
		        }
		        else {
			        Debug.Log("低品质辣条");
			        machines[CurUnlockIndex].Product = new LaTiao("Low_" + flavor + "_LaTiao", LaTiaoName, m.Amount, 100,
				        flavor, "Low", Sour, Sweet, Salty, Spicy);
		        }

	        }
	        else {
				machines[CurUnlockIndex].Product = null;
		        Debug.Log("生产失败");
			}
            //从玩家数据移除原料和调料
            RemoveFromList(player_materials, m);
            RemoveFromList(player_ingredients, i_list);


            machines[CurUnlockIndex].IsWorking = true;
        }

        /// <summary>
        /// 轮询机器状态
        /// </summary>
        void Update() {
            CheckMachinesStatus();
        }

        /// <summary>
        /// 检查机器状态
        /// </summary>
        public void CheckMachinesStatus() {
            foreach (var item in machines) {
                if (item.IsWorking == true) {
                    if (item.EndTime <= DateTime.Now) {
                        item.IsWorking = false;
                        item.IsCompleted = true;
                        //将机器的产品添加到用户数据里
                        player_snacks.Add(item.Product);
                        Save();
                    }
                }
            }
        }
    }
}

