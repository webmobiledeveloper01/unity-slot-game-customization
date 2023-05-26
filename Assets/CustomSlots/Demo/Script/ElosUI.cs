using System;
using System.Collections;
using CSFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using SimpleJSON;
using Random = UnityEngine.Random;

namespace Elona.Slot {
	public class ElosUI : BaseSlotGameUI {
		[Serializable]
		public class Colors {
			public Gradient freeSpinBG;
			public Gradient freeSpinBGSlot;
		}

		[Header("Elos")] public Elos elos;
		public ElosShop shop;
		public Colors colors;
		public Text textLevel, textExp;

		public Image background, highlightFreeSpin, backgroundSlot;
		public Button buttonPlay;
		public Slider sliderExp;
		public GameObject payTable;
		public GameObject[] BGs;
		public AudioMixer mixer;

		public float volumeMaster { set { mixer.SetFloat("VolumeMaster", Mathf.Lerp(-80, 0, value)); } }
		public float volumeBGM { set { mixer.SetFloat("VolumeBGM", Mathf.Lerp(-80, 0, value)); } }
		public float volumeSE { set { mixer.SetFloat("VolumeSE", Mathf.Lerp(-80, 0, value)); } }

		private int indexBG;

		private Tweener _moneyTween;
		public float lastBalance;

		private Elos.Assets assets { get { return elos.assets; } }
		private Elos.ElonaSlotData data { get { return elos.data; } }
		private void OnEnable()
        {
            transform.Find("MusicToggle").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("Music", 1) == 1 ? false : true;
            transform.Find("SoundToggle").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("Sound", 1) == 1 ? false : true;

            SetMusic();
            SetSound();
            assets.bgm.Play();
        }

        public void HandleToggle(GameObject toggleBtn)
        {
            switch (toggleBtn.name)
            {
                case "MusicToggle":
                    if (toggleBtn.GetComponent<Toggle>().isOn)
                    {
                        PlayerPrefs.SetInt("Music", 0);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("Music", 1);
                    }
                    break;
                case "SoundToggle":
                    if (toggleBtn.GetComponent<Toggle>().isOn)
                    {
                        PlayerPrefs.SetInt("Sound", 0);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("Sound", 1);
                    }
                    break;
            }

            SetMusic();
            SetSound();
        }

        public override void Initialize() {
			elos.Load();
			base.Initialize();
			RefreshExp();
			slot.callbacks.onAddBalance.AddListener(OnAddBalance);
			lastBalance = slot.gameInfo.balance;
			shop.gameObject.SetActive(false);
			elos.bonusGame.gameObject.SetActive(false);
		}

		public void RefreshExp() {
			textLevel.text = "" + data.lv;
			textExp.text = "" + data.exp + " / " + data.expNext;
			sliderExp.value = (float) data.exp/data.expNext;
		}

		public override void OnActivated() {
			base.OnActivated();
			if (!slot.debug.skipIntro) {
				assets.audioDemo.Play();
		//v		assets.tweens.tsIntro1.Play();
		//v		assets.tweens.tsIntro2.Play();
			}
		}

		public override void OnRoundStart() {
			base.OnRoundStart();

			//assets.particleSpin.Play();

			if (slot.currentMode != slot.modes.freeSpinMode) buttonPlay.interactable = true;
		}

        public void SetMusic()
        {
            if (PlayerPrefs.GetInt("Music", 1) == 0)
            {
                assets.bgm.mute = true;
            }
        }
        public void SetSound()
        {
            if (PlayerPrefs.GetInt("Sound", 1) == 0)
            {
                assets.audioDemo.mute = true;
                assets.audioEarnSmall.mute = true;
                assets.audioEarnBig.mute = true;
                assets.audioPay.mute = true;
                assets.audioSpin.mute = true;
                assets.audioSpinLoop.mute = true;
                assets.audioReelStop.mute = true;
                assets.audioClick.mute = true;
                assets.audioWinSmall.mute = true;
                assets.audioWinMedium.mute = true;
                assets.audioWinBig.mute = true;
                assets.audioLose.mute = true;
                assets.audioBet.mute = true;
                assets.audioImpact.mute = true;
                assets.audioBeep.mute = true;
            }
        }




        public override void OnReelStart(ReelInfo info) {
			base.OnReelStart(info);
			Debug.Log("OnReelStart called");
			if (info.isFirstReel) {
				assets.audioSpin.Play();
				assets.audioSpinLoop.Play();
				//assets.particleSpin.Play();
			}
		}

		public override void OnReelStop(ReelInfo info) {
			base.OnReelStop(info);
			assets.audioReelStop.Play();
			if (info.isFirstReel && slot.currentMode.spinMode == SlotMode.SpinMode.ManualStopAll)
			{
			//v 	buttonPlay.interactable = false;
			}
			if (info.isLastReel) {

		//v		buttonPlay.interactable = false;
				assets.audioSpinLoop.Stop();
			}
		}

		public override void OnRoundComplete() {
			base.OnRoundComplete();
			if (slot.gameInfo.roundHits == 0) assets.audioLose.Play();
		}

		public override void EnableNextLine() {
			if (!slot.lineManager.EnableNextLine()) assets.audioBeep.Play();
			else assets.audioBet.Play();
		}

		public override void DisableCurrentLine() {
			if (!slot.lineManager.DisableCurrentLine()) assets.audioBeep.Play();
			else {
				assets.audioBet.pitch = 0.6f;
				assets.audioBet.Play();
			}
		}

		public override bool SetBet(int index) {
			if (!base.SetBet(index)) {
				assets.audioBeep.Play();
				return false;
			}
			assets.audioBet.Play();
			return true;
		}

		public void TogglePayTable() {
			assets.audioClick.Play();
			payTable.SetActive(!payTable.activeSelf);
		}

		public void ToggleShop() {
			if (slot.isIdle) shop.Activate();
			else assets.audioBeep.Play();
		}

		public override void ToggleFreeSpin(bool enable) {
			base.ToggleFreeSpin(enable);

            StartCoroutine(SlotManager.SlotManagerInstance.ToggleFreeSpin(enable));
            //vif (enable) {


			//v	assets.particleFreeSpin.Play();
			//v	backgroundSlot.DOGradientColor(colors.freeSpinBGSlot, 0.6f);
			//v	background.DOGradientColor(colors.freeSpinBG, 0.6f);
		//v	} else {


			//v	assets.particleFreeSpin.Stop();
			//v	backgroundSlot.DOColor(Color.white, 2f);
			//v	background.DOColor(Color.white, 2f);
		//v	}
		}

		public override void OnProcessHit(HitInfo info) {
			base.OnProcessHit(info);
			SymbolHolder randomHolder = info.hitHolders[Random.Range(0, info.hitHolders.Count)];
			ElosSymbol symbol = randomHolder.symbol as ElosSymbol;
	//v		Util.InstantiateAt<ElosEffectBalloon>(assets.effectBalloon, slot.transform.parent, randomHolder.transform).Play(symbol.GetRandomTalk());
	//v		foreach (SymbolHolder holder in info.hitHolders) info.sequence.Join(ShowWinAnimation(info, holder));
		}

		// Winning particle and audio effect when a line is a "hit"
		public Tweener ShowWinAnimation(HitInfo info, SymbolHolder holder) {
			return Util.Tween(() => {
				int coins = (info.hitChains - 2)*(info.hitChains - 2)*(info.hitChains - 2) + 1;

				if (info.hitSymbol.payType == Symbol.PayType.Normal) {
					assets.particlePrize.transform.position = holder.transform.position;
					Util.Emit(assets.particlePrize, coins);
					if (info.hitChains <= 3) assets.audioWinSmall.Play();
					else if (info.hitChains == 4) assets.audioWinMedium.Play();
					else assets.audioWinBig.Play();
					if (info.hitChains >= 4) assets.tweens.tsWin.SetText(info.hitChains + "-IN-A-ROW!", info.hitChains*40).Play();
				} else {
					assets.audioWinSpecial.Play();
					if (info.hitSymbol.payType == Symbol.PayType.FreesSpin) assets.tweens.tsWinSpecial.SetText("Free Spin!").Play();
					else assets.tweens.tsWinSpecial.SetText("BONUS!").Play();
				}
			});
		}

		private float _lastBalance;

		private void Update() {
			if (_lastBalance != lastBalance) {
				textMoney.text = "" + string.Format("{0:0.00}", lastBalance);
				_lastBalance = lastBalance;
			}
		}

		public override void RefreshMoney() { }

    /*   IEnumerator UpdateBalance(int bal)
        {
            string uId = PlayerPrefs.GetString("userID");

            WWWForm form1 = new WWWForm();
            form1.AddField("user_id", uId);
            form1.AddField("coin", bal);

            //	http://demo.mankiinfotech.com/fruitslot/api/user_login?email=vv@gmail.com&password=vv&device_token=sdsd&deviceType=android

        //    WWW updateScore = new WWW("http://demo.mankiinfotech.com/fruitslot/api/user_update_coin", form1);
        //    yield return updateScore;
            yield return new WaitForSeconds(1f);
            if (string.IsNullOrEmpty(updateScore.error))
            {
                Debug.Log("Form upload complete! : " + updateScore.text);

                var N = JSONNode.Parse(updateScore.text);
                if (N["message"].Value.Equals("Login Successfully"))
                {
                    Debug.Log("Login Success");
                    Debug.Log("AS : " + N["result"]["coin"].Value.ToString());
                    PlayerPrefs.SetInt("CurrBalance", int.Parse(N["result"]["coin"].Value));
                }
                //	SceneManager.LoadScene ("Demo");
            }
            else
            {
                Debug.Log("Error : " + updateScore.error);
            }
        }*/


        public void OnAddBalance(BalanceInfo info) {
        //    StartCoroutine(UpdateBalance(info.amount));

            if (info.amount == 0) return;

            float duration = 1f;
			if (info.amount < 0) {
			//v	assets.audioPay.Play();
				Util.Emit(assets.particlePay, 3);
			} else {
				if (info.hitInfo != null) {
					if (info.hitInfo.hitChains <= 3) assets.audioEarnSmall.Play();
					else assets.audioEarnBig.Play();
					duration = slot.effects.GetHitEffect(info.hitInfo).duration*0.8f;
				} else {
		//v			assets.audioEarnSmall.Play();
				}
			}
            //v		Util.InstantiateAt<ElosEffectMoney>(assets.effectMoney, transform).SetText(info.amount, info.hitInfo == null ? "" : info.hitInfo.hitChains + " in a row!").Play(100, 3f);
            //SlotManager.SlotManagerInstance.UpdateTerminal();

            assets.audioCoinAdd.Play();

		    if (_moneyTween != null && _moneyTween.IsPlaying()) _moneyTween.Complete();
		        _moneyTween = DOTween.To(() => lastBalance, x => lastBalance = x, slot.gameInfo.balance, duration).OnComplete(() => { _moneyTween = null; assets.audioCoinAdd.Stop();
		        });
        }

        public void SwitchBG() {
			BGs[indexBG].gameObject.SetActive(false);
			indexBG++;
			if (indexBG >= BGs.Length) indexBG = 0;
			BGs[indexBG].gameObject.SetActive(true);
		}
	}
}