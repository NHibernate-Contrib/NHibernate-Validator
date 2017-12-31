namespace NHibernate.Validator.Cfg.Loquacious
{
	public interface INhIntegration
	{
		INhIntegration And { get; }
		
		INhIntegration ApplyingDDLConstraints();
		INhIntegration AvoidingDDLConstraints();

		INhIntegration ApplyingGenerationFromMapping();
		INhIntegration AvoidingGenerationFromMapping();

		void RegisteringListeners();
		void AvoidingListenersRegister();
	}
}