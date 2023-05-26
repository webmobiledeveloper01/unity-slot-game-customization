namespace CSFramework {
	public class SpriteAnimationDemo : BaseSlotGameUI {
		public override void OnActivated() {
			base.OnActivated();
			SymbolManager sm = slot.symbolManager;
			Symbol _from = sm.GetSymbol("A");
			_from.matchType = Symbol.MatchType.Scatter;
		}

		public override void OnProcessHit(HitInfo info) {
			base.OnProcessHit(info);

			/*
				//	Example for swapping symbols
				string n = info.hitSymbol.name;
				if (n == "A" || n == "B") {
					SymbolManager sm = slot.symbolManager;
					Symbol _from = sm.GetSymbol(n);
					Symbol _to = sm.GetSymbol(n == "A" ? "B" : "A");
					sm.ApplySymbolMap(null, new SymbolSwapper {from = _from, to = _to});
					foreach (SymbolHolder holder in info.hitHolders) holder.animator.SetTrigger("OnSwap");
				} else {
					if (info.hitSymbol.animator) foreach (SymbolHolder holder in info.hitHolders) holder.animator.SetTrigger("OnHit");
				}
			*/
			if (info.hitSymbol.animator) foreach (SymbolHolder holder in info.hitHolders) holder.animator.SetTrigger("OnHit");
		}
	}
}