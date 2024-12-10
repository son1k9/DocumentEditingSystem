namespace API.Domain.DocumentManagement.DocumentAggregate;

public class DocumentContent
{
    public string Text { get; set;}
    
    public int DocumentId { get; set; } 
    public Document Document { get; set; }
}