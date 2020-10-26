namespace APP.Models.Sys
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sysTournamentLevel")]
    public partial class sysTournamentLevel
    {
        public int ID { get; set; }

        public int? TournamentId { get; set; }

        public int? LevelId { get; set; }

        public int MinutePlay { get; set; }

        public double? SmallBlind { get; set; }

        public double? BigBlind { get; set; }

        public double? Ante { get; set; }

        public virtual sysTournament sysTournament { get; set; }
    }
}
