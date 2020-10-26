namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysProvince")]
    public partial class sysProvince
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sysProvince()
        {
            sysDistricts = new HashSet<sysDistrict>();
            sysPersons = new HashSet<sysPerson>();
        }

        public int ID { get; set; }

        public int? CountryId { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        public int Order { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public virtual sysCountry sysCountry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysDistrict> sysDistricts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysPerson> sysPersons { get; set; }
    }
}
