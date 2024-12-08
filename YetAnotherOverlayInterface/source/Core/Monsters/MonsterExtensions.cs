using SharpPluginLoader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherOverlayInterface;

internal static class MonsterExtensions
{
	public static bool IsSmallMonster(this MonsterType monsterType)
	{
		return Enum.IsDefined(typeof(SmallMonsters), (int) monsterType);
	}

	public static bool IsLargeMonster(this MonsterType monsterType)
	{
		return Enum.IsDefined(typeof(LargeMonsters), (int) monsterType);
	}

	public static bool IsMiscEntity(this MonsterType monsterType)
	{
		return Enum.IsDefined(typeof(MiscEntities), (int) monsterType);
	}

	public static bool IsSmallMonster(this Monster monster)
	{
		return Enum.IsDefined(typeof(SmallMonsters), (int) monster.Type);
	}

	public static bool IsLargeMonster(this Monster monster)
	{
		return Enum.IsDefined(typeof(LargeMonsters), (int) monster.Type);
	}

	public static bool IsMiscEntity(this Monster monster)
	{
		return Enum.IsDefined(typeof(MiscEntities), (int) monster.Type);
	}
}
