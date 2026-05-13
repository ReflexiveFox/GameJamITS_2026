namespace GameJam
{
    public class GodChibi : Chibi
    {
        protected override void HandleEventRegistrations(bool isRegistering)
        {
            if(isRegistering)
            {
                GameManager.OnSavedLivesUpdated += UpdateChibiSprite;
            }
            else
            {
                GameManager.OnSavedLivesUpdated -= UpdateChibiSprite;
            }
        }

        private void UpdateChibiSprite(int _)
        {
            SetHappyForDuration();
        }
    }
}