using System.ComponentModel.DataAnnotations;

namespace DU_test.Model
{
    public class JobOffer
    {
        public int id {  get; set; }
        public int JobId {  get; set; }
        public string JobOfferId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime datetime { get; set;}
        public string description { get; set;}
        public Address Pickup{ get; set; }
        public Address Dropoff { get; set; }
        public Payment Payment { get; set; }
    }
}
