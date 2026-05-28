using System.Text.Json;
using System.Text.Json.Serialization;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

public class ResearchStudyMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public ResearchStudy Map(string drksJson)
    {
        var drks = JsonSerializer.Deserialize<DrksStudy>(drksJson, JsonOptions)
            ?? throw new ArgumentException("DRKS-JSON konnte nicht deserialisiert werden.");

        return Map(drks);
    }

    public ResearchStudy Map(DrksStudy drks)
    {
        var study = new ResearchStudy();

        IdentifierMapper.Map(drks, study);
        TitleDescriptionMapper.Map(drks, study);
        StatusMapper.Map(drks, study);
        StudyDesignMapper.Map(drks, study);
        ConditionMapper.Map(drks, study);
        RecruitmentMapper.Map(drks, study);
        ContactMapper.Map(drks, study);
        EthicsCommitteeMapper.Map(drks, study);        
        ObjectiveMapper.Map(drks, study);
        ObservationalGroupMapper.Map(drks, study);
        RelatedArtifactMapper.Map(drks, study);
        ExtensionMapper.Map(drks, study);

        return study;
    }
}