namespace OrchardCore.ContentManagement.Handlers;

public class CreateContentContext : ContentContextBase
{
    public CreateContentContext(ContentItem contentItem) : base(contentItem)
    {
    }

    public bool Cancel { get; set; }
}
