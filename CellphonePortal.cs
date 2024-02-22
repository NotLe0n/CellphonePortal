using Terraria.ModLoader;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Terraria;
using Terraria.ID;
using System;

namespace CellphonePortal;

public class CellphonePortal : Mod
{
	public override void Load()
	{
		IL_Player.ItemCheck_Inner += ModifyCellphone;
		base.Load();
	}

	public override void Unload()
	{
		IL_Player.ItemCheck_Inner -= ModifyCellphone;
		base.Unload();
	}

	private void ModifyCellphone(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C#:
				before:
					RemoveAllGrapplingHooks();
					Spawn(PlayerSpawnContext.RecallFromItem);
				after:
					RemoveAllGrapplingHooks();
					var item = player.inventory[player.selectedItem];
					if (item.type == 3124 || item.type == 5358) {
						DoPotionOfReturnTeleportationAndSetTheComebackPoint();
					}
					else {
						Spawn(PlayerSpawnContext.RecallFromItem);
					}
			IL:

			 RemoveAllGrapplingHooks();
				IL_0e48: ldarg.0
				IL_0e49: call instance void Terraria.Player::RemoveAllGrapplingHooks()

			 Spawn(PlayerSpawnContext.RecallFromItem);
				IL_0e4e: ldarg.0
				IL_0e4f: ldc.i4.2
				IL_0e50: call instance void Terraria.Player::Spawn(valuetype Terraria.PlayerSpawnContext)
						<=== here
		*/

		if (!c.TryGotoNext(MoveType.Before,
			i => i.MatchLdarg(0),
			i => i.MatchCall<Player>("RemoveAllGrapplingHooks"),
			i => i.MatchLdarg(0),
			i => i.MatchLdcI4(2),
			i => i.MatchCall<Player>("Spawn")
		)) {
			throw new("[CellphonePortal] ILEdit at CellphonePortal.ModifyCellphone() failed. Contact the mod author!");
		}

		c.Emit(OpCodes.Ldarg_0); // push 'player'
		c.EmitDelegate<Action<Player>>(player =>
		{
			var item = player.inventory[player.selectedItem];
			if (item.type is ItemID.CellPhone or ItemID.Shellphone) {
				if (player.whoAmI == Main.myPlayer) {
					player.DoPotionOfReturnTeleportationAndSetTheComebackPoint();
				}
			}
			else {
				player.Spawn(PlayerSpawnContext.RecallFromItem);
			}
		});
	}
}