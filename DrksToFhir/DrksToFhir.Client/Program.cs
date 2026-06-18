using DrksToFhir.Core.Mapping;
using Hl7.Fhir.Serialization;

if (args.Length < 2)
{
    Console.Error.WriteLine("Verwendung:");
    Console.Error.WriteLine("  Export: DrksToFhir.Client drks <pfad-zur-drks-json> [pfad-zur-ausgabe-json]");
    Console.Error.WriteLine("  Import: DrksToFhir.Client fhir <pfad-zur-fhir-json> [pfad-zur-ausgabe-json]");
    return 1;
}

var mode = args[0].ToLowerInvariant();
var inputPath = args[1];

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Datei nicht gefunden: {inputPath}");
    return 1;
}

try
{
    var inputJson = await File.ReadAllTextAsync(inputPath);
    string outputJson;

    if (mode == "drks")
    {
        var mapper = new ResearchStudyMapper();
        var researchStudy = mapper.Map(inputJson);
        var serializer = new FhirJsonSerializer();
        outputJson = serializer.SerializeToString(researchStudy);
    }
    else if (mode == "fhir")
    {
        var mapper = new FhirToDrksMapper();
        outputJson = mapper.Map(inputJson);
    }
    else
    {
        Console.Error.WriteLine($"Unbekannter Modus: '{mode}'. Verwende 'drks' oder 'fhir'.");
        return 1;
    }

    if (args.Length >= 3)
    {
        var outputPath = args[2];
        await File.WriteAllTextAsync(outputPath, outputJson);
        Console.WriteLine($"Ausgabe gespeichert: {outputPath}");
    }
    else
    {
        Console.WriteLine(outputJson);
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fehler: {ex.Message}");
    return 1;
}