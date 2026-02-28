using app.DTOs;
using app.Services;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class DeletePostFormBase : ComponentBase
{
    [Inject]
    protected PostService? PostService { get; set; }

    [Parameter]
    public required PostResponse Post { get; set; }
    [Parameter]
    public required EventCallback<bool> OnDelete { get; set; }

    protected string? _successMessage;
    protected string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        if (PostService is not null)
        {
            await PostService.Initialize(); // Initierar PostService så att token är tillgänglig för den.
        }
    }

    // Raderar posten.
    protected async Task Delete()
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

        if (Post is null)
        {
            _errorMessage = "The post to be deleted is not available";
            return;
        }

        ApiResult<SuccessfulResponse> result = await PostService.Delete(Post.postID);
        if (!result.IsSuccess)
        {
            _errorMessage = result.Error!.Details[0] ?? "An error occured while deleting post.";
            return;
        }

        if (result.Data is null)
        {
            _errorMessage = "An error occurred regarding the incoming response message.";
            return;
        }

        _errorMessage = null;
        _successMessage = result.Data.Message;
        
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(true);
        }
    }
}