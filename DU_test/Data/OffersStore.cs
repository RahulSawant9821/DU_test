using DU_test.Model;

namespace DU_test.Data
{
    public class OffersStore
    {
        public static List<JobOffer> Offers = new List<JobOffer> {
            new JobOffer {id=1,
                JobId=301,
                JobStatus="Open",
                JobOfferId="UEB12",
                datetime = DateTime.Parse("2023-12-13"),
                description = " Uber eats birmingham",
                Payment = new Payment {
                    PaymentId = "U213",
                    PaymentAmount = 240,
                    PaymentTerms= "Deliever within an hour"
                },
                Pickup= new Address{
                    Street = "123 Main Street",
                    City="Birmingham",
                    PostalCode = "B12 3AB",
                    State = "West Midlands",
                    Company = "Uber Eats"
                },
                Dropoff = new Address{
                    Street = "123 Main Street",
                    City="Birmingham",
                    PostalCode = "B12 3AB",
                    State = "West Midlands",
                    Company=null
            } },
            new JobOffer {id=2,
                JobId=301,
                JobStatus="Open",
                JobOfferId="DS52",
                datetime = DateTime.Parse("2023-12-1"),
                description = " Delivoro birmingham",
                Payment = new Payment {
                    PaymentId = "S213",
                    PaymentAmount = 280,
                    PaymentTerms= "Deliever within an hour and half"
                },
                 Pickup= new Address{
                    Street = "123 Main Street",
                    City="Birmingham",
                    PostalCode = "B12 3AB",
                    State = "West Midlands",
                    Company = "Uber Eats"
                },
                 Dropoff = new Address{
                    Street = "123 Main Street",
                    City="Birmingham",
                    PostalCode = "B12 3AB",
                    State = "West Midlands",
                    Company=null
            }
            },
            new JobOffer {id=3,
                JobId=111,
                JobStatus="In-Process",
                JobOfferId="UEG12",
                datetime = DateTime.Parse("2023-12-23"),
                description = " Uber eats Glassgow",
                Payment = new Payment {
                    PaymentId = "S213",
                    PaymentAmount = 140,
                    PaymentTerms= "Deliever within an hour"
                },
                Pickup= new Address{
                    Street = "123 Main Street",
                    City="Glassgow",
                    PostalCode = "G12 3AB",
                    State = "West ",
                    Company = "Uber Eats"
                },
                Dropoff = new Address{
                    Street = "123 Main Street",
                    City="Glassgow",
                    PostalCode = "G12 3AB",
                    State = "West ",
                    Company=null
            }
            }

        };
    }
}
