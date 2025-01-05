using System.ComponentModel.DataAnnotations;

namespace Tictactoe.Authentication.ViewModels.Authorization;

public class AuthorizeViewModel
{
    [Display(Name = "Application")]
    public string? ApplicationName { get; init; }

    [Display(Name = "Scope")]
    public string? Scope { get; init; }
}
