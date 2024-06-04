#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.Feedbacks
{
	public class MMSpringVignetteIntensity : MMSpringFloatComponent<PostProcessVolume>
	{
		protected Vignette _vignette;
		
		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<PostProcessVolume>();
			}
			Target.profile.TryGetSettings(out _vignette);
			base.Initialization();
		}
		
		public override float TargetFloat
		{
			get => _vignette.intensity;
			set => _vignette.intensity.Override(value);
		}
	}
}
#endif