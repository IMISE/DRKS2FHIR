using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class FhirHelper
{
    internal static FhirString BuildTranslatedString(string? enText, string? deText)
    {
        var element = new FhirString(enText);
        if (deText is not null)
            AddTranslationExtension(element, "de", deText);
        return element;
    }

    internal static Markdown BuildTranslatedMarkdown(string? enText, string? deText)
    {
        var element = new Markdown(enText);
        if (deText is not null)
            AddTranslationExtension(element, "de", deText);
        return element;
    }

    internal static void AddTranslationExtension(PrimitiveType element, string lang, string content)
    {
        var translationExt = new Extension
        {
            Url = "http://hl7.org/fhir/StructureDefinition/translation"
        };
        translationExt.Extension.Add(new Extension("lang", new Code(lang)));
        translationExt.Extension.Add(new Extension("content", new FhirString(content)));
        element.Extension.Add(translationExt);
    }

    internal static Group.CharacteristicComponent? BuildAgeCharacteristic(
        AgeDto? min, AgeDto? max)
    {
        if (min is null && max is null) return null;

        var range = new Hl7.Fhir.Model.Range();

        if (min?.Unit is not null && min.Unit != AgeUnit.NO_MIN_AGE)
        {
            var (unit, code) = MapAgeUnit(min.Unit.Value);
            range.Low = new Quantity
            {
                Value = min.Amount,
                Unit = unit,
                System = "http://unitsofmeasure.org",
                Code = code
            };
        }

        if (max?.Unit is not null && max.Unit != AgeUnit.NO_MAX_AGE)
        {
            var (unit, code) = MapAgeUnit(max.Unit.Value);
            range.High = new Quantity
            {
                Value = max.Amount,
                Unit = unit,
                System = "http://unitsofmeasure.org",
                Code = code
            };
        }

        if (range.Low is null && range.High is null) return null;

        return new Group.CharacteristicComponent
        {
            Code = new CodeableConcept("http://loinc.org", "30525-0", "Age"),
            Value = range,
            Exclude = false
        };
    }

    internal static (string unit, string code) MapAgeUnit(AgeUnit unit) => unit switch
    {
        AgeUnit.DAYS => ("d", "d"),
        AgeUnit.WEEKS => ("wk", "wk"),
        AgeUnit.MONTHS => ("mo", "mo"),
        AgeUnit.YEARS => ("a", "a"),
        AgeUnit.WEEK_OF_PREGNANCY => ("weeks-of-pregnancy", "weeks-of-pregnancy"),
        _ => ("a", "a")
    };

    internal static Organization BuildOrganization(string id, DrksContact? contact)
    {
        var org = new Organization { Id = id };
        if (contact is null) return org;

        org.Name = contact.Affiliation;

        var extContact = new ExtendedContactDetail();
        bool hasContact = false;

        // Name
        if (contact.FirstName is not null || contact.LastName is not null)
        {
            extContact.Name =
            [
                new HumanName
            {
                Family = contact.LastName,
                Given = contact.FirstName is not null ? [contact.FirstName] : null,
                Prefix = contact.Title is not null ? [contact.Title] : null
            }
            ];
            hasContact = true;
        }

        // Telecom
        var telecoms = new List<ContactPoint>();
        if (contact.Email is not null)
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Email, Value = contact.Email });
        if (contact.Phone is not null)
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Phone, Value = contact.Phone });
        if (contact.Fax is not null)
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Fax, Value = contact.Fax });
        if (contact.Url is not null)
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Url, Value = contact.Url });

        if (telecoms.Count > 0)
        {
            extContact.Telecom = telecoms;
            hasContact = true;
        }

        // Adresse
        if (contact.StreetAndNo is not null || contact.City is not null ||
            contact.Zip is not null || contact.Country is not null)
        {
            extContact.Address = new Address
            {
                Line = contact.StreetAndNo is not null ? [contact.StreetAndNo] : null,
                City = contact.City,
                PostalCode = contact.Zip,
                Country = contact.Country
            };
            hasContact = true;
        }

        if (hasContact)
            org.Contact = [extContact];

        return org;
    }
}