using System.ComponentModel.DataAnnotations;

namespace DU_test.Model
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }
        public Boolean Status {  get; set; }
    }
}
