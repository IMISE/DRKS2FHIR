using System.Text.Json;
using System.Text.Json.Serialization;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace DrksToFhir.Core.Mapping;

public class FhirToDrksMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Map(string fhirJson)
    {
        var parser = new FhirJsonParser();
        var study = parser.Parse<ResearchStudy>(fhirJson);
        var drks = Map(study);
        return JsonSerializer.Serialize(drks, JsonOptions);
    }

    public DrksStudy Map(ResearchStudy study)
    {
        var drks = new DrksStudy();

        IdentifierMapper.MapReverse(study, drks);
        TitleDescriptionMapper.MapReverse(study, drks);
        StatusMapper.MapReverse(study, drks);
        StudyDesignMapper.MapReverse(study, drks);
        ConditionMapper.MapReverse(study, drks);
        RecruitmentMapper.MapReverse(study, drks);
        ContactMapper.MapReverse(study, drks);
        EthicsCommitteeMapper.MapReverse(study, drks);
        ObjectiveMapper.MapReverse(study, drks);
        ObservationalGroupMapper.MapReverse(study, drks);
        RelatedArtifactMapper.MapReverse(study, drks);
        ExtensionMapper.MapReverse(study, drks);

        return drks;
    }
}