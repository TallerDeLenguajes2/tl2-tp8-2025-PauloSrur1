using System;
using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_PauloSrur1.ViewModels
{
    public class PresupuestoViewModel
    {
        public int IdPresupuesto { get; set; }

        [Display(Name = "Nombre o Email del Destinatario")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        // [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string? NombreDestinatario { get; set; }

        [Display(Name = "Fecha de Creación")]
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }
    }
}
