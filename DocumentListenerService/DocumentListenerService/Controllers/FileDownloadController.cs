using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DocumentListener.Service.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileDownloadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileDownloadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var document = await _mediator.Send(new GetDocumentByIdQuery { DocumentId = id });

            if (document == null)
                return NotFound();

            var fileBytes = Convert.FromBase64String(document.file_base64);
            return File(fileBytes, "application/pdf", document.file_name);
        }
    }
}
