using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace CSFramework {
	/// <summary>
	/// A class that contains information of a slot.
	/// </summary>
	[Serializable]
	public class GameInfo {
		private CustomSlot slot;
		public int roundsCompleted = 0;
		public int freeSpins = 0;
                public int TotalFreeSpins = 0;
		public float bonuses = 0;
		public float balance = 1000;
		public float bet = 0.01f;
		public float roundBalance;
		public int roundHits;
		public int totalHits;
		public List<HitInfo> scatterHitInfos;
		public virtual float roundCost { get {
                return  float.Parse((Math.Round(double.Parse((slot.currentMode.costPerLine * slot.gameInfo.bet * slot.lineManager.activeLines).ToString()),2)).ToString());
            }
        }
		public GameInfo(CustomSlot slot) { this.slot = slot; }

		internal void OnStartSpin() {
			roundHits = 0;
			roundBalance = 0;
			AddBalance(-roundCost);
		}

		internal void OnStartRound() {
			slot.lineManager.allHitHolders.Clear();
			scatterHitInfos = new List<HitInfo>();
			foreach (Symbol symbol in slot.symbolManager.symbols) if (symbol.matchType == Symbol.MatchType.Scatter || symbol.matchType == Symbol.MatchType.WildScatter) scatterHitInfos.Add(new HitInfo(slot, null, symbol));
		}

		internal void OnRoundComplete() {
			roundsCompleted++;
            //slot.gameInfo.totalHits
            if (bonuses > 0 && slot.currentMode == slot.modes.bonusMode) slot.AddBonus(-1);
            else if (freeSpins > 0 && slot.currentMode == slot.modes.freeSpinMode)
            { 
                slot.AddFreeSpin(-1);
                SlotManager.SlotManagerInstance.mFreeSpinGameCountLabel.GetComponentInChildren<UnityEngine.UI.Text>().text = "BONUSSPINS PLAYED: " + roundsCompleted + " OF " + (slot.gameInfo.freeSpins + roundsCompleted);
            }
		}

        public void AddRoundBalance(float amount, HitInfo info = null)
        {
            if (slot.currentMode == slot.modes.freeSpinMode)
            {
                float amntToAdd = amount * SlotManager.SlotManagerInstance.mFreeSpinMultiplier;
                string strAmount = string.Format("{0:0.00}", amntToAdd);
                float amntToAdd2 = float.Parse(strAmount);
                roundBalance += amntToAdd2;
            }
            else if(Check2WildSymboContains(info) && SlotManager.SlotManagerInstance.mWildSymbolMultiplier > 1)
            {
                float amntToAdd = amount * SlotManager.SlotManagerInstance.mWildSymbolMultiplier;
                string strAmount = string.Format("{0:0.00}", amntToAdd);
                float amntToAdd2 = float.Parse(strAmount);
                roundBalance += amntToAdd2;
            }
            else
            { 
                string strAmount = string.Format("{0:0.00}", amount);
                float amntToAdd2 = float.Parse(strAmount);
                roundBalance += amntToAdd2;
            }
        }

        public bool Check2WildSymboContains(HitInfo info)
        {
            int _MetchCount = 0;
            for (int i = 0; i < info.hitHolders.Count; i++)
            {
                if(info.hitHolders[i].symbol.matchType == Symbol.MatchType.Wild)
                {
                    _MetchCount++;
                }
            }
            if (_MetchCount >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddBalance(float amount, HitInfo info = null) {
            string strAmount = string.Format("{0:0.00}", amount);
            float amntToAdd = float.Parse(strAmount);
            string BeforeAddingbalance = string.Format("{0:0.00}", balance);
            double bal = double.Parse(BeforeAddingbalance);
            bal = bal + amntToAdd;
            string strBalance = string.Format("{0:0.00}", bal);
            balance = float.Parse(strBalance);
			slot.callbacks.onAddBalance.Invoke(new BalanceInfo(amount, info));
		}

		public void AddHit() {
			roundHits++;
			totalHits++;
		}
	}

	[Serializable]
	public class ReelInfo : UnityEvent<ReelInfo> {
		public Reel reel;
		public bool isFirstReel { get { return reel.index == 0; } }
		public bool isLastReel { get { return reel.index == reel.slot.reels.Length - 1; } }
		public ReelInfo() { }
		public ReelInfo(Reel reel) { this.reel = reel; }
	}

	[Serializable]
	public class SlotModeInfo : UnityEvent<SlotModeInfo> {
		public SlotMode lastMode;
		public SlotModeInfo() { }

		public SlotModeInfo(SlotMode lastMode) { this.lastMode = lastMode; }
	}

	[Serializable]
	public class BalanceInfo : UnityEvent<BalanceInfo> {
		public HitInfo hitInfo;
		public float amount;
		public BalanceInfo() { }

		public BalanceInfo(float amount, HitInfo hitInfo = null) {
			this.hitInfo = hitInfo;
			this.amount = amount;
		}
	}

	[Serializable]
	public class LineInfo : UnityEvent<LineInfo> {
		public Line line;
		public bool isLineEnabled { get { return line && line.isLineEnabled; } }

		public LineInfo() { }

		public LineInfo(Line line) { this.line = line; }
	}

	/// <summary>
	/// A class that contains line's information.
	/// Will be rest once Hit Check starts.
	/// </summary>
	[Serializable]
	public class HitInfo : UnityEvent<HitInfo> {
		/// <summary>
		/// A list of all SymbolHolders Hit Check traced.
		/// </summary>
		public SymbolHolder[] holders;

		/// <summary>
		/// A list of SymbolHolders which were actually Hit.
		/// </summary>
		public List<SymbolHolder> hitHolders = new List<SymbolHolder>();

		/// <summary>
		/// DOTween sequence that will be played if the line was a hit.
		/// You can Join/Append your own Tween to how the sequence is played. 
		/// </summary>
		public Sequence sequence;
		public bool isSequencePlayed;

		public Line line;
		public Symbol hitSymbol;
		public int hitChains;
		public float payout;

		private CustomSlot slot;
		private bool isProcessed;
		public bool isLineEnabled { get { return line && line.isLineEnabled; } }

		private bool _isHit;
		public bool isHit { get {
                return _isHit && (hitSymbol.matchType == Symbol.MatchType.Scatter || hitSymbol.matchType == Symbol.MatchType.WildScatter || (line && line.isLineEnabled)); } }
		public HitInfo() { }

		public HitInfo(CustomSlot slot, Line line = null, Symbol symbol = null) {
			this.slot = slot;
			this.line = line;
			this.hitSymbol = symbol;
		}

		public bool ProcessHitCheck() {
			if (isProcessed) return false;
			isProcessed = true;

			if (line) {
				holders = line.GetHoldersOnPath();
				if (holders == null || holders.Length != slot.reels.Length) return false;
				ParseChains(holders);
			} else {
				List<SymbolHolder> list = slot.GetVisibleHolders();
                    foreach (SymbolHolder holder in list)
                    {
                        if (holder.symbol == hitSymbol && (hitSymbol.matchType == Symbol.MatchType.Scatter || hitSymbol.matchType == Symbol.MatchType.WildScatter))
                        {
						hitChains++;
						hitHolders.Add(holder);
					}
				}
				holders = hitHolders.ToArray();
				_isHit = hitChains >= hitSymbol.minChains;
			}

			if (isHit)
				slot.ProcessHit (this);
			
			return isHit;
		}

		internal void Reset() {
			_isHit = false;
			hitChains = 0;
		}

		internal void ParseChains(SymbolHolder[] refHolders) {
			Symbol[] symbols = new Symbol[slot.reels.Length];
			for (int x = 0; x < slot.reels.Length; x++) symbols[x] = holders[x].symbol;
			ParseChains(symbols, refHolders);
		}

		internal void ParseChains(Symbol[] symbols, SymbolHolder[] refHolders = null) {
			bool chainStopped = false;
			for (int i = 0; i < symbols.Length; i++) {
				Symbol symbol = symbols[i];
				if (i == 0) hitSymbol = symbol;
				if (!chainStopped && symbol.CanMatch(hitSymbol)) {
					 if ((hitSymbol.matchType == Symbol.MatchType.Wild || hitSymbol.matchType == Symbol.MatchType.WildScatter) && (symbol.matchType != Symbol.MatchType.Wild && symbol.matchType != Symbol.MatchType.WildScatter)) hitSymbol = symbol;
                        hitChains++;
					if (refHolders != null) hitHolders.Add(refHolders[i]);
				} else chainStopped = true;
			}

			if (!hitSymbol || hitSymbol.minChains == -1 || hitChains < hitSymbol.minChains) return;
			if (slot.config.advanced.alternativeLineCheck && !ValidateAlternativeLine()) return;

			_isHit = true;
			if (slot.config.advanced.alternativeLineCheck) slot.lineManager.allHitHolders.Add(hitHolders);
		}

		internal bool ValidateAlternativeLine() {
			if (hitHolders.Count < slot.config.reelLength) {
				foreach (Row row in slot.rows) {
					if (row.isHiddenRow) continue;
					if (row.holders[hitHolders.Count].symbol.CanMatch(hitSymbol)) return false;
				}
			}
			foreach (List<SymbolHolder> holders in slot.lineManager.allHitHolders) {
				if (holders != hitHolders && holders.Count >= hitHolders.Count) {
					bool samePath = true;
					for (int i = 0; i < hitHolders.Count; i++)
						if (hitHolders[i] != holders[i]) {
							samePath = false;
							break;
						}
					if (samePath) return false;
				}
			}
			return true;
		}
	}
}