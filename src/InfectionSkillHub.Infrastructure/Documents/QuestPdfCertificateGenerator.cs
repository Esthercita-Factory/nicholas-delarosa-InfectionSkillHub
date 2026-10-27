using System.Globalization;
using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Dtos.Certificates;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InfectionSkillHub.Infrastructure.Documents;

/// <summary>
/// Enrollment certificate drawn with the same identity as the participant client: the light
/// "printed agenda" theme, the eight InfectionSkillHub colors and the Red Hat type family.
/// </summary>
public sealed class QuestPdfCertificateGenerator : ICertificateGenerator
{
    private const string Title = "Enrollment Certificate";
    private const string FontResourcePrefix = "InfectionSkillHub.Infrastructure.Fonts.";

    private const string Display = "Red Hat Display";
    private const string Text = "Red Hat Text";
    private const string Mono = "Red Hat Mono";

    // The eight brand colors.
    private const string Raspberry = "#B12C63";
    private const string Eggplant = "#2A0625";
    private const string DeepPurple = "#55225A";
    private const string PastelPink = "#FE8ED5";
    private const string MutedLavender = "#9B68A5";

    // Light theme values of the client, mixed from the brand colors.
    private const string Stock = "#FFF3FA";
    private const string StockSunken = "#FFE8F5";
    private const string Rule = "#E3D4E6";
    private const string InkMuted = "#6E4A78";
    private const string TimeInk = "#5F3F6F";
    private const string SeatCyan = "#008796";

    private const string Mark = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32">
          <rect width="32" height="32" rx="8" fill="#B12C63"/>
          <path d="M11 5v5M21 5v5M7 13h18" fill="none" stroke="#FFFFFF" stroke-width="2.4" stroke-linecap="round"/>
          <rect x="17.5" y="17.5" width="6.5" height="6.5" rx="1.5" fill="#FFFFFF"/>
        </svg>
        """;

    private const string StatusDot = $"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 8 8"><circle cx="4" cy="4" r="4" fill="{SeatCyan}"/></svg>""";

    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    static QuestPdfCertificateGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var assembly = typeof(QuestPdfCertificateGenerator).Assembly;
        foreach (var name in assembly.GetManifestResourceNames().Where(n => n.StartsWith(FontResourcePrefix, StringComparison.Ordinal)))
        {
            using var stream = assembly.GetManifestResourceStream(name)!;
            FontManager.RegisterFontFromStream(stream);
        }
    }

    public byte[] Generate(EnrollmentCertificate certificate)
        => Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(0);
            page.PageColor(Stock);
            page.DefaultTextStyle(style => style.FontFamily(Text).FontSize(10.5f).FontColor(Eggplant).LineHeight(1.4f));

            page.Header().Element(header => Masthead(header, certificate));
            page.Content().PaddingHorizontal(48).PaddingTop(36).Element(content => Body(content, certificate));
            page.Footer().Element(footer => Footer(footer, certificate));
        })).GeneratePdf();

    private static void Masthead(IContainer container, EnrollmentCertificate certificate)
        => container.Background(Eggplant).PaddingHorizontal(48).PaddingVertical(22).Row(row =>
        {
            row.ConstantItem(26).AlignMiddle().Height(26).Svg(Mark);
            row.ConstantItem(10);
            row.RelativeItem().AlignMiddle().Text(text =>
            {
                text.Span("Infection").FontFamily(Display).ExtraBold().FontSize(17).FontColor(Colors.White);
                text.Span("SkillHub").FontFamily(Display).ExtraBold().FontSize(17).FontColor(PastelPink);
            });
            row.AutoItem().AlignMiddle().Text(certificate.OrganizationName + " · short in-person workshops")
                .FontFamily(Mono).Medium().FontSize(8).LetterSpacing(0.06f).FontColor("#C8A9CE");
        });

    private static void Body(IContainer container, EnrollmentCertificate certificate)
        => container.Column(column =>
        {
            column.Item().Text(Title).FontFamily(Display).ExtraBold().FontSize(30).LetterSpacing(-0.02f);
            column.Item().PaddingTop(10).LineHorizontal(3).LineColor(Raspberry);

            column.Item().PaddingTop(16).Text(text =>
            {
                text.Span($"{certificate.OrganizationName} certifies that ");
                text.Span(certificate.ParticipantFullName).FontFamily(Text).SemiBold();
                text.Span(" holds an active enrollment in the workshop below.");
            });

            column.Item().PaddingTop(28).Element(item => Workshop(item, certificate));
            column.Item().PaddingTop(30).Element(item => Section(item, "Participant", [
                ("Full name", certificate.ParticipantFullName, false),
                ("Email", certificate.ParticipantEmail, false)
            ]));
            column.Item().PaddingTop(24).Element(item => Section(item, "Enrollment", [
                ("Enrollment id", certificate.EnrollmentId.ToString(), true),
                ("Enrollment date", $"{certificate.EnrolledAt.ToString("MMMM d, yyyy · HH:mm", Culture)}", false),
                ("Status", "Active", false)
            ]));
        });

    /// <summary>The workshop set like a day of the agenda: large date numeral, then the listing row.</summary>
    private static void Workshop(IContainer container, EnrollmentCertificate certificate)
        => container.Column(column =>
        {
            column.Item().PaddingBottom(10).Row(row =>
            {
                row.AutoItem().AlignBottom().Text(certificate.StartsAt.ToString("%d", Culture))
                    .FontFamily(Display).ExtraBold().FontSize(46).LineHeight(0.9f).LetterSpacing(-0.04f).FontColor(Raspberry);
                row.ConstantItem(12);
                row.RelativeItem().AlignBottom().PaddingBottom(4).Column(label =>
                {
                    label.Item().Text(certificate.StartsAt.ToString("dddd", Culture)).FontFamily(Display).Bold().FontSize(14).FontColor(Raspberry);
                    label.Item().Text(certificate.StartsAt.ToString("MMMM yyyy", Culture).ToUpperInvariant())
                        .FontFamily(Mono).Medium().FontSize(8.5f).LetterSpacing(0.08f).FontColor(DeepPurple);
                });
            });

            column.Item().LineHorizontal(3).LineColor(Raspberry);

            column.Item().Background(StockSunken).PaddingVertical(16).PaddingHorizontal(12).Row(row =>
            {
                row.ConstantItem(84).Column(time =>
                {
                    time.Item().Text(certificate.StartsAt.ToString("HH:mm", Culture)).FontFamily(Mono).SemiBold().FontSize(15).FontColor(TimeInk);
                    time.Item().Text(certificate.EndsAt.ToString("HH:mm", Culture)).FontFamily(Mono).Medium().FontSize(10).FontColor(InkMuted);
                    time.Item().PaddingTop(6).Text(certificate.WorkshopCode).FontFamily(Mono).Medium().FontSize(8.5f).LetterSpacing(0.04f).FontColor(InkMuted);
                });

                row.RelativeItem().Column(what =>
                {
                    what.Item().Text(certificate.WorkshopTitle).FontFamily(Display).ExtraBold().FontSize(17);
                    what.Item().PaddingTop(4).Text($"with {certificate.InstructorFullName}").FontColor(DeepPurple);
                    what.Item().PaddingTop(10).Text(text =>
                    {
                        text.Span("Schedule  ").FontFamily(Mono).Medium().FontSize(8.5f).LetterSpacing(0.06f).FontColor(InkMuted);
                        text.Span($"{certificate.StartsAt.ToString("HH:mm", Culture)} – {certificate.EndsAt.ToString("HH:mm", Culture)}, {TimeZoneLabel(certificate.TimeZoneId, certificate.StartsAt)}")
                            .FontColor(Eggplant);
                    });
                });
            });

            column.Item().LineHorizontal(1).LineColor(Rule);
        });

    private static void Section(IContainer container, string heading, IReadOnlyList<(string Label, string Value, bool Mono)> rows)
        => container.Column(column =>
        {
            column.Item().PaddingBottom(8).Text(heading).FontFamily(Display).Bold().FontSize(13).FontColor(Raspberry);
            column.Item().LineHorizontal(2).LineColor(Raspberry);

            foreach (var (label, value, mono) in rows)
            {
                column.Item().BorderBottom(1).BorderColor(Rule).PaddingVertical(9).PaddingHorizontal(12).Row(row =>
                {
                    row.ConstantItem(140).Text(label.ToUpperInvariant()).FontFamily(Mono).Medium().FontSize(8.5f).LetterSpacing(0.06f).FontColor(InkMuted);

                    if (label == "Status")
                    {
                        row.RelativeItem().Row(status =>
                        {
                            status.ConstantItem(8).AlignMiddle().Height(8).Svg(StatusDot);
                            status.ConstantItem(7);
                            status.RelativeItem().Text(value).FontFamily(Text).SemiBold();
                        });
                    }
                    else if (mono)
                    {
                        row.RelativeItem().Text(value).FontFamily(Mono).Medium().FontSize(9.5f);
                    }
                    else
                    {
                        row.RelativeItem().Text(value);
                    }
                });
            }
        });

    private static void Footer(IContainer container, EnrollmentCertificate certificate)
        => container.PaddingHorizontal(48).PaddingBottom(28).Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(MutedLavender);
            column.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text($"Generated on {certificate.GeneratedAt.ToString("yyyy-MM-dd HH:mm", Culture)} · {TimeZoneLabel(certificate.TimeZoneId, certificate.GeneratedAt)}")
                    .FontFamily(Mono).Medium().FontSize(8).FontColor(InkMuted);
                row.AutoItem().Text(text =>
                {
                    text.DefaultTextStyle(style => style.FontFamily(Mono).Medium().FontSize(8).FontColor(InkMuted));
                    text.Span("Page ");
                    text.CurrentPageNumber();
                });
            });
        });

    private static string TimeZoneLabel(string timeZoneId, DateTimeOffset value)
    {
        var sign = value.Offset < TimeSpan.Zero ? "-" : "+";
        return $"{timeZoneId}, UTC{sign}{value.Offset.Duration():hh\\:mm}";
    }
}
