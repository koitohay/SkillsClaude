using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration config)
    {
        await context.Database.MigrateAsync();

        if (await context.ServiceProviders.AnyAsync())
            return;

        var seedPassword = config["Seed:DefaultPassword"] ?? "Provider123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(seedPassword);

        var bella = ServiceProvider.Create("Salon Bella", "Maria Jensen", "maria@salonbella.dk",
            "+4522334455", "Strøget 10", "København", "11223344", hash);
        bella.AssignCategories([1, 2]);

        var wellness = ServiceProvider.Create("Wellness Aarhus", "Lars Pedersen", "lars@wellnessaarhus.dk",
            "+4533445566", "Åboulevard 5", "Aarhus", "22334455", hash);
        wellness.AssignCategories([3]);

        var tandklinik = ServiceProvider.Create("Odense Tandklinik", "Anne Christensen", "anne@odense-tandklinik.dk",
            "+4544556677", "Kongensgade 22", "Odense", "33445566", hash);
        tandklinik.AssignCategories([4]);

        var kiro = ServiceProvider.Create("Aalborg Kiropraktik", "Søren Nielsen", "søren@aalborg-kiro.dk",
            "+4555667788", "Bispensgade 14", "Aalborg", "44556677", hash);
        kiro.AssignCategories([5]);

        var esbjerg = ServiceProvider.Create("Esbjerg Negle & Massage", "Kirsten Andersen", "kirsten@esbjerg-negle.dk",
            "+4566778899", "Torvet 3", "Esbjerg", "55667788", hash);
        esbjerg.AssignCategories([2, 3]);

        await context.ServiceProviders.AddRangeAsync(bella, wellness, tandklinik, kiro, esbjerg);
        await context.SaveChangesAsync();

        var services = new List<ProviderService>
        {
            ProviderService.Create(bella.Id, 1, "Hårvask & Klip",
                "Professionel hårvask med luksussjampoo efterfulgt af klip og styling. Velegnet til alle hårtyper.", 395),
            ProviderService.Create(bella.Id, 1, "Farvebehandling",
                "Fuld farvebehandling med ammoniakfri farve. Inkluderer hårvask, farve, pleje og finish.", 695),
            ProviderService.Create(bella.Id, 1, "Highlights & Balayage",
                "Håndmalet balayage teknik for naturlige highlights. Inkluderer toning og styling.", 895),
            ProviderService.Create(bella.Id, 2, "Gel-negle",
                "Holdbart gel-neglelak med naturligt look. Valgfri farve, inkluderer neglefil og top coat.", 449),
            ProviderService.Create(bella.Id, 2, "Akrylnegle – Sæt",
                "Fuldstændigt akrylsæt med form, filing og lakering. Holder op til 3 uger.", 649),

            ProviderService.Create(wellness.Id, 3, "Klassisk massage 60 min",
                "Afslappende helkropsmassage med varm olie. Perfekt til stressreduktion og muskelafslapning.", 549),
            ProviderService.Create(wellness.Id, 3, "Dyb vævs-massage 90 min",
                "Intensiv massage der arbejder med dybe muskelvæv. Ideel ved kroniske smerter og spændinger.", 749),
            ProviderService.Create(wellness.Id, 3, "Hot stone massage",
                "Varmestensmassage kombineret med klassisk teknik. Fremmer blodcirkulation og dyb afslapning.", 849),
            ProviderService.Create(wellness.Id, 3, "Rygmassage 30 min",
                "Fokuseret massage på ryg, skuldre og nakke. Hurtig lindring ved kontorspændinger.", 349),

            ProviderService.Create(tandklinik.Id, 4, "Tandrensning & Undersøgelse",
                "Professionel tandrensning, røntgenbilleder og tandlægeundersøgelse. Forebyggende behandling.", 595),
            ProviderService.Create(tandklinik.Id, 4, "Tandfyldning",
                "Hvid komposit tandfyldning. Inkluderer lokalbedøvelse og polering. Pris pr. tand.", 895),
            ProviderService.Create(tandklinik.Id, 4, "Tandbleging",
                "Professionel in-clinic tandbleging med LED-aktivering. Op til 8 nuancer lysere på én session.", 1995),
            ProviderService.Create(tandklinik.Id, 4, "Akut tandsmerter",
                "Hurtigt akutbesøg ved tandsmerter. Diagnose og midlertidig/permanent behandling.", 750),

            ProviderService.Create(kiro.Id, 5, "Første konsultation",
                "Grundig undersøgelse inkl. holdningsanalyse, bevægelsesvurdering og første behandling.", 695),
            ProviderService.Create(kiro.Id, 5, "Kiropraktisk behandling",
                "Stilling og manipulation af rygsøjle og led. Inkluderer blød vævsbehandling.", 495),
            ProviderService.Create(kiro.Id, 5, "Sportsskadebehandling",
                "Specialiseret behandling af sportsskader med aktiv rehabilitering og taping.", 595),
            ProviderService.Create(kiro.Id, 5, "Graviditetsbehandling",
                "Skånsom kiropraktisk behandling tilpasset gravide med bækkensmerter og lændeproblemer.", 545),

            ProviderService.Create(esbjerg.Id, 2, "Klassisk manicure",
                "Neglefil, neglebåndsolie, massage og lakering. Valgfrit neglelak inkluderet.", 299),
            ProviderService.Create(esbjerg.Id, 2, "Gel-pedikur",
                "Fodmassage, hæl-behandling, neglefil og holdbart gel-lak. En fuldstændig forkælelse.", 449),
            ProviderService.Create(esbjerg.Id, 3, "Afslapningsmassage 45 min",
                "Let afslappende massage med duftende olier. Fokus på skuldre, ryg og ben.", 399),
            ProviderService.Create(esbjerg.Id, 3, "Kombinationspakke – Negle & Massage",
                "Manicure + pedikure efterfulgt af 30 min afslapningsmassage. Komplet wellnessoplevelse.", 699),
        };

        await context.ProviderServices.AddRangeAsync(services);
        await context.SaveChangesAsync();
    }
}
