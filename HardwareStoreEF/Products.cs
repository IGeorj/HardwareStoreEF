namespace HardwareStoreEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Products()
        {
            Orders = new HashSet<Orders>();
        }

        [Key]
        public int ProductID { get; set; }
        public string Image { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        public int Amount { get; set; }

        public int Price { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }

        public int CompanyID { get; set; }

        public int? CategoryID { get; set; }

        public virtual Categories Categories { get; set; }

        public virtual Companies Companies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
