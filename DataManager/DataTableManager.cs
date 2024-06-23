using UnityEngine;
using System.Collections.Generic;

namespace Tables
{

	public partial class Ability
	{
		/// <summary> 어빌리티 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 능력치 이름 키값 </summary>
		[Newtonsoft.Json.JsonProperty] public string		AbilityName {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Ability> data = new Dictionary<int, Ability>();
		public static Ability Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Ability Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Character
	{
		/// <summary>  </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 공격력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Attack {get; private set;}
		/// <summary> 체력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			HealthPoint {get; private set;}
		/// <summary> 체력 회복 (초) </summary>
		[Newtonsoft.Json.JsonProperty] public float		HealthPointRegen {get; private set;}
		/// <summary> 방어력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			DefencePoint {get; private set;}
		/// <summary> 공격 속도 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AttackSpeed {get; private set;}
		/// <summary> 공격 거리 </summary>
		[Newtonsoft.Json.JsonProperty] public int			AttackRange {get; private set;}
		/// <summary> 이동 속도 </summary>
		[Newtonsoft.Json.JsonProperty] public float		MoveSpeed {get; private set;}
		/// <summary> 회피 </summary>
		[Newtonsoft.Json.JsonProperty] public float		Dodge {get; private set;}
		/// <summary> 명중 </summary>
		[Newtonsoft.Json.JsonProperty] public float		Hit {get; private set;}
		/// <summary> 치명타 피해량 </summary>
		[Newtonsoft.Json.JsonProperty] public float		CriticalDamage {get; private set;}
		/// <summary> 치명타 확률 </summary>
		[Newtonsoft.Json.JsonProperty] public float		CriticalRate {get; private set;}
		/// <summary> 프리펩 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Prefab {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Character> data = new Dictionary<int, Character>();
		public static Character Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Character Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Define
	{
		/// <summary> 디파인 키 </summary>
		[Newtonsoft.Json.JsonProperty] public string		key {get; private set;}
		/// <summary> 값 </summary>
		[Newtonsoft.Json.JsonProperty] public float		value {get; private set;}

		// 메인 저장소
		public static Dictionary<string, Define> data = new Dictionary<string, Define>();
		public static Define Get(string key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Define Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Dungeon
	{
		/// <summary> 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 던전 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Type {get; private set;}
		/// <summary> 층 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Floor {get; private set;}
		/// <summary> 던전 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		DungeonName {get; private set;}
		/// <summary> 던전 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		DungeonDiscription {get; private set;}
		/// <summary> 스테이지 프리팹 </summary>
		[Newtonsoft.Json.JsonProperty] public string		StagePrefabs {get; private set;}
		/// <summary> 권장 전투력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			RecommendPower {get; private set;}
		/// <summary> 몬스터 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MonsterLv {get; private set;}
		/// <summary> 몬스터 레벨 증가량 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MonsterLvIncrease {get; private set;}
		/// <summary> 제한 시간 </summary>
		[Newtonsoft.Json.JsonProperty] public int			TimeLimit {get; private set;}
		/// <summary> 클리어 시간 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] ClearTime {get; private set;}
		/// <summary> 조기 종료 </summary>
		[Newtonsoft.Json.JsonProperty] public int			PerfectClear {get; private set;}
		/// <summary> 클리어 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] ClearReward {get; private set;}
		/// <summary> 보스 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BossIndex {get; private set;}
		/// <summary> 보스 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BossLv {get; private set;}
		/// <summary> 스폰 그룹 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] SpawnGroup {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Dungeon> data = new Dictionary<int, Dungeon>();
		public static Dungeon Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Dungeon Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class EnhancementData
	{
		/// <summary> 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 요구 재화 수량 </summary>
		[Newtonsoft.Json.JsonProperty] public string		inGamePriceKey {get; private set;}
		/// <summary> 성공 확률 </summary>
		[Newtonsoft.Json.JsonProperty] public float		probabilitySuccess {get; private set;}
		/// <summary> 공격력 </summary>
		[Newtonsoft.Json.JsonProperty] public double		atk {get; private set;}
		/// <summary> 방어력 </summary>
		[Newtonsoft.Json.JsonProperty] public double		def {get; private set;}
		/// <summary> 최대체력 </summary>
		[Newtonsoft.Json.JsonProperty] public double		maxHp {get; private set;}
		/// <summary> 체력회복 </summary>
		[Newtonsoft.Json.JsonProperty] public double		hpgen {get; private set;}
		/// <summary> 공격속도 </summary>
		[Newtonsoft.Json.JsonProperty] public double		attackspeed {get; private set;}
		/// <summary> 치명타 피해 </summary>
		[Newtonsoft.Json.JsonProperty] public double		CriticalDamagePoint {get; private set;}
		/// <summary> 치명타 확률 </summary>
		[Newtonsoft.Json.JsonProperty] public double		CriticalChancePoint {get; private set;}
		/// <summary> 명중 </summary>
		[Newtonsoft.Json.JsonProperty] public double		HitPoint {get; private set;}
		/// <summary> 회피 </summary>
		[Newtonsoft.Json.JsonProperty] public double		DodgePoint {get; private set;}

		// 메인 저장소
		public static Dictionary<int, EnhancementData> data = new Dictionary<int, EnhancementData>();
		public static EnhancementData Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in EnhancementData Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Goods
	{
		/// <summary> 재화 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 재화 목록 표시 유무 0 : 미표시 1 : 표시 </summary>
		[Newtonsoft.Json.JsonProperty] public bool		DisplayType {get; private set;}
		/// <summary> 재화 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		GoodsName {get; private set;}
		/// <summary> 재화 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		GoodsIcon {get; private set;}
		/// <summary> 재화 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		GoodsDescription {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Goods> data = new Dictionary<int, Goods>();
		public static Goods Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Goods Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class InGamePrice
	{
		/// <summary> 키 </summary>
		[Newtonsoft.Json.JsonProperty] public string		key {get; private set;}
		/// <summary> 요구 재화 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			demandGoodsType {get; private set;}
		/// <summary> 요구 수량 </summary>
		[Newtonsoft.Json.JsonProperty] public uint		demandGoodsQty {get; private set;}

		// 메인 저장소
		public static Dictionary<string, InGamePrice> data = new Dictionary<string, InGamePrice>();
		public static InGamePrice Get(string key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in InGamePrice Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Item
	{
		/// <summary> 장비 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 착용 직업 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Job {get; private set;}
		/// <summary> 퀄리티 등급 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Quality_Grade {get; private set;}
		/// <summary> 아이템 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			ItemType {get; private set;}
		/// <summary> 소환 등장 여부 </summary>
		[Newtonsoft.Json.JsonProperty] public int			isSummon {get; private set;}
		/// <summary> 능력치 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] Ability {get; private set;}
		/// <summary> 능력치 값 </summary>
		[Newtonsoft.Json.JsonProperty] public double		[] AbilityValue {get; private set;}
		/// <summary> 강화 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Enhancement {get; private set;}
		/// <summary> 보유 효과 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] PassiveEffect {get; private set;}
		/// <summary> 보유효과 값 </summary>
		[Newtonsoft.Json.JsonProperty] public double		[] PassiveEffectValue {get; private set;}
		/// <summary> 장비 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		ItemName {get; private set;}
		/// <summary> 장비 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		ItemIcon {get; private set;}
		/// <summary> 장비 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		ItemDescription {get; private set;}
		/// <summary> 아이템 등급 </summary>
		[Newtonsoft.Json.JsonProperty] public int			ItemGrade {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Item> data = new Dictionary<int, Item>();
		public static Item Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Item Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Job
	{
		/// <summary> 성장 타입 0 : 광전사 1 : 용병(궁수) 2 : 용병(마법사) 3 : 용병(힐러) 4 : 펫 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 레벨업 시 공격력 증가 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AttackIncrease {get; private set;}
		/// <summary> 레벨업 시 체력 증가 </summary>
		[Newtonsoft.Json.JsonProperty] public float		HealthPointIncrease {get; private set;}
		/// <summary> 레벨업 시 방어력 증가 </summary>
		[Newtonsoft.Json.JsonProperty] public float		DefencePointIncrease {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Job> data = new Dictionary<int, Job>();
		public static Job Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Job Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Material
	{
		/// <summary> 재료 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 재료 아이템 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		MaterialName {get; private set;}
		/// <summary> 재료 아이템 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		MaterialIcon {get; private set;}
		/// <summary> 재료 아이템 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		MaterialDescription {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Material> data = new Dictionary<int, Material>();
		public static Material Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Material Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Monster
	{
		/// <summary> 몬스터 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 몬스터 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Monster_Name {get; private set;}
		/// <summary> 몬스터 이미지 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Monster_Img {get; private set;}
		/// <summary> 프리팹 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Prefabs {get; private set;}
		/// <summary> 사망 이펙트 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Death_Effect {get; private set;}
		/// <summary> 인게임 크기 </summary>
		[Newtonsoft.Json.JsonProperty] public float		Scale {get; private set;}
		/// <summary> 공격타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			AttackType {get; private set;}
		/// <summary> 공격력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Attack {get; private set;}
		/// <summary> 체력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			HealthPoint {get; private set;}
		/// <summary> 체력 회복 (초) </summary>
		[Newtonsoft.Json.JsonProperty] public float		HealthPointRegen {get; private set;}
		/// <summary> 방어력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			DefencePoint {get; private set;}
		/// <summary> 공격 속도 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AttackSpeed {get; private set;}
		/// <summary> 공격 거리 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AttackRange {get; private set;}
		/// <summary> 이동 속도 </summary>
		[Newtonsoft.Json.JsonProperty] public float		MoveSpeed {get; private set;}
		/// <summary> 회피 </summary>
		[Newtonsoft.Json.JsonProperty] public float		Dodge {get; private set;}
		/// <summary> 명중 </summary>
		[Newtonsoft.Json.JsonProperty] public float		Hit {get; private set;}
		/// <summary> 치명타 피해량 </summary>
		[Newtonsoft.Json.JsonProperty] public int			CriticalDamage {get; private set;}
		/// <summary> 치명타 확률 </summary>
		[Newtonsoft.Json.JsonProperty] public float		CriticalRate {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Monster> data = new Dictionary<int, Monster>();
		public static Monster Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Monster Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Quest
	{
		/// <summary> 임무 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 임무 그룹 분류 </summary>
		[Newtonsoft.Json.JsonProperty] public int			QuestGroupType {get; private set;}
		/// <summary> 임무 타입  </summary>
		[Newtonsoft.Json.JsonProperty] public int			QuestType {get; private set;}
		/// <summary> 반복 </summary>
		[Newtonsoft.Json.JsonProperty] public bool		Loop {get; private set;}
		/// <summary> 달성 요구치 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Value {get; private set;}
		/// <summary> 임무 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		QuestName {get; private set;}
		/// <summary> 임무 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		QuestDescription {get; private set;}
		/// <summary> 완료시 지급 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public string		QuestReward {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Quest> data = new Dictionary<int, Quest>();
		public static Quest Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Quest Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Reward
	{
		/// <summary> 리워드 키 </summary>
		[Newtonsoft.Json.JsonProperty] public string		key {get; private set;}
		/// <summary> Goods보상 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] GoodsKey {get; private set;}
		/// <summary> Goods수량 </summary>
		[Newtonsoft.Json.JsonProperty] public double		[] GoodsQty {get; private set;}
		/// <summary> Material보상 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] MaterialKey {get; private set;}
		/// <summary> Material수량 </summary>
		[Newtonsoft.Json.JsonProperty] public double		[] MaterialQty {get; private set;}
		/// <summary> 아이템 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] ItemKey {get; private set;}
		/// <summary> 아이템 수량 </summary>
		[Newtonsoft.Json.JsonProperty] public double		[] ItemQty {get; private set;}

		// 메인 저장소
		public static Dictionary<string, Reward> data = new Dictionary<string, Reward>();
		public static Reward Get(string key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Reward Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Skill
	{
		/// <summary> 스킬 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 스킬 목록 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillListIcon {get; private set;}
		/// <summary> 스킬 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillIcon {get; private set;}
		/// <summary> 스킬 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillName {get; private set;}
		/// <summary> 스킬 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillDescription {get; private set;}
		/// <summary> 스킬 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillType {get; private set;}
		/// <summary> 스킬  </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillWidth {get; private set;}
		/// <summary> 스킬 거리 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillRange {get; private set;}
		/// <summary> 스킬 범위 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillRadius {get; private set;}
		/// <summary> 쿨타임 </summary>
		[Newtonsoft.Json.JsonProperty] public int			CoolTime {get; private set;}
		/// <summary> 데미지 계수 </summary>
		[Newtonsoft.Json.JsonProperty] public float		DamageCoefficient {get; private set;}
		/// <summary> 강화 시 증가 계수 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AddDamageCoefficient {get; private set;}
		/// <summary> 캐스팅 FX </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] ActionFx {get; private set;}
		/// <summary> 스킬 애니 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillAnimation {get; private set;}
		/// <summary> 사운드 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Sound {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Skill> data = new Dictionary<int, Skill>();
		public static Skill Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Skill Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Spawn
	{
		/// <summary> 스폰 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 몬스터 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] MonsterIndex {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Spawn> data = new Dictionary<int, Spawn>();
		public static Spawn Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Spawn Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Stage
	{
		/// <summary> 스테이지 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 지역 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Dfficulty {get; private set;}
		/// <summary> 챕터 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Chapter {get; private set;}
		/// <summary> 스테이지 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Zone {get; private set;}
		/// <summary> 스테이지 권장 공격력 </summary>
		[Newtonsoft.Json.JsonProperty] public double		Stage_Recommend_Atk {get; private set;}
		/// <summary> 스테이지 권장 방어력 </summary>
		[Newtonsoft.Json.JsonProperty] public double		Stage_Recommend_Def {get; private set;}
		/// <summary> 웨이브 개수 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SpawnCount {get; private set;}
		/// <summary> 보스 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BossIndex {get; private set;}
		/// <summary> 보스 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BossLv {get; private set;}
		/// <summary> 스폰몬스터키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SpawnGroup {get; private set;}
		/// <summary> 몬스터 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MonsterLv {get; private set;}
		/// <summary> 스테이지 방치 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public string		StageIdleReward {get; private set;}
		/// <summary> 스테이지 클리어 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public string		StageClearReward {get; private set;}
		/// <summary> 스테이지 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		StageName {get; private set;}
		/// <summary> 스테이지 프리팹 </summary>
		[Newtonsoft.Json.JsonProperty] public string		StagePrefabs {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Stage> data = new Dictionary<int, Stage>();
		public static Stage Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Stage Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class StatReinforce
	{
		/// <summary> 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 텍스트 </summary>
		[Newtonsoft.Json.JsonProperty] public string		NameText {get; private set;}
		/// <summary>  </summary>
		[Newtonsoft.Json.JsonProperty] public int			ReinforceType {get; private set;}
		/// <summary> 적용 대상 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Target {get; private set;}
		/// <summary> 스탯 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			StatType {get; private set;}
		/// <summary> 상승 값 </summary>
		[Newtonsoft.Json.JsonProperty] public float		StatValue {get; private set;}
		/// <summary> 최대 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MaxLv {get; private set;}
		/// <summary> 소모재화타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			PriceType {get; private set;}
		/// <summary> 소모 재화 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Price {get; private set;}
		/// <summary> 참조 아이콘 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Icon {get; private set;}

		// 메인 저장소
		public static Dictionary<int, StatReinforce> data = new Dictionary<int, StatReinforce>();
		public static StatReinforce Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in StatReinforce Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Summon
	{
		/// <summary> 뽑기 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 뽑기 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Type {get; private set;}
		/// <summary> 뽑기 확률 </summary>
		[Newtonsoft.Json.JsonProperty] public float		[] ItemRate {get; private set;}
		/// <summary> 픽업 아이템 등급 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] PickupItemRank {get; private set;}
		/// <summary> 픽업 아이템키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] PickupItemKey {get; private set;}
		/// <summary> 픽업 확률 </summary>
		[Newtonsoft.Json.JsonProperty] public float		[] PickupItemRate {get; private set;}
		/// <summary> 뽑기 재화 </summary>
		[Newtonsoft.Json.JsonProperty] public int			GachaCostType {get; private set;}
		/// <summary> 1회 뽑기 가격 </summary>
		[Newtonsoft.Json.JsonProperty] public int			OneGachaCost {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Summon> data = new Dictionary<int, Summon>();
		public static Summon Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Summon Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class TextKey
	{
		/// <summary> 키 </summary>
		[Newtonsoft.Json.JsonProperty] public string		key {get; private set;}
		/// <summary> 설명(한국어) </summary>
		[Newtonsoft.Json.JsonProperty] public string		Description {get; private set;}

		// 메인 저장소
		public static Dictionary<string, TextKey> data = new Dictionary<string, TextKey>();
		public static TextKey Get(string key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in TextKey Table Key : {0}",key);
				return null;
			}
		}
	}

	public partial class Ticket
	{
		/// <summary> 티켓 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 티켓타입 </summary>
		[Newtonsoft.Json.JsonProperty] public int			TicketType {get; private set;}
		/// <summary> 티켓 표시 유무 </summary>
		[Newtonsoft.Json.JsonProperty] public uint		DisplayType {get; private set;}
		/// <summary> 티켓 아이템 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		TicketName {get; private set;}
		/// <summary> 티켓 아이템 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		TicketDescription {get; private set;}
		/// <summary> 티켓 아이템 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		TicketIcon {get; private set;}
		/// <summary> 티켓 구매 가격 </summary>
		[Newtonsoft.Json.JsonProperty] public int			TicketPrice {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Ticket> data = new Dictionary<int, Ticket>();
		public static Ticket Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarningFormat("This Key doesn't exist in Ticket Table Key : {0}",key);
				return null;
			}
		}
	}

}
