using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class StudyDesignMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.StudyCharacteristic != null)
        {
            var sc = drks.StudyCharacteristic;
            study.StudyDesign = [];

            MapStudyType(sc, study);
            MapAllocation(sc, study);
            MapAssignment(sc, study);
            MapTiming(sc, study);
            MapDuration(sc, study);
            MapBlinding(sc, study);
            MapPurpose(sc, study);
            MapPhase(sc, study);
        }        
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        if (study.StudyDesign.Count > 0 || study.PrimaryPurposeType != null || study.Phase != null)
        {
            drks.StudyCharacteristic ??= new StudyCharacteristic();
            var sc = drks.StudyCharacteristic;

            foreach (var design in study.StudyDesign)
            {
                foreach (var coding in design.Coding)
                {
                    switch (coding.Code)
                    {
                        case "SEVCO:01001": sc.Type = StudyType.INTERVENTIONAL; break;
                        case "SEVCO:01002": sc.Type = StudyType.NON_INTERVENTIONAL; break;
                        case "SEVCO:01003": sc.Allocation = StudyAllocation.RANDOMIZED_CONTROLLED_TRIAL; break;
                        case "SEVCO:01005": sc.Allocation = StudyAllocation.NON_RANDOMIZED_CONTROLLED_TRIAL; break;
                        case "SEVCO:01011": sc.Assignment = StudyAssignment.PARALLEL; break;
                        case "SEVCO:01012": sc.Assignment = StudyAssignment.CROSSOVER; break;
                        case "SEVCO:01016": sc.Assignment = StudyAssignment.SINGLE; break;
                        case "SEVCO:01027": sc.Duration = StudyDuration.CROSS_SECTIONAL; break;
                        case "SEVCO:01028": sc.Duration = StudyDuration.LONGITUDINAL; break;
                        case "SEVCO:01060": EnsureBlindingWho(sc, BlindingWho.PATIENT_SUBJECT); break;
                        case "SEVCO:01061": EnsureBlindingWho(sc, BlindingWho.INVESTIGATOR_THERAPIST); break;
                        case "SEVCO:01062": EnsureBlindingWho(sc, BlindingWho.ASSESSOR); break;
                        case "SEVCO:01063": EnsureBlindingWho(sc, BlindingWho.DATA_ANALYST); break;
                        case "blinding-of-caregiver": EnsureBlindingWho(sc, BlindingWho.CAREGIVER); break;
                        case "factorial-assignment": sc.Assignment = StudyAssignment.FACTORIAL; break;
                        case "other-assignment": sc.Assignment = StudyAssignment.OTHER; break;
                        case "prospective-observation": sc.Timing = StudyTiming.PROSPECTIVE; break;
                        case "retrospective-observation": sc.Timing = StudyTiming.RETROSPECTIVE; break;
                    }
                }
            }

            sc.BlindingType = sc.BlindingWhoList?.Count > 0
                ? BlindingType.BLINDED
                : BlindingType.OPEN;

            if (study.Phase?.Coding.FirstOrDefault()?.Code is string phaseCode)
            {
                sc.Phase = phaseCode switch
                {
                    "n-a" => StudyPhase.N_A,
                    "early-phase-1" => StudyPhase.PHASE_0,
                    "phase-1" => StudyPhase.PHASE_I,
                    "phase-1-phase-2" => StudyPhase.PHASE_I_II,
                    "phase-2" => StudyPhase.PHASE_II,
                    "phase-2-phase-3" => StudyPhase.PHASE_II_III,
                    "phase-3" => StudyPhase.PHASE_III,
                    "phase-4" => StudyPhase.PHASE_IV,
                    _ => null
                };
            }

            if (study.PrimaryPurposeType?.Coding.FirstOrDefault()?.Code is string purposeCode)
            {
                sc.Purpose = purposeCode switch
                {
                    "treatment" => StudyPurpose.TREATMENT,
                    "prevention" => StudyPurpose.PREVENTION,
                    "diagnostic" => StudyPurpose.DIAGNOSTIC,
                    "supportive-care" => StudyPurpose.SUPPORTIVE_CARE,
                    "screening" => StudyPurpose.SCREENING,
                    "health-services-research" => StudyPurpose.HEALTH_CARE_SYSTEM,
                    "basic-science" => StudyPurpose.BASIC_RESEARCH_PHYSIOLOGICAL_STUDY,
                    "prognosis" => StudyPurpose.PROGNOSIS,
                    "pharmacogenetics" => StudyPurpose.PHARMACOGENETICS,
                    "pharmacogenomics" => StudyPurpose.PHARMACOGENOMICS,
                    "health-economics" => StudyPurpose.HEALTH_ECONOMICS,
                    _ => StudyPurpose.OTHER
                };
            }
        }
    }

    private static void MapStudyType(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Type != null)
        {
            var (code, display) = sc.Type switch
            {
                StudyType.INTERVENTIONAL => ("SEVCO:01001", "Interventional research"),
                StudyType.NON_INTERVENTIONAL => ("SEVCO:01002", "Observational research"),
                _ => (null, null)
            };
            if (code != null)
            {
                study.StudyDesign.Add(new CodeableConcept("http://hl7.org/fhir/study-design", code, display));
            }
        }
    }

    private static void MapAllocation(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Allocation != null)
        {
            var (code, display) = sc.Allocation switch
            {
                StudyAllocation.RANDOMIZED_CONTROLLED_TRIAL => ("SEVCO:01003", "Randomized assignment"),
                StudyAllocation.NON_RANDOMIZED_CONTROLLED_TRIAL => ("SEVCO:01005", "Non-randomized assignment"),
                StudyAllocation.SINGLE_ARM_STUDY => ("SEVCO:01016", "Uncontrolled cohort design"),
                _ => (null, null)
            };
            if (code != null)
            {
                study.StudyDesign.Add(new CodeableConcept("http://hl7.org/fhir/study-design", code, display));
            }
        }
    }

    private static void MapAssignment(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Assignment != null)
        {
            var (code, display) = sc.Assignment switch
            {
                StudyAssignment.PARALLEL => ("SEVCO:01011", "Parallel cohort design"),
                StudyAssignment.CROSSOVER => ("SEVCO:01012", "Crossover cohort design"),
                StudyAssignment.SINGLE => ("single-assignment", "Single assignment"),
                StudyAssignment.FACTORIAL => ("factorial-assignment", "Factorial assignment"),
                StudyAssignment.OTHER => ("other-assignment", "Other assignment"),
                _ => (null, null)
            };
            if (code != null)
            {
                var system = sc.Assignment is StudyAssignment.FACTORIAL or StudyAssignment.OTHER
                    ? "https://www.imise.uni-leipzig.de/fhir/CodeSystem/StudyDesign"
                    : "http://hl7.org/fhir/study-design";
                study.StudyDesign.Add(new CodeableConcept(system, code, display));
            }
        }
    }

    private static void MapTiming(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Timing != null)
        {
            var (code, display) = sc.Timing switch
            {
                StudyTiming.RETROSPECTIVE => ("retrospective-observation", "Retrospective observation"),
                StudyTiming.PROSPECTIVE => ("prospective-observation", "Prospective observation"),
                _ => (null, null)
            };
            if (code != null)
            {
                study.StudyDesign.Add(new CodeableConcept("https://www.imise.uni-leipzig.de/fhir/CodeSystem/StudyDesign", code, display));
            }
        }
    }

    private static void MapDuration(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Duration != null)
        {
            var (code, display) = sc.Duration switch
            {
                StudyDuration.CROSS_SECTIONAL => ("SEVCO:01027", "Cross sectional data collection"),
                StudyDuration.LONGITUDINAL => ("SEVCO:01028", "Longitudinal data collection"),
                _ => (null, null)
            };
            if (code != null)
            {
                study.StudyDesign.Add(new CodeableConcept("http://hl7.org/fhir/study-design", code, display));
            }
        }        
    }

    private static void MapBlinding(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.BlindingWhoList != null)
        {
            foreach (var item in sc.BlindingWhoList)
            {
                var (code, display) = item.IdBlindingWho?.BlindingWho switch
                {
                    BlindingWho.PATIENT_SUBJECT => ("SEVCO:01060", "Blinding of study participants"),
                    BlindingWho.INVESTIGATOR_THERAPIST => ("SEVCO:01061", "Blinding of intervention providers"),
                    BlindingWho.ASSESSOR => ("SEVCO:01062", "Blinding of outcome assessors"),
                    BlindingWho.DATA_ANALYST => ("SEVCO:01063", "Blinding of data analysts"),
                    BlindingWho.CAREGIVER => ("blinding-of-caregiver", "Blinding of caregiver"),
                    _ => (null, null)
                };
                if (code != null)
                {
                    var system = item.IdBlindingWho?.BlindingWho is BlindingWho.CAREGIVER
                        ? "https://www.imise.uni-leipzig.de/fhir/CodeSystem/StudyDesign"
                        : "http://hl7.org/fhir/study-design";
                    study.StudyDesign.Add(new CodeableConcept(system, code, display));
                }
            }
        }
    }

    private static void MapPurpose(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Purpose != null)
        {
            var (system, code, display) = sc.Purpose switch
            {
                StudyPurpose.TREATMENT => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "treatment", "Treatment"),
                StudyPurpose.PREVENTION => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "prevention", "Prevention"),
                StudyPurpose.DIAGNOSTIC => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "diagnostic", "Diagnostic"),
                StudyPurpose.SUPPORTIVE_CARE => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "supportive-care", "Supportive Care"),
                StudyPurpose.SCREENING => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "screening", "Screening"),
                StudyPurpose.HEALTH_CARE_SYSTEM => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "health-services-research", "Health Services Research"),
                StudyPurpose.BASIC_RESEARCH_PHYSIOLOGICAL_STUDY => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "basic-science", "Basic Science"),
                StudyPurpose.PROGNOSIS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "prognosis", "Prognosis"),
                StudyPurpose.PHARMACOGENETICS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "pharmacogenetics", "Pharmacogenetics"),
                StudyPurpose.PHARMACOGENOMICS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "pharmacogenomics", "Pharmacogenomics"),
                StudyPurpose.HEALTH_ECONOMICS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "health-economics", "Health economics"),
                StudyPurpose.OTHER => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "other", "Other"),
                _ => (null, null, null)
            };
            if (code != null)
            {
                study.PrimaryPurposeType = new CodeableConcept(system, code, display);
            }
        }
    }

    private static void MapPhase(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Phase != null)
        {
            var (code, display) = sc.Phase switch
            {
                StudyPhase.N_A => ("n-a", "N/A"),
                StudyPhase.PHASE_0 => ("early-phase-1", "Early Phase 1"),
                StudyPhase.PHASE_I => ("phase-1", "Phase 1"),
                StudyPhase.PHASE_I_II => ("phase-1-phase-2", "Phase 1/Phase 2"),
                StudyPhase.PHASE_II => ("phase-2", "Phase 2"),
                StudyPhase.PHASE_II_III => ("phase-2-phase-3", "Phase 2/Phase 3"),
                StudyPhase.PHASE_III => ("phase-3", "Phase 3"),
                StudyPhase.PHASE_IV => ("phase-4", "Phase 4"),
                _ => (null, null)
            };
            if (code != null)
            {
                study.Phase = new CodeableConcept("http://terminology.hl7.org/CodeSystem/research-study-phase", code, display);
            }
        }
    }

    private static void EnsureBlindingWho(StudyCharacteristic sc, BlindingWho who)
    {
        sc.BlindingWhoList ??= [];
        if (!sc.BlindingWhoList.Any(b => b.IdBlindingWho?.BlindingWho == who))
        {
            sc.BlindingWhoList.Add(new BlindingWhoItem
            {
                IdBlindingWho = new IdBlindingWho { BlindingWho = who }
            });
        }
    }
}