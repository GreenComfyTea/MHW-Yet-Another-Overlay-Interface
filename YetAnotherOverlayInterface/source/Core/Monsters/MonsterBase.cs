using SharpPluginLoader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal class MonsterBase
{
	public Monster MonsterRef { get; }

	public bool IsAlive { get; set; } = true;

	public MonsterBase(Monster monsterRef)
	{
		MonsterRef = monsterRef;
	}

	public void OnMonsterDeath()
	{
		IsAlive = false;
	}
}
