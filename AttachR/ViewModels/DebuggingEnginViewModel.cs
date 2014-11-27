using System;

namespace AttachR.ViewModels
{
    public class DebuggingEngineViewModel : DebuggingEngine, ICloneable
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

        public object Clone()
        {
            return new DebuggingEngineViewModel()
            {
                Id = Id,
                Selected = Selected,
                Name = Name
            };
        }
    }
}