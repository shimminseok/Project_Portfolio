﻿using UnityEngine;
using System.Collections.Generic;

namespace Tables
{

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
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Character Table");
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
		/// <summary> 크리스탈 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			CrystalKey {get; private set;}
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
		/// <summary> 실패 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] FailReward {get; private set;}
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
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Dungeon Table");
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
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Goods Table");
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
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Job Table");
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
		/// <summary> 임무 반복 여부 </summary>
		[Newtonsoft.Json.JsonProperty] public int			loop {get; private set;}
		/// <summary> 달성 요구치 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Value {get; private set;}
		/// <summary> 달성 시 증가 요구치 </summary>
		[Newtonsoft.Json.JsonProperty] public int			NextValue {get; private set;}
		/// <summary> 달성 요구 종료치  </summary>
		[Newtonsoft.Json.JsonProperty] public int			Value_End {get; private set;}
		/// <summary> 임무 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		QuestName {get; private set;}
		/// <summary> 임무 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		QuestDescription {get; private set;}
		/// <summary> 완료시 지급 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] QuestReward {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Quest> data = new Dictionary<int, Quest>();
		public static Quest Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Quest Table");
				return null;
			}
		}
	}

	public partial class Skill
	{
		/// <summary> 스킬 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			key {get; private set;}
		/// <summary> 사용 캐릭터 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillUse {get; private set;}
		/// <summary> 스킬 등급 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillTier {get; private set;}
		/// <summary> 스킬 목록 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillListIcon {get; private set;}
		/// <summary> 스킬 아이콘 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillIcon {get; private set;}
		/// <summary> 스킬 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillName {get; private set;}
		/// <summary> 스킬 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		SkillDescription {get; private set;}
		/// <summary> 스킬 계열 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Skillset {get; private set;}
		/// <summary> 스킬 계열 등급 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillsetGrid {get; private set;}
		/// <summary> 스킬 속성 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillProperties {get; private set;}
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
		/// <summary> 사용 마나 </summary>
		[Newtonsoft.Json.JsonProperty] public int			UseMana {get; private set;}
		/// <summary> 데미지 계수 </summary>
		[Newtonsoft.Json.JsonProperty] public float		DamageCoefficient {get; private set;}
		/// <summary> 강화 시 증가 계수 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AddDamageCoefficient {get; private set;}
		/// <summary> 각성 시 증가 계수 </summary>
		[Newtonsoft.Json.JsonProperty] public float		AwakeningCoefficient {get; private set;}
		/// <summary> 각성 효과 설명 </summary>
		[Newtonsoft.Json.JsonProperty] public int			AwakeningDescription {get; private set;}
		/// <summary> 보유 효과 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Retenion_Effect_Index {get; private set;}
		/// <summary> 버프 키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BuffKey {get; private set;}
		/// <summary> 상태이상 발동 시, 인게임 텍스트 </summary>
		[Newtonsoft.Json.JsonProperty] public string		Buff_Effect_Text {get; private set;}
		/// <summary> 캐스팅 FX </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] ActionFx {get; private set;}
		/// <summary> 스킬 애니 </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] SkillAnimation {get; private set;}
		/// <summary> 투사체 FX </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] TrailFx {get; private set;}
		/// <summary> 타겟 FX </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] TargetFx {get; private set;}
		/// <summary> 히트 Fx </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] HitFx {get; private set;}
		/// <summary>  </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] CastingFx {get; private set;}
		/// <summary> 사운드 </summary>
		[Newtonsoft.Json.JsonProperty] public string		[] Sound {get; private set;}
		/// <summary> 캐스팅 타입 </summary>
		[Newtonsoft.Json.JsonProperty] public bool		CastType {get; private set;}
		/// <summary> 캐스팅 시간 </summary>
		[Newtonsoft.Json.JsonProperty] public int			CastingTime {get; private set;}
		/// <summary> 컬렉션 길잡이 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SkillCollectionGuide {get; private set;}

		// 메인 저장소
		public static Dictionary<int, Skill> data = new Dictionary<int, Skill>();
		public static Skill Get(int key)
		{
			if (data.ContainsKey(key))
				return data[key];
			else
			{
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Skill Table");
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
		/// <summary> 스테이지 권장 전투력 </summary>
		[Newtonsoft.Json.JsonProperty] public int			Stage_Battle_Difficulty {get; private set;}
		/// <summary> 웨이브 개수 </summary>
		[Newtonsoft.Json.JsonProperty] public int			SpawnCount {get; private set;}
		/// <summary> 보스 인덱스 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BossIndex {get; private set;}
		/// <summary> 보스 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			BossLv {get; private set;}
		/// <summary> 스폰몬스터키 </summary>
		[Newtonsoft.Json.JsonProperty] public int			[] SpawnGroup {get; private set;}
		/// <summary> 몬스터 레벨 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MonsterLv {get; private set;}
		/// <summary> 일반 몬스터 골드 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MonsterGold {get; private set;}
		/// <summary> 일반 몬스터 경험치 </summary>
		[Newtonsoft.Json.JsonProperty] public int			MonsterExp {get; private set;}
		/// <summary> 스테이지 클리어 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public int			StageClearReward {get; private set;}
		/// <summary> 스테이지 최초 클리어 보상 </summary>
		[Newtonsoft.Json.JsonProperty] public int			StageFirstClearReward {get; private set;}
		/// <summary> 챕터 이름 </summary>
		[Newtonsoft.Json.JsonProperty] public string		ChapterName {get; private set;}
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
				UnityEngine.Debug.LogWarning("This Key doesn't exist in Stage Table");
				return null;
			}
		}
	}

}