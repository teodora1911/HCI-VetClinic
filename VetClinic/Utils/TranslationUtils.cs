﻿using System;
using System.IO.Packaging;

namespace VetClinic.Utils
{
    public class TranslationUtils
    {
         public Lang Language { get; set; }
    }

    public class Lang
    {
        public string AppointmentsButton { get; set; }
        public string MedicineButton { get; set; }
        public string LogoutButton { get; set; }
        public string ServicesButton { get; set; }
        public string PetAndOwnersButton { get; set; }
        public string VeterinariansButton { get; set; }
        public string VetAppointmentsButton { get; set; }
        public string ExaminationsButton { get; set; }
        public string PrescriptionsButton { get; set; }

        public string SearchString { get; set; }
        public string PersonSearchString { get; set; }
        public string PetSearchString { get; set; }
        public string CloseWindowString { get; set; }
        public string ClearSearchQueryString { get; set; }  
        public string MedicineSearchString { get; set; }
        public string ServiceSearchString { get; set; }
        public string AddNewItem { get; set; }

        public string CostHint { get; set; }

        public string GenderMale { get; set; }
        public string GenderFemale { get; set; }
        public string ScheduledString { get; set; }
        public string NonScheduledString { get; set; }
        public string CompletedString { get; set; }
        public string NonCompletedString { get; set; }
        public string CashPayment { get; set; }
        public string CardPayment { get; set; }
        public string PaymentType { get; set; }

        // Data Grids Column Headers
        public string IdHeader { get; set; }
        public string PersonNameHeader { get; set; }
        public string PersonSurnameHeader { get; set; }
        public string PersonFullNameHeader { get; set; }
        public string PersonUsernameHeader { get; set; }
        public string PersonPasswordHeader { get; set; }
        public string PersonEmailHeader { get; set; }
        public string PersonContactHeader { get; set; }
        public string VeterinarianTitleHeader { get; set; }
        public string NonPersonNameHeader { get; set; }
        public string TypeHeader { get; set; }
        public string DescriptionHeader { get; set; }
        public string CostHeader { get; set; }
        public string EstimatedAgeHeader { get; set; }
        public string WeightHeader { get; set; }
        public string GenderHeader { get; set; }
        public string HealthConditionHeader { get; set; }
        public string DiagnosisHeader { get; set; }
        public string SpeciesHeader { get; set; }
        public string BreedHeader { get; set; }
        public string DateTimeHeader { get; set; }
        public string ReasonHeader { get; set; }
        public string PetAppExamHeader { get; set; }
        public string OwnerAppExamHeader { get; set; }
        public string VetAppExamHeader { get; set; }
        public string AddressHeader { get; set; }
        public string QuantityHeader { get; set; }

        public string InstructionsHeader { get; set; }
        public string DoseHeader { get; set; }
        public string FrequencyHeader { get; set; }
        public string DurationHeader { get; set; }

        public string DateHint { get; set; }
        public string TimeHint { get; set; }

        // Dialogs Messages
        public string DeleteConfirmationString { get; set; }
        public string YesNoDialogConfirmationString { get; set; }
        public string YesNoDialogRejectionString { get; set; }
        public string EmptyFieldsErrorMessage { get; set; }
        public string UsernameIsNotAvailable { get; set; }
        public string DecimalStringFormatException { get; set; }
        public string NaturalNumberStringFormatException { get; set; }
        public string InternalServerError { get; set; }
        public string AppointmentIsScheduled { get; set; }
        public string ScheduleAppointmentConfirmationString { get; set; }
        public string CloseEditingConfirmationString { get; set; }
        public string NotFoundExceptionMessage { get; set; }

        public static Lang SerbianLang = new Lang()
        {
            AppointmentsButton = "Zakazivanje pregleda",
            MedicineButton = "Lijekovi",
            LogoutButton = "Odjava sa sistema",
            ServicesButton = "Usluge",
            PetAndOwnersButton = "Životinje & vlasnici",
            VeterinariansButton = "Veterinari",
            SearchString = "Traži",
            PersonSearchString = "Npr. Gogi Gogić",
            CloseWindowString = "Zatvori prozor",
            ClearSearchQueryString = "Obriši",
            AddNewItem = "Kreiraj",
            IdHeader = "R.B.",
            PersonNameHeader = "Ime",
            PersonSurnameHeader = "Prezime",
            PersonUsernameHeader = "Korisničko ime",
            PersonPasswordHeader = "Lozinka",
            PersonEmailHeader = "E-Pošta",
            PersonContactHeader = "Kontakt broj",
            VeterinarianTitleHeader = "Specijalizacija",
            DeleteConfirmationString = "Da li ste sigurni da želite da izbrišete podatke?",
            YesNoDialogConfirmationString = "Potvrdi",
            YesNoDialogRejectionString = "Otkaži",
            EmptyFieldsErrorMessage = "Polja ne mogu da budu prazna!",
            UsernameIsNotAvailable = "Korisničko ime je zauzeto. Pokušajte ponovo.",
            NonPersonNameHeader = "Naziv",
            TypeHeader = "Tip",
            DescriptionHeader = "Detaljan opis",
            InternalServerError = "Desila se greška prilikom izvršavanja akcije.",
            MedicineSearchString = "Npr. Insulin",
            CostHeader = "Cijena",
            ServiceSearchString = "Npr. Hirurgija",
            CostHint = "Npr. 123.456,99",
            DecimalStringFormatException = "Niste unijeli ispravan format broja. Pokušajte npr. 123.456,99",
            PersonFullNameHeader = "Ime & prezime",
            EstimatedAgeHeader = "Starost [u mjesecima]",
            WeightHeader = "Težina",
            GenderHeader = "Pol",
            HealthConditionHeader = "Zdravstveno stanje",
            DiagnosisHeader = "Dijagnoza",
            SpeciesHeader = "Vrsta",
            BreedHeader = "Rasa",
            GenderMale = "Muško",
            GenderFemale = "Žensko",
            PetSearchString = "Npr. Floki",
            ScheduledString = "Zakazani",
            NonScheduledString = "Nezakazani",
            DateTimeHeader = "Datum & Vrijeme",
            ReasonHeader = "Razlog pregleda",
            PetAppExamHeader = "Ljubimac",
            OwnerAppExamHeader = "Vlasnik",
            VetAppExamHeader = "Veterinar",
            AppointmentIsScheduled = "Pregled je već zakazan.",
            AddressHeader = "Adresa",
            ExaminationsButton = "Pregledi",
            VetAppointmentsButton = "Pregledi na čekanju",
            ScheduleAppointmentConfirmationString = "Zakaži",
            DateHint = "Datum",
            TimeHint = "Vrijeme",
            CompletedString = "Završen",
            NonCompletedString = "Zakazan",
            QuantityHeader = "Količina",
            CloseEditingConfirmationString = "Da li želite da završite pregled?",
            InstructionsHeader = "Instrukcije za primjenu",
            DoseHeader = "Doza",
            FrequencyHeader = "Učestalost",
            DurationHeader = "Vremenski period",
            NaturalNumberStringFormatException = "Molimo Vas unesite prirodan broj.",
            PrescriptionsButton = "Recepti",
            CashPayment = "Gotovina",
            CardPayment = "Kreditna kartica",
            PaymentType = "Način plaćanja",
            NotFoundExceptionMessage = "Podaci nisu pronađeni."
        };

        public static Lang EnglishLang = new Lang()
        {
            AppointmentsButton = "Schedule Appointment",
            MedicineButton = "Medicine",
            LogoutButton = "Logout",
            ServicesButton = "Services",
            PetAndOwnersButton = "Pets & Owners",
            VeterinariansButton = "Veterinarians",
            SearchString = "Search",
            PersonSearchString = "e.g. Anna May",
            CloseWindowString = "Close Window",
            ClearSearchQueryString = "Erase Search Query",
            AddNewItem = "Create new item",
            IdHeader = "ID",
            PersonNameHeader = "Name",
            PersonSurnameHeader = "Surname",
            PersonUsernameHeader = "Username",
            PersonPasswordHeader = "Password",
            PersonEmailHeader = "E-Mail",
            PersonContactHeader = "Phone Number",
            VeterinarianTitleHeader = "Title",
            DeleteConfirmationString = "Are you sure you want to delete data?",
            YesNoDialogConfirmationString = "Accept",
            YesNoDialogRejectionString = "Cancel",
            EmptyFieldsErrorMessage = "Fields cannot be empty!",
            UsernameIsNotAvailable = "Username is taken. Try again.",
            NonPersonNameHeader = "Name",
            TypeHeader = "Type",
            DescriptionHeader = "Detailed Description",
            InternalServerError = "Internal Server Error",
            MedicineSearchString = "e.g. Insulin",
            CostHeader = "Cost",
            ServiceSearchString = "e.g. Therapy",
            CostHint = "e.g. 123.456,99",
            DecimalStringFormatException = "Bad decimal number format.Try, for example, 123.456,99",
            PersonFullNameHeader = "Name & Surname",
            EstimatedAgeHeader = "Age [in months]",
            WeightHeader = "Weight",
            GenderHeader = "Gender",
            HealthConditionHeader = "Health Condition",
            DiagnosisHeader = "Diagnosis",
            SpeciesHeader = "Species",
            BreedHeader = "Breed",
            GenderFemale = "Female",
            GenderMale = "Male",
            PetSearchString = "e.g. Chase",
            ScheduledString = "Scheduled",
            NonScheduledString = "Not Scheduled",
            DateTimeHeader = "Date & Time",
            ReasonHeader = "Entry Reason",
            PetAppExamHeader = "Pet",
            OwnerAppExamHeader = "Owner",
            VetAppExamHeader = "Veterinarian",
            AppointmentIsScheduled = "Appointment is already scheduled.",
            AddressHeader = "Address",
            ExaminationsButton = "Examinations",
            VetAppointmentsButton = "Appointments on Hold",
            ScheduleAppointmentConfirmationString = "Schedule",
            DateHint = "Date",
            TimeHint = "Time",
            CompletedString = "Completed",
            NonCompletedString = "Scheduled",
            QuantityHeader = "Quantity",
            CloseEditingConfirmationString = "Do you want to complete this examination?",
            InstructionsHeader = "Instructions",
            DoseHeader = "Dose",
            FrequencyHeader = "Frequency",
            DurationHeader = "Duration",
            NaturalNumberStringFormatException = "Please enter natural number.",
            PrescriptionsButton = "Prescriptions",
            CashPayment = "Cash",
            CardPayment = "Credit Card",
            PaymentType = "Payment Type",
            NotFoundExceptionMessage = "Data not Found (404)"
        };
    }
}
