using System.Security.Cryptography.X509Certificates;

namespace DataAccess.Models
{
    public class ValueRecord
    {
        public Guid Id {  get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
