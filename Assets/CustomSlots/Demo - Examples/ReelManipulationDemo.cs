namespace CSFramework {
	public class ReelManipulationDemo : BaseSlotGameUI {
		public override void OnRoundStart() {
			base.OnRoundStart();

			//slot.SetManipulation(1, "C");

			//	slot.SetManipulation(1, slot.symbolManager.GetSymbol("A"), slot.symbolManager.GetSymbol("C"), slot.symbolManager.GetSymbol("A"), slot.symbolManager.GetSymbol("C"));
			slot.SetManipulation(0, 0, 1, 2, 1, 0);
		}
	}
}