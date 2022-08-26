using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProductType
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Display(Name = "Data ostatniej modyfikacji")]
    [Required(ErrorMessage = "Pole '{0}' jest wymagane")]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime EditDate { get; set; }
    [Display(Name = "Produkt")]
    public string Name { get; set; }
    [Display(Name = "Ilość")]
    public int Quantity { get; set; }
    [Display(Name = "Cena")]
    public decimal Prize { get; set; }
    public bool IsHidden { get; set; }

}
