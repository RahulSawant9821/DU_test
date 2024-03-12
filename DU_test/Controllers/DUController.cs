﻿using DU_test.Data;
using DU_test.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DU_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DUController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<JobOffer>> GetOffers()
        {
            return Ok(OffersStore.Offers);
        }

        [HttpGet("Offer/{JobId}", Name = "GetOffer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<JobOffer> GetOffer(string JobId)
        {
            if (string.IsNullOrEmpty(JobId))
            {
                return BadRequest();
            }
            var offer = OffersStore.Offers.FirstOrDefault(u => u.JobOfferId == JobId);
            if (offer == null)
            {
                return NotFound();
            }

            return Ok(offer);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<JobOffer>> authenticateAndRequestSimilarOffers([FromBody] Driver driver)
        {
            if (driver == null)
            {
                return BadRequest(driver);
            }

            if (driver.username == null || driver.password == null)
            {
                return StatusCode(403);
            }

            // Authenticate the driver
            var authenticatedDriver = DriverStore.DriverDetail.FirstOrDefault(d => d.username == driver.username && d.password == driver.password);
            if (authenticatedDriver == null)
            {
                return StatusCode(401);
            }

            var similarOffers = OffersStore.Offers.Where(o => o.JobOfferId == driver.offer).ToList();

            if (similarOffers.Count == 0)
            {
                // If no similar offers found, return 404
                return NotFound();
            }

            return Ok(similarOffers); ;
        }


    }
}