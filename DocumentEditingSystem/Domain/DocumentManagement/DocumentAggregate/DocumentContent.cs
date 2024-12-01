namespace API.Domain.DocumentManagement.DocumentAggregate;

public class DocumentContent
{
    public int Id { get; }
    public string Text { get; set;}
    
    public int DocumentId { get; set; } 
    public Document Document { get; set; }
}