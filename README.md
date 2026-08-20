# DRKS2FHIR

Prototypische Implementierung eines bidirektionalen Mappings zwischen dem **Deutschen Register Klinischer Studien (DRKS)** im JSON-Format und der **HL7 FHIR R5**-Ressource [`ResearchStudy`](https://www.hl7.org/fhir/R5/researchstudy.html).

Entwickelt im Rahmen der Bachelorarbeit *„Überführung von Studienregisterdaten in den HL7 FHIR-Standard – Analyse, Konzeptentwicklung und Umsetzung am Beispiel des Deutschen Registers Klinischer Studien"* (Lorenz Sitte, Universität Leipzig, 2026).

---

## Hintergrund

Das DRKS ist das nationale Register klinischer Studien in Deutschland. Da das DRKS derzeit keine standardisierte FHIR-Schnittstelle bereitstellt, wurde im Rahmen dieser Arbeit ein Prototyp entwickelt, der DRKS-Studieneinträge (JSON, Schema-Version 2.0.19) in FHIR-R5-konforme `ResearchStudy`-Ressourcen überführt und wieder zurück.

Es werden **81 von 84 relevanten Feldern** des DRKS-Schemas abgebildet. Die verbleibenden 3 Felder werden beim Rückweg rechnerisch rekonstruiert. Die Verlustfreiheit des Round-Trips wurde mit zwei Testdatensätzen empirisch verifiziert.

---

## Projektstruktur

```
DrksToFhir/
├── DrksToFhir.Core/               # Kernbibliothek
│   ├── Enums/                     # C#-Enumerationen (spiegeln DRKS-Enumerationen wider)
│   ├── Models/                    # C#-Modellklassen für das DRKS-JSON-Schema
│   └── Mapping/                   # Mapper-Klassen (Hin- und Rückweg)
│       ├── ResearchStudyMapper.cs      # Koordination DRKS → FHIR
│       ├── FhirToDrksMapper.cs         # Koordination FHIR → DRKS
│       ├── IdentifierMapper.cs
│       ├── TitleDescriptionMapper.cs
│       ├── StatusMapper.cs
│       ├── StudyDesignMapper.cs
│       ├── ConditionMapper.cs
│       ├── RecruitmentMapper.cs
│       ├── ContactMapper.cs
│       ├── EthicsCommitteeMapper.cs
│       ├── ObjectiveMapper.cs
│       ├── ObservationalGroupMapper.cs
│       ├── RelatedArtifactMapper.cs
│       ├── ExtensionMapper.cs
│       └── FhirHelper.cs               # Gemeinsame FHIR-Hilfsmethoden
└── DrksToFhir.Client/             # Kommandozeilenprogramm
    ├── Program.cs
    ├── DRKS_trial-DRKS00038775.json    # Realer Testdatensatz
    ├── DRKS_trial-DRKS00099999.json    # Synthetischer Testdatensatz
    ├── DRKS00038775_FHIR.json          # Generierter FHIR-Datensatz
    └── DRKS00099999_FHIR.json          # Generierter FHIR-Datensatz
implementation-guide/              # FHIR Implementation Guide
    ├── CodeSystem_*.json               # 11 eigene CodeSystems
    ├── ValueSet_*.json                 # 11 ValueSets
    └── Extension_*.json                # 7 StructureDefinitions
```

---

## Voraussetzungen

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Internetverbindung beim ersten Build (NuGet-Paket wird automatisch geladen)

## Verwendete Bibliotheken

| Paket | Version | Zweck |
|---|---|---|
| [Hl7.Fhir.R5](https://www.nuget.org/packages/hl7.fhir.r5) (Firely .NET SDK) | 6.2.0 | Typisierte FHIR-R5-Ressourcenmodelle & Serialisierung |

`System.Text.Json` wird für die DRKS-JSON-Deserialisierung genutzt und ist Bestandteil des .NET-SDK (keine separate NuGet-Referenz nötig).

## Installation

```bash
git clone https://github.com/IMISE/DRKS2FHIR.git
```
```bash
cd DrksToFhir
```
```bash
dotnet build
```

---

## Verwendung

Projekt bauen:

```bash
dotnet build
```

### DRKS → FHIR

```bash
dotnet run --project DrksToFhir.Client -- drks <eingabe.json> <ausgabe.json>
```

**Beispiel:**

```bash
dotnet run --project DrksToFhir.Client -- drks DRKS_trial-DRKS00038775.json DRKS00038775_FHIR.json
```

### FHIR → DRKS

```bash
dotnet run --project DrksToFhir.Client -- fhir <eingabe.json> <ausgabe.json>
```

**Beispiel:**

```bash
dotnet run --project DrksToFhir.Client -- fhir DRKS00038775_FHIR.json DRKS00038775_zurueck.json
```

Wird kein Ausgabepfad angegeben, wird das Ergebnis auf der Standardausgabe ausgegeben.

---

## Validierung

Die erzeugten `ResearchStudy`-Ressourcen können mit dem [HL7 FHIR Validator](https://confluence.hl7.org/display/FHIR/Using+the+FHIR+Validator) unter Einbindung des Implementation Guides validiert werden:

```bash
java -jar validator_cli.jar <datei.json> -version 5.0 -ig implementation-guide/
```

Die Validierung der beiden Testdatensätze ergab jeweils **0 Errors**.

---

## Architektur

Jede der 12 thematischen Mapper-Klassen verantwortet sowohl den Hinweg als auch den Rückweg für ihren Bereich. Die übergeordneten Koordinationsklassen `ResearchStudyMapper` und `FhirToDrksMapper` rufen die Teilmapper sequenziell auf.

Verwendete Mapping-Strategien:

| Strategie | Anzahl | Beschreibung |
|---|---|---|
| Direct Mapping | 39 | Feld wird wertunverändert übertragen |
| Transformed Mapping | 26 | Vokabular- oder Strukturumwandlung erforderlich |
| Extension Mapping | 16 | Kein natives FHIR-Äquivalent; eigene Extension definiert |
| No Mapping | 3 | Beim Rückweg rechnerisch rekonstruiert |

---

## Weiterführende Links

- [DRKS – Deutsches Register Klinischer Studien](https://drks.de)
- [HL7 FHIR R5 ResearchStudy](https://www.hl7.org/fhir/R5/researchstudy.html)
- [Firely .NET SDK](https://github.com/FirelyTeam/firely-net-sdk)
- [IMISE – Universität Leipzig](https://www.imise.uni-leipzig.de)
