#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tripix.Context;
using Tripix.Entities;
using Tripix.Services;
using Tripix.View_Models;

namespace Tripix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbcontext _context;
        private readonly HttpClient httpclient;
        private readonly bininfoRepo bininforepo;

        public PaymentController ( ApplicationDbcontext context, HttpClient _httpclient, bininfoRepo bininforepo )
        {
            _context = context;
            httpclient = _httpclient;
            this.bininforepo = bininforepo;
        }

        [HttpPost("add-card")]
        public async Task<IActionResult> AddCard ( [FromBody] CreditDTO card )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newcard = new CreditCard()
            {
                CardNumber = card.CardNumber.Replace(" ", "").Trim(),
                CardHolderName = card.CardHolderName,
                ExpiryDate = $"{card.ExpiryMonth}/{card.ExpiryYear}",
                CVV = card.CVV,
            };

            if (string.IsNullOrEmpty(card.CardNumber) || !card.CardNumber.All(char.IsDigit))
                return BadRequest(new { message = "Invalid card number format." });


            if (!LuhnCheck(card.CardNumber))
                return BadRequest(new { message = "Invalid credit card number." });

            var binInfo = await bininforepo.GetBinInfo(card.CardNumber.Substring(0, 6));
            if (binInfo == null)
                return BadRequest(new { message = "Unable to verify card details." });

            newcard.Schema = binInfo.Scheme;
            newcard.BankName = binInfo.Bank.Name;
            newcard.Type = binInfo.Type;

            _context.CreditCards.Add(newcard);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Card added successfully", binInfo });
        }

        private bool LuhnCheck ( string cardNumber )
        {
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                sum += digit;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }

        private async Task<object> GetBinInfo ( string bin )
        {
            try
            {
                var response = await httpclient.GetStringAsync($"https://lookup.binlist.net/{bin}");
                return JsonSerializer.Deserialize<object>(response);
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("get-cards")]
        public async Task<IActionResult> GetCards ()
        {
            var cards = await _context.CreditCards.ToListAsync();
            return Ok(cards);
        }

        [HttpDelete("remove-card/{id}")]
        public async Task<IActionResult> RemoveCard ( int id )
        {
            var card = await _context.CreditCards.FindAsync(id);
            if (card == null)
                return NotFound(new { message = "Card not found" });

            _context.CreditCards.Remove(card);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Card removed successfully" });
        }
    }

}
