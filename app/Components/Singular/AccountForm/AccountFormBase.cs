using app.DTOs;
using app.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace app.Bases;

public class AccountFormBase : ComponentBase
{
    [Inject]
    protected IValidator<SignInRequest>? SignInValidator { get; set; }
    [Inject]
    protected IValidator<SignUpRequest>? SignUpValidator { get; set; }
    [Inject]
    protected AuthenticationService? AuthenticationService { get; set; }
    [Inject]
    protected SessionService? SessionService { get; set; }

    protected bool _signInOrSignUp { get; set; } = true; // true = sign in, false = sign up

    // Inmatningsfältens bindande variabler:
    protected string _username = string.Empty;
    protected string _email = string.Empty;
    protected string _password = string.Empty;
    protected string _timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault().Id; // Förifyllt med systemets tidszon.
    protected bool _showExplicitAnimes = false;
    protected bool _allowReminders = false;

    protected IEnumerable<string> _timeZones = DateTimeZoneProviders.Tzdb.Ids; // Används för att fylla selectboxen med tidszoner.
    protected Dictionary<string, string[]> errors = []; // Dictionary för att lagra valideringsfel, där nyckeln är fältnamnet och värdet är en array av felmeddelanden.
    protected string _successMessage = string.Empty;

    // Hanterar inloggning av användaren. Validerar först inmatningen och anropar sedan AuthenticationService för att logga in.
    // Om det lyckas, sparas profilinformationen i sessionen. Om det misslyckas, visas ett felmeddelande.
    protected async Task SignIn()
    {
        SignInRequest request = new()
        {
            Email = _email,
            Password = _password
        };

        FluentValidation.Results.ValidationResult result = await SignInValidator!.ValidateAsync(request);

        if (!result.IsValid)
        {
            ErrorsToDictionary(result);
            return;
        }

        ApiResult<ProfileResponse> response = await AuthenticationService!.SignIn(request);
        if (response.IsSuccess)
        {
            _successMessage = "Login successful!";
            errors.Clear();
            await SessionService!.SetSessionProfile(response.Data!);
        }
        else
        {
            _successMessage = string.Empty;
            errors = new Dictionary<string, string[]>
            {
                { "Authentication", new[] { response.Error?.Details[0] ?? "An unknown error occurred." } }
            };
        }
    }

    // Hanterar registrering av användaren. Validerar först inmatningen och anropar sedan AuthenticationService för att skapa ett nytt konto.
    // Om det lyckas, sparas profilinformationen i sessionen. Om det misslyckas, visas ett felmeddelande.
    protected async Task SignUp()
    {
        SignUpRequest request = new()
        {
            Username = _username,
            Email = _email,
            Password = _password,
            InitialSettings = new UserSettings
            {
                TimeZone = _timeZone,
                ShowExplicitAnime = _showExplicitAnimes,
                AllowReminders = _allowReminders
            }
        };

        FluentValidation.Results.ValidationResult result = await SignUpValidator!.ValidateAsync(request);

        if (!result.IsValid)
        {
            ErrorsToDictionary(result);
            return;
        }

        ApiResult<ProfileResponse> response = await AuthenticationService!.SignUp(request);
        if (response.IsSuccess)
        {
            _successMessage = "Registration successful!";
            errors.Clear();
            await SessionService!.SetSessionProfile(response.Data!);
        }
        else
        {
            _successMessage = string.Empty;
            errors = new Dictionary<string, string[]>
            {
                { "Authentication", new[] { response.Error?.Details[0] ?? "An unknown error occurred." } }
            };
        }
    }

    // Växlar mellan inloggnings- och registreringsformuläret. Rensar även eventuella felmeddelanden och framgångsmeddelanden när användaren byter formulär.
    protected void ToggleSignInOrSignUp()
    {
        _signInOrSignUp = !_signInOrSignUp;
        errors.Clear();
        _successMessage = string.Empty;
    }

    // Omvandlar FluentValidations ValidationResult till en Dictionary som kan användas för att visa felmeddelanden i UI:t.
    private void ErrorsToDictionary(FluentValidation.Results.ValidationResult validationResult)
    {
        errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}