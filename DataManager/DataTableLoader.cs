﻿using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class DataTableLoader
{
	public static bool Loaded { get; set; } = false;
	/// <summary>
	/// tables 을 로딩한다.
	/// </summary
	public static void Load()
	{
#if UNITY_EDITOR
		List<TextAsset> txts = new List<TextAsset>();
		DirectoryInfo di = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources/GameData/"));
		FileInfo[] fileInfo = di.GetFiles();
		for (int i = 0; i < fileInfo.Length; i++)
		{
			if (fileInfo[i].Extension.CompareTo(".json") != 0)
				continue;
			TextAsset txt = new TextAsset(fileInfo[i].OpenText().ReadToEnd());
			txt.name = fileInfo[i].Name.Replace(".json", "");
			txts.Add(txt);
		}
#else
		TextAsset[] txts = Resources.LoadAll<TextAsset>("GameData/");
#endif
		foreach (TextAsset e in txts)
			FromJsonConvert(e);
		Loaded = true;
	}
	[Serializable]
	private class Wrapper<T>
	{
		public T[] Items = null;
	}
	public static T[] FromJson<T>(string json)
	{
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}
	public static void FromJsonConvert(TextAsset txt)
	{
		switch (txt.name)
		{
			case "Ability":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Ability.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Ability>());
					Debug.Log("Ability is loaded");
				}
				break;
			case "Character":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Character.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Character>());
					Debug.Log("Character is loaded");
				}
				break;
			case "Define":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Define.data = json.GetDeserializedObject("value", new Dictionary<string, Tables.Define>());
					Debug.Log("Define is loaded");
				}
				break;
			case "Dungeon":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Dungeon.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Dungeon>());
					Debug.Log("Dungeon is loaded");
				}
				break;
			case "EnhancementData":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.EnhancementData.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.EnhancementData>());
					Debug.Log("EnhancementData is loaded");
				}
				break;
			case "Goods":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Goods.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Goods>());
					Debug.Log("Goods is loaded");
				}
				break;
			case "InGamePrice":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.InGamePrice.data = json.GetDeserializedObject("value", new Dictionary<string, Tables.InGamePrice>());
					Debug.Log("InGamePrice is loaded");
				}
				break;
			case "Item":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Item.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Item>());
					Debug.Log("Item is loaded");
				}
				break;
			case "Job":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Job.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Job>());
					Debug.Log("Job is loaded");
				}
				break;
			case "Material":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Material.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Material>());
					Debug.Log("Material is loaded");
				}
				break;
			case "Monster":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Monster.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Monster>());
					Debug.Log("Monster is loaded");
				}
				break;
			case "Quest":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Quest.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Quest>());
					Debug.Log("Quest is loaded");
				}
				break;
			case "Reward":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Reward.data = json.GetDeserializedObject("value", new Dictionary<string, Tables.Reward>());
					Debug.Log("Reward is loaded");
				}
				break;
			case "Skill":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Skill.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Skill>());
					Debug.Log("Skill is loaded");
				}
				break;
			case "Spawn":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Spawn.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Spawn>());
					Debug.Log("Spawn is loaded");
				}
				break;
			case "Stage":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Stage.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Stage>());
					Debug.Log("Stage is loaded");
				}
				break;
			case "StatReinforce":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.StatReinforce.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.StatReinforce>());
					Debug.Log("StatReinforce is loaded");
				}
				break;
			case "Summon":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Summon.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Summon>());
					Debug.Log("Summon is loaded");
				}
				break;
			case "TextKey":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.TextKey.data = json.GetDeserializedObject("value", new Dictionary<string, Tables.TextKey>());
					Debug.Log("TextKey is loaded");
				}
				break;
			case "Ticket":
				{
					JObject json = JObject.Parse(txt.text);
					Tables.Ticket.data = json.GetDeserializedObject("value", new Dictionary<int, Tables.Ticket>());
					Debug.Log("Ticket is loaded");
				}
				break;
			default:
				Debug.Log(string.Concat("Not Fount equal txt.name Data : ", txt.name));
				break;
		}
	}
}
