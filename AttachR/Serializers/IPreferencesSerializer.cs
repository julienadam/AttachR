using AttachR.Models;

namespace AttachR.Serializers
{
    public interface IPreferencesSerializer
    {
        Preferences Load();
        void Save(Preferences model);
    }
}