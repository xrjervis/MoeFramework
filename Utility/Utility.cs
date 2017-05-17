using UnityEngine;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Security.Cryptography;
using BaseClass;

namespace MoeFramework {
    public class Utility {
        public struct GameObjectRelate {
            /// <summary>
            /// 清除此父物件下子物件
            /// </summary>
            public static void ClearChildren(Transform Obj) {
                for (int i = Obj.childCount - 1; i >= 0; --i) {
                    Transform Item = Obj.GetChild(i);
                    Item.SetParent(null);
                    MonoBehaviour.DestroyImmediate(Item.gameObject);
                }
            }

            /// <summary>
            /// 在父物件下建立子物件(使用名称建立一個新物件)
            /// </summary>
            public static GameObject InstantiateGameObject(GameObject parent, string name) {
                GameObject go = new GameObject(name);

                if (parent != null) {
                    Transform t = go.transform;
                    t.SetParent(parent.transform);
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;

                    RectTransform rect = go.transform as RectTransform;
                    if (rect != null) {
                        rect.anchoredPosition = Vector3.zero;
                        rect.localRotation = Quaternion.identity;
                        rect.localScale = Vector3.one;

                        //判斷anchor是否在同一點
                        if (rect.anchorMin.x != rect.anchorMax.x && rect.anchorMin.y != rect.anchorMax.y) {
                            rect.offsetMin = Vector2.zero;
                            rect.offsetMax = Vector2.zero;
                        }
                    }

                    go.layer = parent.layer;
                }
                return go;
            }

            /// <summary>
            /// 在父物件下建立子物件
            /// </summary>
            public static GameObject InstantiateGameObject(GameObject parent, GameObject prefab) {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;

                if (go != null && parent != null) {
                    Transform t = go.transform;
                    t.SetParent(parent.transform);
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;

                    RectTransform rect = go.transform as RectTransform;
                    if (rect != null) {
                        rect.anchoredPosition = Vector3.zero;
                        rect.localRotation = Quaternion.identity;
                        rect.localScale = Vector3.one;

                        //判斷anchor是否在同一點
                        if (rect.anchorMin.x != rect.anchorMax.x && rect.anchorMin.y != rect.anchorMax.y) {
                            rect.offsetMin = Vector2.zero;
                            rect.offsetMax = Vector2.zero;
                        }
                    }

                    go.layer = parent.layer;
                }
                return go;
            }

            /// <summary>
            /// 查詢子物件
            /// </summary>
            public static Transform SearchChild(Transform target, string name) {
                if (target.name == name) return target;

                for (int i = 0; i < target.childCount; ++i) {
                    var result = SearchChild(target.GetChild(i), name);

                    if (result != null) return result;
                }

                return null;
            }

            /// <summary>
            /// 查詢多個子物件
            /// </summary>
            public static List<Transform> SearchChildsPartName(Transform target, string name) {
                List<Transform> objs = new List<Transform>();
                Transform child = null;

                for (int i = 0; i < target.childCount; ++i) {
                    child = target.GetChild(i);

                    if (child != null) {
                        if (child.name.IndexOf(name, 0) >= 0)
                            objs.Add(child);
                    }
                }

                return objs;
            }

            /// <summary>
            /// 使用GetInstance比較GameObject
            /// </summary>
            public static bool CompareGameObject(GameObject A, GameObject B) {
                return A.GetInstanceID() == B.GetInstanceID() ? true : false;
            }

            /// <summary>
            /// GameObject Array 全开/全关
            /// </summary>
            public static void SetObjectArrayActive(GameObject[] gos, bool isActive) {
                for (int i = 0; i < gos.Length; i++)
                    gos[i].SetActive(isActive);
            }

            /// <summary>
            /// GameObject 开关
            /// </summary>
            public static void SetObjectActiveToggle(GameObject go) {
                go.SetActive(!go.activeSelf);
            }


            public delegate void SmallTabHandler();

            /// <summary>
            /// GameObject Array 排序
            /// </summary>
            public static void SortGameObjectArray(ref GameObject[] gos) {
                System.Array.Sort(gos, (a, b) => a.name.CompareTo(b.name));
            }

            /// <summary>
            /// GameObject Child 排序
            /// </summary>
            public static void SortHierarchyObjectChildByName(Transform parent) {
                List<Transform> children = new List<Transform>();
                for (int i = parent.childCount - 1; i >= 0; i--) {
                    {
                        Transform child = parent.GetChild(i);
                        children.Add(child);
                        child.parent = null;
                    }
                }

                children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
                foreach (Transform child in children) {
                    child.parent = parent;
                }
            }

            /// <summary>
            /// 使用已存在的 Compoent 加入 GameObject
            /// </summary>
            public static T AddComponent<T>(GameObject go, T toAdd) where T : Component {
                return GetCopyOf(go.AddComponent<T>(), toAdd) as T;
            }

            public static T GetCopyOf<T>(Component comp, T other) where T : Component {
                Type type = comp.GetType();
                if (type != other.GetType()) return null; // type mis-match
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                     BindingFlags.Default | BindingFlags.DeclaredOnly;
                PropertyInfo[] pinfos = type.GetProperties(flags);
                foreach (var pinfo in pinfos) {
                    if (pinfo.CanWrite) {
                        try {
                            pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                        }
                        catch {
                        }
                        // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                    }
                }
                FieldInfo[] finfos = type.GetFields(flags);
                foreach (var finfo in finfos) {
                    finfo.SetValue(comp, finfo.GetValue(other));
                }
                return comp as T;
            }


        }

        public struct AssetRelate {
            /// <summary>
            /// Resources.Load 并检查是否null
            /// </summary>
            public static T ResourcesLoadCheckNull<T>(string name) where T : UnityEngine.Object {
                T loadGo = Resources.Load<T>(name);

                if (loadGo == null) {
                    Debug.LogError("Resources.Load [ " + name + " ] is Null !!");
                    return default(T);
                }

                return loadGo;
            }

            /// <summary>
            /// Resources.Load Sprite
            /// </summary>
            public static Sprite ResourcesLoadSprite(string name) {
                return ResourcesLoadCheckNull<Sprite>("Sprites/" + name);
            }
        }

        public struct TestRelate {
            /// <summary>
            /// 生成一个调味料物品，并保存
            /// </summary>
            /// <param name="list">临时列表</param>
            /// <param name="id">物品ID</param>
            /// <param name="name">物品名称</param>
            /// <param name="intro">物品介绍</param>
            /// <param name="amount">物品数量</param>
            /// <param name="price">物品价格</param>
            /// <param name="sour">酸度值</param>
            /// <param name="sweet">甜度值</param>
            /// <param name="salty">咸度值</param>
            /// <param name="spicy">辣度值</param>
            static void CreateIngredient(List<Ingredient> list, string id, string name, string intro, int amount,
                int price, int sour, int sweet,
                int salty, int spicy) {
                Ingredient i;
                i = new Ingredient();
                i.ID = id;
                i.Name = name;
                i.Amount = amount;
                i.Intro = intro;
                i.Price = price;
                i.SourVal = sour;
                i.SweetVal = sweet;
                i.SaltyVal = salty;
                i.SpicyVal = spicy;
                list.Add(i);
            }


            public static void GenerateTestData() {
                #region 数据测试
                //自定义初始数据
                GameManager.Instance().CurrentDay = 1;
                GameManager.Instance().StaminaVal = 150;
                GameManager.Instance().StaminaRate = 2;
                GameManager.Instance().MaxStaminaVal = 1000;
                GameManager.Instance().HotHeartVal = 0;
                GameManager.Instance().IntelligenceVal = 66;
                GameManager.Instance().Money = 10000;
                GameManager.Instance().HighQualityProbability = 50;
                GameManager.Instance().TotalStaminaVal = 0;

                //自定义剧情数据
                Story story11 = new Story();
                story11.AppearNeedStaminVals = 10;
                story11.IntroText = "我是一个特殊的小孩11";
                story11.DetailText = "今天碰到了一个特殊的小孩11，好特殊啊好特殊，好特殊啊好特殊好特殊啊好特殊！";
                Story story12 = new Story();
                story12.AppearNeedStaminVals = 20;
                story12.IntroText = "我是一个特殊的小孩12";
                story12.DetailText = "今天碰到了一个特殊的小孩12，好特殊啊好特殊，好特殊啊好特殊好特殊啊好特殊！";
                Story story13 = new Story();
                story13.AppearNeedStaminVals = 30;
                story13.IntroText = "我是一个特殊的小孩13";
                story13.DetailText = "今天碰到了一个特殊的小孩13，好特殊啊好特殊，好特殊啊好特殊好特殊啊好特殊！";
                Story story21 = new Story();
                story21.AppearNeedStaminVals = 40;
                story21.IntroText = "我是一个特殊的小孩21";
                story21.DetailText = "今天碰到了一个特殊的小孩21，好特殊啊好特殊，好特殊啊好特殊好特殊啊好特殊！";
                Story story22 = new Story();
                story22.AppearNeedStaminVals = 50;
                story22.IntroText = "我是一个特殊的小孩22";
                story22.DetailText = "今天碰到了一个特殊的小孩22，好特殊啊好特殊，好特殊啊好特殊好特殊啊好特殊！";
                Story story23 = new Story();
                story23.AppearNeedStaminVals = 60;
                story23.IntroText = "我是一个特殊的小孩23";
                story23.DetailText = "今天碰到了一个特殊的小孩23，好特殊啊好特殊，好特殊啊好特殊好特殊啊好特殊！";
<<<<<<< HEAD
                //自定义初始数据
                GameManager.Instance().CurrentDay = 1;
                GameManager.Instance().StaminaVal = 150;
                GameManager.Instance().StaminaRate = 2;
                GameManager.Instance().MaxStaminaVal = 1000;
                GameManager.Instance().HotHeartVal = 0;
                GameManager.Instance().IntelligenceVal = 66;
                GameManager.Instance().Money = 10000;
	            GameManager.Instance().HighQualityProbability = 50;
	            GameManager.Instance().LaTiaoName = "萌石牌辣条";

				//定义生产失败概率
				List<int> t_prodFailProb = new List<int>();
	            t_prodFailProb.Add(100);
	            t_prodFailProb.Add(10);
	            t_prodFailProb.Add(5);
	            t_prodFailProb.Add(0);
	            GameManager.Instance().Data["ProduceFailProbability"] = t_prodFailProb;

				//临时List定义
				List<BaseClass.Material> t_materials = new List<BaseClass.Material>();
	            BaseClass.Material m;

	            List<Ingredient> t_ingredients = new List<Ingredient>();
	            List<Snack> t_snacks = new List<Snack>();
	            Snack s;
				List<Gift> t_gifts = new List<Gift>();
				Gift g;
				List<Drug> t_drugs = new List<Drug>();
	            Drug d;
				List<Cash> t_cashs = new List<Cash>();
	            Cash c;
				List<Map> t_maps = new List<Map>();
	            Map map;
				List<Machine> t_machines = new List<Machine>();
	            Machine machine;


				#region 原料店
				m = new BaseClass.Material();
	            m.ID = "DouPi";
	            m.Name = "豆皮";
	            m.Amount = 5;
	            m.Intro = "用于制作辣条";
	            m.Price = 11;
	            t_materials.Add(m);

	            GameManager.Instance().Data["Store_Materials"] = t_materials;
				#endregion

				#region 调料店

	            CreateIngredient(t_ingredients, "FanQieJiang", "番茄酱", "酸酸甜甜的用于制作辣条", 8, 22, 40, 30, 0, 0);
	            CreateIngredient(t_ingredients, "HuaJiaoFen", "花椒粉", "咸咸的", 4, 33, 0, 0, 30, 0);
	            CreateIngredient(t_ingredients, "JiangYou", "酱油", "用于制作辣条", 10, 44, 0, 0, 60, 0);
	            CreateIngredient(t_ingredients, "JieMo", "芥末", "用于制作辣条", 6, 55, 0, 0, 0, 50);
	            CreateIngredient(t_ingredients, "LaJiaoFen", "辣椒粉", "用于制作辣条", 7, 66, 0, 0, 0, 60);
	            CreateIngredient(t_ingredients, "LaoGanMa", "老干妈", "用于制作辣条", 9, 77, 0, 0, 20,30);
	            CreateIngredient(t_ingredients, "NingMengJiang", "柠檬酱", "用于制作辣条", 2, 88, 50, 0, 0, 0);
	            CreateIngredient(t_ingredients, "Tang", "糖", "用于制作辣条", 3, 99, 0, 60, 0, 0);
	            CreateIngredient(t_ingredients, "Yan", "盐", "用于制作辣条", 6, 111, 0, 0, 60, 0);

	            GameManager.Instance().Data["Store_Ingredients"] = t_ingredients;

				#endregion

				#region 小吃店
				s = new Snack();
	            s.ID = "BangBangTang";
	            s.Name = "棒棒糖";
	            s.Amount = 1;
	            s.Intro = "棒棒糖";
	            s.Price = 11;
				t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "BaoZi";
	            s.Name = "包子";
				s.Amount = 2;
	            s.Intro = "包子";
	            s.Price = 22;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "ChaYeDan";
	            s.Name = "茶叶蛋";
				s.Amount = 3;
	            s.Intro = "茶叶蛋";
	            s.Price = 33;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "DouJiang";
	            s.Name = "豆浆";
				s.Amount = 4;
	            s.Intro = "豆浆";
	            s.Price = 44;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "GuoZhi";
	            s.Name = "果汁";
				s.Amount = 5;
	            s.Intro = "果汁";
	            s.Price = 55;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "JiChi";
	            s.Name = "鸡翅";
				s.Amount = 6;
	            s.Intro = "鸡翅";
	            s.Price = 66;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "JiTui";
	            s.Name = "鸡腿";
				s.Amount = 7;
	            s.Intro = "鸡腿";
	            s.Price = 77;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "KaoLengMian";
	            s.Name = "烤冷面";
				s.Amount = 8;
	            s.Intro = "烤冷面";
	            s.Price = 88;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "MianBao";
	            s.Name = "面包";
				s.Amount = 9;
	            s.Intro = "面包";
	            s.Price = 99;
	            t_snacks.Add(s);
	            s = new Snack();
	            s.ID = "PaoPaoTang";
	            s.Name = "泡泡糖";
				s.Amount = 10;
	            s.Intro = "泡泡糖";
	            s.Price = 111;
	            t_snacks.Add(s);

	            GameManager.Instance().Data["Store_Snacks"] = t_snacks;

	            #endregion

	            #region 礼品店
				g = new Gift();
	            g.ID = "FeiBiao";
	            g.Name = "飞镖";
	            g.Amount = 1;
	            g.Intro = "飞镖";
	            g.Price = 11;
				t_gifts.Add(g);
	            g = new Gift();
	            g.ID = "NianNianQiu";
	            g.Name = "黏黏球";
	            g.Amount = 2;
	            g.Intro = "黏黏球";
	            g.Price = 22;
	            t_gifts.Add(g);
	            g = new Gift();
	            g.ID = "PaoPao";
	            g.Name = "泡泡";
				g.Amount = 3;
	            g.Intro = "泡泡";
	            g.Price = 33;
	            t_gifts.Add(g);
	            g = new Gift();
	            g.ID = "ShaBao";
	            g.Name = "沙包";
				g.Amount = 4;
	            g.Intro = "沙包";
	            g.Price = 44;
	            t_gifts.Add(g);
	            g = new Gift();
	            g.ID = "ShuiQiang";
	            g.Name = "水枪";
				g.Amount = 5;
	            g.Intro = "水枪";
	            g.Price = 55;
	            t_gifts.Add(g);
	            g = new Gift();
	            g.ID = "TuoLuo";
	            g.Name = "陀螺";
				g.Amount = 6;
	            g.Intro = "陀螺";
	            g.Price = 66;
	            t_gifts.Add(g);
	            g = new Gift();
	            g.ID = "YouYouQiu";
	            g.Name = "悠悠球";
				g.Amount = 7;
	            g.Intro = "悠悠球";
	            g.Price = 77;
	            t_gifts.Add(g);

	            GameManager.Instance().Data["Store_Gifts"] = t_gifts;

				#endregion

				#region 医院药品
	            d = new Drug();
	            d.ID = "BanLanGen";
	            d.Name = "板蓝根";
	            d.Amount = 1;
	            d.StaminaIncreaseVal = 10;
	            d.Intro = "功效： 体力值 +" + d.StaminaIncreaseVal;
	            d.Price = 123;
	            t_drugs.Add(d);
	            d = new Drug();
	            d.ID = "BoHe";
	            d.Name = "薄荷";
	            d.Amount = 2;
	            d.StaminaIncreaseVal = 5;
	            d.Intro = "功效： 体力值 +" + d.StaminaIncreaseVal;
	            d.Price = 60;
	            t_drugs.Add(d);

	            GameManager.Instance().Data["Drugs"] = t_drugs;

				#endregion

	            #region 银行数据
				c = new Cash();
	            c.ID = "YiYuan";
	            c.Name = "一元";
	            c.MoneyVal = 5000;
	            c.Price = 1;
				t_cashs.Add(c);
				c = new Cash();
	            c.ID = "WuYuan";
	            c.Name = "五元";
	            c.MoneyVal = 30000;
	            c.Price = 5;
				t_cashs.Add(c);

				GameManager.Instance().Data["Cashs"] = t_cashs;
				#endregion

	            #region 地图数据
			 map = new Map();
        map.ID = "GuLuMap";
=======
                
                //定义生产失败概率
                List<int> t_prodFailProb = new List<int>();
                t_prodFailProb.Add(100);
                t_prodFailProb.Add(10);
                t_prodFailProb.Add(5);
                t_prodFailProb.Add(0);
                GameManager.Instance().Data["ProduceFailProbability"] = t_prodFailProb;

                //临时List定义
                List<BaseClass.Material> t_materials = new List<BaseClass.Material>();
                BaseClass.Material m;

                List<Ingredient> t_ingredients = new List<Ingredient>();
                List<Snack> t_snacks = new List<Snack>();
                Snack s;
                List<Gift> t_gifts = new List<Gift>();
                Gift g;
                List<Drug> t_drugs = new List<Drug>();
                Drug d;
                List<Cash> t_cashs = new List<Cash>();
                Cash c;
                List<Map> t_maps = new List<Map>();
                Map map;
                List<Machine> t_machines = new List<Machine>();
                Machine machine;

                #region 原料店

                m = new BaseClass.Material();
                m.ID = "DouPi";
                m.Name = "豆皮";
                m.Amount = 5;
                m.Intro = "用于制作辣条";
                m.Price = 11;
                t_materials.Add(m);

                GameManager.Instance().Data["Store_Materials"] = t_materials;

                #endregion

                #region 调料店

                CreateIngredient(t_ingredients, "FanQieJiang", "番茄酱", "酸酸甜甜的用于制作辣条", 8, 22, 40, 30, 0, 0);
                CreateIngredient(t_ingredients, "HuaJiaoFen", "花椒粉", "咸咸的", 4, 33, 0, 0, 30, 0);
                CreateIngredient(t_ingredients, "JiangYou", "酱油", "用于制作辣条", 10, 44, 0, 0, 60, 0);
                CreateIngredient(t_ingredients, "JieMo", "芥末", "用于制作辣条", 6, 55, 0, 0, 0, 50);
                CreateIngredient(t_ingredients, "LaJiaoFen", "辣椒粉", "用于制作辣条", 7, 66, 0, 0, 0, 60);
                CreateIngredient(t_ingredients, "LaoGanMa", "老干妈", "用于制作辣条", 9, 77, 0, 0, 20, 30);
                CreateIngredient(t_ingredients, "NingMengJiang", "柠檬酱", "用于制作辣条", 2, 88, 50, 0, 0, 0);
                CreateIngredient(t_ingredients, "Tang", "糖", "用于制作辣条", 3, 99, 0, 60, 0, 0);
                CreateIngredient(t_ingredients, "Yan", "盐", "用于制作辣条", 6, 111, 0, 0, 60, 0);

                GameManager.Instance().Data["Store_Ingredients"] = t_ingredients;

                #endregion

                #region 小吃店

                s = new Snack();
                s.ID = "BangBangTang";
                s.Name = "棒棒糖";
                s.Amount = 1;
                s.Intro = "棒棒糖";
                s.Price = 11;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "BaoZi";
                s.Name = "包子";
                s.Amount = 2;
                s.Intro = "包子";
                s.Price = 22;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "ChaYeDan";
                s.Name = "茶叶蛋";
                s.Amount = 3;
                s.Intro = "茶叶蛋";
                s.Price = 33;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "DouJiang";
                s.Name = "豆浆";
                s.Amount = 4;
                s.Intro = "豆浆";
                s.Price = 44;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "GuoZhi";
                s.Name = "果汁";
                s.Amount = 5;
                s.Intro = "果汁";
                s.Price = 55;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "JiChi";
                s.Name = "鸡翅";
                s.Amount = 6;
                s.Intro = "鸡翅";
                s.Price = 66;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "JiTui";
                s.Name = "鸡腿";
                s.Amount = 7;
                s.Intro = "鸡腿";
                s.Price = 77;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "KaoLengMian";
                s.Name = "烤冷面";
                s.Amount = 8;
                s.Intro = "烤冷面";
                s.Price = 88;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "MianBao";
                s.Name = "面包";
                s.Amount = 9;
                s.Intro = "面包";
                s.Price = 99;
                t_snacks.Add(s);
                s = new Snack();
                s.ID = "PaoPaoTang";
                s.Name = "泡泡糖";
                s.Amount = 10;
                s.Intro = "泡泡糖";
                s.Price = 111;
                t_snacks.Add(s);

                GameManager.Instance().Data["Store_Snacks"] = t_snacks;

                #endregion

                #region 礼品店

                g = new Gift();
                g.ID = "FeiBiao";
                g.Name = "飞镖";
                g.Amount = 1;
                g.Intro = "飞镖";
                g.Price = 11;
                t_gifts.Add(g);
                g = new Gift();
                g.ID = "NianNianQiu";
                g.Name = "黏黏球";
                g.Amount = 2;
                g.Intro = "黏黏球";
                g.Price = 22;
                t_gifts.Add(g);
                g = new Gift();
                g.ID = "PaoPao";
                g.Name = "泡泡";
                g.Amount = 3;
                g.Intro = "泡泡";
                g.Price = 33;
                t_gifts.Add(g);
                g = new Gift();
                g.ID = "ShaBao";
                g.Name = "沙包";
                g.Amount = 4;
                g.Intro = "沙包";
                g.Price = 44;
                t_gifts.Add(g);
                g = new Gift();
                g.ID = "ShuiQiang";
                g.Name = "水枪";
                g.Amount = 5;
                g.Intro = "水枪";
                g.Price = 55;
                t_gifts.Add(g);
                g = new Gift();
                g.ID = "TuoLuo";
                g.Name = "陀螺";
                g.Amount = 6;
                g.Intro = "陀螺";
                g.Price = 66;
                t_gifts.Add(g);
                g = new Gift();
                g.ID = "YouYouQiu";
                g.Name = "悠悠球";
                g.Amount = 7;
                g.Intro = "悠悠球";
                g.Price = 77;
                t_gifts.Add(g);

                GameManager.Instance().Data["Store_Gifts"] = t_gifts;

                #endregion

                #region 医院药品

                d = new Drug();
                d.ID = "BanLanGen";
                d.Name = "板蓝根";
                d.Amount = 1;
                d.StaminaIncreaseVal = 10;
                d.Intro = "功效： 体力值 +" + d.StaminaIncreaseVal;
                d.Price = 123;
                t_drugs.Add(d);
                d = new Drug();
                d.ID = "BoHe";
                d.Name = "薄荷";
                d.Amount = 2;
                d.StaminaIncreaseVal = 5;
                d.Intro = "功效： 体力值 +" + d.StaminaIncreaseVal;
                d.Price = 60;
                t_drugs.Add(d);

                GameManager.Instance().Data["Drugs"] = t_drugs;

                #endregion

                #region 银行数据

                c = new Cash();
                c.ID = "YiYuan";
                c.Name = "一元";
                c.MoneyVal = 5000;
                c.Price = 1;
                t_cashs.Add(c);
                c = new Cash();
                c.ID = "WuYuan";
                c.Name = "五元";
                c.MoneyVal = 30000;
                c.Price = 5;
                t_cashs.Add(c);

                GameManager.Instance().Data["Cashs"] = t_cashs;

                #endregion

                #region 地图数据

                map = new Map();
                map.ID = "GuLuMap";
>>>>>>> 5f4cbf517a106a26aaeac4220be9f20e4f55683c
                map.IsLocked = false;
                map.UnlockMoney = 5000;
                map.StoryList.Add(story11);
                map.StoryList.Add(story12);
                map.StoryList.Add(story13);
                t_maps.Add(map);
                map = new Map();
                map.ID = "MuseumMap";
                map.IsLocked = true;
                map.UnlockMoney = 8000;
                map.StoryList.Add(story21);
                map.StoryList.Add(story22);
                map.StoryList.Add(story23);
                t_maps.Add(map);
                map = new Map();
                map.ID = "PrimaryMap";
                map.IsLocked = true;
                map.UnlockMoney = 10000;
                map.StoryList.Add(story11);
                map.StoryList.Add(story12);
                map.StoryList.Add(story13);
                t_maps.Add(map);
                map = new Map();
                map.ID = "MiddleMap";
                map.IsLocked = true;
                map.UnlockMoney = 12000;
                map.StoryList.Add(story11);
                map.StoryList.Add(story12);
                map.StoryList.Add(story13);
                t_maps.Add(map);
                map = new Map();
                map.ID = "StreetMap";
                map.IsLocked = true;
                map.UnlockMoney = 15000;
                map.StoryList.Add(story11);
                map.StoryList.Add(story12);
                map.StoryList.Add(story13);
                t_maps.Add(map);

                GameManager.Instance().Data["Maps"] = t_maps;

                #endregion

                #region 生产机器数据

                machine = new Machine();
                machine.IsLocked = false;
                machine.IsCompleted = false;
                machine.UnlockMoney = 0;
                machine.Product = null;
                t_machines.Add(machine);
                machine = new Machine();
                machine.IsLocked = true;
                machine.IsCompleted = false;
                machine.UnlockMoney = 15000;
                machine.Product = null;
                t_machines.Add(machine);
                machine = new Machine();
                machine.IsLocked = true;
                machine.IsCompleted = false;
                machine.Product = null;
                machine.UnlockMoney = 20000;
                t_machines.Add(machine);

                GameManager.Instance().Data["Machines"] = t_machines;

                #endregion

                #endregion

                GameManager.Instance().Data.Save();
            }
        }
    }
}