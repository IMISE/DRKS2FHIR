using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class StudyDesignMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.StudyCharacteristic is null) return;

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

    private static void MapStudyType(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Type is null) return;

        var (code, display) = sc.Type switch
        {
            StudyType.INTERVENTIONAL => ("SEVCO:01001", "Interventional research"),
            StudyType.NON_INTERVENTIONAL => ("SEVCO:01002", "Observational research"),
            _ => (null, null)
        };
        if (code is not null)
            study.StudyDesign.Add(new CodeableConcept(
                "http://hl7.org/fhir/study-design", code, display));
    }

    private static void MapAllocation(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Allocation is null) return;

        var (code, display) = sc.Allocation switch
        {
            StudyAllocation.RANDOMIZED_CONTROLLED_TRIAL => ("SEVCO:01003", "Randomized assignment"),
            StudyAllocation.NON_RANDOMIZED_CONTROLLED_TRIAL => ("SEVCO:01005", "Non-randomized assignment"),
            StudyAllocation.SINGLE_ARM_STUDY => ("SEVCO:01016", "Uncontrolled cohort design"),
            _ => (null, null)
        };
        if (code is not null)
            study.StudyDesign.Add(new CodeableConcept(
                "http://hl7.org/fhir/study-design", code, display));
    }

    private static void MapAssignment(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Assignment is null) return;

        var (code, display) = sc.Assignment switch
        {
            StudyAssignment.PARALLEL => ("SEVCO:01011", "Parallel cohort design"),
            StudyAssignment.CROSSOVER => ("SEVCO:01012", "Crossover cohort design"),
            StudyAssignment.SINGLE => ("SEVCO:01016", "Uncontrolled cohort design"),
            StudyAssignment.FACTORIAL => ("FACTORIAL_ASSIGNMENT", "Factorial assignment"),
            StudyAssignment.OTHER => ("OTHER_ASSIGNMENT", "Other assignment"),
            _ => (null, null)
        };
        if (code is not null)
        {
            var system = sc.Assignment is StudyAssignment.FACTORIAL or StudyAssignment.OTHER
                ? "https://www.imise.uni-leipzig.de/fhir/CodeSystem/StudyDesign"
                : "http://hl7.org/fhir/study-design";
            study.StudyDesign.Add(new CodeableConcept(system, code, display));
        }
    }

    private static void MapTiming(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Timing is null) return;

        var (code, display) = sc.Timing switch
        {
            StudyTiming.RETROSPECTIVE => ("retrospective-observation", "Retrospective observation"),
            StudyTiming.PROSPECTIVE => ("prospective-observation", "Prospective observation"),
            _ => (null, null)
        };
        if (code is not null)
            study.StudyDesign.Add(new CodeableConcept(
                "https://www.imise.uni-leipzig.de/fhir/CodeSystem/StudyDesign", code, display));
    }

    private static void MapDuration(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Duration is null) return;

        var (code, display) = sc.Duration switch
        {
            StudyDuration.CROSS_SECTIONAL => ("SEVCO:01027", "Cross sectional data collection"),
            StudyDuration.LONGITUDINAL => ("SEVCO:01028", "Longitudinal data collection"),
            _ => (null, null)
        };
        if (code is not null)
            study.StudyDesign.Add(new CodeableConcept(
                "http://hl7.org/fhir/study-design", code, display));
    }

    private static void MapBlinding(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.BlindingWhoList is null) return;

        foreach (var item in sc.BlindingWhoList)
        {
            var (code, display) = item.IdBlindingWho?.BlindingWho switch
            {
                BlindingWho.PATIENT_SUBJECT => ("SEVCO:01060", "Blinding of study participants"),
                BlindingWho.INVESTIGATOR_THERAPIST => ("SEVCO:01061", "Blinding of intervention providers"),
                BlindingWho.ASSESSOR => ("SEVCO:01062", "Blinding of outcome assessors"),
                BlindingWho.DATA_ANALYST => ("SEVCO:01063", "Blinding of data analysts"),
                BlindingWho.CAREGIVER => ("BLINDING_OF_CAREGIVER", "Blinding of caregiver"),
                _ => (null, null)
            };
            if (code is not null)
            {
                var system = item.IdBlindingWho?.BlindingWho is BlindingWho.CAREGIVER
                    ? "https://www.imise.uni-leipzig.de/fhir/CodeSystem/StudyDesign"
                    : "http://hl7.org/fhir/study-design";
                study.StudyDesign.Add(new CodeableConcept(system, code, display));
            }
        }
    }

    private static void MapPurpose(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Purpose is null) return;

        var (system, code, display) = sc.Purpose switch
        {
            StudyPurpose.TREATMENT => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "treatment", "Treatment"),
            StudyPurpose.PREVENTION => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "prevention", "Prevention"),
            StudyPurpose.DIAGNOSTIC => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "diagnostic", "Diagnostic"),
            StudyPurpose.SUPPORTIVE_CARE => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "supportive-care", "Supportive Care"),
            StudyPurpose.SCREENING => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "screening", "Screening"),
            StudyPurpose.HEALTH_CARE_SYSTEM => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "health-services-research", "Health Services Research"),
            StudyPurpose.BASIC_RESEARCH_PHYSIOLOGICAL_STUDY => ("http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type", "basic-science", "Basic Science"),
            StudyPurpose.PROGNOSIS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "PROGNOSIS", "Prognosis"),
            StudyPurpose.PHARMACOGENETICS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "PHARMACOGENETICS", "Pharmacogenetics"),
            StudyPurpose.PHARMACOGENOMICS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "PHARMACOGENOMICS", "Pharmacogenomics"),
            StudyPurpose.HEALTH_ECONOMICS => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "HEALTH_ECONOMICS", "Health economics"),
            StudyPurpose.OTHER => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/PurposeType", "OTHER", "Other"),
            _ => (null, null, null)
        };
        if (code is not null)
            study.PrimaryPurposeType = new CodeableConcept(system, code, display);
    }

    private static void MapPhase(StudyCharacteristic sc, ResearchStudy study)
    {
        if (sc.Phase is null) return;

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
        if (code is not null)
            study.Phase = new CodeableConcept(
                "http://terminology.hl7.org/CodeSystem/research-study-phase", code, display);
    }
}