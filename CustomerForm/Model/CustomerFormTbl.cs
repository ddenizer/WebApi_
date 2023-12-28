using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerForm.Model
{
    public class CustomerFormTbl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerFormId { get; set; }
        public string FullName { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Explantion { get; set; }
        public Guid Guid { get; set; }
        public bool Kvkk { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAddress { get; set; }
        public string TableName { get; set; }
        public int CustomerResultId { get; set; }
        public int Active { get; set; }
        public string ResourceUrl { get; set; }
        public string SpeColumn1 { get; set; }
        public string SpeColumn2 { get; set; }
        public string SpeColumn3 { get; set; }
        public string SpeColumn4 { get; set; }
        public string SpeColumn5 { get; set; }
    }
}
