using Microsoft.EntityFrameworkCore;
using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds 20 demo providers with services, 10 demo clients with open service requests,
/// and 30+ offers so the OffersFeedAgent has real data to curate.
///
/// Safe to edit: add providers, change prices, tweak descriptions.
/// Guard: runs only when ServiceProviders table has fewer than 6 rows
/// (the 5 base providers from DatabaseSeeder are already there).
/// </summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(AppDbContext context, string passwordHash)
    {
        if (await context.ServiceProviders.CountAsync() >= 6)
            return;

        // ── 20 demo providers ────────────────────────────────────────────────
        var providers = new List<ServiceProvider>
        {
            // Hair / Beauty
            Build("Frederiksberg Frisør",  "Camilla Holm",     "camilla@frbfrisor.dk",      "+4520111001", "Gammel Kongevej 12", "København",  "10001001", passwordHash, [1, 2]),
            Build("Randers Hårstudie",     "Mikkel Dahl",      "mikkel@randershaar.dk",     "+4520111002", "Torvegade 5",        "Randers",    "10001002", passwordHash, [1]),
            Build("Kolding Skønhedssalon", "Sofie Lund",       "sofie@kolding-beauty.dk",   "+4520111003", "Akseltorv 7",        "Kolding",    "10001003", passwordHash, [1, 2]),
            Build("Vejle Nail Studio",     "Anna Bloch",       "anna@vejlenaile.dk",        "+4520111004", "Brummersvej 3",      "Vejle",      "10001004", passwordHash, [2]),
            Build("Horsens Beauty Lab",    "Trine Møller",     "trine@horsensbeauty.dk",    "+4520111005", "Søndergade 18",      "Horsens",    "10001005", passwordHash, [2]),

            // Massage / Wellness
            Build("Helsingør Wellness",    "Rasmus Bak",       "rasmus@helsingwellness.dk", "+4520111006", "Stengade 22",        "Helsingør",  "10001006", passwordHash, [3]),
            Build("Roskilde Spa",          "Julie Kjær",       "julie@roskildespa.dk",      "+4520111007", "Skomagergade 9",     "Roskilde",   "10001007", passwordHash, [3]),
            Build("Silkeborg Massage",     "Martin Frost",     "martin@silkemassage.dk",    "+4520111008", "Papirfabrikken 1",   "Silkeborg",  "10001008", passwordHash, [3]),
            Build("Næstved Wellnesscenter","Ida Winther",      "ida@naestvedwellness.dk",   "+4520111009", "Riddergade 4",       "Næstved",    "10001009", passwordHash, [3]),
            Build("Fredericia Massage",    "Claus Damgaard",   "claus@fredmassage.dk",      "+4520111010", "Gothersgade 11",     "Fredericia", "10001010", passwordHash, [3]),

            // Dental
            Build("Herning Tandpleje",     "Lene Hvid",        "lene@herningtand.dk",       "+4520111011", "Bredgade 6",         "Herning",    "10001011", passwordHash, [4]),
            Build("Aarhus Tandklinik Nord","Peter Vang",       "peter@aarhustands.dk",      "+4520111012", "Ryesgade 34",        "Aarhus",     "10001012", passwordHash, [4]),
            Build("Aalborg Tandlæge",      "Mette Skov",       "mette@aalborgdent.dk",      "+4520111013", "Algade 22",          "Aalborg",    "10001013", passwordHash, [4]),

            // Chiro / Physio
            Build("Odense Fysioklinik",    "Henrik Lausen",    "henrik@odensefysio.dk",     "+4520111014", "Vestergade 15",      "Odense",     "10001014", passwordHash, [5]),
            Build("Esbjerg Kiropraktik",   "Stine Fog",        "stine@esbkiro.dk",          "+4520111015", "Skolegade 8",        "Esbjerg",    "10001015", passwordHash, [5]),
            Build("Vejle Sportsklinik",    "Anders Riis",      "anders@vejlesport.dk",      "+4520111016", "Nørretorv 2",        "Vejle",      "10001016", passwordHash, [5]),

            // Mixed / multi-category
            Build("Randers Total Beauty",  "Birgit Noe",       "birgit@randerstotal.dk",    "+4520111017", "Storegade 3",        "Randers",    "10001017", passwordHash, [1, 2, 3]),
            Build("Kolding Velvære",       "Thomas Juul",      "thomas@koldingvelvaere.dk", "+4520111018", "Klostergade 5",      "Kolding",    "10001018", passwordHash, [2, 3]),
            Build("Horsens Klinik",        "Dorthe Ravn",      "dorthe@horsensklinik.dk",   "+4520111019", "Bankagervej 1",      "Horsens",    "10001019", passwordHash, [4, 5]),
            Build("Silkeborg Sundhedshus", "Niels Ø. Bro",     "niels@silkebro.dk",         "+4520111020", "Vestergade 20",      "Silkeborg",  "10001020", passwordHash, [3, 5]),
        };

        await context.ServiceProviders.AddRangeAsync(providers);
        await context.SaveChangesAsync();

        // ── ProviderServices ─────────────────────────────────────────────────
        var services = BuildProviderServices(providers);
        await context.ProviderServices.AddRangeAsync(services);
        await context.SaveChangesAsync();

        // ── 10 demo clients ──────────────────────────────────────────────────
        var clients = new List<Client>
        {
            Client.Create("Jens Hansen",      "jens@demo.dk",      "+4521001001", passwordHash),
            Client.Create("Pernille Koch",    "pernille@demo.dk",  "+4521001002", passwordHash),
            Client.Create("Oliver Strand",    "oliver@demo.dk",    "+4521001003", passwordHash),
            Client.Create("Mia Toft",         "mia@demo.dk",       "+4521001004", passwordHash),
            Client.Create("Frederik Brun",    "frederik@demo.dk",  "+4521001005", passwordHash),
            Client.Create("Laura Engel",      "laura@demo.dk",     "+4521001006", passwordHash),
            Client.Create("Magnus Dall",      "magnus@demo.dk",    "+4521001007", passwordHash),
            Client.Create("Emma Nygaard",     "emma@demo.dk",      "+4521001008", passwordHash),
            Client.Create("Simon Agger",      "simon@demo.dk",     "+4521001009", passwordHash),
            Client.Create("Nora Lindberg",    "nora@demo.dk",      "+4521001010", passwordHash),
        };

        await context.Clients.AddRangeAsync(clients);
        await context.SaveChangesAsync();

        // ── Open service requests + offers ───────────────────────────────────
        // Requests are spread across cities and categories so the agent
        // can pick a diverse homepage feed.
        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var nextWeek = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var morning  = new TimeOnly(9, 0);
        var afternoon = new TimeOnly(14, 0);

        var requests = new List<ServiceRequest>
        {
            ServiceRequest.Create(clients[0].Id, 1, null, tomorrow,  morning,   "København"),
            ServiceRequest.Create(clients[1].Id, 2, null, tomorrow,  afternoon, "Randers"),
            ServiceRequest.Create(clients[2].Id, 3, null, nextWeek,  morning,   "Kolding"),
            ServiceRequest.Create(clients[3].Id, 3, null, tomorrow,  afternoon, "Vejle"),
            ServiceRequest.Create(clients[4].Id, 4, null, nextWeek,  morning,   "Horsens"),
            ServiceRequest.Create(clients[5].Id, 5, null, tomorrow,  morning,   "Helsingør"),
            ServiceRequest.Create(clients[6].Id, 1, null, nextWeek,  afternoon, "Roskilde"),
            ServiceRequest.Create(clients[7].Id, 2, null, tomorrow,  morning,   "Silkeborg"),
            ServiceRequest.Create(clients[8].Id, 3, null, nextWeek,  afternoon, "Næstved"),
            ServiceRequest.Create(clients[9].Id, 4, null, tomorrow,  morning,   "Aalborg"),
        };

        // Attach offers — multiple providers compete on the same request
        // so the agent can demonstrate competitive curation
        requests[0].AddOffer(providers[0].Id,  399, "Frisk klip og vask – professionel behandling");
        requests[0].AddOffer(providers[1].Id,  349, "Tilbud på klip inkl. behandling");
        requests[0].AddOffer(providers[2].Id,  420, "Premium klip med styling");

        requests[1].AddOffer(providers[3].Id,  449, "Gel-negle med valgfri farve");
        requests[1].AddOffer(providers[4].Id,  399, "Klassisk neglebehandling + lak");
        requests[1].AddOffer(providers[16].Id, 380, "Negle og neglebåndspleje");

        requests[2].AddOffer(providers[5].Id,  549, "60 min klassisk massage");
        requests[2].AddOffer(providers[6].Id,  499, "Afslapningsmassage med varm olie");
        requests[2].AddOffer(providers[7].Id,  579, "Dyb vævsmassage – stressreduktion");

        requests[3].AddOffer(providers[8].Id,  450, "45 min afslappende massage");
        requests[3].AddOffer(providers[9].Id,  520, "Velvære-massage 60 min");
        requests[3].AddOffer(providers[19].Id, 480, "Kombineret ryg og skuldre");

        requests[4].AddOffer(providers[10].Id, 595, "Tandrensning og undersøgelse");
        requests[4].AddOffer(providers[11].Id, 550, "Kontrol + professionel rensning");

        requests[5].AddOffer(providers[13].Id, 695, "Første kirokonsultation + behandling");
        requests[5].AddOffer(providers[14].Id, 645, "Kiropraktisk behandling");
        requests[5].AddOffer(providers[15].Id, 595, "Sportsklinik – vurdering og behandling");

        requests[6].AddOffer(providers[0].Id,  375, "Roskilde-tilbud på klip");
        requests[6].AddOffer(providers[16].Id, 420, "Farve + klip pakke");

        requests[7].AddOffer(providers[17].Id, 299, "Klassisk manicure");
        requests[7].AddOffer(providers[4].Id,  349, "Gel-negle sæt");

        requests[8].AddOffer(providers[7].Id,  399, "30 min rygmassage");
        requests[8].AddOffer(providers[8].Id,  449, "Nakke og skulder massage");
        requests[8].AddOffer(providers[19].Id, 420, "Afslapning – tilpasset session");

        requests[9].AddOffer(providers[12].Id, 595, "Tandrensning og røntgen");
        requests[9].AddOffer(providers[10].Id, 620, "Akut tandbehandling + undersøgelse");

        await context.ServiceRequests.AddRangeAsync(requests);
        await context.SaveChangesAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static ServiceProvider Build(
        string company, string contact, string email, string phone,
        string address, string city, string cvr, string hash, int[] categories)
    {
        var p = ServiceProvider.Create(company, contact, email, phone, address, city, cvr, hash);
        p.AssignCategories(categories);
        return p;
    }

    private static List<ProviderService> BuildProviderServices(List<ServiceProvider> p) =>
    [
        // Hair providers (0-2)
        ProviderService.Create(p[0].Id,  1, "Dameklip & Styling",    "Klip, vask og styling med professionelle produkter.",          380),
        ProviderService.Create(p[0].Id,  2, "Gel-negle",             "Holdbart gel-lak, valgfri farve, inkl. négelfil.",             440),
        ProviderService.Create(p[1].Id,  1, "Herre- og Dameklip",    "Hurtig og præcis klippeservice for alle hårtyper.",            320),
        ProviderService.Create(p[2].Id,  1, "Balayage",              "Håndmalet balayage for naturlige highlights.",                 850),
        ProviderService.Create(p[2].Id,  2, "Akrylnegle",            "Komplet akrylsæt, holding op til 3 uger.",                    620),

        // Nail providers (3-4)
        ProviderService.Create(p[3].Id,  2, "Gel-negle sæt",         "Fuldstændigt gel-neglesæt inkl. farve og top coat.",           449),
        ProviderService.Create(p[3].Id,  2, "Gel-pedikur",           "Fodmassage og gel-lak på tænegle.",                           399),
        ProviderService.Create(p[4].Id,  2, "Klassisk Manicure",     "Neglefil, neglebåndspleje og lakering.",                       280),
        ProviderService.Create(p[4].Id,  2, "Nail Art",              "Kreativt nail art – alle designs mulige.",                     350),

        // Massage providers (5-9)
        ProviderService.Create(p[5].Id,  3, "Klassisk massage 60 min", "Afslappende helkropsmassage med varm olie.",                 529),
        ProviderService.Create(p[6].Id,  3, "Ryg & skulder massage",   "Fokuseret massage på ryg, skuldre og nakke – 45 min.",       429),
        ProviderService.Create(p[7].Id,  3, "Dyb vævsmassage 90 min",  "Intensiv massage til dybe muskelvæv, kroniske smerter.",     749),
        ProviderService.Create(p[8].Id,  3, "Hot stone massage",        "Varmestensmassage kombineret med klassisk teknik.",          799),
        ProviderService.Create(p[9].Id,  3, "Afslapningsmassage 45 min","Let massage med duftende olier, fokus på stressreduktion.",  399),

        // Dental providers (10-12)
        ProviderService.Create(p[10].Id, 4, "Tandrensning",           "Professionel rensning + røntgen + undersøgelse.",             579),
        ProviderService.Create(p[11].Id, 4, "Tandkontrol",            "Kontrol + professionel tandrensning.",                        540),
        ProviderService.Create(p[12].Id, 4, "Akut Tandsmerter",       "Hurtigt akutbesøg og behandling.",                            720),

        // Chiro providers (13-15)
        ProviderService.Create(p[13].Id, 5, "Første konsultation",    "Undersøgelse, holdningsanalyse og første behandling.",        680),
        ProviderService.Create(p[14].Id, 5, "Kiropraktisk behandling","Justering af rygsøjle og led.",                               480),
        ProviderService.Create(p[15].Id, 5, "Sportsskadebehandling",  "Behandling af sportsskader + rehabilitering.",                590),

        // Mixed providers (16-19)
        ProviderService.Create(p[16].Id, 1, "Klip & Farve Pakke",     "Klip + farvebehandling til fast pris.",                       780),
        ProviderService.Create(p[16].Id, 2, "Neglepleje",             "Manicure og pedikure kombineret.",                            520),
        ProviderService.Create(p[16].Id, 3, "Express massage 30 min", "Hurtig afslapning – nakke og skuldre.",                       299),
        ProviderService.Create(p[17].Id, 2, "Klassisk manicure",      "Neglefil, olie og lakering.",                                 279),
        ProviderService.Create(p[17].Id, 3, "Afslapningsmassage",     "Skræddersyet massage – valgfri varighed.",                    449),
        ProviderService.Create(p[18].Id, 4, "Tandkontrol + rensning", "Helhedstjek og professionel tandrensning.",                   560),
        ProviderService.Create(p[18].Id, 5, "Kiropraktion & fysio",   "Kombineret behandling, første besøg inkl. vurdering.",        710),
        ProviderService.Create(p[19].Id, 3, "Wellnessmassage 60 min", "Dyb afslappende massage i rolige omgivelser.",                499),
        ProviderService.Create(p[19].Id, 5, "Sportsfysioterapi",      "Vurdering og behandling af sport og bevægelsesskader.",        620),
    ];
}
