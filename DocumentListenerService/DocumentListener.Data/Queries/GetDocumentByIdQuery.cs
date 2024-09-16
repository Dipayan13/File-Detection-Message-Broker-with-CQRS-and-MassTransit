using DocumentListenerService.Data;
using DocumentListenerService.Models;
using MediatR;


public class GetDocumentByIdQuery : IRequest<DocumentRecord>
{
    public Guid DocumentId { get; set; }
}

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentRecord>
{
    private readonly DocumentDbContext _dbContext;

    public GetDocumentByIdQueryHandler(DocumentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DocumentRecord> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.documents.FindAsync(request.DocumentId);
    }
}

