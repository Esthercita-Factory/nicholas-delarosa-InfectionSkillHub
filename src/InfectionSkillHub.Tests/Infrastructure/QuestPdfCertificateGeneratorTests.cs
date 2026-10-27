using System.Text;
using InfectionSkillHub.Application.Dtos.Certificates;
using InfectionSkillHub.Infrastructure.Documents;

namespace InfectionSkillHub.Tests.Infrastructure;

public class QuestPdfCertificateGeneratorTests
{
    [Fact]
    public void Generate_ReturnsAPdfDocument()
    {
        var local = TimeSpan.FromHours(-5);
        var certificate = new EnrollmentCertificate(
            "InfectionSkillHub",
            Guid.NewGuid(),
            "Jorge Andrés Pérez Rojas",
            "jorge.perez@correo.com",
            "FOT-001",
            "Fotografía con celular",
            "Andrés Felipe Mejía Arango",
            new DateTimeOffset(2026, 12, 5, 8, 0, 0, local),
            new DateTimeOffset(2026, 12, 5, 12, 0, 0, local),
            new DateTimeOffset(2026, 11, 1, 10, 30, 0, local),
            new DateTimeOffset(2026, 11, 2, 9, 0, 0, local),
            "America/Bogota");

        var pdf = new QuestPdfCertificateGenerator().Generate(certificate);

        Assert.True(pdf.Length > 1000);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(pdf, 0, 4));
    }
}
