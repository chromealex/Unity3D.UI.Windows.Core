using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Windows.Extensions;
using UnityEngine.UI.Windows.Components.Events;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Windows.Components {

	public class ProgressComponent : WindowComponentNavigation, IProgressComponent {
		
		[Header("Base")]
		public float duration = 1f;
		public float minNormalizedValue = 0f;
		public Extensions.Slider bar;
		
		[Header("Continious")]
		public bool continious;
		[ReadOnly("continious", state: false)][Range(0f, 1f, order = 2)]
		public float continiousWidth = 0.4f;
		[ReadOnly("continious", state: false)]
		public float continiousAngleStep = 0f;
		[ReadOnly("continious", state: false)]
		public ME.Ease.Type continiousEaseType = ME.Ease.Type.Linear;
		[ReadOnly("continious", state: false)]
		public bool continiousReflect = true;

		private ComponentEvent<float> callback = new ComponentEvent<float>();
		private ComponentEvent<ProgressComponent, float> callbackButton = new ComponentEvent<ProgressComponent, float>();
		[Range(0f, 1f)]
		public float currentValueNormalized = 0f;
		private float currentValue = 0f;

		private float progressCount;
		private float progressMaxCount;

		public void ResetProgressValues() {

			this.progressCount = 0;
			this.progressMaxCount = 0;
			this.SetValue(0f, immediately: true);

		}

		public void AddMaxProgress(int count) {

			this.progressMaxCount += count;
			this.SetValue(this.progressCount / this.progressMaxCount);

		}

		public void AddProgress() {

			this.progressCount += 1;
			this.SetValue(this.progressCount / this.progressMaxCount);

		}

		public virtual Selectable GetSelectable() {

			return this.bar;

		}

		#region macros UI.Windows.ButtonComponent.States 
	/*
	 * This code is auto-generated by Macros Module
	 * Do not change anything
	 */
	[SerializeField]
			private bool hoverOnAnyButtonState = false;
	
			[SerializeField]
			private bool hoverCursorDefaultOnInactive = false;
	
			public bool IsHoverCursorDefaultOnInactive() {
	
				return this.IsInteractable() == false && this.hoverCursorDefaultOnInactive == true;
	
			}
	
			public void Select() {
	
				this.GetSelectable().Select();
	
			}
	
			public virtual IInteractableComponent SetEnabledState(bool state) {
	
				if (state == true) {
	
					this.SetEnabled();
	
				} else {
	
					this.SetDisabled();
	
				}
	
				return this;
	
			}
	
			public virtual IInteractableComponent SetDisabled() {
	
				var sel = this.GetSelectable();
				if (sel != null) {
	
					if (sel.interactable != false) {
	
						sel.interactable = false;
						this.OnInteractableChanged();
	
					}
	
				}
	
				return this;
	
			}
	
			public virtual IInteractableComponent SetEnabled() {
	
				var sel = this.GetSelectable();
				if (sel != null) {
	
					if (sel.interactable != true) {
	
						sel.interactable = true;
						this.OnInteractableChanged();
	
					}
	
				}
	
				return this;
	
			}
	
			public IInteractableComponent SetHoverOnAnyButtonState(bool state) {
	
				this.hoverOnAnyButtonState = state;
	
				return this;
	
			}
	
			protected override bool ValidateHoverPointer() {
	
				if (this.hoverOnAnyButtonState == false && this.IsInteractable() == false) return false;
				return base.ValidateHoverPointer();
	
			}
	
			public bool IsInteractable() {
	
				var sel = this.GetSelectable();
				return (sel != null ? sel.IsInteractable() : false);
	
			}
	
			public virtual void OnInteractableChanged() {
	
			}
	#endregion

		public bool IsInteractableAndHasEvents() {

			return this.IsInteractable() == true /*&&
				(
					this.callback.GetPersistentEventCount() > 0 ||
					this.callbackButton.GetPersistentEventCount() > 0 ||
					this.bar.onValueChanged.GetPersistentEventCount() > 0
				)*/;

		}

		public override void Setup(IComponentParameters parameters) {
			
			base.Setup(parameters);
			
			var inputParameters = parameters as ProgressComponentParameters;
			{
				if (inputParameters != null) inputParameters.Setup(this as IProgressComponent);
			}

		}

		public override void OnInit() {

			base.OnInit();

			this.currentValue = (this.bar != null) ? this.bar.value : 0f;
			this.bar.continuousAngleStep = this.continiousAngleStep;

			this.bar.onValueChanged.RemoveListener(this.OnValueChanged_INTERNAL);
			this.bar.onValueChanged.AddListener(this.OnValueChanged_INTERNAL);

			this.bar.value = 0f;

		}

		public override void OnDeinit(System.Action callback) {

	        base.OnDeinit(callback);

			this.BreakAnimation();

            this.bar.onValueChanged.RemoveListener(this.OnValueChanged_INTERNAL);
            this.callback.RemoveAllListeners();
            this.callbackButton.RemoveAllListeners();
	        this.getValue = null;

	    }

        public override void OnShowBegin() {

			base.OnShowBegin();

			if (this.continious == false) {

				this.SetAsDefault();

			} else {

				this.SetAsContinuous(this.continiousWidth);

			}

			this.getValueActive = true;

		}

		public override void OnHideEnd() {

			base.OnHideEnd();

			this.BreakAnimation();

			this.getValueActive = false;

		}

		public override bool IsNavigationControlledSide(NavigationSide side) {

			var slider = this.GetSelectable() as Slider;
			var horizontal = (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft);
			if (horizontal == true) {

				if (side == NavigationSide.Left ||
				    side == NavigationSide.Right) {

					return true;

				}

			} else {

				if (side == NavigationSide.Up ||
					side == NavigationSide.Down) {

					return true;

				}

			}

			return false;

		}

		public override void OnNavigate(NavigationSide side) {

			base.OnNavigate(side);

			var slider = this.GetSelectable() as Slider;
			var horizontal = (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft);
			if (horizontal == true) {

				if (side == NavigationSide.Left) this.MovePrev();
				if (side == NavigationSide.Right) this.MoveNext();

			} else {

				if (side == NavigationSide.Up) this.MoveNext();
				if (side == NavigationSide.Down) this.MovePrev();

			}

		}

		private float GetStep() {

			var allWidth = 1f;
			var step = allWidth / 10f;

			return step;

		}

		public void MovePrev() {

			var value = this.GetValueNormalized();
			value -= this.GetStep();
			value = Mathf.Clamp01(value);

			this.SetValue(value);

		}

		public void MoveNext() {

			var value = this.GetValueNormalized();
			value += this.GetStep();
			value = Mathf.Clamp01(value);

			this.SetValue(value);

		}

		public void SetDuration(float value) {
			
			this.duration = value;
			
		}
		
		public void SetMinNormalizedValue(float value) {
			
			this.minNormalizedValue = value;
			
		}
		
		public void SetContiniousState(bool state) {
			
			this.continious = state;
			
		}
		
		public void SetContiniousWidth(float value) {
			
			this.continiousWidth = value;
			
		}
		
		public void SetContiniousAngleStep(float value) {
			
			this.continiousAngleStep = value;
			
		}

		public virtual void SetCallback(System.Action<float> callback) {

			this.callback.AddListenerDistinct(callback);
			this.callbackButton.RemoveAllListeners();

		}
		
		public virtual void SetCallback(System.Action<ProgressComponent, float> callback) {
			
			this.callbackButton.AddListenerDistinct(callback);
			this.callback.RemoveAllListeners();

		}

		private void OnValueChanged_INTERNAL(float value) {
			
			this.currentValue = value;

			if (this.callback != null) this.callback.Invoke(this.currentValue);
			if (this.callbackButton != null) this.callbackButton.Invoke(this, this.currentValue);

		}

		public void SetStep(float minValue, float maxValue, bool roundToInt) {

			this.bar.minValue = minValue;
			this.bar.maxValue = maxValue;
			this.bar.wholeNumbers = roundToInt;

		}

		public void SetAsContinuous(float width = 0.4f) {
			
			this.bar.continuous = true;
			this.bar.continuousWidth = width;

			this.SetAnimation();

		}

		public void SetAsDefault() {

			this.bar.continuous = false;

			this.BreakAnimation();

		}

		private float continiousDirection = 0f;
		public float GetContiniousDirection() {

			return this.continiousDirection;

		}

		public void BreakAnimation() {

			if (TweenerGlobal.instance != null) TweenerGlobal.instance.removeTweens(this);

		}

		public void SetAnimation() {

			if (TweenerGlobal.instance == null) return;

			if (this.continious == true && this.bar.canReceiveEvents == false) {

				var lastValue = 0f;
				TweenerGlobal.instance.removeTweens(this);
				var tween = TweenerGlobal.instance.addTween(this, this.duration, 0f, 1f).tag(this).ease(ME.Ease.GetByType(this.continiousEaseType)).repeat().onUpdate((obj, value) => {
					
					if (obj != null) {
						
						this.continiousDirection = Mathf.Sign(value - lastValue);
						obj.SetValue(value, immediately: true);
						lastValue = value;
						
					}
					
				});
				if (this.continiousReflect == true) tween.reflect();

			}

			if (this.bar != null && this.bar.continuous == true) {

				var delta = Time.unscaledDeltaTime;

				var value = this.bar.normalizedValue;
				value += delta;


			}

		}

		private bool getValueActive = false;
		private System.Func<float> getValue;
		private bool getValueImmediately;
		public void SetValue(System.Func<float> getValue, bool immediately = false) {

			this.getValue = getValue;
			this.getValueImmediately = immediately;
			this.getValueActive = true;

		}

		public virtual void LateUpdate() {

			if (this.getValueActive == true && this.getValue != null) {
				
				this.SetValue(this.getValue(), this.getValueImmediately);

			}

		}

		public IProgressComponent SetValue(float value, bool immediately = false, System.Action callback = null) {

			if (this.continious == true && immediately == false) {

				if (callback != null) callback.Invoke();
				return this;

			}

			this.sfxOnClick.Play();

			value = Mathf.Clamp01(value);

			if (this.bar != null) {

				if (immediately == false && this.duration > 0f) {

					var currentValueNormalized = this.bar.normalizedValue;

					TweenerGlobal.instance.removeTweens(this.bar);
					TweenerGlobal.instance.addTween(this.bar, this.duration, currentValueNormalized, value).onUpdate((obj, val) => {

						if (obj != null) {

							this.SetValue_INTERNAL(val);

						}

					}).tag(this.bar).onComplete(callback);

				} else {

					this.SetValue_INTERNAL(value);
					if (callback != null) callback.Invoke();

				}

			}

			return this;

		}

		private void SetValue_INTERNAL(float value) {
			
			this.currentValue = value;
			this.bar.normalizedValue = value;

			if (this.continious == false) {

				this.bar.normalizedValue = Mathf.Clamp(this.bar.normalizedValue, this.minNormalizedValue, 1f);

			}

		}

		public float GetValue() {

			return (this.bar != null) ? this.bar.value : 0f;

		}

		public float GetValueNormalized() {
			
			return (this.bar != null) ? this.bar.normalizedValue : 0f;

		}

#if UNITY_EDITOR
		public override void OnValidateEditor() {

			base.OnValidateEditor();

			if (Application.isPlaying == true) return;

			this.SetValue(this.currentValueNormalized, immediately: true);

			ME.Utilities.FindReference<Extensions.Slider>(this, ref this.bar);

		}
#endif

	}

}