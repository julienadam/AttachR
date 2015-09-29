using System.IO;
using System.Linq;
using AttachR.Models;
using AttachR.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AttachR.Serializers
{
    public class DebuggingTargetFileSerializer
    {
        static DebuggingTargetFileSerializer()
        {
            AutoMapper.Mapper.CreateMap<DebuggingProfileViewModel, DebuggingProfile>();
            AutoMapper.Mapper.CreateMap<DebuggingProfile, DebuggingProfileViewModel>();

            AutoMapper.Mapper
                .CreateMap<DebuggingTargetViewModel, DebuggingTarget>()
                .AfterMap((model, target) =>
                {
                    target.SelectedDebuggingEngines = model
                        .DebuggingEngines
                        .Where(e => e.Selected)
                        .Select(e => e.Name)
                        .ToList();
                });
            AutoMapper.Mapper
                .CreateMap<DebuggingTarget, DebuggingTargetViewModel>()
                .AfterMap((target, model) =>
                {
                    if (target.SelectedDebuggingEngines == null)
                    {
                        return;
                    }
                    foreach (var foundEngine in target.SelectedDebuggingEngines
                        .Select(localEngine => model.DebuggingEngines.FirstOrDefault(x => x.Name == localEngine))
                        .Where(foundEngine => foundEngine != null))
                    {
                        foundEngine.Selected = true;
                    }
                });
        }

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        };

        public void Save(string path, DebuggingProfileViewModel profile)
        {
            var result = AutoMapper.Mapper.Map<DebuggingProfile>(profile);
            File.WriteAllText(path, JsonConvert.SerializeObject(result, settings));
        }

        public DebuggingProfileViewModel Open(string filepath)
        {
            var source = JsonConvert.DeserializeObject<DebuggingProfile>(File.ReadAllText(filepath), settings);
            return AutoMapper.Mapper.Map<DebuggingProfileViewModel>(source);
        }
    }
}