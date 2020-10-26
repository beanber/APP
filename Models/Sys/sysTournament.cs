namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysTournament")]
    public partial class sysTournament
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sysTournament()
        {
            sysTournamentLevels = new HashSet<sysTournamentLevel>();
        }

        public int ID { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public decimal? PercentForStaff { get; set; }

        public decimal? PercentForPrize { get; set; }

        public decimal? Buyin_Price { get; set; }

        public decimal? Buyin_MoneyForFee { get; set; }

        public decimal? Buyin_MoneyForBounty { get; set; }

        public int? Buyin_NumberChip { get; set; }

        public decimal? Rebuy_Price { get; set; }

        public decimal? Rebuy_MoneyForFee { get; set; }

        public decimal? Rebuy_MoneyForBounty { get; set; }

        public int? Rebuy_NumberChip { get; set; }

        public int Rebuy_Max { get; set; }

        public decimal? Addon_Price { get; set; }

        public decimal? Addon_MoneyForFee { get; set; }

        public decimal? Addon_MoneyForBounty { get; set; }

        public int? Addon_NumberChip { get; set; }

        public int Addon_Max { get; set; }

        public int Order { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(150)]
        public string CreatedBy { get; set; }

        public DateTime? LastEditDate { get; set; }

        public string EditHistory { get; set; }

        public bool IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysTournamentLevel> sysTournamentLevels { get; set; }
    }
}
