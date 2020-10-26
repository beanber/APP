namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysModule")]
    public partial class sysModule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sysModule()
        {
            sysPages = new HashSet<sysPage>();
        }

        public int ID { get; set; }

        [StringLength(250)]
        public string Avata { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Link { get; set; }

        public int Order { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysPage> sysPages { get; set; }
    }
}
