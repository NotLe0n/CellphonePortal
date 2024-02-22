using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CellphonePortal;
internal class ChangeCellphoneTooltip : GlobalItem
{
	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (item.type == ItemID.CellPhone) {
			var potionTooltip = Lang.GetTooltip(ItemID.PotionOfReturn);
			tooltips.Add(new TooltipLine(Mod, "Tooltip2", potionTooltip.GetLine(0)));
			tooltips.Add(new TooltipLine(Mod, "Tooltip3", potionTooltip.GetLine(1)));
		}

		base.ModifyTooltips(item, tooltips);
	}
}
