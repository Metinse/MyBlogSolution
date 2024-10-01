﻿using FluentValidation;
using MyBlog.Entities;

namespace MyBlog.Business.Validators
{
    public class ArticleValidator : AbstractValidator<Article>
    {
        public ArticleValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(70).WithMessage("Title cannot exceed 70 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters long");

            RuleFor(x => x.CategoryID)
                .GreaterThan(0).WithMessage("CategoryID must be greater than 0");
        }
    }
}
