using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL
{
    /// <summary>
    /// Lois en vigueur au lancement du site, reprises du carnet « Lois et droits ». Les admins les gèrent ensuite depuis le site
    /// </summary>
    public static class InitialLaws
    {
        public static readonly Law[] Laws =
        {
            new Law { Id = 1, Number = "1", Content = "Pour accéder à la citoyenneté d'Adélys, il faut déposer une demande, qui sera examinée par le parlement." },
            new Law { Id = 2, Number = "2", Content = "L'assassinat, l'attaque, la destruction et le vol de bien sont interdit." },
            new Law { Id = 3, Number = "3", Content = "Pour chaque demandes (plainte, parcelle, construction, devenir membre du parlement...), il faut la déposer sur le discord. Elle seront examinées par le parlement." },
            new Law { Id = 4, Number = "4", Content = "Les sanctions possibles lors d'un jugement sont :\n\n- l'avertissement\n- la mort\n- dommage et intérêt\n- le bannissement" },
            new Law { Id = 5, Number = "5", Content = "La monnaie utilisée est le Cuivre Brut, le Fer Brut et l'Or Brut (1 Or Brut = 9 Fer Brut / 1 Fer Brut = 9 Cuivre Brut)" },
            new Law { Id = 6, Number = "6", Content = "Il peut être demandé aux citoyens une participation exceptionnelle sous forme de main d'œuvre ou de matériaux lors de la mise en place de grands projets communs (ex: la banque, le parlement)." },
            new Law { Id = 7, Number = "7", Content = "Pour des raison de performance, il est interdit de posséder un cheptel d'animaux trop nombreux (20 individus pour chaque espèces), hormis exceptions validées par le parlement." },
            new Law { Id = 8, Number = "8", Content = "Il est interdit de construire de manière non esthétique, de défigurer le paysage, de créer des mines à ciel ouvert sans accord du parlement." },
            new Law { Id = 9, Number = "9", Content = "L'abus de pouvoir est interdit" },
            new Law { Id = 10, Number = "10", Content = "- Les portails du nether sont limités à 1 par colonie\n- Le portail d'Adélys se trouve sous l'hôtel de ville\n- La construction d'un portail doit obligatoirement faire l'objet d'une demande" },
            new Law { Id = 11, Number = "11", Content = "Pour le commerce, certain prix sont fixes :\n\n- 16 lingots de fer = 5 lingots d'or\n- 1 diamant = 3 lingots d'or\n- 1 lingot de netherite = 40 lingots d'or" },
            new Law { Id = 12, Number = "12", Content = "L'exploration ne peux se faire que sous certaines conditions :\n\n- L'exploration en ligne droite dans une direction est prohibé (exploration anarchique)\n- L'exploration dois être mappée\n- L'exploration doit se faire à côté d'une zone explorée" },
            new Law { Id = 13, Number = "13", Content = "En cas d'abandon du serveur par un membre du parlement, ce dernier sera destituer de sa place passé un délai de 6 mois et sans indications contraires de sa part. En cas de retour sur le serveur la personne sera à nouveau citoyen et non membre du parlement." },
            new Law { Id = 14, Number = "13bis", Content = "En cas d'un départ du serveur, les commerces et monopoles sont \"gelés\" et chacun peut revendiquer le monopole passé les 6 mois. La ville reprend le commerce durant les 6 mois et engage un citoyen pour la reprise éventuelle du commerce.\n\nLes stocks précédents le départ des propriétaires seront stockés à la banque ou à l'hôtel de ville en cas d'un éventuel retour. La ville repartira avec un stock neuf. L'argent de ces 6 mois reviens à la ville mais la personne partenaire touchera un pourcentage." },
            new Law { Id = 15, Number = "14", Content = "Tout citoyen ayant quitté le serveur peux y revenir à tout moment hors bannissement. Les lois 13 et 13bis seront à appliquer pour la répartition des biens selon la durée du départ." },
            new Law { Id = 16, Number = "15", Content = "Tout event organisé par un citoyen d'Adélys est fortement encouragé mais doit être approuvé par au minimum un membre du parlement." }
        };

        public static readonly LawAmendment[] Amendments =
        {
            new LawAmendment { Id = 1, LawId = 5, AmendedOn = new DateTime(2021, 12, 4), Note = "changement de monnaie" },
            new LawAmendment { Id = 2, LawId = 6, AmendedOn = new DateTime(2020, 12, 17), Note = "nouvelle formulation" }
        };
    }
}
