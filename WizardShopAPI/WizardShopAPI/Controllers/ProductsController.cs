﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WizardShopAPI.DTOs;
using WizardShopAPI.Mappers;
using WizardShopAPI.Models;
using WizardShopAPI.Storage;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WizardShopDbContext _context;

        public ProductsController(WizardShopDbContext context)
        {
            _context = context;
        }

        // GET: api/Products/
        // GET: api/Products/?orderby=price
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
            string orderby = Request.Query["orderby"].ToString();
        
            IQueryable<Product> productsQuery = _context.Products;
        
            if (!string.IsNullOrEmpty(orderby)) {
              string orderbyLower = orderby.ToLower();
              if (orderbyLower == "price") {
                  productsQuery = productsQuery.OrderBy(p => p.Price);
              }
              else if (orderbyLower == "price-desc") {
                  productsQuery = productsQuery.OrderByDescending(p => p.Price);
              }
              else if (orderbyLower == "rating") {
                  productsQuery = productsQuery.OrderBy(p => p.Rating);
              }
              else if (orderbyLower == "rating-desc") {
                  productsQuery = productsQuery.OrderByDescending(p => p.Rating);
              }
              else if (orderbyLower == "popularity") {
                  productsQuery = productsQuery.OrderBy(p => p.Popularity);
              }
              else if (orderbyLower == "popularity-desc") {
                  productsQuery = productsQuery.OrderByDescending(p => p.Popularity);
              }
              else if (orderbyLower == "name") {
                  productsQuery = productsQuery.OrderBy(p => p.Name);
              }
              else if (orderbyLower == "name-desc") {
                  productsQuery = productsQuery.OrderByDescending(p => p.Name);
              }
              else if (orderbyLower == "category") {
                  productsQuery = productsQuery.OrderBy(p => p.CategoryId);
              }
              else if (orderbyLower == "category-desc") {
                  productsQuery = productsQuery.OrderByDescending(p => p.CategoryId);
              }
          }
        
            var products = await productsQuery.ToListAsync();
        
            if (products == null) {
                return NotFound();
            }
        
            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            //validation check
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid values");
            }
            //check if category exists
            if (!CategoryExists(product.CategoryId))
            {
                return NotFound("No category with that id");
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [Description]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody]ProductDto productDto)
        {
            //validation check
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid values");
            }
            //check if category exists
            if (!CategoryExists(productDto.CategoryId))
            {
                return NotFound("No category with that id");
            }

            int productId = this.GetNewProductId();
            Product product = ProductMapper.ProductDtoToProduct(ref productDto, ref productId);

            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Calculates new, unique product id
        /// </summary>
        /// <returns>pruduct id</returns>
        private int GetNewProductId()
        {
            if (!_context.Products.Any())
            {
                return 1;
            }

            return _context.Products.Max(x => x.Id) + 1;
        }

        bool CategoryExists(int id)
        {
            return _context.Categories.Any(x=>x.Id==id);
        }
    }
}
