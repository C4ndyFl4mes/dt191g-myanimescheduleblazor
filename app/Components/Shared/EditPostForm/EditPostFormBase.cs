using app.DTOs;
using app.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class EditPostFormBase : ComponentBase
{
    [Inject]
    protected IValidator<PostRequest>? PostRequestValidator { get; set; }
    [Inject]
    protected PostService? PostService { get; set; }

    [Parameter]
    public required PostResponse Post { get; set; }
    [Parameter]
    public required EventCallback<bool> OnSave { get; set; }

    protected PostRequest? _updatingPost;

    protected Dictionary<string, string[]> errors = [];
    protected string? _successMessage;
    protected string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        _updatingPost = new()
        {
            TargetID = Post.postID,
            Content = Post.Content
        };

        if (PostService is not null)
        {
            await PostService.Initialize(); // Initierar PostService så att token är tillgänglig för den.
        }
    }

    // Sparar ändringar.
    protected async Task Save()
    {
        // Resettar meddelanden.
        _errorMessage = null;
        _successMessage = null;

        // Ser till att PostService finns tillgänglig.
        if (PostService is null)
        {
            _errorMessage = "Service not initialized.";
            return;
        }

        if (_updatingPost is null)
        {
            _errorMessage = "The post to be updated is not available";
            return;
        }

        if (PostRequestValidator is null)
        {
            _errorMessage = "Validator is not available";
            return;
        }

        FluentValidation.Results.ValidationResult validationResult = PostRequestValidator.Validate(_updatingPost);
        if (!validationResult.IsValid)
        {
            ErrorsToDictionary(validationResult);
            return;
        }

        ApiResult<SuccessfulResponse> result = await PostService.Put(_updatingPost);
        if (!result.IsSuccess)
        {
            _successMessage = null;
            errors = new Dictionary<string, string[]>
            {
                { "ApiError", new[] { result.Error?.Details[0] ?? "An unknown error occurred." } }
            };
            return;
        }
        
        if (result.Data is null)
        {
            _errorMessage = "An error occurred regarding the incoming response message.";
            return;
        }
        errors = [];
        _errorMessage = null;
        _successMessage = result.Data.Message;

        if (OnSave.HasDelegate)
        {
            await OnSave.InvokeAsync(true);
        }
    }


    // För att få fram felmeddelanden som en Dictionary där felen är grupperad efter egenskap.
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