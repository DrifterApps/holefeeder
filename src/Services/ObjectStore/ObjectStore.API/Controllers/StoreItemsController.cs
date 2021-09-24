﻿using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.API.Authorization;
using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Application.Queries;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DrifterApps.Holefeeder.ObjectStore.API.Controllers
{
    [Authorize]
    [Route("api/v2/[controller]")]
    [RequiredScope(Scopes.REGISTERED_USER)]
    public class StoreItemsController : ControllerBase
    {
        private struct Routes
        {
            public const string GET_STORE_ITEMS = "get-store-items";
            public const string GET_STORE_ITEM = "get-store-item";
            public const string CREATE_STORE_ITEM = "create-store-item";
            public const string MODIFY_STORE_ITEM = "modify-store-item";
        }

        private readonly IMediator _mediator;

        public StoreItemsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet(Name = Routes.GET_STORE_ITEMS)]
        [ProducesResponseType(typeof(QueryResult<StoreItemViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetStoreItems([FromQuery] GetStoreItemsQuery queryParams,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(queryParams, cancellationToken);

            Response?.Headers.Add("X-Total-Count", $"{result.TotalCount}");

            return Ok(result);
        }

        [HttpGet("{id:guid}", Name = Routes.GET_STORE_ITEM)]
        [ProducesResponseType(typeof(StoreItemViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetStoreItem(Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == default)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new GetStoreItemQuery { Id = id }, cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost(Routes.CREATE_STORE_ITEM, Name = Routes.CREATE_STORE_ITEM)]
        [ProducesResponseType(typeof(CommandResult<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateStoreItem([FromBody] CreateStoreItemCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                return CreatedAtRoute(Routes.GET_STORE_ITEM, new { Id = result.Result }, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult<Guid>.Create(CommandStatus.BadRequest, Guid.Empty,
                    ex.Errors.Select(e => e.ToString())));
            }
        }

        [HttpPost(Routes.MODIFY_STORE_ITEM, Name = Routes.MODIFY_STORE_ITEM)]
        [ProducesResponseType(typeof(CommandResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> ModifyStoreItem([FromBody] ModifyStoreItemCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ModelState.Values.Select(x => x.ToString())));
            }

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(CommandResult.Create(CommandStatus.BadRequest,
                    ex.Errors.Select(e => e.ToString())));
            }
        }
    }
}
