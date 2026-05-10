public interface IExorcisable
{
    bool IsStunned();
    int GetSavedProgress();
    void SetSavedProgress(int progress);
    void TriggerDisable();
    void ApplySlow(float amount);
    void ApplyStun(float intensity);
    void ClearLightEffects();
}
