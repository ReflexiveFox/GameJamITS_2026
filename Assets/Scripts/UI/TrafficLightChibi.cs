namespace GameJam
{
    public class TrafficLightChibi : Chibi
    {
        protected override void HandleEventRegistrations(bool isRegistering)
        {
            if (isRegistering)
            {
                GameManager.OnTargetLostLivesUpdated += UpdateChibiSprite;
            }
            else
            {
                GameManager.OnTargetLostLivesUpdated -= UpdateChibiSprite;
            }
        }
        private void UpdateChibiSprite(int _)
        {
            SetHappyForDuration();
        }
    }
}