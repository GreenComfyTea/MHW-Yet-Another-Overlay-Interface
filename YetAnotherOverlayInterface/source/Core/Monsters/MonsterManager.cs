using SharpPluginLoader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class MonsterManager
{
	private static readonly Lazy<MonsterManager> _lazy = new(() => new MonsterManager());
	public static MonsterManager Instance => _lazy.Value;

	public void Initialize() {
		GetAllMonsters();
	}

	public void OnMonsterInitialized(Monster monsterRef)
	{
		InitializeMonster(monsterRef);
	}

	public void OnMonsterDestroy(Monster monsterRef)
	{
		if(monsterRef.IsLargeMonster())
		{
			LargeMonsterManager.Instance.DestroyMonster(monsterRef);
		}

		if(monsterRef.IsSmallMonster())
		{
			SmallMonsterManager.Instance.DestroyMonster(monsterRef);
		}
	}

	public void OnMonsterDeath(Monster monsterRef)
	{
		if(monsterRef.IsLargeMonster())
		{
			LargeMonsterManager.Instance.OnMonsterDeath(monsterRef);
		}

		if(monsterRef.IsSmallMonster())
		{
			SmallMonsterManager.Instance.OnMonsterDeath(monsterRef);
		}
	}

	private void InitializeMonster(Monster monsterRef)
	{
		if(monsterRef.IsLargeMonster())
		{
			LargeMonsterManager.Instance.InitializeMonster(monsterRef);
		}

		if(monsterRef.IsSmallMonster())
		{
			SmallMonsterManager.Instance.InitMonster(monsterRef);
		}
	}

	private void GetAllMonsters()
	{
		foreach (var monsterRef in Monster.GetAllMonsters())
		{
			InitializeMonster(monsterRef);
		}
	}
}
