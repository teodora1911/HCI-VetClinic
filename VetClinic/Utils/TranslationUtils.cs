using System;
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

        // Dialogs Messages
        public string DeleteConfirmationString { get; set; }
        public string YesNoDialogConfirmationString { get; set; }
        public string YesNoDialogRejectionString { get; set; }
        public string EmptyFieldsErrorMessage { get; set; }
        public string UsernameIsNotAvailable { get; set; }
        public string DecimalStringFormatException { get; set; }
        public string InternalServerError { get; set; }
        public string AppointmentIsScheduled { get; set; }

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
            PersonFullNameHeader = "Ime i prezime",
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
            AddressHeader = "Adresa"
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
            PersonFullNameHeader = "Name and Surname",
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
            AddressHeader = "Address"
        };
    }
}
