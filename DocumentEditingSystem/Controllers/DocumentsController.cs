using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using API.Dtos.Read;
using API.Dtos.Write;
using API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class DocumentsController : ControllerBase
	{
		private IDocumentManagementService _documentManagementService;

		public DocumentsController(IDocumentManagementService documentManagementService)
		{
			_documentManagementService = documentManagementService;
		}

		[HttpPost("LoadDocument")]
		[Authorize]
		public async Task<IResult> LoadDocument(IFormFile file)
		{
			if (file.ContentType != "text/plain"){
				return Results.BadRequest("File type is incorrect");
			}

			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			string userName = User.Identity.Name;

			try
			{
				Username username = new Username(userName);
				await _documentManagementService.LoadDocument(file, userId, username);
			}
			catch (ArgumentException ex)
			{
				Results.BadRequest(ex);
			}

			return Results.Ok();
			
		}

		[HttpPost("UpdateDocument")]
		[Authorize]
		public async Task<IResult> UpdateDocument(IFormFile file, int documentId)
		{
			if (file.ContentType != "text/plain")
			{
				return Results.BadRequest("File type is incorrect");
			}

			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			DocumentR document = await _documentManagementService.UpdateDocument(file, userId, documentId);

			return Results.Ok(document);

		}

		[HttpGet("GetAvailableDocuments")]
		[Authorize]
		public async Task<IResult> GetAvailableDocuments()
		{
			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			List<DocumentR> result = await _documentManagementService.GetAvailableDocuments(userId);

			return Results.Ok(result);

		}

		[HttpPost("DeleteDocument")]
		[Authorize]
		public async Task<IResult> DeleteDocument(int documentId)
		{

			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			await _documentManagementService.DeleteDocument(documentId, userId);

			return Results.Ok();

		}


	}
}
