using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("ordenes_produccion")]
    public class OrdenProduccion
    {
        [Key]
        [Column("id_orden")]
        public int Id { get; set; }

        [Column("id_producto")]
        public int ProductoId { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_entrega")]
        public DateTime? FechaEntrega { get; set; }

        public Producto Producto { get; set; }
        public ICollection<SeguimientoProduccion> Seguimientos { get; set; }
    }
}
