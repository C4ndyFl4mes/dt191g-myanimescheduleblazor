using app.DTOs;
using Microsoft.AspNetCore.Components;

namespace app.Bases;

public class PostBase : ComponentBase
{
    [Parameter]
    public required PostResponse Post { get; set; }
    [Parameter]
    public required bool ShowProfileImage { get; set; }
    [Parameter]
    public required bool IsViewerOwner { get; set; }
    [Parameter]
    public required bool IsViewerModerator { get; set; }
    [Parameter]
    public EventCallback<PostResponse> OnUpdate { get; set; }
    [Parameter]
    public EventCallback<PostResponse> OnDelete { get; set; }


    protected async Task SendUpdateRequest()
    {
        if (OnUpdate.HasDelegate)
        {
            await OnUpdate.InvokeAsync(Post);
        }
    }

    protected async Task SendDeleteRequest()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(Post);
        }
    }
}