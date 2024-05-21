using System.ComponentModel.DataAnnotations;

public class BidViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Veuillez entrer une mise.")]
    [Range(1, int.MaxValue, ErrorMessage = "Votre mise doit être supérieure au prix annoncé et à la mise la plus récente.")]
    public double Price { get; set; }
    public int ListingId { get; set; }
}

