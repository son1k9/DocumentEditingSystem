using API.Dtos.Read;
using API.Dtos.Write;
using API.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentEditingController : ControllerBase
	{
		private readonly IDocumentEditingService _documentEditingService;

		public DocumentEditingController(IDocumentEditingService documentEditingService)
		{
			_documentEditingService = documentEditingService;
		}

		[HttpPost]
		[Authorize]
		public async Task<IResult> AddChanges(List<ChangeW> changes, int documentId)
		{
			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			try
			{
				List<ChangeR> result = await _documentEditingService.AddChangesToDocument(changes, documentId, userId);
				return Results.Ok(result);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex);
			}

			
		}

		[HttpGet]
		[Authorize]
		public async Task<IResult> GetChanges(List<ChangeW> changes, int documentId)
		{
			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			try
			{
				List<ChangeR> result = await _documentEditingService.GetDocumentChanges(documentId, userId);
				return Results.Ok(result);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex);
			}

			
		}

		[HttpGet("AddEditor")]
		[Authorize]
		public async Task<IResult> AddEditor(int documentId, int editorId)
		{
			throw new NotImplementedException();
			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			try
			{
				await _documentEditingService.AddEditor(documentId, editorId, userId);
				return Results.Ok();
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex);
			}
		}

		[HttpGet("RemoveEditor")]
		[Authorize]
		public async Task<IResult> RemoveEditor(int documentId, int editorId)
		{
			throw new NotImplementedException();
			int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			try
			{
				await _documentEditingService.RemoveEditor(documentId, editorId, userId);
				return Results.Ok();
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex);
			}
		}
	}
}
