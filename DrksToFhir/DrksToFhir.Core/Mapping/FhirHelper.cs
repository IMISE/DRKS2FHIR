using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class FhirHelper
{
    internal static FhirString BuildTranslatedString(string? enText, string? deText)
    {
        var element = new FhirString(enText);
        if (deText != null)
        {
            AddTranslationExtension(element, "de", deText);
        }            
        return element;
    }

    internal static Markdown BuildTranslatedMarkdown(string? enText, string? deText)
    {
        var element = new Markdown(enText);
        if (deText != null)
        {
            AddTranslationExtension(element, "de", deText);
        }
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
        if (min == null && max == null)
        {
            return null;
        }

        var range = new Hl7.Fhir.Model.Range();

        if (min?.Unit != null && min.Unit != AgeUnit.NO_MIN_AGE)
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

        if (max?.Unit != null && max.Unit != AgeUnit.NO_MAX_AGE)
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

        if (range.Low == null && range.High == null)
        {
            return null;
        }

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
        if (contact == null)
        {
            return org;
        }

        org.Name = contact.Affiliation;

        var extContact = new ExtendedContactDetail();
        bool hasContact = false;

        if (contact.FirstName != null || contact.LastName != null)
        {
            extContact.Name =
            [
                new HumanName
            {
                Family = contact.LastName,
                Given = contact.FirstName != null ? [contact.FirstName] : null,
                Prefix = contact.Title != null ? [contact.Title] : null
            }
            ];
            hasContact = true;
        }

        var telecoms = new List<ContactPoint>();
        if (contact.Email != null)
        {
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Email, Value = contact.Email });
        }            
        if (contact.Phone != null)
        {
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Phone, Value = contact.Phone });
        }            
        if (contact.Fax != null)
        {
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Fax, Value = contact.Fax });
        }            
        if (contact.Url != null)
        {
            telecoms.Add(new ContactPoint { System = ContactPoint.ContactPointSystem.Url, Value = contact.Url });
        }            

        if (telecoms.Count > 0)
        {
            extContact.Telecom = telecoms;
            hasContact = true;
        }

        if (contact.StreetAndNo != null || contact.City != null || contact.Zip != null || contact.Country != null)
        {
            extContact.Address = new Address
            {
                Line = contact.StreetAndNo != null ? [contact.StreetAndNo] : null,
                City = contact.City,
                PostalCode = contact.Zip,
                Country = contact.Country
            };
            hasContact = true;
        }

        if (hasContact)
        {
            org.Contact = [extContact];
        }            

        return org;
    }
}