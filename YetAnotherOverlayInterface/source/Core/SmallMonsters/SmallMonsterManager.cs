using SharpPluginLoader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class SmallMonsterManager
{
	private static readonly Lazy<SmallMonsterManager> _lazy = new(() => new SmallMonsterManager());
	public static SmallMonsterManager Instance => _lazy.Value;

	public Dictionary<Monster, SmallMonster> SmallMonsters { get; } = new();

	public SmallMonsterManager() { }

	public void Initialize()
	{

	}

	public void InitMonster(Monster monster)
	{
		if(SmallMonsters.ContainsKey(monster)) return;

		SmallMonsters.Add(monster, new SmallMonster(monster));
	}

	public void DestroyMonster(Monster monsterRef)
	{
		if(!SmallMonsters.ContainsKey(monsterRef)) return;

		SmallMonsters.Remove(monsterRef);
	}

	public void OnMonsterDeath(Monster monsterRef)
	{
		if(!SmallMonsters.ContainsKey(monsterRef)) return;

		SmallMonster smallMonster;
		var isSuccess = SmallMonsters.TryGetValue(monsterRef, out smallMonster);

		if(!isSuccess) return;

		smallMonster.OnMonsterDeath();
	}
}