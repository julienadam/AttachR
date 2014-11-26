namespace AttachR.ViewModels
{
    public class DebuggingEngineViewModel : DebuggingEngine
    {
        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value.Equals(selected)) return;
                selected = value;
                OnPropertyChanged();
            }
        }
    }
}