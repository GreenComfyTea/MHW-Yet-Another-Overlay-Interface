using SharpPluginLoader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class LargeMonsterManager
{
	private static readonly Lazy<LargeMonsterManager> _lazy = new(() => new LargeMonsterManager());
	public static LargeMonsterManager Instance => _lazy.Value;

	public Dictionary<Monster, LargeMonster> LargeMonsters { get; } = new();

	public LargeMonsterManager() {}

	public void Initialize()
	{

	}

	public void InitializeMonster(Monster monsterRef)
	{
		if(LargeMonsters.ContainsKey(monsterRef)) return;

		LargeMonsters.Add(monsterRef, new LargeMonster(monsterRef));
	}

	public void DestroyMonster(Monster monsterRef)
	{
		if(!LargeMonsters.ContainsKey(monsterRef)) return;

		LargeMonsters.Remove(monsterRef);
	}

	public void OnMonsterDeath(Monster monsterRef)
	{
		if(!LargeMonsters.ContainsKey(monsterRef)) return;

		LargeMonster largeMonster;
		var isSuccess = LargeMonsters.TryGetValue(monsterRef, out largeMonster);

		if(!isSuccess) return;

		largeMonster.OnMonsterDeath();
	}
}
