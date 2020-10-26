namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysPage")]
    public partial class sysPage
    {
        public int ID { get; set; }

        public int? ModuleId { get; set; }

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

        public virtual sysModule sysModule { get; set; }
    }
}
