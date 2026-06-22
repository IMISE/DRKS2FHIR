# FHIR Implementation Guide – DRKS to FHIR R5 Mapping

Dieses Verzeichnis enthält den FHIR Implementation Guide für das Mapping des Deutschen Registers Klinischer Studien (DRKS) auf den HL7 FHIR R5-Standard. Er wurde im Rahmen der Bachelorarbeit *„Überführung von Studienregisterdaten in den HL7 FHIR-Standard – Analyse, Konzeptentwicklung und Umsetzung am Beispiel des Deutschen Registers Klinischer Studien"* (Lorenz Sitte, Universität Leipzig, 2026) entwickelt.

## Inhalt

### `codesystems/` (11 Dateien)
Eigene CodeSystems für DRKS-spezifische Enumerationen ohne Entsprechung in standardisierten FHIR-Terminologiesystemen.

| Datei | Beschreibung |
|---|---|
| `CodeSystem_AgeUnit.json` | Einheiten für Altersangaben (inkl. WEEK_OF_PREGNANCY) |
| `CodeSystem_ArmType.json` | Typen von Studienarmen |
| `CodeSystem_ContactType.json` | Rollen von Kontaktpersonen und Sponsoren |
| `CodeSystem_EligibilityCriteria.json` | Einschlusskriterien (z. B. healthyVolunteers) |
| `CodeSystem_EthicVote.json` | Ergebnisse von Ethikvoten |
| `CodeSystem_InstituteType.json` | Typen von Rekrutierungsinstitutionen |
| `CodeSystem_PrimaryPurposeType.json` | Studienzwecke (Erweiterung des FHIR-Standard-ValueSets) |
| `CodeSystem_PublicationType.json` | Typen von Publikationen und Studienergebnissen |
| `CodeSystem_RecruitingStatus.json` | Rekrutierungsstatus-Werte des DRKS |
| `CodeSystem_StudyDesign.json` | Studiendesign-Codes (Erweiterung des SEVCO-Vokabulars) |
| `CodeSystem_TrialStatus.json` | Interner DRKS-Bearbeitungsstatus |

### `valuesets/` (11 Dateien)
ValueSets, die die jeweiligen CodeSystems binden und in den StructureDefinitions referenziert werden.

| Datei | Beschreibung |
|---|---|
| `ValueSet_AgeUnit.json` | Zulässige Alterseinheiten |
| `ValueSet_ArmType.json` | Zulässige Studienarm-Typen |
| `ValueSet_ContactType.json` | Zulässige Kontaktrollen |
| `ValueSet_EligibilityCriteria.json` | Zulässige Einschlusskriterien |
| `ValueSet_EthicVote.json` | Zulässige Ethikvoten |
| `ValueSet_InstituteType.json` | Zulässige Institutionstypen |
| `ValueSet_PrimaryPurposeType.json` | Zulässige Studienzwecke |
| `ValueSet_PublicationType.json` | Zulässige Publikationstypen |
| `ValueSet_RecruitingStatus.json` | Zulässige Rekrutierungsstatus-Werte |
| `ValueSet_StudyDesign.json` | Zulässige Studiendesign-Codes |
| `ValueSet_TrialStatus.json` | Zulässige DRKS-Bearbeitungsstatus-Werte |

### `extensions/` (7 Dateien)
StructureDefinitions für FHIR-Extensions, die DRKS-Felder ohne natives FHIR-Äquivalent abbilden.

| Datei | Beschreibung |
|---|---|
| `Extension_drks-ethics-committee.json` | Ethikkommission mit Antragsdatum, Votum und Kontaktdaten |
| `Extension_drks-hidden-allocation-description.json` | Freitext zur Art der verdeckten Zuteilung |
| `Extension_drks-is-main-condition.json` | Kennzeichnung als Haupt- oder Nebenproblem |
| `Extension_drks-recruiting-status-reason-description.json` | Begründung für Abbruch oder Rückzug der Rekrutierung |
| `Extension_drks-sequence-generation-description.json` | Freitext zur Art der Sequenzgenerierung |
| `Extension_drks-trial-result.json` | Ergebnisdaten (IPD-Sharing, Publikationsdaten) |
| `Extension_drks-trial-status.json` | Interner DRKS-Bearbeitungsstatus |

## Basis-URL

Alle Artefakte verwenden den Namensraum: https://www.imise.uni-leipzig.de/fhir/

## Validierung

Der Implementation Guide wurde mit dem **HL7 FHIR Validator 6.9.10** gegen die FHIR-R5-Basisspezifikation validiert und ergab **0 Errors**.

## Lizenz

Siehe [LICENSE](../LICENSE) im Root-Verzeichnis.
