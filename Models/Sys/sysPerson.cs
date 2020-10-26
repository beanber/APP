namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysPerson")]
    public partial class sysPerson
    {
        public int ID { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(150)]
        public string NickName { get; set; }

        [StringLength(50)]
        public string FName { get; set; }

        [StringLength(50)]
        public string MName { get; set; }

        [StringLength(50)]
        public string LName { get; set; }

        [StringLength(150)]
        public string Avata { get; set; }

        public DateTime? DOB { get; set; }

        public int? SexId { get; set; }

        [StringLength(50)]
        public string Mobile { get; set; }

        [StringLength(50)]
        public string Telephone { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        public int? CountryId { get; set; }

        public int? ProvinceId { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(20)]
        public string IdentityNo { get; set; }

        public DateTime? IdentityDate { get; set; }

        [StringLength(250)]
        public string IdentityAddress { get; set; }

        [StringLength(250)]
        public string IdentityPlace { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(150)]
        public string CreatedBy { get; set; }

        public DateTime? LastEditDate { get; set; }

        public string EditHistory { get; set; }

        public bool IsDeleted { get; set; }

        public virtual sysCountry sysCountry { get; set; }

        public virtual sysProvince sysProvince { get; set; }

        public virtual sysSex sysSex { get; set; }
    }
}
