using System.ComponentModel.DataAnnotations;

namespace DU_test.Model
{
    public class Address
    {
        public String Street {  get; set; }
        public String City { get; set; }
        [Required]
        public String PostalCode { get; set; }
        public String State { get; set; }

        public String? Company { get; set; }

    }
}
