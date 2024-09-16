using DocumentListenerService.Data;
using DocumentListenerService.Models;
using MediatR;

namespace DocumentListener.Data.Commands
{
    public class AddDocumentCommand : IRequest<Unit>
    {
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }

    public class AddDocumentCommandHandler : IRequestHandler<AddDocumentCommand, Unit>
    {
        private readonly DocumentDbContext _dbContext;

        public AddDocumentCommandHandler(DocumentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(AddDocumentCommand command, CancellationToken cancellationToken)
        {
            var document = new DocumentRecord
            {
                id = Guid.NewGuid(),
                file_name = command.FileName,
                file_base64 = command.FileBase64,
                time_stamp = DateTime.UtcNow
            };

            _dbContext.documents.Add(document);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
