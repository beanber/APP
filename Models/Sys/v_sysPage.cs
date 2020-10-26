namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_sysPage
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? ModuleId { get; set; }

        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(267)]
        public string Avata { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(267)]
        public string Link { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Order { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(20)]
        public string ModuleCode { get; set; }

        [StringLength(250)]
        public string ModuleName { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ModuleOrder { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool IsDeleted { get; set; }
    }
}
