using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Models
{
    public class Claim
    {
        private string _companyName;
        private string _address;
        private IList<string> _phoneNumber;
        private IList<CliamItem> _partyClaims;
        private DateTime _createdDate;
        private int _year;
        private int _month;

        public Claim(string companyName, string address = "", DateTime? createdDate = null, int? year = null, int? month = null)
        {
            if(createdDate == null)
            {
                createdDate = DateTime.Now;
            }
            _createdDate = createdDate??DateTime.Now;
            Year = year ?? DateTime.Now.Year;
            Month = month ?? DateTime.Now.Month;
            _partyClaims = new List<CliamItem>();
            _companyName = companyName.Trim().ToUpper();
            _address = address.Trim().ToUpper();
            _phoneNumber = new List<string>();
        }

        public string CompanyName { get { return _companyName; } }

        public string Address { get { return _address; } }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            private set { _createdDate = value; }
        }
        public int Year
        {
            get => _year;
            private set => _year = value;
        }

        public int Month
        {
            get => _month;
            private set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentOutOfRangeException("Value of Month should be between 1 and 12");
                _month = value;
            }
        }

        public decimal TotalClaimAmount
        {
            get => decimal.Round(_partyClaims.Sum(x => x.ClaimAmount), 2, MidpointRounding.ToPositiveInfinity);
        }

        public IEnumerable<CliamItem> GetPartyClaims()
        {
            return _partyClaims;
        }

        public void AddPartyClaim(CliamItem party_claim)
        {
            int sn = _partyClaims.Count;
            party_claim.SN = sn + 1;
            _partyClaims.Add(party_claim);
        }

        public void AddPartyClaims(IEnumerable<CliamItem> partyClaims)
        {
            foreach (var claim in partyClaims)
            {
                AddPartyClaim(claim);
            }
        }
        public void AddPhoneNumber(string phoneNumber)
        {
            var numbers = phoneNumber.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach(var number in numbers)
            {
                _phoneNumber.Add(number);
            }
        }

        public string GetPhoneNumber()
        {
            return String.Join(",", _phoneNumber);
        }
    }

    public class CliamItem
    {
        public int SN { get; set; }
        public string Party { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal ClaimAmount
        {
            get => decimal.Round(Amount * (Discount), 2, MidpointRounding.ToPositiveInfinity);
            set => this.Amount = decimal.Round(value / (Discount), 2, MidpointRounding.ToPositiveInfinity);
        }
    }
}
