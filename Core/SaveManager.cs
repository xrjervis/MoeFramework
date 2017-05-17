using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MoeFramework {
	public class SaveManager{
		#region PublicStaticReadonlyFields

		public static readonly string extension = ".uml";

		#endregion

		#region PublicFields

		public string _saveDataFileName = "saveData";


		public string[] serializedTypes;


		public DataContainer[] serializedData;

		#endregion

		#region PublicParameters


		public System.Object this[string key] {
			get { return _data[key]; }
			set {
				if (typeof(Component).IsAssignableFrom(value.GetType())) throw new System.InvalidOperationException("Cannot serialize classes derived from Component!");
				_data[key] = value;
			}
		}

		#endregion

		#region PrivateFields


		private Dictionary<string, System.Object> _data = new Dictionary<string, object>();

		#endregion

		#region Constructors


		public SaveManager() { }

		public SaveManager(string fileName) {
			this._saveDataFileName = fileName;
		}

		#endregion

		#region PublicStaticFunctions

		public static SaveManager LoadFromStreamingAssets(string fileName) {
			return Load(Application.persistentDataPath + "\\" + fileName);
		}

		public static SaveManager Load(string path) {
			if (File.Exists(path) && Path.GetExtension(path) == extension) {
				List<System.Type> additionalTypes = new List<System.Type>();
				XmlDocument document = new XmlDocument();
				document.Load(path);
				XmlNode objectNode = document.ChildNodes[1];

				foreach (XmlNode node in objectNode["serializedTypes"].ChildNodes) {
					additionalTypes.Add(System.Type.GetType(node.InnerXml));
				}

				XmlSerializer serializer = new XmlSerializer(typeof(SaveManager), additionalTypes.ToArray());
				TextReader textReader = new StreamReader(path);
				SaveManager instance = (SaveManager)serializer.Deserialize(textReader);
				textReader.Close();

				foreach (DataContainer container in instance.serializedData) {
					instance[container.key] = container.value;
				}

				instance.serializedData = null;

				return instance;
			} else throw new System.InvalidOperationException("File does not exist!");
		}

		#endregion

		#region PublicFunctions

		public bool HasKey(string key) {
			return _data.ContainsKey(key);
		}

		public T GetValue<T>(string key) {
			return (T)_data[key];
		}

		public bool TryGetValue(string key, out System.Object result) {
			return _data.TryGetValue(key, out result);
		}

		public bool TryGetValue<T>(string key, out T result) {
			System.Object resultOut;

			if (_data.TryGetValue(key, out resultOut) && resultOut.GetType() == typeof(T)) {
				result = (T)resultOut;
				return true;
			} else {
				result = default(T);
				return false;
			}
		}

		public void Save() { Save(Application.persistentDataPath + "\\" + _saveDataFileName + extension); }

		public void Save(string path) {
			List<System.Type> additionalTypes = new List<System.Type>();
			List<string> typeNameList = new List<string>();
			List<DataContainer> dataList = new List<DataContainer>();

			System.Object result;
			System.Type resultType;

			foreach (string key in _data.Keys) {
				result = _data[key];
				resultType = result.GetType();

				if (!resultType.IsPrimitive && !additionalTypes.Contains(resultType)) {
					additionalTypes.Add(resultType);
					typeNameList.Add(resultType.AssemblyQualifiedName);
				}

				dataList.Add(new DataContainer(key, result));
			}

			serializedData = dataList.ToArray();
			serializedTypes = typeNameList.ToArray();

			XmlSerializer serializer = new XmlSerializer(typeof(SaveManager), additionalTypes.ToArray());
			TextWriter textWriter = new StreamWriter(path);
			serializer.Serialize(textWriter, this);
			textWriter.Close();

			serializedData = null;
			serializedTypes = null;
		}

		
		#endregion

		#region Utility

		public class DataContainer {
			public string key;
			public System.Object value;

			public DataContainer() { }
			public DataContainer(string key, System.Object value) {
				this.key = key;
				this.value = value;
			}
		}

		#endregion
	}
}


