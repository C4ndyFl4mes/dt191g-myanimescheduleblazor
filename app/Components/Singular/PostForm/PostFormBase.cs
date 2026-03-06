using app.DTOs;
using app.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class PostFormBase : ComponentBase
{
    [Inject]
    protected IValidator<PostRequest>? PostRequestValidator { get; set; }
    [Inject]
    protected PostService? PostService { get; set; }

    [Parameter]
    public required int TargetID { get; set; }
    [Parameter]
    public EventCallback<bool> OnPost { get; set; }

    protected Dictionary<string, string[]> _errors = [];
    protected string? _successMessage;
    protected PostRequest? _postForm;

    protected override async Task OnInitializedAsync()
    {
        _postForm = new()
        {
            TargetID = TargetID,
            Content = ""
        };
    }

    // Lägger upp ett inlägg.
    protected async Task Post()
    {
        _errors = [];
        _successMessage = null;

        if (PostService is null)
        {
            _errors = new()
            {
                { "Gemeral", new[] { "PostService is unavailable." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (_postForm is null)
        {
            _errors = new()
            {
                { "General", new[] { "Post fields are null." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (PostRequestValidator is null)
        {
            _errors = new()
            {
                { "General", new[] { "Validator is unavailable." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        FluentValidation.Results.ValidationResult validationResult = PostRequestValidator.Validate(_postForm);
        if (!validationResult.IsValid)
        {
            ErrorsToDictionary(validationResult);
            await InvokeAsync(StateHasChanged);
            return;
        }

        ApiResult<SuccessfulResponse> result = await PostService.Post(_postForm);
        if (!result.IsSuccess)
        {
            _successMessage = null;
            _errors = new Dictionary<string, string[]>
            {
                { "General", new[] { result.Error?.Details[0] ?? "An unknown error occurred." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (result.Data is null)
        {
            _errors = new()
            {
                { "General", new[] { "An error occured regarding the incoming response message." } }
            };
            await InvokeAsync(StateHasChanged);
            return;
        }

        _errors = [];
        _successMessage = result.Data.Message;
        _postForm = new()
        {
            TargetID = TargetID,
            Content = ""
        };

        if (OnPost.HasDelegate)
        {
            await OnPost.InvokeAsync(true);
        }
        await InvokeAsync(StateHasChanged);
    }

    protected static string SelectIcon(string icon)
    {
        return $"icons/{icon}.svg";
    }

    // För att få fram felmeddelanden som en Dictionary där felen är grupperad efter egenskap.
    private void ErrorsToDictionary(FluentValidation.Results.ValidationResult validationResult)
    {
        _errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}