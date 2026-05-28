using DrksToFhir.Core.Mapping;
using Hl7.Fhir.Serialization;

if (args.Length < 1)
{
    Console.Error.WriteLine("Verwendung: DrksToFhir.Cli <pfad-zur-drks-json> [pfad-zur-ausgabe-json]");
    return 1;
}

var inputPath = args[0];

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Datei nicht gefunden: {inputPath}");
    return 1;
}

try
{
    var drksJson = await File.ReadAllTextAsync(inputPath);

    var mapper = new ResearchStudyMapper();
    var researchStudy = mapper.Map(drksJson);

    var serializer = new FhirJsonSerializer();
    var fhirJson = serializer.SerializeToString(researchStudy);

    if (args.Length >= 2)
    {
        var outputPath = args[1];
        await File.WriteAllTextAsync(outputPath, fhirJson);
        Console.WriteLine($"FHIR-JSON gespeichert: {outputPath}");
    }
    else
    {
        Console.WriteLine(fhirJson);
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}");
    return 1;
}