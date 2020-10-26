namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysPosition")]
    public partial class sysPosition
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public int Order { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }
    }
}
