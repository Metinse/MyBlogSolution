using FluentValidation;
using MyBlog.Entities.DTOs;
public class GalleryDTOValidator : AbstractValidator<GalleryDTO>
{
    public GalleryDTOValidator()
    {
        RuleFor(x => x.Title).MaximumLength(150).When(x => !string.IsNullOrEmpty(x.Title)); // Zorunlu değil
        RuleFor(x => x.Type).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Type));   // Zorunlu değil
        RuleFor(x => x.Description).MaximumLength(300).When(x => !string.IsNullOrEmpty(x.Description)); // Zorunlu değil
    }
}