using DocSpot.Core.Contracts;
using DocSpot.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Ocsp;
using System.Runtime;

namespace DocSpot.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> options) 
            => emailSettings = options.Value;

        public async Task SendAppointmentConfirmationAsync(AppointmentDto appointmentDto, CancellationToken ct)
        {
            var subject = $"Appointment confirmed: {appointmentDto.VisitType} on {appointmentDto.AppointmentDate} at {appointmentDto.AppointmentTime}";

            var bodyHtml = $@"
            <div style='font-family: Arial, sans-serif; line-height: 1.5;'>
              <h2>Appointment Confirmed</h2>
              <p>Hello <b>{Escape(appointmentDto.PatientName)}</b>,</p>
              <p>Your appointment is confirmed.</p>
              <ul>
                <li><b>Visit type:</b> {appointmentDto.VisitType}</li>
                <li><b>Date:</b> {Escape(appointmentDto.AppointmentDate.ToString())}</li>
                <li><b>Time:</b> {Escape(appointmentDto.AppointmentTime.ToString())}</li>
                <li><b>Phone:</b> {Escape(appointmentDto.PatientPhone)}</li>
              </ul>
              {(string.IsNullOrWhiteSpace(appointmentDto.Message) ? "" : $"<p><b>Your message:</b> {Escape(appointmentDto.Message!)}</p>")}
              <p>— DocSpot</p>
            </div>";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(appointmentDto.PatientEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // Connect with TLS (STARTTLS on 587 is typical)
            var socketOpt = emailSettings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;
            await smtp.ConnectAsync(emailSettings.SmtpHost, emailSettings.SmtpPort, socketOpt, ct);

            await smtp.AuthenticateAsync(emailSettings.Username, emailSettings.AppPassword, ct);
            await smtp.SendAsync(message, ct);
            await smtp.DisconnectAsync(true, ct);
        }

        private static string Escape(string input)
            => System.Net.WebUtility.HtmlEncode(input);
    }
}
