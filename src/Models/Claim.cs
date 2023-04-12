using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Models
{
    public class Claim
    {
        private IList<WholesaleClaimItem> _partyClaims;
        private DateTime _createdDate;
        private int _year;
        private int _month;

        public Claim()
        {
            _createdDate = DateTime.Now;
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
            _partyClaims = new List<WholesaleClaimItem>();
        }

        public Claim(DateTime createdDate, int year, int month)
        {
            CreatedDate = createdDate;
            Year = year;
            Month = month;
            _partyClaims = new List<WholesaleClaimItem>();
        }

        public Claim(int year, int month)
        {
            _createdDate = DateTime.Now;
            Year = year;
            Month = month;
            _partyClaims = new List<WholesaleClaimItem>();
        }

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

        public IEnumerable<WholesaleClaimItem> GetPartyClaims()
        {
            return _partyClaims;
        }

        public void AddPartyClaim(WholesaleClaimItem party_claim)
        {
            int sn = _partyClaims.Count;
            party_claim.SN = sn + 1;
            _partyClaims.Add(party_claim);
        }

        public void AddPartyClaims(IEnumerable<WholesaleClaimItem> partyClaims)
        {
            foreach (var claim in partyClaims)
            {
                AddPartyClaim(claim);
            }
        }
    }

    public class WholesaleClaimItem
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
